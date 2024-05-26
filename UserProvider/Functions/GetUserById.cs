using Data.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace UserProvider.Functions;

public class GetUserById(ILogger<GetUserById> logger, DataContext context)
{
    private readonly ILogger<GetUserById> _logger = logger;
    private readonly DataContext _context = context;

    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        try
        {
            var userId = req.Query["userId"];

            var user = await _context.Users
                  .Include(u => u.UserProfile)
                  .Include(u => u.UserAddress)
                  .FirstOrDefaultAsync(u => u.Id == userId);
            return new OkObjectResult(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by id");
            return new BadRequestObjectResult("Error getting user by id");
        }
    }
}
