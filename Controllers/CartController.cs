using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZenStore.API.Data;
using ZenStore.API.Models;
using System.Security.Claims;

namespace ZenStore.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly AppDBContext _context;

    public CartController(AppDBContext context)
    {
        _context = context;
    }

    [HttpPost("add")]
    [Authorize]
    public async Task<IActionResult> AddToCart(int productId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var product = await _context.products.FindAsync(productId);
        if (product == null)
            return NotFound("Product not found");

        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.userID == userId);

        if (cart == null)
        {
            cart = new Cart
            {
                userID = userId
            };

            _context.Carts.Add(cart);
        }

        var existingItem = cart.Items
            .FirstOrDefault(i => i.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                ProductId = productId,
                Quantity = 1
            });
        }

        await _context.SaveChangesAsync();
        return Ok("Added to cart");
    }
    [HttpPost("Delete")]
    public async Task<IActionResult> DeleteCart(int productId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var product = await _context.products.FindAsync(productId);
        if (product == null)
            return NotFound("Product not found");

        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.userID == userId);

        if (cart == null)
        {
            return NotFound("Cart is Empty");
        }

        var existingItem = cart.Items
            .FirstOrDefault(i => i.ProductId == productId);

        if (existingItem != null)
            cart.Items.Remove(existingItem);
        else
        {
            return Ok("Product Already Removed");
        }

        await _context.SaveChangesAsync();
        return Ok("Removed from cart");
    }

    [HttpPost("decrease")]
    public async Task<IActionResult> Decrease(int productId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.userID == userId);

        if (cart == null)
            return NotFound("Cart is Empty");

        var existingItem = cart.Items
            .FirstOrDefault(i => i.ProductId == productId);

        if (existingItem == null)
            return NotFound("Item not found");

        if (existingItem.Quantity > 1)
            existingItem.Quantity--;
        else
            cart.Items.Remove(existingItem);

        await _context.SaveChangesAsync();
        return Ok();
    }


    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.userID == userId);

        return Ok(cart);
    }
}
