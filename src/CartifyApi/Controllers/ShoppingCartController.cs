using Cartify.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cartify.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ShoppingCartController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ShoppingCartController(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetCart()
    {
        var userEmail = _httpContextAccessor.HttpContext?.User.Identity?.Name;
        if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

        var cart = await _context.ShoppingCarts
            .Include(sc => sc.Products)
            .FirstOrDefaultAsync(sc => sc.User == userEmail);

        if (cart == null) return Ok(Enumerable.Empty<Product>());

        return Ok(cart.Products);
    }

    [HttpPost("{productId}")]
    public async Task<IActionResult> AddToCart(Guid productId)
    {
        var userEmail = _httpContextAccessor.HttpContext?.User.Identity?.Name;
        if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

        var product = await _context.Products.FindAsync(productId);
        if (product == null) return NotFound();

        var cart = await _context.ShoppingCarts
            .Include(sc => sc.Products)
            .FirstOrDefaultAsync(sc => sc.User == userEmail);

        if (cart == null)
        {
            cart = new ShoppingCart
            {
                User = userEmail,
                Products = new List<Product> { product }
            };
            _context.ShoppingCarts.Add(cart);
        }
        else
        {
            if (!cart.Products.Any(p => p.Id == productId))
            {
                cart.Products.Add(product);
            }
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> RemoveFromCart(Guid productId)
    {
        var userEmail = _httpContextAccessor.HttpContext?.User.Identity?.Name;
        if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

        var cart = await _context.ShoppingCarts
            .Include(sc => sc.Products)
            .FirstOrDefaultAsync(sc => sc.User == userEmail);

        if (cart == null) return NotFound("Cart not found");

        var product = cart.Products.FirstOrDefault(p => p.Id == productId);
        if (product == null) return NotFound("Product not found in cart");

        cart.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}