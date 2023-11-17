using Microsoft.AspNetCore.Identity;

namespace BlazorServerCms.Data
{
    public class UserModel : IdentityUser
    {
        public string? Image { get; set; }
    }
}
