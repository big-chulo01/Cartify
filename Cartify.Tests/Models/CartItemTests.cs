using Cartify.Core.Models;
using Xunit;

namespace Cartify.Tests.Models;

public class CartItemTests
{
    [Fact]
    public void CartItem_Should_InitializeWithDefaultQuantity()
    {
        // Arrange & Act
        var cartItem = new CartItem();
        
        // Assert
        Assert.Equal(1, cartItem.Quantity);
    }

    [Fact]
    public void CartItem_Should_StoreUnitPrice()
    {
        // Arrange
        var product = new Product { Price = 19.99m };
        
        // Act
        var cartItem = new CartItem 
        { 
            Product = product,
            UnitPrice = product.Price
        };
        
        // Assert
        Assert.Equal(19.99m, cartItem.UnitPrice);
    }

    [Fact]
    public void CartItem_Should_CalculateLineTotal()
    {
        // Arrange
        var cartItem = new CartItem
        {
            UnitPrice = 10.50m,
            Quantity = 3
        };
        
        // Act
        var lineTotal = cartItem.UnitPrice * cartItem.Quantity;
        
        // Assert
        Assert.Equal(31.50m, lineTotal);
    }

    [Fact]
    public void CartItem_Should_ReferenceProduct()
    {
        // Arrange
        var product = new Product { Id = Guid.NewGuid() };
        
        // Act
        var cartItem = new CartItem
        {
            ProductId = product.Id,
            Product = product
        };
        
        // Assert
        Assert.Equal(product.Id, cartItem.ProductId);
        Assert.Equal(product, cartItem.Product);
    }
}