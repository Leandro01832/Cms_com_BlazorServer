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
        private Filtro fil2 = null;
        private Filtro fil3 = null;
        private Filtro fil4 = null;
        private Filtro fil5 = null;

        protected int indiceAcesso;

        private bool criptografar = false;

        protected string preferencia = null;
        
        protected bool exemplo = false;
        protected bool mudanca = false;
        protected int cap = 1;
        protected bool automatico = false;

        protected int padroes = 0;
        protected string classCss = "";
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
                    if (auto == 0 && Timer.desligarAuto == null )
                    habilitarAuto();
                    else
                    desabilitarAuto();
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
                .FirstOrDefault(u => u.HashUserName != null &&
                u.HashUserName == compartilhou &&
                Decrypt(u.HashUserName, u.UserName));

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
                var user = repositoryPagina.UserModel
                .FirstOrDefault(u => u.UserName == value);
                if (user != null && user.HashUserName == null)
                {
                    compartilhou = Encrypt(user.UserName);
                    user.HashUserName = compartilhou;
                    Context.Update(user);
                }
                else if (user != null) compartilhou = user.HashUserName;
                else compartilhou = "comp";
            }
        }

        private string? compartilhante = null;
        [Parameter] public string? Compartilhante 
        {
            get
            {
                var user = repositoryPagina.UserModel
                .FirstOrDefault(u => u.HashUserName != null &&
                u.HashUserName == compartilhante &&
                Decrypt(u.HashUserName, u.UserName));

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
                var user = repositoryPagina.UserModel
                .FirstOrDefault(u => u.UserName == value);
                if (user != null && user.HashUserName == null)
                {
                    compartilhante = Encrypt(user.UserName);
                    user.HashUserName = compartilhante;
                    Context.Update(user);
                }
                else if (user != null) compartilhante = user.HashUserName;
                else compartilhante = "comp";
            }
        }

        private string? compartilhante2 = null;
        [Parameter] public string? Compartilhante2
        {
            get 
            {
                    var user = repositoryPagina.UserModel
               .FirstOrDefault(u => u.HashUserName != null &&
               u.HashUserName == compartilhante2 &&
               Decrypt(u.HashUserName, u.UserName));

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
                var user = repositoryPagina.UserModel
                .FirstOrDefault(u => u.UserName == value);
                if (user != null && user.HashUserName == null)
                {
                    compartilhante2 = Encrypt(user.UserName);
                    user.HashUserName = compartilhante2;
                    Context.Update(user);
                }
                else if (user != null) compartilhante2 = user.HashUserName;
                else compartilhante2 = "comp";
            }
        }

        private string? compartilhante3 = null;
        [Parameter] public string? Compartilhante3
        {
            get 
            {
                var user = repositoryPagina.UserModel
                .FirstOrDefault(u => u.HashUserName != null &&
                u.HashUserName == compartilhante3 &&
                Decrypt(u.HashUserName, u.UserName));

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
                var user = repositoryPagina.UserModel
                .FirstOrDefault(u => u.UserName == value);
                if (user != null && user.HashUserName == null)
                {
                    compartilhante3 = Encrypt(user.UserName);
                    user.HashUserName = compartilhante3;
                    Context.Update(user);
                }
                else if (user != null) compartilhante3 = user.HashUserName;
                else compartilhante3 = "comp";
            }
        }

        private string? compartilhante4 = null;
        [Parameter] public string? Compartilhante4
        {
            get 
            {
                var user = repositoryPagina.UserModel
                .FirstOrDefault(u => u.HashUserName != null &&
                u.HashUserName == compartilhante4 &&
                Decrypt(u.HashUserName, u.UserName));

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
                var user = repositoryPagina.UserModel
                .FirstOrDefault(u => u.UserName == value);
                if (user != null && user.HashUserName == null)
                {
                    compartilhante4 = Encrypt(user.UserName);
                    user.HashUserName = compartilhante4;
                    Context.Update(user);
                }
                else if (user != null) compartilhante4 = user.HashUserName;
                else compartilhante4 = "comp";
            }
        }

        private string? compartilhante5 = null;
        [Parameter] public string? Compartilhante5
        {
            get 
            {
                var user = repositoryPagina.UserModel
               .FirstOrDefault(u => u.HashUserName != null &&
               u.HashUserName == compartilhante5 &&
               Decrypt(u.HashUserName, u.UserName));

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
                var user = repositoryPagina.UserModel
                .FirstOrDefault(u => u.UserName == value);
                if (user != null && user.HashUserName == null)
                {
                    compartilhante5 = Encrypt(user.UserName);
                    user.HashUserName = compartilhante5;
                    Context.Update(user);
                }
                else if (user != null) compartilhante5 = user.HashUserName;
                else compartilhante5 = "comp";
            }
        }

        private string? compartilhante6 = null;
        [Parameter] public string? Compartilhante6
        {
            get 
            {
                var user = repositoryPagina.UserModel
                .FirstOrDefault(u => u.HashUserName != null &&
                u.HashUserName == compartilhante6 &&
                Decrypt(u.HashUserName, u.UserName));

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
                var user = repositoryPagina.UserModel
                .FirstOrDefault(u => u.UserName == value);
                if (user != null && user.HashUserName == null)
                {
                    compartilhante6 = Encrypt(user.UserName);
                    user.HashUserName = compartilhante6;
                    Context.Update(user);
                }
                else if (user != null) compartilhante6 = user.HashUserName;
                else compartilhante6 = "comp";
            }
        }


        [Parameter] public string? pontos { get; set; } = "";

        [Parameter] public int outroHorizonte { get; set; }

        [Parameter] public string? filtrar { get; set; } = null;
        [Parameter] public string? rotas { get; set; } = null;
        

    }
}
