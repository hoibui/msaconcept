using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.ApiResponse;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.Service.Commands;
using Product.Service.Queries;

namespace Product.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet]
        public IActionResult GetClaims()
        {
            var userClaims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(userClaims);
        }

        [HttpGet("name")]
        public IActionResult GetName()
        {
            string name = User.FindFirstValue(ClaimTypes.Name);
            return Ok(name);
        }

        [HttpGet("roles")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetRoles()
        {
            IEnumerable<Claim> roleClaims = User.FindAll(ClaimTypes.Role);
            IEnumerable<string> roles = roleClaims.Select(r => r.Value);
            return Ok(roles);
        }
        
        [HttpPost(ApiRoutes.Product.CREATE)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAsync(CreateProductRequest request)
        {
            return await _mediator.Send(request);
        }
    
        [HttpGet(ApiRoutes.Product.GET_LANDING_PAGE)]
        [AllowAnonymous]
        public async Task<IActionResult> GetLandingPageAsync([FromQuery] GetProductTrendRequest request)
        {
            return await _mediator.Send(request);
        }
        
        [HttpGet(ApiRoutes.Product.GET_LIST)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetListAsync([FromQuery] GetProductRequest request)
        {
            System.Threading.Thread.Sleep(1000);
            return await _mediator.Send(request);
        }
    }
}
