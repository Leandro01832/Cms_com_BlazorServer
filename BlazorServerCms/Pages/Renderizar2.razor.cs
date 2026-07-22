using BlazorServerCms.Data;
using BlazorServerCms.servicos;
using business.business.sistema;
using business.business.conteudo;
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
        [Inject] public LiveKitService LiveService { get; set; }
        [Inject] public RepositoryPagina? repositoryPagina { get; set; }
        [Inject] public ITourService TourService { get; set; }
        [Inject] public NavigationManager? navigation { get; set; }
        [Inject] UserManager<UserModel> userManager { get; set; }

        [Inject] BlazorTimer? Timer { get; set; }
        [Inject] protected IJSRuntime? js { get; set; }
        [Inject] AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
        [Parameter] public string? nomeLivro { get; set; } = "";
        [Parameter] public int? capitulo { get; set; } = 1;


        [Parameter] public int Indice { get; set; }

        private async void alterarIndice(int valor)
        {
                Indice = valor;
                SlideAtual = (Indice - 1) / QuantDiv;  
            
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
                Console.WriteLine($"----|||||||||||||||||||----");
                Console.WriteLine($"----|||||||||||||||||||----");
                Console.WriteLine($"----|||||||||||||||||||----");
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
                if (value != null)
                {
                    int count = 0;
                    if (TipoClass != typeof(Baralho))
                        count = CountPagesInFilterAsync((long)Filtro!, livro, TipoClass);
                    else
                    {
                        if (usuario != null)
                        {
                            var arr = usuario.TipoBaralho!.Split(',');
                            foreach (var item in arr)
                            {
                                Type tip = Type.GetType(item.Trim())!;
                                count += CountPagesInFilterAsync((long)Filtro!, livro, tip);
                            }
                        }
                        count += CountPagesInFilterAsync((long)Filtro!, livro, typeof(Page));
                        count += CountPagesInFilterAsync((long)Filtro!, livro, typeof(ProductContent));
                    }
                    var f = listaFiltro.FirstOrDefault(f => f.Id == Filtro);                    

                    Ind = listaFiltro.IndexOf(f);
                    Ind2 = tipos.IndexOf(TipoClass);

                    if (TipoClass == typeof(Link))
                    {
                        var l = listaFiltro.Where(f => f.ComCriterio == f.Id).ToList();
                        count = l.Count;
                    }

                    if (arrayContent[Ind][Ind2] == null)
                        arrayContent[Ind][Ind2] = new long?[count];
                }
            }
        }

        

        private DemoContextFactory db = new DemoContextFactory();
        private ApplicationDbContext Context;
        private int? auto = 0;    

        protected bool carregandoStreaming = true;
        private IJSObjectReference? moduloJs;    

        protected bool larg = false;

        private int quantDiv = 15;
        protected int QuantDiv
        {
            get { return repositoryPagina!.QuantDiv; }
            set
            {
                quantDiv = value;
            }
        }

        private int quantDivCriterio = 6;
        protected int QuantDivCriterio
        {
            get 
            { 
                return repositoryPagina!.QuantDivCriterio;
            }
            set
            {
                quantDivCriterio = value;
                repositoryPagina!.QuantDivCriterio = value;
            }
        }

        protected Criterio criterio = null;

        protected UserModel profile = null;

        private List<Content> contentAdd = new List<Content>();

        private int ind = 0;
        protected int Ind
        {
            get { return ind; }
            set
            {
                ind = value;
                //  var fil = listaFiltro.FirstOrDefault(f => f.Id == Filtro);
                //  buscarRelogio(fil);
            }
        }

        private int ind2 = 0;
        protected int Ind2
        {
            get { return ind2; }
            set
            {
                ind2 = value;
                //  var fil = listaFiltro.FirstOrDefault(f => f.Id == Filtro);
                //  buscarRelogio(fil);
            }
        }

        // Guarda a posição horizontal (X) de onde o toque começou
        private double toqueInicioX;

        // Guarda a posição horizontal (X) de onde o toque terminou
        private double toqueFimX;

        // Distância mínima em pixels para considerar que foi um deslize real e não um clique sem querer
        private const double DistanciaMinimaParaSwipe = 50;

        private Type tipoClass = typeof(Page);

        protected Type TipoClass
        {
            get { return tipoClass; }
            set
            {
                tipoClass = value;
                if (Filtro != null)
                {
                    int count = 0;
                    if (TipoClass != typeof(Baralho))
                        count = CountPagesInFilterAsync((long)Filtro!, livro, value);
                    else
                    {
                        if (usuario != null)
                        {
                            var assemblyDoProjeto = typeof(Content).Assembly;
                            var arr = usuario.TipoBaralho!.Split(',');
                            foreach (var item in arr)
                            {
                                Type tip = assemblyDoProjeto.GetType(item.Trim())!;
                                count += CountPagesInFilterAsync((long)Filtro!, livro, tip);
                            }
                        }
                        count += CountPagesInFilterAsync((long)Filtro!, livro, typeof(Page));
                        count += CountPagesInFilterAsync((long)Filtro!, livro, typeof(ProductContent));
                    }
                    var f = listaFiltro.FirstOrDefault(f => f.Id == Filtro);

                    Ind = listaFiltro.IndexOf(f);
                    Ind2 = tipos.IndexOf(value);

                    if (TipoClass == typeof(Link))
                    {
                        var l = listaFiltro.Where(f => f.ComCriterio == f.Id).ToList();
                        count = l.Count;
                    }

                    if (arrayContent[Ind][Ind2] == null)
                        arrayContent[Ind][Ind2] = new long?[count];
                }

            }
        }

        private long? _ultimoIdProcessado = null; // Armazena o último ID processado para comparação
        private Story _story = null;
        private Livro? livro = null;        
        private bool alterouPasta = false;

        protected long?[][][] arrayContent;
        protected List<Camada> camadas = null;
        protected List<SubFiltro> listaFiltro = null;
        protected List<SubFiltro> UltimasPastas = null;
        protected List<Type> tipos = null;

        private int tempoVideo = 0;
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

        private int slideAtual = 10000;
        protected int SlideAtual
        {
            get
            {
                return slideAtual;
            }
            set
            {
                slideAtual = value;
            }
        }
        protected int slideAtualCriterio = 0;
        protected List<long?>[] array = new List<long?>[1];
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

        private string nameGroup = "";
        protected string NameGroup
        {
            get
            {
                var dom = "";
                if (livro != null)
                    dom = livro.Nome;
                else
                    dom = new Uri(navigation.BaseUri).Host;
                if (Model != null)
                    return nameGroup +
                     $" ({Activator.CreateInstance(tipoClass)!.ToString()}) [{Model.Titulo}] | {dom} ";
                else
                    return nameGroup +
                     $" ({Activator.CreateInstance(tipoClass)!.ToString()}) | {dom} ";
            }
            set { nameGroup = value; }
        }

        protected string nameGroup2 = "";
        protected UserModel usuario;
        protected ClaimsPrincipal user;
        protected Match Match;
        protected List<UserPreferencesImage>? usuarios = new List<UserPreferencesImage>();
        protected SubFiltro? Model2;
        protected string opcional = "";
        protected bool liked = false;

        private Content? model = null;
        protected Content? Model
        {
            get { return model; }
            set
            {
                model = value;
                if (model != null)
                    SetModelAsync(value);

            }
        }

        protected Content? Comment{get;set;}

        private async Task<string> setarHtml(Content c)
        {
            return await repositoryPagina!.renderizarPagina(c);
        }

        private async Task SetModelAsync(Content? value)
        {

            if (value != null)
            {
                Html = await setarHtml(value);
            }
        }

        private string? html = "";
        protected string? Html
        {
            get { return html; }
            set
            {
                html = value;
                markup = new MarkupString(value);
              //  var c = Model.Comentario.First(m => m.ContentId == Model.Id);
            }
        }

        

        protected string? nameStory { get; set; } = null;
        protected int quantidadeLista { get; set; } = 0;
        protected int quantidadeFiltro { get; set; } = 0;
        protected bool ultimaPasta { get; set; }
        protected bool condicaoFiltro { get; set; } = false;


        public int retroceder { get; set; } = 0;

        public int timeproduto { get; set; } = 11;

        public int? carregando { get; set; } = 35;
        public bool carregou = false;

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
