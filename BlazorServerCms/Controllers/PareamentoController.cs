
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
        public PareamentServices Servico { get; }

        public PareamentoController(PareamentServices servico)
        {
            Servico = servico;
        }

        [HttpGet]
        public void Get([FromQuery] string dominio, int capitulo, int indiceFiltro, int preferencia)
        {

            Servico.parearDominio = dominio;
            Servico.parearCapitulo = capitulo;
            Servico.parearIndice = indiceFiltro;
            Servico.parearPreferencia = preferencia;
        }

       
    }
}