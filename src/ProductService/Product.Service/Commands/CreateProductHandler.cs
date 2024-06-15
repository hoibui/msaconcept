using AutoMapper;
using Common.ApiResponse;
using Common.ErrorResult;
using DataHelper.EntityFramework.UnitOfwork.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Product.Data.DbContext;

namespace Product.Service.Commands;

public class CreateProductHandler: IRequestHandler<CreateProductRequest, ApiResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public CreateProductHandler(IUnitOfWork<WriteDbContext> unitOfWork, IMapper mapper, ILogger<CreateProductHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResult> Handle(CreateProductRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var comment = new Domain.Entities.Product()
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Title = request.Title,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                Stock = request.Stock,
                Price = request.Price,
                CategoryId = Guid.Parse("0662f9e4-ece4-4272-be5b-fc11bef5a32d"),
                UpdatedBy = "Hoi Bui",
                CreatedBy = "Hoi Bui"
            };

            await _unitOfWork.GetRepository<Domain.Entities.Product>().InsertAsync(comment, cancellationToken);
            await _unitOfWork.CommitAsync();
        
            return ApiResult.Succeeded();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return ApiResult.Failed(HttpCode.InternalServerError);
        }
        
    }
}