namespace Common.Models;

public class OrderRequest
{
    public Guid OrderId { get; set; }
    public Guid CatalogId { get; set; }
    public int Units { get; set; }
    public string Name { get; set; }
}