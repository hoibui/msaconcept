using Common.ApiResponse;
using Common.Enums;
using MediatR;

namespace Product.Service.Queries;

public class GetProductRequest: IRequest<ApiResult>
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 20;
    
    public SortType SortType { get; set; } = SortType.DESC;
}