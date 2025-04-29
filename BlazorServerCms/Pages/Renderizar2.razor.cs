using BlazorServerCms.Data;
using BlazorServerCms.servicos;
using business;
using business.business;
using business.Group;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using PSC.Blazor.Components.Tours.Interfaces;
using System.Security.Claims;

namespace BlazorCms.Client.Pages
{
    public partial class RenderizarBase : ComponentBase
    {
        [Inject] public IStoryService storyService { get; set; }
        [Inject] public RepositoryPagina? repositoryPagina { get; set; }
        [Inject] public ITourService TourService { get; set; }
        [Inject] public NavigationManager? navigation { get; set; }
        [Inject] UserManager<UserModel> userManager { get; set; }
        [Inject] HttpClient? Http { get; set; }
        [Inject] BlazorTimer? Timer { get; set; }
        [Inject] protected IJSRuntime? js { get; set; }
        [Inject] AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
        
        private DemoContextFactory db = new DemoContextFactory();
        private ApplicationDbContext Context;
        private int? auto = 0;
        private int quantDiv = 0;
        private int? indiceMarcador;
        private Story story = null;
        private string? title = null;
        private string? resumo = null;
        private Filtro fil2 = null;
        private Filtro fil3 = null;
        private Filtro fil4 = null;
        private Filtro fil5 = null;
        private bool alterouPasta;

        protected int indiceAcesso;

        private bool criptografar = false;

        protected int slideAtual = 0;
        protected List<Content>[] array;
        protected List<Content> listaContent = new List<Content>();
        protected bool tellStory = true;
        protected string divPagina = "";
        protected string placeholder = "";
        protected string preferencia = null;
        
        protected bool exemplo = false;
        protected bool mudanca = false;
        protected int cap = 1;
        protected bool automatico = false;

        protected int padroes = 0;
        protected bool erro = false;
        protected string classCss = "";
        protected string DivPag = "";
        protected MarkupString markup;
        protected ElementReference firstInput;
        protected string? Mensagem = null;
        protected string nameGroup = "";
        protected UserModel usuario;
        protected ClaimsPrincipal user;
        protected List<UserPreferencesImage>? usuarios = new List<UserPreferencesImage>();
        protected Content? Model = null;
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
        [Parameter] public long? storyid { get; set; } = 1;

        private long? filtro = null;
        [Parameter] public long? Filtro 
        {
            get { return filtro; }
            set 
            { 
                if(value != Filtro)
                alterouPasta = true;
                filtro = value;
            }
        } 

        [Parameter] public int? Auto
        {
            get { return auto; }
            set { 
                    if (auto == 0)
                    habilitarAuto();
                    else
                    desabilitarAuto();

                    if (auto == null)
                        auto = 0;
                    else
                        auto = value;
                }
        
        } 
      
        [Parameter] public string? redirecionar { get; set; } = "";
        [Parameter] public string? dominio { get; set; } = "dominio";

        private string? compartilhou = null; 
        [Parameter] public string? Compartilhou 
        {
            get 
            { 
                var user = repositoryPagina.UserModel
                .FirstOrDefault(u => u.HashUserName == compartilhou);

                    if (!criptografar)
                    {
                        if (user != null)
                            return user.UserName;
                        else return "comp";

                    }
                else if (user != null) return user.HashUserName;
                else return "comp";
            }
            set
            {
                compartilhou = value;
            }
        }

        [Parameter] public long? Compartilhante { get; set; } = 0;
        [Parameter] public long? Compartilhante2 { get; set; } = 0;
        [Parameter] public long? Compartilhante3 { get; set; } = 0;
        [Parameter] public long? Compartilhante4 { get; set; } = 0;
        [Parameter] public long? Compartilhante5 { get; set; } = 0;
        [Parameter] public long? Compartilhante6 { get; set; } = 0;
        [Parameter] public long? Compartilhante7 { get; set; } = 0;
        [Parameter] public long? Compartilhante8 { get; set; } = 0;
        [Parameter] public long? Compartilhante9 { get; set; } = 0;
        [Parameter] public long? Compartilhante10 { get; set; } = 0;


        [Parameter] public string? pontos { get; set; } = "";

        [Parameter] public int outroHorizonte { get; set; }

        [Parameter] public string? filtrar { get; set; } = null;
        [Parameter] public string? rotas { get; set; } = null;
        

    }
}
