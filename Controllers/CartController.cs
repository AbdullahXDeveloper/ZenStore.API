using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using ZenStore.API.Data;
using ZenStore.API.Models;

namespace ZenStore.API.Controllers;

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
    public async Task<IActionResult> AddToCart(int productId)
    {
        var product = await _context.products.FindAsync(productId);
        if (product == null)
            return NotFound("Product not found");

        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync();

        if (cart == null)
        {
            cart = new Cart();
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

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync();

        return Ok(cart);
    }
}
