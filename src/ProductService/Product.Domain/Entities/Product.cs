using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Entities;

namespace Product.Domain.Entities;

public class Product : IBaseEntity<Guid>, ICreatedEntity, IUpdatedEntity
{
    [ForeignKey("Category")]
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    [MaxLength(500)]
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    [Required]
    public int Stock { get; set; }
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    [Required]
    public string Title { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string UpdatedBy { get; set; }
}