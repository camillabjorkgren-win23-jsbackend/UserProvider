using Data.Contexts;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace UserProvider.Functions;

public class UpdateProfileImage(ILogger<UpdateProfileImage> logger, DataContext context)
{
    private readonly ILogger<UpdateProfileImage> _logger = logger;
    private readonly DataContext _context = context;

    [Function("UpdateProfileImage")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function,  "post")] HttpRequest req)
    {
      try
        {
            var userProfileModel = req.ReadFromJsonAsync<UserProfileModel>().Result;
            var user = await _context.Users
             .Include(u => u.UserProfile)
             .FirstOrDefaultAsync(u => u.Id == userProfileModel.UserId);

            if (user != null)
            {
                if (user.UserProfile == null)
                {
                    user.UserProfile = new UserProfile();
                }
                user.UserProfile.ProfileImg = userProfileModel.ProfileImg;
                await _context.SaveChangesAsync();
                return new OkResult();

            }
            else
            {
                return new NotFoundResult();
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile image");
            return new BadRequestResult();
        }
    }
}

public class UserProfileModel
{
    public string UserId { get; set; }
    public string ProfileImg { get; set; }
}
