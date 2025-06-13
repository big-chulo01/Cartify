// Models/ShoppingCart.cs
public class ShoppingCart
{
    public int Id { get; set; }
    public string User { get; set; } // This will store the user's email
    public List<Product> Products { get; set; } = new List<Product>();
}