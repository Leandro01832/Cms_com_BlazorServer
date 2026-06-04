using BlazorServerCms.Data;
using BlazorServerCms.servicos;
using business;
using business.business;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using PSC.Blazor.Components.Tours.Interfaces;
using System.Security.Claims;
using business.business.Book;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.RegularExpressions;
using business.business.Group;
using BlazorServerCms.Pages;

namespace BlazorCms.Client.Pages
{
    public partial class RenderizarBase : ComponentBase
    {
        [Inject] public IStoryService storyService { get; set; }
        [Inject] public RepositoryPagina? repositoryPagina { get; set; }
        [Inject] public ITourService TourService { get; set; }
        [Inject] public NavigationManager? navigation { get; set; }
        [Inject] UserManager<UserModel> userManager { get; set; }
        
        [Inject] BlazorTimer? Timer { get; set; }
        [Inject] protected IJSRuntime? js { get; set; }
        [Inject] AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
        [Parameter] public string? nomeLivro { get; set; } = "";
        [Parameter] public int? capitulo { get; set; } = 1;
        private int indice = 0;
        [Parameter]
         public int Indice 
         { 
            get{ return indice; }
            set
            { 
                indice = value;
              //  if(quantidadeLista != 0)
                
                // else
                //  js!.InvokeVoidAsync("PreencherProgressBar2", 1);
            }
        }
        [Parameter]
         public string Tipo { get; set; }        

        private async void PreencherProgresso()
        {
             try
            {
                int porc = 100 * Indice / quantidadeLista;
                await js!.InvokeAsync<object>("PreencherProgressBar2", porc);
            }
            catch (Exception ex)
            {
                try
                {
                    await js!.InvokeAsync<object>("PreencherProgressBar2", 1);                  
                }
                catch (Exception ex2)
                {
                    Console.WriteLine($"Erro!!!");
                }
                Console.WriteLine($"-------------------------------------|||||||||||||||||||---------------------------------------------");
                Console.WriteLine($"-------------------------------------|||||||||||||||||||---------------------------------------------");
                Console.WriteLine($"-------------------------------------|||||||||||||||||||---------------------------------------------");
                Console.WriteLine($"-------------------------------------|||||||||||||||||||---------------------------------------------");
                Console.WriteLine($"-------------------------------------|||||||||||||||||||---------------------------------------------");
                Console.WriteLine($"-------------------------------------|||||||||||||||||||---------------------------------------------");
                Console.WriteLine($"-------------------------------------|||||||||||||||||||---------------------------------------------");
                Console.WriteLine($"-------------------------------------|||||||||||||||||||---------------------------------------------");
                Console.WriteLine($"-------------------------------------|||||||||||||||||||---------------------------------------------");
                Console.WriteLine($"-------------------------------------|||||||||||||||||||---------------------------------------------");
                Console.WriteLine($"-------------------------------------|||||||||||||||||||---------------------------------------------");
                Console.WriteLine($"-------------------------------------|||||||||||||||||||---------------------------------------------");
                Console.WriteLine($"-------------------------------------|||||||||||||||||||---------------------------------------------");
                Console.WriteLine($"Erro ao preencher progress bar: quantidadeLista = {quantidadeLista}, Indice = {Indice} " + ex.Message);
                Console.WriteLine($"-------------------------------------|||||||||||||||||||---------------------------------------------");
                Console.WriteLine($"-------------------------------------|||||||||||||||||||---------------------------------------------");
                Console.WriteLine($"-------------------------------------|||||||||||||||||||---------------------------------------------");
                Console.WriteLine($"-------------------------------------|||||||||||||||||||---------------------------------------------");
                Console.WriteLine($"-------------------------------------|||||||||||||||||||---------------------------------------------");
                Console.WriteLine($"-------------------------------------|||||||||||||||||||---------------------------------------------");
                Console.WriteLine($"-------------------------------------|||||||||||||||||||---------------------------------------------");
                Console.WriteLine($"-------------------------------------|||||||||||||||||||---------------------------------------------");
                Console.WriteLine($"-------------------------------------|||||||||||||||||||---------------------------------------------");
                Console.WriteLine($"-------------------------------------|||||||||||||||||||---------------------------------------------");
                Console.WriteLine($"-------------------------------------|||||||||||||||||||---------------------------------------------");
                Console.WriteLine($"-------------------------------------|||||||||||||||||||---------------------------------------------");
                Console.WriteLine($"-------------------------------------|||||||||||||||||||---------------------------------------------");
              
            }
        }

