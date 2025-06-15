namespace CartifyApi.Infrastructure.Models;

public class Cartify
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string User { get; set; } = null!;
    public List<Product> Products { get; set; } = new();
}