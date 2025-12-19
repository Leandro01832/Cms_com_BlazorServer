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
using business.business.Book;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
        [Parameter] public string? nomeLivro { get; set; } = "";
        [Parameter] public int? storyid { get; set; } = 1;
        [Parameter] public int Indice { get; set; } = 1;
        [Parameter] public string? Compartilhou { get; set; } = "comp";
        [Parameter] public string? rotas { get; set; } = null;
        [Parameter] public long? contentid { get; set; } = null;
        private long? filtro = null;
        [Parameter]
        public long? Filtro
        {
            get { return filtro; }
            set
            {
                if ( Filtro != value && value != null)                
                perguntar((long)value);
                filtro = value;
                alterouPasta = true;
                
            }
        }
        private DemoContextFactory db = new DemoContextFactory();
        private ApplicationDbContext Context;
        // private bool _executado = false;
        private int? auto = 0;
        private int quantidadePaginas = 0;
        protected int quantDiv = 0;
        private Story _story = null;
        private Livro? livro = null;
        private string? title = null;
        private string? resumo = null;
        private bool alterouPasta = false;
        protected List<SubFiltro> listaFiltro = null;
        private int indiceChave = 0;
        private int tempoVideo = 0;
        private List<FiltroContent> result = new List<FiltroContent>();
        protected List<int> porcentagens = new List<int>();
        public bool AlterouCamada { get; set; }
        private bool alterouModel = true;
        private bool AlterouModel 
        {
            get{return alterouModel;}
            set
            {
                if(value && !AlterouCamada)
                RemoverPlay();
                alterouModel = value;
            }
        }

        protected bool InfoSemCriterio = false;
        protected bool showModal = false;

        protected string? id_video = null;
        protected int indiceAcesso;
        private double Progress { get; set; } = 0;
        private bool criptografar = false;
        protected int chave = 0;
        protected int Versiculo = 0;
        protected int slideAtual = 0;
        protected List<Content>[] array;
        protected List<Content> listaContent = new List<Content>();
        protected bool tellStory = false;
        protected string inputs = "";
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
        protected SubFiltro? Model2;
        protected string[]? classificacoes = null;
        protected string opcional = "";
        protected bool liked = false;
        protected bool Content = false;        
        
        protected Content? Model { get; set; } = null;
        protected string? html { get; set; } = "";
        protected string? nameStory { get; set; } = null;
        protected int? CapituloComentario { get; set; } = null;
        protected int? VersoComentario { get; set; } = null;
        protected int quantidadeLista { get; set; } = 0;
        protected bool ultimaPasta { get; set; }
        protected bool condicaoFiltro { get; set; } = false;
        

        protected int? indice_Filtro { get; set; }
        protected int? vers { get; set; }

        public int retroceder { get; set; } = 0;

        public int timeproduto { get; set; } = 11;
        public int? conteudo { get; set; } = 1;
       
        public int? carregando { get; set; } = 40;

        public int? Auto
        {
            get { return auto; }
            set
            {
                if (value == 1)
                    habilitarAuto();
                else
                    desabilitarAuto();
                auto = value;
            }

        }

        public string? redirecionar { get; set; } = "";
        public string? dominio { get; set; } = "dominio";        

        protected int outroHorizonte { get; set; }

    }
}
