using Cartify.Core.Models;
using Cartify.Core.Data;
using Cartify.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Cartify.Tests.Controllers;

public class CategoryControllerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly CategoryController _controller;

    public CategoryControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new AppDbContext(options);
        _controller = new CategoryController(_context);
        
        // Seed test data
        _context.Categories.AddRange(
            new Category { Id = Guid.NewGuid(), Description = "Electronics" },
            new Category { Id = Guid.NewGuid(), Description = "Books" }
        );
        _context.SaveChanges();
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task GetAll_ReturnsAllCategories()
    {
        // Act
        var result = await _controller.GetAll();
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var categories = Assert.IsAssignableFrom<IEnumerable<Category>>(okResult.Value);
        Assert.Equal(2, categories.Count());
    }

    [Fact]
    public async Task GetById_ReturnsCategory_WhenExists()
    {
        // Arrange
        var testCategory = _context.Categories.First();
        
        // Act
        var result = await _controller.GetById(testCategory.Id);
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var category = Assert.IsType<Category>(okResult.Value);
        Assert.Equal(testCategory.Description, category.Description);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenNotExists()
    {
        // Act
        var result = await _controller.GetById(Guid.NewGuid());
        
        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedCategory()
    {
        // Arrange
        var newCategory = new Category { Description = "Clothing" };
        
        // Act
        var result = await _controller.Create(newCategory);
        
        // Assert
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(3, _context.Categories.Count());
    }

    [Fact]
    public async Task Update_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        var existingCategory = _context.Categories.First();
        existingCategory.Description = "Updated Electronics";
        
        // Act
        var result = await _controller.Update(existingCategory.Id, existingCategory);
        
        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Equal("Updated Electronics", _context.Categories.First().Description);
    }

    [Fact]
    public async Task Delete_RemovesCategory()
    {
        // Arrange
        var categoryToDelete = _context.Categories.First();
        
        // Act
        var result = await _controller.Delete(categoryToDelete.Id);
        
        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Single(_context.Categories);
    }
}