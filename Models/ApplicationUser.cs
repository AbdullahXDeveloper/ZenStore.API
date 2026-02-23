using Microsoft.AspNetCore.Identity;

namespace ZenStore.API.Models;

public class ApplicationUser : IdentityUser
{
}
public class RegisterDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}
public class EditUserDto
{
    public string Email { get; set; }
    public string Role { get; set; } 
}
