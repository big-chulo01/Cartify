using CartifyApi.Infrastructure.Data;
using CartifyApi.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CartifyApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ShoppingCartController : ControllerBase
{
    private readonly AppDbContext _context;

    public ShoppingCartController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetCart()
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var cart = await _context.Cartifies
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.User == userEmail);

        if (cart == null)
        {
            return Ok(new List<Product>());
        }

        return Ok(cart.Products);
    }

    [HttpPost("{productId}")]
    public async Task<IActionResult> AddToCart(Guid productId)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var product = await _context.Products.FindAsync(productId);

        if (product == null)
        {
            return NotFound();
        }

        var cart = await _context.Cartifies
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.User == userEmail);

        if (cart == null)
        {
            cart = new Cartify { User = userEmail };
            _context.Cartifies.Add(cart);
        }

        if (!cart.Products.Any(p => p.Id == productId))
        {
            cart.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        return NoContent();
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> RemoveFromCart(Guid productId)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var cart = await _context.Cartifies
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.User == userEmail);

        if (cart == null)
        {
            return NotFound();
        }

        var product = cart.Products.FirstOrDefault(p => p.Id == productId);
        if (product == null)
        {
            return NotFound();
        }

        cart.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}