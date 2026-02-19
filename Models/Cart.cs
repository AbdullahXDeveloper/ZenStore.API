namespace ZenStore.API.Models;

public class Cart
{
    public int Id { get; set; }

    public List<CartItem> Items { get; set; } = new List<CartItem>();
}
