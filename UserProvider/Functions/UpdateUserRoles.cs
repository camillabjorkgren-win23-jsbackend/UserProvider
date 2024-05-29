using Data.Contexts;
using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace UserProvider.Functions;

public class UpdateUserRoles(ILogger<UpdateUserRoles> logger, DataContext context)
{
    private readonly ILogger<UpdateUserRoles> _logger = logger;
    private readonly DataContext _context = context;

    [Function("UpdateUserRoles")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        try
        {
            var userAdminForm = req.ReadFromJsonAsync<UserAdminForm>().Result;

            if (userAdminForm == null)
            {
                return new BadRequestObjectResult("Invalid request");
            }
            else
            {
                var user = _context.Users.Find(userAdminForm.UserId);

                if (user != null)
                {
                    var userRoles = await _context.UserRoles
            .Where(ur => ur.UserId == userAdminForm.UserId)
            .ToListAsync();

                    string usersRoleID = _context.Roles.FirstOrDefault(r => r.Name == "User").Id;
                    string adminRoleID = _context.Roles.FirstOrDefault(r => r.Name == "Admin").Id;
                    string cioRoleID = _context.Roles.FirstOrDefault(r => r.Name == "CIO").Id;
                    string superAdminRoleID = _context.Roles.FirstOrDefault(r => r.Name == "SuperAdmin").Id;

                    if (userAdminForm.IsUser)
                    {

                        var addUserRole = new IdentityUserRole<string>
                        {
                            UserId = userAdminForm.UserId,
                            RoleId = usersRoleID
                        };
                        if (!userRoles.Any(ur => ur.RoleId == addUserRole.RoleId))
                        {
                            _context.UserRoles.Add(addUserRole);
                        }
                    }
                    else if (userAdminForm.IsUser == false && userRoles.Any(ur => ur.RoleId == usersRoleID))
                    {
                        var removeUserRole = userRoles.FirstOrDefault(ur => ur.RoleId == usersRoleID);
                        _context.UserRoles.Remove(removeUserRole);
                    }
                    if (userAdminForm.IsAdmin)
                    {

                        var addAdminRole = new IdentityUserRole<string>
                        {
                            UserId = userAdminForm.UserId,
                            RoleId = adminRoleID
                        };
                        if (!userRoles.Any(ur => ur.RoleId == addAdminRole.RoleId))
                        {
                            _context.UserRoles.Add(addAdminRole);
                        }
                    }
                    else if (userAdminForm.IsAdmin == false && userRoles.Any(ur => ur.RoleId == adminRoleID))
                    {
                        var removeAdminRole = userRoles.FirstOrDefault(ur => ur.RoleId == adminRoleID);
                        _context.UserRoles.Remove(removeAdminRole);
                    }
                    if (userAdminForm.IsCIO)
                    {

                        var addCIORole = new IdentityUserRole<string>
                        {
                            UserId = userAdminForm.UserId,
                            RoleId = cioRoleID
                        };
                        if (!userRoles.Any(ur => ur.RoleId == addCIORole.RoleId))
                        {
                            _context.UserRoles.Add(addCIORole);
                        }
                    }
                    else if (userAdminForm.IsCIO == false && userRoles.Any(ur => ur.RoleId == cioRoleID))
                    {
                        var removeCIORole = userRoles.FirstOrDefault(ur => ur.RoleId == cioRoleID);
                        _context.UserRoles.Remove(removeCIORole);
                    }
                    if (userAdminForm.IsSuperAdmin)
                    {

                        var addSuperAdminRole = new IdentityUserRole<string>
                        {
                            UserId = userAdminForm.UserId,
                            RoleId = superAdminRoleID
                        };
                        if (!userRoles.Any(ur => ur.RoleId == addSuperAdminRole.RoleId))
                        {
                            _context.UserRoles.Add(addSuperAdminRole);
                        }
                    }
                    else if (userAdminForm.IsSuperAdmin == false && userRoles.Any(ur => ur.RoleId == superAdminRoleID))
                    {
                        var removeSuperAdminRole = userRoles.FirstOrDefault(ur => ur.RoleId == superAdminRoleID);
                        _context.UserRoles.Remove(removeSuperAdminRole);
                    }
                    await _context.SaveChangesAsync();

                    var newUserRolesList = await _context.UserRoles
           .Where(ur => ur.UserId == userAdminForm.UserId)
           .ToListAsync();
                    if (userRoles != null)
                    {
                        return new OkObjectResult(newUserRolesList);
                    }
                    else
                    {
                        return new NotFoundResult();
                    }

                }
            }
            return new BadRequestResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user roles");
            return new BadRequestResult();
        }
    }

}
