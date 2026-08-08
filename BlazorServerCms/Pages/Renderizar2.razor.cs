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
using System.Text.RegularExpressions;
using business.business.Group;
using Microsoft.EntityFrameworkCore;

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

        [Parameter]
        public string Tipo { get; set; }

        [Parameter] public string? Compartilhou { get; set; } = null;
       
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
                    if (TipoClass != typeof(Baralho) && TipoClass != typeof(BaralhoHashTag))
                        count = CountPagesInFilterAsync((long)Filtro!, livro, TipoClass);
                    else if (TipoClass == typeof(BaralhoHashTag))
                        count = listaHashtag.Count;
                    else
                    {
                        if (usuario != null)
                        {
                            string[]? arr = null;
                             if(usuario.TipoBaralho != null)
                             arr = usuario.TipoBaralho!.Split(',');
                             if(arr != null)
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

         private long? filtro = null;
        private IJSObjectReference? moduloJs;
        private int? auto = 0;
        private int quantDivCriterio = 6;
        private int ind = 0;

        private int? versiculo = null;
        private int ind2 = 0;
        private int quantDiv = 15;
        private List<Content> contentAdd = new List<Content>();
        private DemoContextFactory db = new DemoContextFactory();

        private ApplicationDbContext Context;
        // Guarda a posição horizontal (X) de onde o toque começou
        private double toqueInicioX;

        // Guarda a posição horizontal (X) de onde o toque terminou
        private double toqueFimX;

        // Distância mínima em pixels para considerar que foi um deslize real e não um clique sem querer
        private const double DistanciaMinimaParaSwipe = 50;
        private int indiceAnterior;
        private Type tipoAnterior = null;
        private Type tipoClass = typeof(Page);
        private long? _ultimoIdProcessado = null; // Armazena o último ID processado para comparação
        private Story _story = null;
        private Livro? livro = null;
        private bool alterouPasta = false;
        private long? hashtagId = null;
        private double Progress { get; set; } = 0;
        private int slideAtual = 10000;
        private string nameGroup = "";
        private bool alterouModel = true;
        private int tempoVideo = 0;
        private Content? model = null;
        private string? html = "";

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
        public bool OcultarMenu { get; set; } = true;
        public bool HashTag { get; set; } = false;
        public bool AlterouCamada { get; set; }
        protected bool carregandoStreaming = true;

        protected int QuantDiv
        {
            get { return repositoryPagina!.QuantDiv; }
            set
            {
                quantDiv = value;
            }
        }

        protected string contentCSS = "";
        protected bool larg = false;
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

        protected Type TipoClass
        {
            get { return tipoClass; }
            set
            {
                tipoClass = value;
                if (Filtro != null)
                {
                    int count = 0;
                    if (TipoClass != typeof(Baralho) && TipoClass != typeof(BaralhoHashTag))
                        count = CountPagesInFilterAsync((long)Filtro!, livro, value);
                    else if (TipoClass == typeof(BaralhoHashTag))
                        count = listaHashtag.Count;
                    else
                    {
                        if (usuario != null)
                        {
                            var assemblyDoProjeto = typeof(Content).Assembly;
                            string[]? arr = null;
                             if(usuario.TipoBaralho != null)
                             arr = usuario.TipoBaralho!.Split(',');
                             if(arr != null)
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

        protected long?[][][] arrayContent;
        protected List<Camada> camadas = null;
        protected List<SubFiltro> listaFiltro = null;
        protected List<SubFiltro> UltimasPastas = null;
        protected List<Type> tipos = null;

        protected Hashtag hashtag = null;

        
        protected long? HashtagId
        {
            get { return hashtagId; }
            set
            {
                hashtagId = value;

                hashtag = Context.Hashtag
                    .Include(u => u.HashtagContent)
                    .FirstOrDefault(u => u.Id == value)!;

                preencherListaHashtag();
            }
        }

        private async void preencherListaHashtag()
        {
            listaHashtag.Clear();
            foreach (var item in hashtag.HashtagContent.OrderBy(hc => hc.Data).ToList())
            {
                if (await Context.FiltroContent.AnyAsync(fc => fc.FiltroId == Filtro
                && fc.ContentId == item.ContentId))
                {
                    listaHashtag.Add(item.ContentId);
                    StateHasChanged();
                }

            }
        }

        
        protected List<long> listaHashtag = new List<long>();
        protected List<int> porcentagens = new List<int>();
        
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
        protected bool showModal3 = false;

        protected int Pasta = 0;

        protected Comment comment = new Comment();

        protected string? id_video = null;
        
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
        protected List<SubFiltro>[] array2;
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

        protected Content? Comment { get; set; }
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
    
        private async void alterarIndice(int valor)
        {
            Indice = valor;
            SlideAtual = (Indice - 1) / QuantDiv;

        }
    }
}
