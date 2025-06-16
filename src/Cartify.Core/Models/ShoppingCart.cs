public class ShoppingCart
{
    public Guid Id { get; set; }
    public string User { get; set; } = string.Empty;
    public List<CartItem> Items { get; set; } = new(); 
}

