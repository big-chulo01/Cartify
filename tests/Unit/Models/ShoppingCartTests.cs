using Cartify.Core.Models;
using Xunit;

namespace Cartify.Tests.Models;

public class ShoppingCartTests
{
    [Fact]
    public void ShoppingCart_Id_Should_Be_NonEmpty_Guid_ByDefault()
    {
        var cart = new ShoppingCart();
        Assert.NotEqual(Guid.Empty, cart.Id);
    }

    [Fact]
    public void ShoppingCart_User_Should_Be_Initialized_As_EmptyString()
    {
        var cart = new ShoppingCart();
        Assert.Equal(string.Empty, cart.User);
    }

    [Fact]
    public void ShoppingCart_Products_Should_Initialize_Empty_List()
    {
        var cart = new ShoppingCart();
        Assert.NotNull(cart.Products);
        Assert.Empty(cart.Products);
    }

    [Fact]
    public void ShoppingCart_Can_Add_And_Remove_Products()
    {
        var cart = new ShoppingCart();
        var product = new Product { Id = Guid.NewGuid(), Name = "Test Product" };
        
        // Add product
        cart.Products.Add(product);
        Assert.Single(cart.Products);
        Assert.Equal("Test Product", cart.Products[0].Name);
        
        // Remove product
        cart.Products.Remove(product);
        Assert.Empty(cart.Products);
    }
}