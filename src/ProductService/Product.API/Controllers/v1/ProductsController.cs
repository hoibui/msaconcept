using Common.ApiResponse;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.Service.Commands;
using Product.Service.Queries;

namespace Product.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost(ApiRoutes.Product.CREATE)]
    public async Task<IActionResult> CreateAsync(CreateProductRequest request)
    {
        return await _mediator.Send(request);
    }
    
    [HttpGet(ApiRoutes.Product.GET_LANDING_PAGE)]
    public async Task<IActionResult> GetLandingPageAsync([FromQuery] GetProductTrendRequest request)
    {
        return await _mediator.Send(request);
    }
    
    [HttpGet(ApiRoutes.Product.GET_LIST)]
    public async Task<IActionResult> GetListAsync([FromQuery] GetProductRequest request)
    {
        return await _mediator.Send(request);
    }
}