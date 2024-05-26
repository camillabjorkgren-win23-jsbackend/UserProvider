using Data.Contexts;
using Data.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace UserProvider.Functions;

public class GetUserAndRoles(ILogger<GetUserAndRoles> logger, UserService userService)
{
    private readonly ILogger<GetUserAndRoles> _logger = logger;
    private readonly UserService _userService = userService;

    [Function("GetUserAndRoles")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        try
        {
            var body = new StreamReader(req.Body).ReadToEnd();
            var data = JsonConvert.DeserializeObject<UserId>(body);

            var user = await _userService.GetUserByIdAsync(data.userId);
            if (user == null)
            {
                var userRoles = await _userService.GetRolesAsync(user.Id);

                var userAdminDisplay = new UserAdminDisplay
                {
                    FirstName = user.UserProfile.FirstName,
                    LastName = user.UserProfile.LastName,
                    Email = user.Email,
                    Roles = userRoles
                };
                return new OkObjectResult(userAdminDisplay);

            }
            return new NotFoundResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new BadRequestResult();
        }
    }
}

public class UserAdminDisplay
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public IList<string> Roles { get; set; } = new List<string>();

}

public class UserId
{
    public string userId { get; set; } = null!;
}