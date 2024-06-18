namespace Product.Service.ViewModels;

public class ProductViewModel
{
    public ProductViewModel()
    {
    }

    public ProductViewModel(Domain.Entities.Product entity)
    {
        Title = entity.Title;
        Name = entity.Name;
        Stock = entity.Stock;
        ImageUrl = entity.ImageUrl;
        Id = entity.Id;
    }

    public int Page { get; set; } = 1;
    public int Size { get; set; } = 20;
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public Guid Id { get; set; }
}