        [Parameter] public string? Compartilhou { get; set; } = null;
        [Parameter] public string? rotas { get; set; } = null;
        
        private long? filtro = null;

        private int? versiculo = null;
        [Parameter]
        public int? Versiculo 
        { 
            get { return versiculo; }
            set 
            { 
                versiculo = value;
            }
        }

        [Parameter]
        public long? Filtro
        {
            get { return filtro; }
            set
            {
                var teste = value;
                if (Filtro != value && value != null)
                    perguntar((long)value);
                filtro = value;
                alterouPasta = true;

            }
        }
        private DemoContextFactory db = new DemoContextFactory();
        private ApplicationDbContext Context;
        private int? auto = 0;

        private bool limpar = false;
        
        protected int quantDiv = 0;
        protected int quantDivCriterio = 0;

        protected Criterio criterio = null;

        protected UserModel profile = null;

       // private string typeClass = "Pagina";
        protected Type type = typeof(Pagina);

        private long? _ultimoIdProcessado = null; // Armazena o último ID processado para comparação
        private Story _story = null;
        private Livro? livro = null;
        private string? title = null;
        private string? resumo = null;
        private bool alterouPasta = false;
        protected List<Camada> camadas = null;
        protected List<SubFiltro> listaFiltro = null;
        protected List<SubFiltro> UltimasPastas = null;
        protected List<Type> tipos = null;
        private int indiceChave = 0;
        private int tempoVideo = 0;
     // private List<FiltroContent> result = new List<FiltroContent>();
     // protected List<Content> listaContent = new List<Content>();
        protected List<int> porcentagens = new List<int>();
        public bool AlterouCamada { get; set; }
        private bool alterouModel = true;
        private bool AlterouModel
        {
            get { return alterouModel; }
            set
            {
                if (value && !AlterouCamada)
                    RemoverPlay();
                alterouModel = value;
            }
        }

       
        protected bool showModal = false;
        protected bool showModal2 = false;
        protected int Pasta = 0;

        protected Comment comment = new Comment();

        protected string? id_video = null;
        
        private double Progress { get; set; } = 0;
        
        protected int chave = 0;
        protected int slideAtual = 0;
        protected int slideAtualCriterio = 0;
        protected List<Content>[] array;
        protected List<Filtro>[] array2;
        protected bool tellStory = false;
        protected string inputs = "";
        protected string divPagina = "";
        protected string placeholder = "";
        protected string preferencia = null;
        
        protected int cap = 1;
        protected bool automatico = false;        
        protected string classCss = "";
        protected string DivPag = "";
        protected MarkupString markup;
        protected ElementReference firstInput;
        protected string? Mensagem = null;
        protected string nameGroup = "";
        protected string nameGroup2 = "";
        protected UserModel usuario;
        protected ClaimsPrincipal user;
        protected Match Match;
        protected List<UserPreferencesImage>? usuarios = new List<UserPreferencesImage>();
        protected SubFiltro? Model2;
        protected string opcional = "";
        protected bool liked = false;

        protected Content? Model { get; set; } = null;
        protected string? html { get; set; } = "";
        protected string? nameStory { get; set; } = null;
        protected int quantidadeLista { get; set; } = 0;
        protected int quantidadeFiltro { get; set; } = 0;
        protected bool ultimaPasta { get; set; }
        protected bool condicaoFiltro { get; set; } = false;




        public int retroceder { get; set; } = 0;

        public int timeproduto { get; set; } = 11;

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

     

    }
}
