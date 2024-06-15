using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.ApiResponse;
using MediatR;

namespace Product.Service.Commands;

public class CreateProductRequest: IRequest<ApiResult>
{
    public string Name { get; set; }
    [MaxLength(500)]
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string Title { get; set; }
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    [Required]
    public int Stock { get; set; }
}