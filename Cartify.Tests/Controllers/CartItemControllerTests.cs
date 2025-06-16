using Cartify.Core.Models;
using Cartify.Core.Data;
using Cartify.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Cartify.Tests.Controllers;

public class CartItemControllerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly CartItemController _controller;
    private readonly Guid _testCartId = Guid.NewGuid();
    private readonly Guid _testProductId = Guid.NewGuid();
    private const string TestUserEmail = "test@example.com";

    public CartItemControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new AppDbContext(options);
        
        // Setup test data
        var product = new Product 
        { 
            Id = _testProductId, 
            Name = "Test Product", 
            Price = 9.99m 
        };
        
        var cart = new ShoppingCart
        {
            Id = _testCartId,
            User = TestUserEmail,
            Items = new List<CartItem>()
        };
        
        _context.Products.Add(product);
        _context.ShoppingCarts.Add(cart);
        _context.SaveChanges();
        
        _controller = new CartItemController(_context);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task UpdateQuantity_ReturnsOk_WhenValid()
    {
        // Arrange
        var cartItem = new CartItem
        {
            Id = Guid.NewGuid(),
            ShoppingCartId = _testCartId,
            ProductId = _testProductId,
            Quantity = 1
        };
        _context.CartItems.Add(cartItem);
        await _context.SaveChangesAsync();
        
        // Act
        var result = await _controller.UpdateQuantity(cartItem.Id, 3);
        
        // Assert
        Assert.IsType<OkResult>(result);
        var updatedItem = await _context.CartItems.FindAsync(cartItem.Id);
        Assert.Equal(3, updatedItem?.Quantity);
    }

    [Fact]
    public async Task UpdateQuantity_ReturnsNotFound_WhenInvalidId()
    {
        // Act
        var result = await _controller.UpdateQuantity(Guid.NewGuid(), 2);
        
        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Remove_DeletesItem()
    {
        // Arrange
        var cartItem = new CartItem
        {
            Id = Guid.NewGuid(),
            ShoppingCartId = _testCartId,
            ProductId = _testProductId
        };
        _context.CartItems.Add(cartItem);
        await _context.SaveChangesAsync();
        
        // Act
        var result = await _controller.Remove(cartItem.Id);
        
        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Null(await _context.CartItems.FindAsync(cartItem.Id));
    }
}