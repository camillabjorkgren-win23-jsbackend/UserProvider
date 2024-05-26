using Data.Contexts;
using Data.Entities;
using Data.Services;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddScoped<UserService>();
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlServer(Environment.GetEnvironmentVariable("ConnectionString"));
        });

        //services.AddPooledDbContextFactory<DataContext>(x =>
        //{
        //    x.UseSqlServer(Environment.GetEnvironmentVariable("ConnectionString")).UseRootApplicationServiceProvider();
        //});


        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 8;
        })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddSignInManager<SignInManager<ApplicationUser>>()
    .AddUserManager<UserManager<ApplicationUser>>()
    .AddDefaultTokenProviders();

        services.AddAuthorization(x =>
        {
            x.AddPolicy("SuperAdmins", policy => policy.RequireRole("SuperAdmin"));
            x.AddPolicy("CIO", policy => policy.RequireRole("SuperAdmin", "CIO"));
            x.AddPolicy("Admins", policy => policy.RequireRole("SuperAdmin", "CIO", "Admin"));
            x.AddPolicy("Users", policy => policy.RequireRole("SuperAdmin", "CIO", "Admin", "User"));
        });

        services.AddHttpsRedirection(o => o.HttpsPort = 7074);
       
    })

.Build();
//var sp = services.BuildServiceProvider();
//using var scope = sp.CreateScope();
//var dbcontextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<DataContext>>();
//using var context = dbcontextFactory.CreateDbContext();
//context.Database.EnsureCreated();


using (var scope = host.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = ["SuperAdmin", "CIO", "Admin", "User"];
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

host.Run();