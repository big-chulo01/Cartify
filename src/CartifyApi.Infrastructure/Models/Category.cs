namespace CartifyApi.Infrastructure.Models;

public class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Description { get; set; } = null!;
}