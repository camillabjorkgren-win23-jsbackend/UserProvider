using Data.Contexts;
using Data.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace UserProvider.Functions;

public class GetUsersWithRoles
{
    private readonly ILogger<GetUsersWithRoles> _logger;
    private readonly UserService _userService;
    private readonly DataContext _context;
    public GetUsersWithRoles(ILogger<GetUsersWithRoles> logger, UserService userService, DataContext context)
    {
        _logger = logger;
        _userService = userService;
        _context = context;
    }

    [Function("GetUsersWithRoles")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        try
        {
            var users = await _context.Users
              .Include(u => u.UserProfile)
              .ToListAsync();

            var result = await _userService.GetAllUsersWithRolesAsync(users);
            return new OkObjectResult(result);

            //return new OkObjectResult(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new BadRequestResult();
        }
        return new NotFoundResult();
    }
}
