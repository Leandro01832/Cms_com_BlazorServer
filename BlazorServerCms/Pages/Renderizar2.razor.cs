﻿using BlazorServerCms.Data;
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

        private string comp1 = "";
        private string comp2 = "";
        private string comp3 = "";
        private string comp4 = "";
        private string comp5 = "";
        private string comp6 = "";
        private string comp7 = "";

        private bool criptografar = true;

        protected int indiceAcesso;

        
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
        [Parameter] public string? redirecionar { get; set; } = "";
        [Parameter] public string? dominio { get; set; } = "dominio";

        private string? compartilhou = null; 
        [Parameter] public string? Compartilhou 
        {
            get { return compartilhou; }
            set
            {
                compartilhou = value;
                criptografar = true;
            }
        }

        private string? compartilhante = null;
        [Parameter] public string? Compartilhante 
        {
            get { return compartilhante; }
            set 
            {
                compartilhante = value;
                criptografar = true;
            }
        }

        private string? compartilhante2 = null;
        [Parameter] public string? Compartilhante2
        {
            get { return compartilhante2; }
            set
            {
                compartilhante2 = value;
                criptografar = true;
            }
        }

        private string? compartilhante3 = null;
        [Parameter] public string? Compartilhante3
        {
            get { return compartilhante3; }
            set
            {
                compartilhante3 = value;
                criptografar = true;
            }
        }

        private string? compartilhante4 = null;
        [Parameter] public string? Compartilhante4
        {
            get { return compartilhante4; }
            set
            {
                compartilhante4 = value;
                criptografar = true;
            }
        }

        private string? compartilhante5 = null;
        [Parameter] public string? Compartilhante5
        {
            get { return compartilhante5; }
            set
            {
                compartilhante5 = value;
                criptografar = true;
            }
        }

        private string? compartilhante6 = null;
        [Parameter] public string? Compartilhante6
        {
            get { return compartilhante6; }
            set
            {
                compartilhante6 = value;
                criptografar = true;
            }
        }


        [Parameter] public string? pontos { get; set; } = "";

        [Parameter] public int outroHorizonte { get; set; }

        [Parameter] public string? filtrar { get; set; } = null;
        [Parameter] public string? rotas { get; set; } = null;
        

    }
}
