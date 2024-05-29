using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models;
public class UserAdminForm
{
    public string UserId { get; set; } = null!;
    public bool IsAdmin { get; set; }
    public bool IsUser { get; set; }
    public bool IsCIO { get; set; }
    public bool IsSuperAdmin { get; set; }

}
