namespace Common.Models;

public class CatalogResponse
{
    public Guid OrderId { get; set; }
    public Guid CatalogId { get; set; }
    public bool IsSuccess { get; set; }
}