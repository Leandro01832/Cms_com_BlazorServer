
using BlazorServerCms.Data;
using BlazorServerCms.servicos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor_Catalogo.Server.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class PareamentoController : ControllerBase
    {
        public ApplicationDbContext Context { get; }

        public PareamentoController( ApplicationDbContext context)
        {
            Context = context;
        }

        [HttpGet]
        public async void Get([FromQuery] string email, string cupom)
        {
            var comp = await Context.Compartilhante.FirstOrDefaultAsync(u => u.Admin == email);

            if(comp != null)
            {                
                comp.CupomDesconto = cupom;
                Context.Update(comp);
               await  Context.SaveChangesAsync();
            }            
        }
    }
}