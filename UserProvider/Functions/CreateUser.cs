using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace UserProvider.Functions
{
    public class CreateUser
    {
        private readonly ILogger<CreateUser> _logger;

        public CreateUser(ILogger<CreateUser> logger)
        {
            _logger = logger;
        }

        [Function("CreateUser")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            // Skapa en user via UserManager? Eller addera roller till denna?
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
