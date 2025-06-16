using Cartify.Core.Models;
using Xunit;

namespace Cartify.Tests.Models;

public class ProductTests
{
    [Fact]
    public void Product_Id_Should_Be_NonEmpty_Guid_ByDefault()
    {
        var product = new Product();
        Assert.NotEqual(Guid.Empty, product.Id);
    }

    [Fact]
    public void Product_Name_Should_Be_Initialized_As_EmptyString()
    {
        var product = new Product();
        Assert.Equal(string.Empty, product.Name);
    }

    [Fact]
    public void Product_Price_Should_Be_Zero_ByDefault()
    {
        var product = new Product();
        Assert.Equal(0, product.Price);
    }

    [Fact]
    public void Product_Should_Have_Category_Reference_But_Null_ByDefault()
    {
        var product = new Product();
        Assert.Null(product.Category);
        Assert.Equal(Guid.Empty, product.CategoryId);
    }

    [Fact]
    public void Product_Description_Should_Be_Initialized_As_EmptyString()
    {
        var product = new Product();
        Assert.Equal(string.Empty, product.Description);
    }
}