using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ZenStore.API.Models;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")] // Only Admin can access
public class UserManagementController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserManagementController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // ================= GET ALL USERS =================
    [HttpGet]
    public IActionResult GetAllUsers()
    {
        return Ok(_userManager.Users.ToList());
    }

    // ================= DELETE USER =================
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        await _userManager.DeleteAsync(user);
        return Ok("User deleted");
    }

    // ================= EDIT USER =================
    [HttpPut("{id}")]
    public async Task<IActionResult> EditUser(string id, [FromBody] EditUserDto model)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound("User not found");

        // Update basic info
        user.Email = model.Email ?? user.Email;
        user.UserName = model.Email ?? user.UserName; // keep username same as email
        // optionally update other fields if you added them in ApplicationUser

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        // Update roles if changed
        if (model.Role != null)
        {
            var currentRoles = await _userManager.GetRolesAsync(user);

            // remove existing roles
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // add new role
            if (!await _roleManager.RoleExistsAsync(model.Role))
                return BadRequest("Role does not exist");

            await _userManager.AddToRoleAsync(user, model.Role);
        }

        return Ok("User updated successfully");
    }
}