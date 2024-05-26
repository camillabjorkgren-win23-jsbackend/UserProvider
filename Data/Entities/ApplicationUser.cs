using Microsoft.AspNetCore.Identity;

namespace Data.Entities;
public class ApplicationUser : IdentityUser
{
    public string? UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }

    public DateTime? Created { get; set; }
    public DateTime? Modified { get; set; }

    public string? UserAddressId { get; set; }
    public UserAddress? UserAddress { get; set; }
    public string? PreferredEmail { get; set; }
    public bool SubscribeNewsletter { get; set; } = false;
    public bool DarkMode { get; set; } = false;
}

public class UserAddress
{
    public string Id { get; set; }
    public string AddressType { get; set; } = null!;
    public string AddressLine_1 { get; set; } = null!;
    public string? AddressLine_2 { get; set; }

    public string PostalCode { get; set; } = null!;
    public string City { get; set; } = null!;
}

public class UserProfile
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public string? ProfileImg { get; set; }
    public string? Bio { get; set; }
}
