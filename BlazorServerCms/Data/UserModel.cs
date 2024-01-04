using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorServerCms.Data
{
    public class UserModel : IdentityUser
    {
        private string dominio;
        public string? Image { get; set; }
        public string? Dominio
        {
            get { return dominio; }
            set { value = dominio; Permissao = false; }
        }
        public bool Permissao { get; set; }

        
    }
}
