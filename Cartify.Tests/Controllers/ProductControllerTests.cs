using Cartify.Core.Models;
using Cartify.Core.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.Extensions.Logging;

namespace Cartify.Tests.Controllers;

public class ProductControllerTests
{
    private readonly Mock<ILogger<ProductController>> _mockLogger;
    private readonly DbContextOptions<AppDbContext> _dbOptions;

    public ProductControllerTests()
    {
        _mockLogger = new Mock<ILogger<ProductController>>();
        _dbOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private AppDbContext GetDbContext()
    {
        return new AppDbContext(_dbOptions);
    }

    // Helper method to seed test data
    private void SeedTestData(AppDbContext context)
    {
        var category = new Category { Id = Guid.NewGuid(), Description = "Electronics" };
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Name = "Laptop", Price = 999.99m, CategoryId = category.Id },
            new() { Id = Guid.NewGuid(), Name = "Phone", Price = 699.99m, CategoryId = category.Id }
        };
        
        context.Categories.Add(category);
        context.Products.AddRange(products);
        context.SaveChanges();
    }

    [Fact]
    public async Task GetProducts_ReturnsAllProducts()
    {
        // Arrange
        using var context = GetDbContext();
        SeedTestData(context);
        var controller = new ProductController(context, _mockLogger.Object);

        // Act
        var result = await controller.GetProducts();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<Product>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var products = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
        Assert.Equal(2, products.Count());
    }

    [Fact]
    public async Task GetProduct_ReturnsProduct_WhenExists()
    {
        // Arrange
        using var context = GetDbContext();
        SeedTestData(context);
        var testProduct = context.Products.First();
        var controller = new ProductController(context, _mockLogger.Object);

        // Act
        var result = await controller.GetProduct(testProduct.Id);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Product>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var product = Assert.IsType<Product>(okResult.Value);
        Assert.Equal(testProduct.Id, product.Id);
    }

    [Fact]
    public async Task GetProduct_ReturnsNotFound_WhenNotExists()
    {
        // Arrange
        using var context = GetDbContext();
        var controller = new ProductController(context, _mockLogger.Object);
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await controller.GetProduct(nonExistentId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateProduct_ReturnsCreatedResponse_WithValidInput()
    {
        // Arrange
        using var context = GetDbContext();
        SeedTestData(context);
        var categoryId = context.Categories.First().Id;
        var controller = new ProductController(context, _mockLogger.Object);
        var newProduct = new Product 
        { 
            Name = "Tablet", 
            Price = 299.99m,
            CategoryId = categoryId
        };

        // Act
        var result = await controller.CreateProduct(newProduct);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Product>>(result);
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        var product = Assert.IsType<Product>(createdAtActionResult.Value);
        Assert.Equal(newProduct.Name, product.Name);
        Assert.Equal(3, context.Products.Count());
    }

    [Fact]
    public async Task CreateProduct_ReturnsBadRequest_WithInvalidModel()
    {
        // Arrange
        using var context = GetDbContext();
        var controller = new ProductController(context, _mockLogger.Object);
        controller.ModelState.AddModelError("Name", "Required");
        var invalidProduct = new Product { Price = 100m };

        // Act
        var result = await controller.CreateProduct(invalidProduct);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        using var context = GetDbContext();
        SeedTestData(context);
        var existingProduct = context.Products.First();
        var controller = new ProductController(context, _mockLogger.Object);
        existingProduct.Price = 899.99m;

        // Act
        var result = await controller.UpdateProduct(existingProduct.Id, existingProduct);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Equal(899.99m, context.Products.First().Price);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsBadRequest_WhenIdMismatch()
    {
        // Arrange
        using var context = GetDbContext();
        var controller = new ProductController(context, _mockLogger.Object);
        var product = new Product { Id = Guid.NewGuid(), Name = "Test" };

        // Act
        var result = await controller.UpdateProduct(Guid.NewGuid(), product);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteProduct_ReturnsNoContent_WhenExists()
    {
        // Arrange
        using var context = GetDbContext();
        SeedTestData(context);
        var productToDelete = context.Products.First();
        var controller = new ProductController(context, _mockLogger.Object);

        // Act
        var result = await controller.DeleteProduct(productToDelete.Id);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Equal(1, context.Products.Count());
    }

    [Fact]
    public async Task DeleteProduct_ReturnsNotFound_WhenNotExists()
    {
        // Arrange
        using var context = GetDbContext();
        var controller = new ProductController(context, _mockLogger.Object);
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await controller.DeleteProduct(nonExistentId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}