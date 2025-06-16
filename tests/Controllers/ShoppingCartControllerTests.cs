using Cartify.Core.Models;
using Cartify.Core.Data;
using Cartify.API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace Cartify.Tests.Controllers;

public class ShoppingCartControllerTests
{
    private readonly Mock<ILogger<ShoppingCartController>> _mockLogger = new();
    private readonly DbContextOptions<AppDbContext> _dbOptions;
    private const string TestUserEmail = "test@example.com";

    public ShoppingCartControllerTests()
    {
        _dbOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private AppDbContext GetDbContext()
    {
        return new AppDbContext(_dbOptions);
    }

    private ShoppingCartController GetController(AppDbContext context, string? userEmail = TestUserEmail)
    {
        var httpContext = new DefaultHttpContext();
        if (userEmail != null)
        {
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, userEmail)
            }));
        }

        var controller = new ShoppingCartController(context, new HttpContextAccessor { HttpContext = httpContext }, _mockLogger.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext }
        };

        return controller;
    }

    private void SeedTestData(AppDbContext context)
    {
        var product = new Product { Id = Guid.NewGuid(), Name = "Test Product", Price = 9.99m };
        var cart = new ShoppingCart
        {
            Id = Guid.NewGuid(),
            User = TestUserEmail,
            Items = new List<CartItem>
            {
                new() { Id = Guid.NewGuid(), ProductId = product.Id, Product = product, Quantity = 1 }
            }
        };

        context.Products.Add(product);
        context.ShoppingCarts.Add(cart);
        context.SaveChanges();
    }

    [Fact]
    public async Task GetCart_ReturnsProducts_ForAuthenticatedUser()
    {
        // Arrange
        using var context = GetDbContext();
        SeedTestData(context);
        var controller = GetController(context);

        // Act
        var result = await controller.GetCart();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var products = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
        Assert.Single(products);
        Assert.Equal("Test Product", products.First().Name);
    }

    [Fact]
    public async Task GetCart_ReturnsEmpty_WhenNoCartExists()
    {
        // Arrange
        using var context = GetDbContext();
        var controller = GetController(context);

        // Act
        var result = await controller.GetCart();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var products = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
        Assert.Empty(products);
    }

    [Fact]
    public async Task GetCart_ReturnsUnauthorized_WhenUserNotAuthenticated()
    {
        // Arrange
        using var context = GetDbContext();
        var controller = GetController(context, userEmail: null);

        // Act
        var result = await controller.GetCart();

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task AddToCart_CreatesNewCart_WhenNoneExists()
    {
        // Arrange
        using var context = GetDbContext();
        var product = new Product { Id = Guid.NewGuid(), Name = "New Product" };
        context.Products.Add(product);
        context.SaveChanges();
        
        var controller = GetController(context);

        // Act
        var result = await controller.AddToCart(product.Id);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Equal(1, context.ShoppingCarts.Count());
        Assert.Equal(TestUserEmail, context.ShoppingCarts.First().User);
    }

    [Fact]
    public async Task AddToCart_AddsProduct_ToExistingCart()
    {
        // Arrange
        using var context = GetDbContext();
        SeedTestData(context);
        var newProduct = new Product { Id = Guid.NewGuid(), Name = "Another Product" };
        context.Products.Add(newProduct);
        context.SaveChanges();
        
        var controller = GetController(context);

        // Act
        var result = await controller.AddToCart(newProduct.Id);

        // Assert
        Assert.IsType<NoContentResult>(result);
        var cart = context.ShoppingCarts.Include(sc => sc.Items).First();
        Assert.Equal(2, cart.Items.Count);
    }

    [Fact]
    public async Task AddToCart_ReturnsNotFound_ForInvalidProduct()
    {
        // Arrange
        using var context = GetDbContext();
        var controller = GetController(context);

        // Act
        var result = await controller.AddToCart(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task RemoveFromCart_DeletesItem_WhenExists()
    {
        // Arrange
        using var context = GetDbContext();
        SeedTestData(context);
        var productId = context.Products.First().Id;
        var controller = GetController(context);

        // Act
        var result = await controller.RemoveFromCart(productId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        var cart = context.ShoppingCarts.Include(sc => sc.Items).First();
        Assert.Empty(cart.Items);
    }

    [Fact]
    public async Task RemoveFromCart_ReturnsNotFound_WhenCartNotExists()
    {
        // Arrange
        using var context = GetDbContext();
        var controller = GetController(context);

        // Act
        var result = await controller.RemoveFromCart(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task RemoveFromCart_ReturnsNotFound_WhenItemNotInCart()
    {
        // Arrange
        using var context = GetDbContext();
        SeedTestData(context);
        var controller = GetController(context);

        // Act
        var result = await controller.RemoveFromCart(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}