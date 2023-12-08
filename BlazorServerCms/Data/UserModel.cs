using Microsoft.AspNetCore.Identity;

namespace BlazorServerCms.Data
{
    public class UserModel : IdentityUser
    {
        public string? Image { get; set; }
        public string? Dominio { get; set; }
        public bool Permissao { get; set; }
    }
}
