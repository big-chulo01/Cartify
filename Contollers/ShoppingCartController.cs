// Controllers/ShoppingCartController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ShoppingCartController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ShoppingCartController> _logger;

    public ShoppingCartController(ApplicationDbContext context, ILogger<ShoppingCartController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Get all products in the user's shopping cart
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetShoppingCart()
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var shoppingCart = await _context.ShoppingCarts
            .Include(sc => sc.Products)
            .FirstOrDefaultAsync(sc => sc.User == userEmail);

        if (shoppingCart == null)
        {
            return Ok(new List<Product>()); // Return empty list if no cart exists
        }

        return Ok(shoppingCart.Products);
    }

    // Add a product to the shopping cart
    [HttpPost("{productId}")]
    public async Task<IActionResult> AddToCart(int productId)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var product = await _context.Products.FindAsync(productId);

        if (product == null)
        {
            return NotFound("Product not found");
        }

        var shoppingCart = await _context.ShoppingCarts
            .Include(sc => sc.Products)
            .FirstOrDefaultAsync(sc => sc.User == userEmail);

        if (shoppingCart == null)
        {
            shoppingCart = new ShoppingCart
            {
                User = userEmail,
                Products = new List<Product> { product }
            };
            _context.ShoppingCarts.Add(shoppingCart);
        }
        else
        {
            if (!shoppingCart.Products.Any(p => p.Id == productId))
            {
                shoppingCart.Products.Add(product);
            }
            else
            {
                return Conflict("Product already in cart");
            }
        }

        await _context.SaveChangesAsync();
        return Ok();
    }

    // Remove a product from the shopping cart
    [HttpDelete("{productId}")]
    public async Task<IActionResult> RemoveFromCart(int productId)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var shoppingCart = await _context.ShoppingCarts
            .Include(sc => sc.Products)
            .FirstOrDefaultAsync(sc => sc.User == userEmail);

        if (shoppingCart == null)
        {
            return NotFound("Shopping cart not found");
        }

        var productToRemove = shoppingCart.Products.FirstOrDefault(p => p.Id == productId);
        if (productToRemove == null)
        {
            return NotFound("Product not found in cart");
        }

        shoppingCart.Products.Remove(productToRemove);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}