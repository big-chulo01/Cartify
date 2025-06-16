using Cartify.Core.Models;
using Xunit;

namespace Cartify.Tests.Models;

public class CategoryTests
{
    [Fact]
    public void Category_Id_Should_Be_NonEmpty_Guid_ByDefault()
    {
        var category = new Category();
        Assert.NotEqual(Guid.Empty, category.Id);
    }

    [Fact]
    public void Category_Description_Should_Be_Initialized_As_EmptyString()
    {
        var category = new Category();
        Assert.Equal(string.Empty, category.Description);
    }

    [Fact]
    public void Category_Description_Should_Be_Settable()
    {
        var category = new Category { Description = "Electronics" };
        Assert.Equal("Electronics", category.Description);
    }

    [Fact]
    public void Category_Should_Not_Be_Null()
    {
        var category = new Category();
        Assert.NotNull(category);
    }
}