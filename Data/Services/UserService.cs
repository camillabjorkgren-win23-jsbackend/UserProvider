using Data.Contexts;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Services;
public class UserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly DataContext _context;

    public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, DataContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
       
    }

    //public async Task<List<ApplicationUser>> GetAllUsersAsync()
    //{
    //    try
    //    {
    //        var users = await _userManager.Users.ToListAsync();
    //        if (users != null)
    //        {
    //            foreach (var user in users)
    //            {
    //                 = await GetRolesAsync(user.Id);
    //            }
    //            return users;
    //        }
    //        return null!;
    //    }
    //    catch (Exception)
    //    {
    //        return null!;
    //    }
    //}

    public async Task<ApplicationUser> GetUserByIdAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
    {
        return await _userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            return await _userManager.DeleteAsync(user);
        }
        return IdentityResult.Failed(new IdentityError { Description = "User not found" });
    }

    public async Task<IdentityResult> AddUserToRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null && await _roleManager.RoleExistsAsync(role))
        {
            return await _userManager.AddToRoleAsync(user, role);
        }
        return IdentityResult.Failed(new IdentityError { Description = "User or role not found" });
    }

    public async Task<IList<string>> GetRolesAsync(string userId)
    {
        var rolesList = new List<UserRoles>();
        var user = await _userManager.FindByIdAsync(userId);
        var roles = await _userManager.GetRolesAsync(user);
        //foreach (var roleName in roles)
        //{
        //    var role = await _roleManager.FindByNameAsync(roleName);
        //    if (role != null)
        //    {
        //        rolesList.Add(new UserRoles
        //        {
        //            RoleName = role.Name,
        //            RoleId = role.Id
        //        });
        //    }
        //}
        return roles;
    }

    public async Task<List<UsersWithRolesDisplay>> GetAllUsersWithRolesAsync(List<ApplicationUser> users)
    {
        try
        {
            var userWithRolesList = new List<UsersWithRolesDisplay>();

            foreach (var user in users)
            {
                var roles = await _context.Roles
                  .Where(r => _context.UserRoles
                   .Where(ur => ur.UserId == user.Id)
                  .Select(ur => ur.RoleId)
                  .Contains(r.Id))
                  .Select(r => r.Name)
                  .ToListAsync();

                var userWithRoles = new UsersWithRolesDisplay
                {
                    FirstName = user.UserProfile?.FirstName ?? string.Empty,
                    LastName = user.UserProfile?.LastName ?? string.Empty,
                    Email = user.Email,
                    Roles = roles.Any() ? roles : null
                };

                userWithRolesList.Add(userWithRoles);
            }    

            return userWithRolesList;
        }
        catch (Exception ex)
        {
            return null!;
        }
    }
}
public class UsersWithRolesDisplay
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string>? Roles { get; set; }
}
public class UserRoles
{
    public string RoleName { get; set; } = null!;
    public string RoleId { get; set; } = null!;
}