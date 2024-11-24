using BlazorServerCms.Data;
using BlazorServerCms.servicos;
using business;
using business.business;
using business.Group;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Models;
using PSC.Blazor.Components.Tours.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Policy;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

namespace BlazorCms.Client.Pages
{
    public partial class RenderizarBase : ComponentBase
    {
        [Inject] public RepositoryPagina? repositoryPagina { get; set; }
        [Inject] public ITourService TourService { get; set; }
        [Inject] public NavigationManager? navigation { get; set; }
        [Inject] UserManager<UserModel> userManager { get; set; }
        [Inject] HttpClient? Http { get; set; }
        [Inject] BlazorTimer? Timer { get; set; }
        [Inject] IJSRuntime? js { get; set; }
        [Inject] AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
        public ClassArray Arr = new ClassArray();
        private DemoContextFactory db = new DemoContextFactory();
        private ApplicationDbContext Context;
        private int? auto = 1;
        private int? indiceMarcador;
        private Story story = null;
        private string? title = null;
        private string? resumo = null;
        protected int indiceAcesso;

        
        protected bool exemplo = false;
        protected int cap = 1;
        protected bool automatico = false;

        protected MarkupString markup;
        protected ElementReference firstInput;
        protected string? Mensagem = null;
        protected string nameGroup = "";
        protected UserModel usuario;
        protected ClaimsPrincipal user;
        protected List<UserPreferencesImage>? usuarios = new List<UserPreferencesImage>();
        protected Content? pag;
        protected Pagina? Model = new Pagina();
        protected Filtro? Model2;
        protected string[]? classificacoes = null;
        protected string opcional = "";
        protected bool liked = false;       
        protected bool Content = false;       
        protected long quantLiked = 0;

        protected string? html { get; set; } = "";
        protected string? nameStory { get; set; } = null;
        protected int? CapituloComentario { get; set; } = null;
        protected int? VersoComentario { get; set; } = null;        
        protected int quantidadeLista { get; set; } = 0;
        protected bool ultimaPasta { get; set; }
         protected bool condicaoFiltro { get; set; } = false;
               
        protected int? indice_Filtro { get; set; }         
        protected int? vers { get; set; }

        [Parameter] public int indiceLivro { get; set; } = 0;
        [Parameter] public int retroceder { get; set; } = 0;

        [Parameter] public int timeproduto { get; set; } = 11;  
        [Parameter] public int? conteudo { get; set; } = 1;  
        [Parameter] public int indice { get; set; } = 1;
        [Parameter] public int capitulo { get; set; } = 1;

        [Parameter] public int? substory { get; set; } = null; [Parameter] public int? grupo { get; set; } = null;

        [Parameter] public int? subgrupo { get; set; } = null; [Parameter] public int? subsubgrupo { get; set; } = null;

        [Parameter] public int? camadaseis { get; set; } = null; [Parameter] public int? camadasete { get; set; } = null;

        [Parameter] public int? camadaoito { get; set; } = null; [Parameter] public int? camadanove { get; set; } = null;

        [Parameter] public int? camadadez { get; set; } = null;
        [Parameter] public int? Auto
        {
            get { return auto; }
            set { 
                    auto = value;
                    if (auto == 1 && Timer.desligarAuto == null )
                    habilitarAuto();
                    else
                    desabilitarAuto();
                }
        
        } 

        [Parameter] public string? redirecionar { get; set; } = null; 
        [Parameter] public string? dominio { get; set; } = "dominio";

        [Parameter] public string? compartilhou { get; set; } = null;
        [Parameter] public string? compartilhante { get; set; } = null;
        [Parameter] public string? compartilhante2 { get; set; } = null;
        [Parameter] public string? compartilhante3 { get; set; } = null;
        [Parameter] public string? compartilhante4 { get; set; } = null;
        [Parameter] public string? compartilhante5 { get; set; } = null;
        [Parameter] public string? compartilhante6 { get; set; } = null;
        [Parameter] public string? pontos { get; set; } = "";

        [Parameter] public int outroHorizonte { get; set; }

        [Parameter] public string? filtrar { get; set; } = null;
        [Parameter] public string? rotas { get; set; } = null;
        

    }
}
