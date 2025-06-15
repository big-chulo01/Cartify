using CartifyApi.Controllers;
using CartifyApi.Infrastructure.Data;
using CartifyApi.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CartifyApi.Tests;

public class ShoppingCartControllerTests
{
    [Fact]
    public async Task GetCart_ReturnsEmptyList_WhenCartDoesNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        using var context = new AppDbContext(options);
        var controller = new ShoppingCartController(context);

        // Act
        var result = await controller.GetCart();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var products = Assert.IsType<List<Product>>(okResult.Value);
        Assert.Empty(products);
    }
}