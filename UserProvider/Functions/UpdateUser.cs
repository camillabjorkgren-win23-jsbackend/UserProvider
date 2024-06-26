using Data.Contexts;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace UserProvider.Functions;

public class UpdateUser(ILogger<UpdateUser> logger, DataContext context)
{
    private readonly ILogger<UpdateUser> _logger = logger;
    private readonly DataContext _context = context;

    [Function("UpdateUser")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        try
        {
            var userForm = await req.ReadFromJsonAsync<UserForm>();

            if (userForm != null)
            {
                var user = await _context.Users
                    .Include(u => u.UserProfile)
                    .Include(u => u.UserAddress)
                    .FirstOrDefaultAsync(u => u.Id == userForm.Id);

                if (user != null)
                {
                    user.PreferredEmail = userForm.PreferredEmail;
                    user.Modified = DateTime.Now;

                    if (user.UserAddress != null)
                    {
                        user.UserAddress.AddressType = userForm.AddressType;
                        user.UserAddress.AddressLine_1 = userForm.AddressLine_1;
                        user.UserAddress.AddressLine_2 = userForm.AddressLine_2;
                        user.UserAddress.PostalCode = userForm.PostalCode;
                        user.UserAddress.City = userForm.City;
                    }
                    else
                    {
                        user.UserAddress = new UserAddress
                        {
                            AddressType = userForm.AddressType,
                            AddressLine_1 = userForm.AddressLine_1,
                            AddressLine_2 = userForm.AddressLine_2,
                            PostalCode = userForm.PostalCode,
                            City = userForm.City
                        };
                    }

                    if (user.UserProfile != null)
                    {
                        user.UserProfile.FirstName = userForm.FirstName;
                        user.UserProfile.LastName = userForm.LastName;
                    }
                    else
                    {
                        user.UserProfile = new UserProfile
                        {
                            FirstName = userForm.FirstName,
                            LastName = userForm.LastName
                        };
                    }

                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    return new OkObjectResult(user);
                }
                else
                {
                    return new NotFoundObjectResult("User not found");
                }
            }
            else
            {
                return new BadRequestObjectResult("Invalid user form");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new BadRequestObjectResult(ex.Message);
        }
    }

}

public class UserForm
{
    public string Id { get; set; } = null!;
    public string? PreferredEmail { get; set; }

    public string AddressType { get; set; } = null!;
    public string AddressLine_1 { get; set; } = null!;
    public string? AddressLine_2 { get; set; }

    public string PostalCode { get; set; } = null!;
    public string City { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

}