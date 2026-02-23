using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZenStore.API.Data;
using ZenStore.API.Models;

namespace ZenStore.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly AppDBContext _context;

    public ProductsController(AppDBContext context)
    {
        _context = context;
    }

    // ================= GET ALL PRODUCTS =================
    // ✅ Customer aur Admin dono dekh sakte hain
    [HttpGet]
    [Authorize] // any logged-in user
    public async Task<IActionResult> GetProducts()
    {
        var products = await _context.products.ToListAsync();
        return Ok(products);
    }

    // ================= ADD PRODUCT =================
    // 🛠️ Sirf Admin add kar sakta hai
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddProduct(Product product)
    {
        _context.products.Add(product);
        await _context.SaveChangesAsync();
        return Ok(product);
    }

    // ================= UPDATE PRODUCT =================
    // 🛠️ Sirf Admin edit kar sakta hai
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateProduct(int id, Product updatedProduct)
    {
        var product = await _context.products.FindAsync(id);
        if (product == null)
            return NotFound("Product not found");

        // update fields
        product.Name = updatedProduct.Name;
        product.Description = updatedProduct.Description;
        product.Price = updatedProduct.Price;
        product.Stock = updatedProduct.Stock;

        await _context.SaveChangesAsync();
        return Ok(product);
    }

    // ================= DELETE PRODUCT =================
    // 🛠️ Sirf Admin delete kar sakta hai
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.products.FindAsync(id);
        if (product == null)
            return NotFound("Product not found");

        _context.products.Remove(product);
        await _context.SaveChangesAsync();
        return Ok("Product deleted successfully");
    }
}