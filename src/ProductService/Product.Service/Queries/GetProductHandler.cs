using System.Linq.Expressions;
using System.Text.Json;
using Common.ApiResponse;
using Common.Constants;
using Common.Enums;
using Common.ErrorResult;
using Common.Extensions;
using DataHelper.EntityFramework.UnitOfwork.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Paginate;
using Product.Data.DbContext;
using Product.Service.ViewModels;

namespace Product.Service.Queries;

public class GetProductHandler: IRequestHandler<GetProductRequest, ApiResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache;
        private readonly ILogger _logger;

        public GetProductHandler(IUnitOfWork<ReadDbContext> unitOfWork, IDistributedCache cache, ILogger<GetProductHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResult> Handle(GetProductRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = CacheKey.LIST_PRODUCT + JsonSerializer.Serialize(request);
                var prodcuts = await _cache.GetCacheValueAsync<Paginate<ProductViewModel>>(cacheKey);

                if (prodcuts == null)
                {
                    prodcuts = await GetPostsInDatabase(request, cancellationToken);
                    _ = _cache.SetCacheValueAsync(cacheKey, prodcuts, 60 * 1);
                }

                return ApiResult.Succeeded(prodcuts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return ApiResult.Failed(HttpCode.InternalServerError);
            }
        }

        private async Task<Paginate<ProductViewModel>> GetPostsInDatabase(GetProductRequest request, CancellationToken cancellationToken)
        {
            var products = await _unitOfWork.GetRepository<Domain.Entities.Product>().GetPagingListAsync(
                selector: n => new ProductViewModel(n),
                page: request.Page,
                size: request.Size,
                cancellationToken: cancellationToken);

            return (Paginate<ProductViewModel>)products;
        }
    }