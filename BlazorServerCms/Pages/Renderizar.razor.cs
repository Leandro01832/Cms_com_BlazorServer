using BlazorServerCms.Data;
using BlazorServerCms.Migrations;
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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace BlazorCms.Client.Pages
{
    public class RenderizarBase : ComponentBase
    {
        [Inject] public RepositoryPagina? repositoryPagina { get; set; }
        [Inject] public NavigationManager? navigation { get; set; }
        [Inject] HttpClient? Http { get; set; }
        [Inject] BlazorTimer? Timer { get; set; }
        [Inject] IJSRuntime? js { get; set; }
        [Inject] AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
        public ClassArray Arr = new ClassArray();
        private DemoContextFactory db = new DemoContextFactory();
        private ApplicationDbContext Context;

        protected MarkupString markup;
        protected ElementReference firstInput;
        protected string? Mensagem = null;
        protected string nameGroup = "";

        protected UserPreferences preferences = null;
        protected ClaimsPrincipal user;
        protected List<Story>? stories;
        protected List<Pagina>? listaFiltradaComConteudo = null;
        protected List<UserPreferences>? usuarios = new List<UserPreferences>();
        protected Pagina? Model;
        protected Filtro? Model2;
        protected string[]? classificacoes = null;
        protected string opcional = "";
        protected bool liked = false;       
        protected long quantLiked = 0;       

        protected string? html { get; set; } = "";
        protected string? nameStory { get; set; } = null;
        protected int? indice_Filtro { get; set; } = null;
        protected int? vers { get; set; } = null;
        protected int? CapituloComentario { get; set; } = null;
        protected int? VersoComentario { get; set; } = null;
        protected int quantidadePaginas { get; set; }
        protected int quantidadeFiltros { get; set; }
        protected int quantidadeLista { get; set; }
        protected int anterior { get; set; }
        protected int proximo { get; set; }
        protected int prefCapitulo { get; set; }
        protected int prefPasta { get; set; }
        protected string prefCompartilhante { get; set; }
        protected long ultimaPasta { get; set; }
        protected List<Pagina> list = null;

        [Parameter] public int indiceLivro { get; set; } = 0; [Parameter] public int retroceder { get; set; } = 0;

        [Parameter] public int lista { get; set; } = 1; [Parameter] public int preferencia { get; set; } = 0;
        [Parameter] public int timeproduto { get; set; } = 1;   [Parameter] public int indice { get; set; } = 1;
        [Parameter] public int capitulo { get; set; } = 1;

        [Parameter] public int? substory { get; set; } = null; [Parameter] public int? grupo { get; set; } = null;

        [Parameter] public int? subgrupo { get; set; } = null; [Parameter] public int? subsubgrupo { get; set; } = null;

        [Parameter] public int? camadaseis { get; set; } = null; [Parameter] public int? camadasete { get; set; } = null;

        [Parameter] public int? camadaoito { get; set; } = null; [Parameter] public int? camadanove { get; set; } = null;

        [Parameter] public int? camadadez { get; set; } = null; [Parameter] public int auto { get; set; } = 1;

        [Parameter] public string? redirecionar { get; set; } = null; [Parameter] public string? dominio { get; set; } = "dominio";
        [Parameter] public string? compartilhante { get; set; } = "dominio";
        [Parameter] public string? compartilhante2 { get; set; } = "user";

        [Parameter] public int outroHorizonte { get; set; }

        [Parameter] public string? filtrar { get; set; } = null;
        [Parameter] public int? p1 { get; set; } = 0;
        [Parameter] public int? p2 { get; set; } = 0;
        [Parameter] public int? p3 { get; set; } = 0;
        [Parameter] public int? p4 { get; set; } = 0;
        [Parameter] public int? p5 { get; set; } = 0;
        [Parameter] public int? p6 { get; set; } = 0;
        [Parameter] public int? p7 { get; set; } = 0;
        [Parameter] public int? p8 { get; set; } = 0;
        [Parameter] public int? p9 { get; set; } = 0;
        [Parameter] public int? p10 { get; set; } = 0;

        protected override async Task OnParametersSetAsync()
        {

                if (Context.Pagina!.Include(p => p.Story)
                .Where(p => p.Story!.PaginaPadraoLink == capitulo).ToList().Count == 0
                && capitulo > stories!.Last().PaginaPadraoLink)            
                capitulo = 1;

            await renderizar();

                if(prefCapitulo != capitulo && substory != null || prefPasta != indice_Filtro && substory != null ||
                prefCompartilhante != compartilhante2 && substory != null)
            {

                   preferences = repositoryPagina!.preferencias!
                   .FirstOrDefault(u => u.user == compartilhante2 && u.capitulo == capitulo &&
                   u.pasta == indice_Filtro)!;
                if(indice_Filtro != null)
                {
                    prefCapitulo = capitulo;
                    prefPasta = (int) indice_Filtro!;
                    prefCompartilhante = compartilhante2!;
                }
            }

            if (Context.Pagina!.Include(p => p.Story)
                .Where(p => p.Story!.PaginaPadraoLink == capitulo).ToList().Count > 0
                && auto == 1)
                    StartTimer(Model!);

            try
            {
                if (indice > quantidadeLista)
                        await js!.InvokeAsync<object>("MarcarIndice", "1");
                else
                        await js!.InvokeAsync<object>("MarcarIndice", $"{indice}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            Context = db.CreateDbContext(null);
           
            Model = new Pagina
            {
                Story = new Story(),

            };
            vers = null;

            var authState = await AuthenticationStateProvider
                .GetAuthenticationStateAsync();
            user = authState.User;

            if (compartilhante == null)
            compartilhante = repositoryPagina!.buscarAdmin();
            if (compartilhante2 == null)
                compartilhante2 = "user";
            if (dominio == null)
                dominio = repositoryPagina!.buscarDominio();

            if (auto == 0 && Timer!.desligarAuto! != null
                && Timer!.desligarAuto!.Enabled == true)
            {
                Timer!.desligarAuto!.Elapsed -= desligarAuto_Elapsed;
                Timer!.desligarAuto!.Enabled = false;
                Timer.desligarAuto.Dispose();
            }
            if (indice == 0)
                indice = 1;
            if (timeproduto == 0)
                timeproduto = 11;


            stories = await Context.Story!.Where(st => st.Nome != "Padrao").OrderBy(st => st.Id).ToListAsync();            

        }

        protected async Task renderizar()
        {
                List<Filtro> filtros = null;
           
            if (repositoryPagina!.paginas!
                .Where(p => p.Story!.PaginaPadraoLink == capitulo).OrderBy(f => f.Id).ToList().Count == 0
                 && !repositoryPagina.aguarde)
            {
                Mensagem = "aguarde um momento...";
                repositoryPagina.aguarde = true;               
                list = await repositoryPagina.includes()
                .Where(p => p.Story!.PaginaPadraoLink == capitulo).OrderBy(f => f.Id).ToListAsync();                
                repositoryPagina!.paginas!.AddRange(list);
                repositoryPagina.aguarde = false;
                foreach(var item in list.First().Story.Filtro)
                {
                    if (item is SubStory )
                        repositoryPagina.filtros.Add(await Context.SubStory.Include(g => g.Grupo).ThenInclude(g => g.Pagina).FirstAsync(g => g.Id == item.Id));
                    if (item is Grupo )
                        repositoryPagina.filtros.Add(await Context.Grupo.Include(g => g.SubGrupo).ThenInclude(g => g.Pagina).FirstAsync(g => g.Id == item.Id));
                    if (item is SubGrupo )
                        repositoryPagina.filtros.Add(await Context.SubGrupo.Include(g => g.SubSubGrupo).ThenInclude(g => g.Pagina).FirstAsync(g => g.Id == item.Id));
                    if (item is SubSubGrupo )
                        repositoryPagina.filtros.Add(await Context.SubSubGrupo.Include(g => g.CamadaSeis).ThenInclude(g => g.Pagina).FirstAsync(g => g.Id == item.Id));
                    if (item is CamadaSeis )
                        repositoryPagina.filtros.Add(await Context.CamadaSeis.Include(g => g.CamadaSete).ThenInclude(g => g.Pagina).FirstAsync(g => g.Id == item.Id));
                    if (item is CamadaSete )
                        repositoryPagina.filtros.Add(await Context.CamadaSete.Include(g => g.CamadaOito).ThenInclude(g => g.Pagina).FirstAsync(g => g.Id == item.Id));
                    if (item is CamadaOito )
                        repositoryPagina.filtros.Add(await Context.CamadaOito.Include(g => g.CamadaNove).ThenInclude(g => g.Pagina).FirstAsync(g => g.Id == item.Id));
                    if (item is CamadaNove )
                        repositoryPagina.filtros.Add(await Context.CamadaNove.Include(g => g.CamadaDez).ThenInclude(g => g.Pagina).FirstAsync(g => g.Id == item.Id));
                    if (item is CamadaDez )
                        repositoryPagina.filtros.Add(await Context.CamadaDez.Include(g => g.Pagina).FirstAsync(g => g.Id == item.Id));
                }
            }
            else if (repositoryPagina!.paginas!
                .Where(p => p.Story!.PaginaPadraoLink == capitulo).OrderBy(f => f.Id).ToList().Count > 0)
            {
                Mensagem = null;
                list = repositoryPagina!.paginas!
                .Where(p => p.Story!.PaginaPadraoLink == capitulo).OrderBy(f => f.Id).ToList();
            }
            else
            Mensagem = "aguarde um momento...";

            if (list.First().Story!.Filtro != null)
                  filtros = list.First().Story!.Filtro!.OrderBy(f => f.Id).ToList();
                proximo = indice + 1;
                anterior = indice - 1;

                quantidadePaginas = list.Count();
                if (filtros != null)
                    quantidadeFiltros = filtros.Count;

                if (outroHorizonte == 0)
                    quantidadeLista = quantidadePaginas;
                if (filtros != null && filtros.Count > 0 && outroHorizonte == 1)
                    quantidadeLista = quantidadeFiltros;

                
                Model = list.Skip((int)indice - 1).FirstOrDefault();

            if (filtrar == null && substory == null)
            {



                if (Model == null)
                    nameStory = list.First().Story!.Nome;
                else
                    nameStory = Model.Story.Nome;
                              
                if(outroHorizonte == 1)
                 Model2 = filtros.Skip((int)indice - 1).FirstOrDefault();

                if(Model != null)
                {
                    long id_ultimo = 0;
                    if(Model.Filtro! != null && Model.Filtro!.Count > 0)
                    {

                             if (Model.Filtro!.FirstOrDefault(f => f.Filtro is CamadaDez) != null) id_ultimo =   Model.Filtro!.First(f => f.Filtro is CamadaDez)  .FiltroId;
                        else if (Model.Filtro!.FirstOrDefault(f => f.Filtro is CamadaNove) != null) id_ultimo =  Model.Filtro!.First(f => f.Filtro is CamadaNove) .FiltroId;
                        else if (Model.Filtro!.FirstOrDefault(f => f.Filtro is CamadaOito) != null) id_ultimo =  Model.Filtro!.First(f => f.Filtro is CamadaOito) .FiltroId;
                        else if (Model.Filtro!.FirstOrDefault(f => f.Filtro is CamadaSete) != null) id_ultimo =  Model.Filtro!.First(f => f.Filtro is CamadaSete) .FiltroId;
                        else if (Model.Filtro!.FirstOrDefault(f => f.Filtro is CamadaSeis) != null) id_ultimo =  Model.Filtro!.First(f => f.Filtro is CamadaSeis) .FiltroId;
                        else if (Model.Filtro!.FirstOrDefault(f => f.Filtro is SubSubGrupo) != null) id_ultimo = Model.Filtro!.First(f => f.Filtro is SubSubGrupo).FiltroId;
                        else if (Model.Filtro!.FirstOrDefault(f => f.Filtro is SubGrupo) != null) id_ultimo =    Model.Filtro!.First(f => f.Filtro is SubGrupo)   .FiltroId;
                        else if (Model.Filtro!.FirstOrDefault(f => f.Filtro is Grupo) != null) id_ultimo =       Model.Filtro!.First(f => f.Filtro is Grupo)      .FiltroId;
                        else if (Model.Filtro!.FirstOrDefault(f => f.Filtro is SubStory) != null) id_ultimo =    Model.Filtro!.First(f => f.Filtro is SubStory)   .FiltroId;
                    }
                    
                    if (id_ultimo != 0)
                    {
                        var fil = filtros.First(f => f.Id == id_ultimo);
                        ultimaPasta = filtros.IndexOf(fil) + 1;
                    }
                    else ultimaPasta = 0;
                }
                

                if (Model == null && Model2 == null)
                {
                    Mensagem = $"Por favor digite um numero menor que {quantidadeLista}.";
                    return;
                }

                

                    //  ViewBag.story = pagina.Story.Nome;
                    string html = "";
                if (Model != null && Model!.Content != null || Model != null && Model.ContentUser != null)
                {
                    try
                    {
                        html = await repositoryPagina!.renderizarPagina(Model);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erro: " + ex.Message);
                    }
                }
                else if (Model != null && Model.Produto == null)
                    html = RepositoryPagina.Capa; 
                
                        markup = new MarkupString(html);

                if (Model != null && Model.Comentario != 0 && Model.Comentario != null)
                {
                    var page = Context.Pagina!.Include(p => p.Story).FirstOrDefault(p => p.Id == Model.Comentario);
                    
                    CapituloComentario = page!.Story!.PaginaPadraoLink;
                    var paginas = Context.Pagina!.Include(p => p.Story)
                    .Where(p => p.Story!.PaginaPadraoLink == page.Story.PaginaPadraoLink)
                    .OrderBy(p => p.Id)
                    .ToList();
                    VersoComentario = paginas.IndexOf(page) + 1;


                    if (Model.Filtro != null)
                    {
                        classificacoes = new string[Model.Filtro.Count];
                        for(var i = 0; i < classificacoes.Length; i++)
                        {
                            classificacoes[i] = Model.Filtro[i].Filtro!.Nome;
                        }
                    }

                }

            }
          
            else if (filtrar != null && filtros != null || substory != null && filtros != null)
            {                  
                
                if (filtrar != null)
                {
                    lista = 1;
                    var livro = await Context.Livro!.FirstOrDefaultAsync(l => l.Compartilhando);
                    if (livro != null && redirecionar == null)
                        navigation!.NavigateTo($"{livro.url}/filtro/{livro.Capitulo}/{filtrar}/0/0/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0/1");

                    preferencia = 0;
                    int[] arr = null;
                    var indiceFiltro = int.Parse(filtrar.Replace("pasta-", ""));
                    var fi = Model.Story!.Filtro!.OrderBy(f => f.Id).ToList()[indiceFiltro - 1];                        
                       
                    if (fi is CamadaDez)
                    arr = Arr.RetornarArray(repositoryPagina.filtros, Model.Story!, 3, (long)fi.Id, capitulo, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                    else if (fi is CamadaNove)
                        arr = Arr.RetornarArray(repositoryPagina.filtros, Model.Story!, 3, (long)fi.Id, capitulo, 1, 1, 1, 1, 1, 1, 1, 1);
                    else if (fi is CamadaOito)
                        arr = Arr.RetornarArray(repositoryPagina.filtros, Model.Story!, 3, (long)fi.Id, capitulo, 1, 1, 1, 1, 1, 1, 1);
                    else if (fi is CamadaSete)
                        arr = Arr.RetornarArray(repositoryPagina.filtros, Model.Story!, 3, (long)fi.Id, capitulo, 1, 1, 1, 1, 1, 1);
                    else if (fi is CamadaSeis)
                        arr = Arr.RetornarArray(repositoryPagina.filtros, Model.Story!, 3, (long)fi.Id, capitulo, 1, 1, 1, 1, 1);
                    else if (fi is SubSubGrupo)
                        arr = Arr.RetornarArray(repositoryPagina.filtros, Model.Story!, 3, (long)fi.Id, capitulo, 1, 1, 1,  1);
                    else if (fi is SubGrupo)
                        arr = Arr.RetornarArray(repositoryPagina.filtros, Model.Story!, 3, (long)fi.Id, capitulo, 1, 1,  1);
                    else if (fi is Grupo)
                        arr = Arr.RetornarArray(repositoryPagina.filtros, Model.Story!, 3, (long)fi.Id, capitulo, 1, 1);
                    else if (fi is SubStory)
                        arr = Arr.RetornarArray(repositoryPagina.filtros, Model.Story!, 3, (long)fi.Id, capitulo, 1);
                    indice = 1;
                    compartilhante = "dominio";
                    if (arr.Length == 10)
                    {
                        substory = arr[1];
                        grupo = arr[2];
                        subgrupo = arr[3];
                        subsubgrupo = arr[4];
                        camadaseis = arr[5];
                        camadasete = arr[6];
                        camadaoito = arr[7];
                        camadanove = arr[8];
                        camadadez = arr[9];
                        navigation!.NavigateTo($"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    }
                    else if (arr.Length == 9)
                    {
                        substory = arr[1];
                        grupo = arr[2];
                        subgrupo = arr[3];
                        subsubgrupo = arr[4];
                        camadaseis = arr[5];
                        camadasete = arr[6];
                        camadaoito = arr[7];
                        camadanove = arr[8];
                        navigation!.NavigateTo($"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    }
                    else if (arr.Length == 8)
                    {
                        substory = arr[1];
                        grupo = arr[2];
                        subgrupo = arr[3];
                        subsubgrupo = arr[4];
                        camadaseis = arr[5];
                        camadasete = arr[6];
                        camadaoito = arr[7];
                        navigation!.NavigateTo($"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    }
                    else if (arr.Length == 7)
                    {
                        substory = arr[1];
                        grupo = arr[2];
                        subgrupo = arr[3];
                        subsubgrupo = arr[4];
                        camadaseis = arr[5];
                        camadasete = arr[6];
                        navigation!.NavigateTo($"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    }
                    else if (arr.Length == 6)
                    {
                        substory = arr[1];
                        grupo = arr[2];
                        subgrupo = arr[3];
                        subsubgrupo = arr[4];
                        camadaseis = arr[5];
                        navigation!.NavigateTo($"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    }
                    else if (arr.Length == 5)
                    {
                        substory = arr[1];
                        grupo = arr[2];
                        subgrupo = arr[3];
                        subsubgrupo = arr[4];
                        navigation!.NavigateTo($"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    }
                    else if (arr.Length == 4)
                    {
                        substory = arr[1];
                        grupo = arr[2];
                        subgrupo = arr[3];
                        navigation!.NavigateTo($"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    }
                    else if (arr.Length == 3)
                    {
                        substory = arr[1];
                        grupo = arr[2];
                        navigation!.NavigateTo($"/grupo/{capitulo}/{substory}/{grupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    }
                    else if (arr.Length == 2)
                    {
                        substory = arr[1];
                        navigation!.NavigateTo($"/substory/{capitulo}/{substory}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    }
                }
                else if (substory != null)
                {
                    Filtro? group = null;
                    Filtro? group2 = null;
                    Filtro? group3 = null;
                    Filtro? group4 = null;
                    Filtro? group5 = null;
                    Filtro? group6 = null;
                    Filtro? group7 = null;
                    Filtro? group8 = null;
                    Filtro? group9 = null;


                    group = Model.Story!.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).Skip((int)substory! - 1).First();
                    nameGroup = group!.Nome!;
                    if (preferencia == 0)
                    {
                        if (lista == 1 && Model.ContentUser == null)
                            listaFiltradaComConteudo = retornarListaComConteudo(list,
                         group.Pagina!.Select(p => p.Pagina).ToList()!, (int)camadadez!);
                        else listaFiltradaComConteudo = group.Pagina!.Select(p => p.Pagina).ToList()!;
                    }

                    if (grupo != null)
                    {
                        var fil1 = (SubStory)repositoryPagina.filtros.First(f => f.Id == group.Id);
                        group2 = fil1.Grupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).Skip((int)grupo! - 1).First();
                        nameGroup = group2!.Nome!;
                        if (preferencia == 0)
                        {
                            if (lista == 1 && Model.ContentUser == null)
                                listaFiltradaComConteudo = retornarListaComConteudo(list,
                             group2.Pagina!.Select(p => p.Pagina).ToList()!, (int)camadadez!);
                            else listaFiltradaComConteudo = group2.Pagina!.Select(p => p.Pagina).ToList()!;
                        }

                    }

                    if (subgrupo != null)
                    {
                        var fil2 = (Grupo)repositoryPagina.filtros.First(f => f.Id == group2.Id);
                        group3 = fil2.SubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).Skip((int)subgrupo! - 1).First();
                        nameGroup = group3!.Nome!;
                        if (preferencia == 0)
                        {
                            if (lista == 1 && Model.ContentUser == null)
                                listaFiltradaComConteudo = retornarListaComConteudo(list,
                             group3.Pagina!.Select(p => p.Pagina).ToList()!, (int)camadadez!);
                            else listaFiltradaComConteudo = group3.Pagina!.Select(p => p.Pagina).ToList()!;
                        }

                    }

                    if (subsubgrupo != null)
                    {
                        var fil3 = (SubGrupo)repositoryPagina.filtros.First(f => f.Id == group3.Id);
                        group4 = fil3.SubSubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).Skip((int)subsubgrupo! - 1).First();
                        nameGroup = group4!.Nome!;
                        if (preferencia == 0)
                        {
                            if ( lista == 1 && Model.ContentUser == null)
                                listaFiltradaComConteudo = retornarListaComConteudo(list,
                             group4.Pagina!.Select(p => p.Pagina).ToList()!, (int)camadadez!);
                            else listaFiltradaComConteudo = group4.Pagina!.Select(p => p.Pagina).ToList()!;
                        }

                    }

                    if (camadaseis != null)
                    {
                        var fil4 = (SubSubGrupo)repositoryPagina.filtros.First(f => f.Id == group4.Id);
                        group5 = fil4.CamadaSeis.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).Skip((int)camadaseis! - 1).First();
                        nameGroup = group5!.Nome!;
                        if (preferencia == 0)
                        {
                            if ( lista == 1 && Model.ContentUser == null)
                                listaFiltradaComConteudo = retornarListaComConteudo(list,
                             group5.Pagina!.Select(p => p.Pagina).ToList()!, (int)camadadez!);
                            else listaFiltradaComConteudo = group5.Pagina!.Select(p => p.Pagina).ToList()!;
                        }

                    }

                    if (camadasete != null)
                    {
                        var fil5 = (CamadaSeis)repositoryPagina.filtros.First(f => f.Id == group5.Id);
                        group6 = fil5.CamadaSete.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).Skip((int)camadasete! - 1).First();
                        nameGroup = group6!.Nome!;
                        if (preferencia == 0)
                        {
                            if ( lista == 1 && Model.ContentUser == null)
                                listaFiltradaComConteudo = retornarListaComConteudo(list,
                             group6.Pagina!.Select(p => p.Pagina).ToList()!, (int)camadadez!);
                            else listaFiltradaComConteudo = group6.Pagina!.Select(p => p.Pagina).ToList()!;
                        }

                    }

                    if (camadaoito != null)
                    {
                        var fil6 = (CamadaSete)repositoryPagina.filtros.First(f => f.Id == group6.Id);
                        group7 = fil6.CamadaOito.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).Skip((int)camadaoito! - 1).First();
                        nameGroup = group7!.Nome!;
                        if (preferencia == 0)
                        {
                            if (lista == 1 && Model.ContentUser == null)
                                listaFiltradaComConteudo = retornarListaComConteudo(list,
                             group7.Pagina!.Select(p => p.Pagina).ToList()!, (int)camadadez!);
                            else listaFiltradaComConteudo = group7.Pagina!.Select(p => p.Pagina).ToList()!;
                        }

                    }

                    if (camadanove != null)
                    {
                        var fil7 = (CamadaOito)repositoryPagina.filtros.First(f => f.Id == group7.Id);
                        group8 = fil7.CamadaNove.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).Skip((int)camadanove! - 1).First();
                        nameGroup = group8!.Nome!;
                        if (preferencia == 0)
                        {
                            if ( lista == 1 && Model.ContentUser == null)
                                listaFiltradaComConteudo = retornarListaComConteudo(list,
                             group8.Pagina!.Select(p => p.Pagina).ToList()!, (int)camadadez!);
                            else listaFiltradaComConteudo = group8.Pagina!.Select(p => p.Pagina).ToList()!;
                        }

                    }

                    if (camadadez != null)
                    {
                        var fil8 = (CamadaNove)repositoryPagina.filtros.First(f => f.Id == group8.Id);
                        group9 = fil8.CamadaDez.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).Skip((int)camadadez! - 1).First();
                        nameGroup = group9!.Nome!;
                        if (preferencia == 0)
                        {
                            if (lista == 1 && Model.ContentUser == null)
                                listaFiltradaComConteudo = retornarListaComConteudo(list,
                             group9.Pagina!.Select(p => p.Pagina).ToList()!, (int)camadadez!);
                            else listaFiltradaComConteudo = group9.Pagina!.Select(p => p.Pagina).ToList()!;
                        }

                    }


                    if (preferencia == 1) listaFiltradaComConteudo = retornarListaPreferencial();

                    if (retroceder == 1)
                    {
                        indice = listaFiltradaComConteudo!.Count;
                    }

                    Pagina pag2 = listaFiltradaComConteudo!.OrderBy(p => p.Id).Skip((int)indice - 1).FirstOrDefault();
                        Filtro Filtro = null;
                    if (group8 != null)
                        Filtro = Model.Story!.Filtro!.First(f => f.Id == group8!.Id);
                    else if (group7 != null)
                        Filtro = Model.Story!.Filtro!.First(f => f.Id == group7!.Id);
                    else if (group6 != null)
                        Filtro = Model.Story!.Filtro!.First(f => f.Id == group6!.Id);
                    else if (group5 != null)
                        Filtro = Model.Story!.Filtro!.First(f => f.Id == group5!.Id);
                    else if (group4 != null)
                        Filtro = Model.Story!.Filtro!.First(f => f.Id == group4!.Id);
                    else if (group3 != null)
                        Filtro = Model.Story!.Filtro!.First(f => f.Id == group3!.Id);
                    else if (group2 != null)
                        Filtro = Model.Story!.Filtro!.First(f => f.Id == group2!.Id);
                    else if (group != null)
                        Filtro = Model.Story!.Filtro!.First(f => f.Id == group!.Id);
                    indice_Filtro = Model.Story.Filtro!.OrderBy(f => f.Id).ToList().IndexOf(Filtro) + 1;

                    if (pag2 == null)
                    {
                        navigation.NavigateTo($"/renderizar/{capitulo}/{indice_Filtro}/0/11/1/1/0/0/0/{dominio}/{compartilhante}/{compartilhante2}");
                    }

                    Model = list.First(p => p.Id == pag2.Id);
                    vers = list.IndexOf(Model) + 1;

                    

                    quantidadeLista = listaFiltradaComConteudo!.Count;
                    html = await repositoryPagina!.renderizarPagina(Model);
                    markup = new MarkupString(html);
                    proximo = indice + 1;
                    anterior = indice - 1;

                    

                    if (Model != null)
                    {
                        long id_ultimo = 0;
                        
                                 if (Model.Filtro != null && Model.Filtro.FirstOrDefault(f => f.Filtro  is CamadaDez  ) != null )  id_ultimo = Model.Filtro.First(f =>  f.Filtro  is CamadaDez  ).FiltroId;
                            else if (Model.Filtro != null && Model.Filtro.FirstOrDefault(f => f.Filtro  is CamadaNove ) != null )  id_ultimo = Model.Filtro.First(f =>  f.Filtro  is CamadaNove ).FiltroId;
                            else if (Model.Filtro != null && Model.Filtro.FirstOrDefault(f => f.Filtro  is CamadaOito ) != null )  id_ultimo = Model.Filtro.First(f =>  f.Filtro  is CamadaOito ).FiltroId;
                            else if (Model.Filtro != null && Model.Filtro.FirstOrDefault(f => f.Filtro  is CamadaSete ) != null )  id_ultimo = Model.Filtro.First(f =>  f.Filtro  is CamadaSete ).FiltroId;
                            else if (Model.Filtro != null && Model.Filtro.FirstOrDefault(f => f.Filtro  is CamadaSeis ) != null )  id_ultimo = Model.Filtro.First(f =>  f.Filtro  is CamadaSeis ).FiltroId;
                            else if (Model.Filtro != null && Model.Filtro.FirstOrDefault(f => f.Filtro  is SubSubGrupo) != null )  id_ultimo = Model.Filtro.First(f =>  f.Filtro  is SubSubGrupo).FiltroId;
                            else if (Model.Filtro != null && Model.Filtro.FirstOrDefault(f => f.Filtro  is SubGrupo   ) != null )  id_ultimo = Model.Filtro.First(f =>  f.Filtro  is SubGrupo   ).FiltroId;
                            else if (Model.Filtro != null && Model.Filtro.FirstOrDefault(f => f.Filtro  is Grupo      ) != null )  id_ultimo = Model.Filtro.First(f =>  f.Filtro  is Grupo      ).FiltroId;
                            else if (Model.Filtro != null && Model.Filtro.FirstOrDefault(f => f.Filtro is  SubStory   ) != null)  id_ultimo = Model.Filtro.First(f =>   f.Filtro is  SubStory)   .FiltroId;

                        
                        if (id_ultimo != 0)
                        {
                            var fil = filtros.First(f => f.Id == id_ultimo);
                            ultimaPasta = filtros.IndexOf(fil) + 1;
                        }
                        else ultimaPasta = 0;
                    }

                  
                }
            }
            try
            {
                await firstInput.FocusAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                if (indice > quantidadeLista)
                    await js!.InvokeAsync<object>("MarcarIndice", "1");
                else
                    await js!.InvokeAsync<object>("MarcarIndice", $"{indice}");

            }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            PageLiked p = null;
            if(substory != null)
             p = repositoryPagina.paginasCurtidas!.FirstOrDefault(p => p.capitulo == capitulo
                && p.verso == vers
                && p.user == user.Identity!.Name)!;
            else
                p = repositoryPagina.paginasCurtidas!.FirstOrDefault(p => p.capitulo == capitulo
                    && p.verso == indice
                    && p.user == user.Identity!.Name)!;

            if (p != null)
                liked = true;

            quantLiked = CountLikes(ApplicationDbContext._connectionString);

        }
          
        private List<Pagina> retornarListaComConteudo(List<Pagina> content, List<Pagina> produtos, int grupo)
        {
            int pular = (int)(content.Count * 0.2);
            List<Pagina> conteudo = content.Where(p => p!.Content != null).ToList();
            List<Pagina> conteudoPorGrupo = conteudo.Skip((grupo - 1) * pular).Take(pular).ToList();
            List<Pagina> listaComConteudo = new List<Pagina>();
            int interacao = 0;

            while (produtos.Skip(interacao * 2).ToList().Count >= 2)
            {
                listaComConteudo.AddRange(produtos.Skip(interacao * 2).Take(2).ToList());
                if (conteudoPorGrupo.Skip(interacao).FirstOrDefault() != null)
                    listaComConteudo.Add(conteudoPorGrupo.Skip(interacao).First());

                interacao++;
            }

            if (listaComConteudo.Count == 0) return produtos;
            if (!listaComConteudo.Contains(produtos.Last()))
                listaComConteudo.Add(produtos.Last());

            return listaComConteudo;
        }

        private List<Pagina> retornarListaPreferencial()
        {
            List<Pagina> retorno = new List<Pagina>();
            var list = Context.Pagina!.Include(p => p.Story)
                .Where(p => p.Story!.PaginaPadraoLink == capitulo)
                .OrderBy(p => p.Id)
                .ToList();

            var p = list.First();

            if(p.ContentUser != null)
            {
                if (p1 != 0)
                    retorno.Add(list.Skip((int)(p1 - 1)!).First());
                if (p2 != 0)
                    retorno.Add(list.Skip((int)(p2 - 1)!).First());
                if (p3 != 0)
                    retorno.Add(list.Skip((int)(p3 - 1)!).First());
                if (p4 != 0)
                    retorno.Add(list.Skip((int)(p4 - 1)!).First());
                if (p5 != 0)
                    retorno.Add(list.Skip((int)(p5 - 1)!).First());
                if (p6 != 0)
                    retorno.Add(list.Skip((int)(p6 - 1)!).First());
                if (p7 != 0)
                    retorno.Add(list.Skip((int)(p7 - 1)!).First());
                if (p8 != 0)
                    retorno.Add(list.Skip((int)(p8 - 1)!).First());
                if (p9 != 0)
                    retorno.Add(list.Skip((int)(p9 - 1)!).First());
                if (p10 != 0)
                    retorno.Add(list.Skip((int)(p10 - 1)!).First());
            }
            else
            {
                List<Pagina> produtos = new List<Pagina>();
                List<Pagina> conteudo = new List<Pagina>();

                if (p1 != 0 && list.Skip((int)(p1 - 1)!).First().Produto == null)
                    conteudo.Add(list.Skip((int)(p1 - 1)!).First());
                else if (p1 != 0 && list.Skip((int)(p1 - 1)!).First().Produto != null)
                    produtos.Add(list.Skip((int)(p1 - 1)!).First());

                if (p2 != 0 && list.Skip((int)(p2 - 1)!).First().Produto == null)
                    conteudo.Add(list.Skip((int)(p2 - 1)!).First());
                else if (p2 != 0 && list.Skip((int)(p2 - 1)!).First().Produto != null)
                    produtos.Add(list.Skip((int)(p2 - 1)!).First());

                if (p3 != 0 && list.Skip((int)(p3 - 1)!).First().Produto == null)
                    conteudo.Add(list.Skip((int)(p3 - 1)!).First());
                else if (p3 != 0 && list.Skip((int)(p3 - 1)!).First().Produto != null)
                    produtos.Add(list.Skip((int)(p3 - 1)!).First());

                if (p4 != 0 && list.Skip((int)(p4 - 1)!).First().Produto == null)
                    conteudo.Add(list.Skip((int)(p4 - 1)!).First());
                else if (p4 != 0 && list.Skip((int)(p4 - 1)!).First().Produto != null)
                    produtos.Add(list.Skip((int)(p4 - 1)!).First());

                if (p5 != 0 && list.Skip((int)(p5 - 1)!).First().Produto == null)
                    conteudo.Add(list.Skip((int)(p5 - 1)!).First());
                else if (p5 != 0 && list.Skip((int)(p5 - 1)!).First().Produto != null)
                    produtos.Add(list.Skip((int)(p5 - 1)!).First());

                if (p6 != 0 && list.Skip((int)(p6 - 1)!).First().Produto == null)
                    conteudo.Add(list.Skip((int)(p6 - 1)!).First());
                else if (p6 != 0 && list.Skip((int)(p6 - 1)!).First().Produto != null)
                    produtos.Add(list.Skip((int)(p6 - 1)!).First());

                if (p7 != 0 && list.Skip((int)(p7 - 1)!).First().Produto == null)
                    conteudo.Add(list.Skip((int)(p7 - 1)!).First());
                else if (p7 != 0 && list.Skip((int)(p7 - 1)!).First().Produto != null)
                    produtos.Add(list.Skip((int)(p7 - 1)!).First());

                if (p8 != 0 && list.Skip((int)(p8 - 1)!).First().Produto == null)
                    conteudo.Add(list.Skip((int)(p8 - 1)!).First());
                else if (p8 != 0 && list.Skip((int)(p8 - 1)!).First().Produto != null)
                    produtos.Add(list.Skip((int)(p8 - 1)!).First());

                if (p9 != 0 && list.Skip((int)(p9 - 1)!).First().Produto == null)
                    conteudo.Add(list.Skip((int)(p9 - 1)!).First());
                else if (p9 != 0 && list.Skip((int)(p9 - 1)!).First().Produto != null)
                    produtos.Add(list.Skip((int)(p9 - 1)!).First());

                if (p10 != 0 && list.Skip((int)(p10 - 1)!).First().Produto == null)
                    conteudo.Add(list.Skip((int)(p10 - 1)!).First());
                else if (p10 != 0 && list.Skip((int)(p10 - 1)!).First().Produto != null)
                    produtos.Add(list.Skip((int)(p10 - 1)!).First());

                int interacao = 0;

                while (produtos.Skip(interacao * 2).ToList().Count >= 2)
                {
                    retorno.AddRange(produtos.Skip(interacao * 2).Take(2).ToList());
                    if (conteudo.Skip(interacao).FirstOrDefault() != null)
                        retorno.Add(conteudo.Skip(interacao).First());
                    interacao++;
                }

                if (retorno.Count == 0) return produtos;
                if (!retorno.Contains(produtos.Last()))
                    retorno.Add(produtos.Last());
            }

            return retorno;
        }

        private async void StartTimer(Pagina p)
        {
            if (auto == 1)
            {
                try
                {
                    if ( p != null && p.Content != null && p.Content.Contains("iframe") && outroHorizonte == 0)
                    {
                        var arr = p.Content.Split("/");
                        var id_video = "";
                        for (var index = 0; index < arr.Length; index++)
                        {
                            if (arr[index] == "embed")
                            {
                                var text = arr[index + 1];
                                var arr2 = text.Split("?");
                                id_video = arr2[0];
                                break;
                            }
                        }
                        var tempoVideo = await GetYouTubeVideo(id_video);
                        await js!.InvokeAsync<object>("PreencherProgressBar", tempoVideo);
                        Timer!.SetTimer(tempoVideo);
                    }

                    else
                    {
                        await js!.InvokeAsync<object>("PreencherProgressBar", timeproduto * 1000);
                        Timer!.SetTimer(timeproduto * 1000);
                    }
                        
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Timer._timer!.Elapsed += _timer_Elapsed;
                Console.WriteLine("Timer Started.");
                if(Timer!.desligarAuto! == null || Timer!.desligarAuto!.Enabled == false)
                {
                    Timer!.SetTimerAuto();
                    Timer!.desligarAuto!.Elapsed += desligarAuto_Elapsed;
                }
            }
        }
        
        private void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (auto == 1)
            {
                if (substory == null)
                {
                    if (capitulo == 0 && indice >= quantidadeLista)
                        navigation!.NavigateTo($"/Renderizar/{capitulo}/1/1/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}");
                   
                    else if (capitulo != 0 && indice >= quantidadeLista && outroHorizonte == 0)
                        navigation!.NavigateTo($"/Renderizar/{capitulo + 1}/1/1/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}");
                    else if (capitulo != 0 && indice >= quantidadeLista && outroHorizonte == 1)
                        navigation!.NavigateTo($"/Renderizar/{capitulo}/1/1/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}");
                    else
                        navigation!.NavigateTo($"/Renderizar/{capitulo}/{proximo}/1/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}");
                }
                else
                {
                    navegarSubgrupos(false);
                }
            }

            Console.WriteLine("Timer Elapsed.");
            Timer!._timer!.Elapsed -= _timer_Elapsed;
        }

        protected void TeclaPressionada(KeyboardEventArgs args)
        {
            
            if (substory == null)
            {
                
                if (args.Key == "Enter" && capitulo == 0)
                {
                    navigation!.NavigateTo($"/Renderizar/{indice}/1/1/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}");
                }
                else if (args.Key == "Enter")
                {
                    navigation!.NavigateTo($"/Renderizar/0/{capitulo}/1/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}");
                }
            }
            else if (args.Key == "Enter")
            {
                navegarSubgrupos(true);
            }

        }

        protected void Casinha()
        {
            auto = 0;
            navigation!.NavigateTo($"/info/{dominio}/{compartilhante}/{compartilhante2}");
        }

        protected async void Pesquisar()
        {
            bool condicao = false;
            try
            {
                var n = int.Parse(opcional);
                auto = 0;
                condicao = true;

            }
            catch(Exception ex)
            {
                condicao = false;
            }
           
                if(substory == null && condicao)
                {
                    var url = $"/Renderizar/{capitulo}/{opcional}/{auto}/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}";
                    navigation!.NavigateTo(url);

                }
                else if (condicao)
            {
                    int indiceListaFiltrada = 0;
                    foreach(var item in listaFiltradaComConteudo!)
                    {
                        var p = list.First(p => p.Id == item.Id);
                        if( int.Parse(opcional) == list.IndexOf(p) + 1)
                        {
                            indiceListaFiltrada = listaFiltradaComConteudo.IndexOf(item) + 1;
                            break;
                        }
                    }

                    if(indiceListaFiltrada == 0)
                    {
                        indiceListaFiltrada = indice;
                        await js!.InvokeAsync<object>("DarAlert", $"Não foi encontrado o versiculo");
                    }                
                
                        var url = "";
                        if(camadadez != null)
                        url = $"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{indiceListaFiltrada}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
                      else  if(camadanove != null)
                        url = $"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{indiceListaFiltrada}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
                        else if (camadaoito != null)
                        url = $"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{indiceListaFiltrada}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
                        else if (camadasete != null)
                            url = $"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{indiceListaFiltrada}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
                        else if (camadaseis != null)
                            url = $"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{indiceListaFiltrada}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
                        else if (subsubgrupo != null)
                            url = $"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{indiceListaFiltrada}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
                        else if (subgrupo != null)
                            url = $"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{indiceListaFiltrada}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
                        else if (grupo != null)
                            url = $"/grupo/{capitulo}/{substory}/{grupo}/{indiceListaFiltrada}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
                        else if (substory != null)
                            url = $"/substory/{capitulo}/{substory}/{indiceListaFiltrada}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
                        navigation!.NavigateTo(url);                
                }
                   
        }

        protected void habilitarAuto()
        {
            Timer!.SetTimerAuto();
            Timer!.desligarAuto!.Elapsed += desligarAuto_Elapsed;

            var url = "";
            if (camadadez != null)
                url = $"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{indice}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            else if (camadanove != null)
                url = $"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{indice}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            else if (camadaoito != null)
                url = $"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{indice}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            else if (camadasete != null)
                url = $"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{indice}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            else if (camadaseis != null)
                url = $"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{indice}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            else if (subsubgrupo != null)
                url = $"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{indice}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            else if (subgrupo != null)
                url = $"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{indice}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            else if (grupo != null)
                url = $"/grupo/{capitulo}/{substory}/{grupo}/{indice}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            else if (substory != null)
                url = $"/substory/{capitulo}/{substory}/{indice}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            else
            url = $"/Renderizar/{capitulo}/{indice}/1/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}";
            navigation!.NavigateTo(url);
        }

        protected void desabilitarAuto()
        {
            if(Timer!.desligarAuto != null)
            {
                Timer!.desligarAuto!.Elapsed -= desligarAuto_Elapsed;
                Timer!.desligarAuto!.Enabled = false;
                Timer.desligarAuto.Dispose();
                auto = 0;
            }            
        }

        protected async void FazerComentario()
        {
            if (auto == 1)
                desabilitarAuto();
            navigation!.NavigateTo($"/comentario/{capitulo}/{indice}");
        }

        private void desligarAuto_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (auto == 1)
                auto = 0;

            Console.WriteLine("Timer Elapsed auto.");
            Timer!.desligarAuto!.Elapsed -= desligarAuto_Elapsed;
            Timer.desligarAuto.Dispose();
            navigation!.NavigateTo("/");
        }

        private async Task<int> GetYouTubeVideo(string id_video)
        {
            int calculo = 0;
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "",
                ApplicationName = this.GetType().ToString()
            });
            var searchListRequest = youtubeService.Videos.List("snippet,contentDetails,statistics,status");
            searchListRequest.Id = id_video;
            var search = await searchListRequest.ExecuteAsync();
            var duration = search.Items[0].ContentDetails.Duration;
            if (duration.Contains("M"))
            {
                duration = duration.Replace("PT", "");
                var minutos = int.Parse(duration.Split('M')[0]);
                var segundos = int.Parse(duration.Replace(minutos.ToString() + "M", "").Replace("S", ""));
                var minutosEmMileSegundos = minutos * 60 * 1000;
                var segundosEmMileSegundos = segundos * 1000;
                calculo = minutosEmMileSegundos + segundosEmMileSegundos;
            }
            else
                calculo = int.Parse(duration.Replace("PT", "").Replace("S", "")) * 1000;


            return calculo;
        }        
       
        protected async void marcarPreferencia(int Versiculo)
        {
            auto = 0;
            var p = repositoryPagina!.preferencias!.FirstOrDefault(u => u.user == user.Identity!.Name && u.capitulo == capitulo && u.pasta == indice_Filtro);
            UserPreferences preferences = null;
            if (p1 == 0)
            {
                p1 = Versiculo;                
                await js!.InvokeAsync<object>("DarAlert", $"versiculo {p1} foi marcado como 1º preferência");
            }       
                           
            else if (p2 == 0)
            {
                p2 = Versiculo;
                await js!.InvokeAsync<object>("DarAlert", $"versiculo {p2} foi marcado como 2º preferência");
            }
            else if (p3 == 0)
            {
                p3 = Versiculo;
                await js!.InvokeAsync<object>("DarAlert", $"versiculo {p3} foi marcado como 3º preferência");
            }
            else if (p4 == 0)
            {
                p4 = Versiculo;
                await js!.InvokeAsync<object>("DarAlert", $"versiculo {p4} foi marcado como 4º preferência");
            }
            else if (p5 == 0)
            {
                p5 = Versiculo;
                await js!.InvokeAsync<object>("DarAlert", $"versiculo {p5} foi marcado como 5º preferência");
            }
            else if (p6 == 0)
            {
                p6 = Versiculo;
                await js!.InvokeAsync<object>("DarAlert", $"versiculo {p6} foi marcado como 6º preferência");
            }
            else if (p7 == 0)
            {
                p7 = Versiculo;
                await js!.InvokeAsync<object>("DarAlert", $"versiculo {p7} foi marcado como 7º preferência");
            }
            else if (p8 == 0)
            {
                p8 = Versiculo;
                await js!.InvokeAsync<object>("DarAlert", $"versiculo {p8} foi marcado como 8º preferência");
            }
            else if (p9 == 0)
            {
                p9 = Versiculo;
                await js!.InvokeAsync<object>("DarAlert", $"versiculo {p9} foi marcado como 9º preferência");
            }
            else if (p10 == 0)
            {
                p10 = Versiculo;
                await js!.InvokeAsync<object>("DarAlert", $"versiculo {p10} foi marcado como 10º preferência");
            }
            else            
            await js!.InvokeAsync<object>("DarAlert", "Você só pode marcar 10 preferências");

                preferences = new UserPreferences
                {
                    user = user.Identity!.Name!,
                    capitulo = capitulo,
                    pasta = (int)indice_Filtro!,
                    p1 = (int)p1!,
                    p2 = (int)p2!,
                    p3 = (int)p3!,
                    p4 = (int)p4!,
                    p5 = (int)p5!,
                    p6 = (int)p6!,
                    p7 = (int)p7!,
                    p8 = (int)p8!,
                    p9 = (int)p9!,
                    p10 = (int)p10!
                };           

            if (p10 == 0)
            {
                if (p != null)
                {
                    Context.Add(preferences);
                    Context.SaveChanges();
                    repositoryPagina.preferencias.Add(preferences);
                }
                else
                {
                    p = preferences;
                    Context.Update(p);
                    Context.SaveChanges();

                    var preferencia = repositoryPagina.preferencias.FirstOrDefault(pre => pre.Id == p.Id);
                    repositoryPagina.preferencias.Remove(preferencia);
                    repositoryPagina.preferencias.Add(p);
                }


                if (camadadez != null)
            navigation!
                    .NavigateTo($"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
            else if (camadanove != null)
                navigation!
                        .NavigateTo($"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");

            else  if (camadaoito != null)
                navigation!
                        .NavigateTo($"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");

            else if (camadasete != null)
                navigation!
                        .NavigateTo($"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");

            else if (camadaseis != null)
                navigation!
                        .NavigateTo($"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
               
            else if (subsubgrupo != null)
                navigation!
                        .NavigateTo($"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");

            else if (subgrupo != null)
                navigation!
                        .NavigateTo($"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");

            else if (grupo != null)
                navigation!
                        .NavigateTo($"/grupo/{capitulo}/{substory}/{grupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");

            else if (substory != null)
                navigation!
                        .NavigateTo($"/substory/{capitulo}/{substory}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");

            }

        }

        protected async void desmarcar()
        {
            auto = 0;
            var p = repositoryPagina!.preferencias!.FirstOrDefault(u => u.user == user.Identity!.Name && u.capitulo == capitulo && u.pasta == indice_Filtro);
            var p2 = Context.UserPreferences.FirstOrDefault(u => u.user == user.Identity!.Name && u.capitulo == capitulo && u.pasta == indice_Filtro);
            if(p2 != null && p2.user == user.Identity!.Name)
            {
                p.p1 = 0; p.p2 = 0; p.p3 = 0; p.p4 = 0; p.p5 = 0;
                p.p6 = 0; p.p7 = 0; p.p8 = 0; p.p9 = 0; p.p10 = 0;
                p2.p1 = 0; p2.p2 = 0; p2.p3 = 0; p2.p4 = 0; p2.p5 = 0;
                p2.p6 = 0; p2.p7 = 0; p2.p8 = 0; p2.p9 = 0; p2.p10 = 0;
                Context.Update(p2);
                Context.SaveChanges();
            }

            if (camadadez != null)
                    navigation!
                            .NavigateTo($"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                else if (camadanove != null)
                    navigation!
                            .NavigateTo($"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");

                else if (camadaoito != null)
                    navigation!
                            .NavigateTo($"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");

                else if (camadasete != null)
                    navigation!
                            .NavigateTo($"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");

                else if (camadaseis != null)
                    navigation!
                            .NavigateTo($"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");

                else if (subsubgrupo != null)
                    navigation!
                            .NavigateTo($"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");

                else if (subgrupo != null)
                    navigation!
                            .NavigateTo($"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");

                else if (grupo != null)
                    navigation!
                            .NavigateTo($"/grupo/{capitulo}/{substory}/{grupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");

                else if (substory != null)
                    navigation!
                            .NavigateTo($"/substory/{capitulo}/{substory}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
          
        }

        protected async void acessarPasta()
        {
                indice_Filtro = indice;

            if (Model2!.Pagina == null || Model2.Pagina.Count == 0)
            {
                await js!.InvokeAsync<object>("DarAlert", $"Esta pasta não possui versiculos");
                navigation!.NavigateTo($"/renderizar/{capitulo}/{indice}/{auto}/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}");
            }
            else
            {
                if (auto == 1)
                    desabilitarAuto();
                navigation!.NavigateTo($"/filtro/{capitulo}/pasta-{indice_Filtro}/{preferencia}/0/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");

            }

        }
        protected void listarPasta()
        {
            
            int tamanho = 0;

            if (listaFiltradaComConteudo!.FirstOrDefault(p => p.ContentUser != null) != null)
                tamanho = 5;
            else
                tamanho = 20;

            if (auto == 1)
                desabilitarAuto();
            navigation!.NavigateTo($"/lista-filtro/1/teste/1/11/{tamanho}/{capitulo}/{indice_Filtro}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
        }

        protected void acessarHorizontePastas()
        {
            if (auto == 1)
                desabilitarAuto();
            outroHorizonte = 1;
            navigation!.NavigateTo($"/renderizar/{capitulo}/1/0/11/1/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}");
        }

        protected void acessarHorizonteVersos()
        {
            if (auto == 1)
                desabilitarAuto();
            outroHorizonte = 0;
            navigation!.NavigateTo($"/renderizar/{capitulo}/1/0/11/1/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}");
        }
           
        protected int buscarCamada(Filtro fil)
        {
            if (fil is SubStory)
                return 1;
          else  if (fil is Grupo)
               return 2;
          else  if (fil is SubGrupo)
               return 3;
          else  if (fil is SubSubGrupo)
               return 4;
          else  if (fil is CamadaSeis)
               return 5;
          else  if (fil is CamadaSete)
               return 6;
          else  if (fil is CamadaOito)
                return 7;
          else  if (fil is CamadaNove)
                return 8;
          else  if (fil is CamadaDez)
                return 9;

            return 0;
        }    
   
        protected int? buscarPastaFiltrada(int camada)
        {
            long? IdGrupo = 0;
            Pagina  pag = repositoryPagina!.paginas!
                    .FirstOrDefault(p =>  p.Filtro!.FirstOrDefault(f => f.FiltroId == Model2!.Id) != null)!;

            if (pag != null)
            {
                Filtro  fil = pag.Filtro!.FirstOrDefault(f => f.FiltroId == Model2!.Id)!.Filtro!;

                if (camada != 0)
                {
                    Filtro fl = null;
                    var filtros = Context.Pagina!.Include(p => p.Story).ThenInclude(p => p.Filtro)
                        .Where(p => p.Story!.PaginaPadraoLink == capitulo)
                       .First().Story!.Filtro!.OrderBy(f => f.Id).ToList();
                    if(fil is SubStory)
                    {
                        SubStory gr = (SubStory)fil;
                        fl = filtros!.First(f => f.Id == gr.Id);
                    }
                    if(fil is Grupo)
                    {
                        Grupo gr = (Grupo)fil;
                        fl = filtros!.First(f => f.Id == gr.SubStoryId);
                    }
                    if(fil is SubGrupo)
                    {
                        SubGrupo gr = (SubGrupo)fil;
                        fl = filtros!.First(f => f.Id == gr.GrupoId);
                    }
                    if(fil is SubSubGrupo)
                    {
                        SubSubGrupo gr = (SubSubGrupo)fil;
                        fl = filtros!.First(f => f.Id == gr.SubGrupoId);
                    }
                    if(fil is CamadaSeis)
                    {
                        CamadaSeis gr = (CamadaSeis)fil;
                        fl = filtros!.First(f => f.Id == gr.SubSubGrupoId);
                    }
                    if(fil is CamadaSete)
                    {
                        CamadaSete gr = (CamadaSete)fil;
                        fl = filtros!.First(f => f.Id == gr.CamadaSeisId);
                    }
                    if(fil is CamadaOito)
                    {
                        CamadaOito gr = (CamadaOito)fil;
                        fl = filtros!.First(f => f.Id == gr.CamadaSeteId);
                    }
                    if(fil is CamadaNove)
                    {
                        CamadaNove gr = (CamadaNove)fil;
                        fl = filtros!.First(f => f.Id == gr.CamadaOitoId);
                    }
                    if(fil is CamadaDez)
                    {
                        CamadaDez gr = (CamadaDez)fil;
                        fl = filtros!.First(f => f.Id == gr.CamadaNoveId);
                    }
                    var pasta = filtros!.IndexOf(fl) + 1;

                    return pasta;
                }
                return 0;                
            }
            else
                return null;            
        }
   
        protected void buscarProximo()
        {
            auto = 0;
            if (proximo <= quantidadeLista)
            {
                if (camadadez != null)
                    navigation!.NavigateTo($"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{proximo}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else if (camadanove != null)
                    navigation!.NavigateTo($"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{proximo}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else if (camadaoito != null)
                    navigation!.NavigateTo($"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{proximo}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else if (camadasete != null)
                    navigation!.NavigateTo($"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{proximo}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else if (camadaseis != null)
                    navigation!.NavigateTo($"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{proximo}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else if (subsubgrupo != null)
                    navigation!.NavigateTo($"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{proximo}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else if (subgrupo != null)
                    navigation!.NavigateTo($"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{proximo}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else if (grupo != null)
                    navigation!.NavigateTo($"/grupo/{capitulo}/{substory}/{grupo}/{proximo}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else if (substory != null)
                    navigation!.NavigateTo($"/substory/{capitulo}/{substory}/{proximo}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else
                    navigation!.NavigateTo($"/Renderizar/{capitulo}/{proximo}/{auto}/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}");
            }
            else if(substory != null)
                navegarSubgrupos(false);
            else
                navigation!.NavigateTo($"/Renderizar/{capitulo + 1}/1/{auto}/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}");


        }
        private void navegarSubgrupos(bool somenteSubgrupos)
        {
            if (somenteSubgrupos) auto = 0;
            if (camadadez != null)
            {
                if(indice >= quantidadeLista || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray( repositoryPagina.filtros, Model!.Story!, 1, 0, capitulo, (int)substory!,
                        grupo, subgrupo, subsubgrupo, camadaseis, camadasete, camadaoito, camadanove, camadadez);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/camadadez/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/{arr[5]}/{arr[6]}/{arr[7]}/{arr[8]}/{arr[9]}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    else
                        navigation!.NavigateTo($"/camadanove/{capitulo}/1/1/1/1/1/1/1/1/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");

                }
                else
                {
                    navigation!
                    .NavigateTo($"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{proximo}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                }
            }
            else  if (camadanove != null)
            {
                if(indice >= quantidadeLista || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray(repositoryPagina.filtros, Model!.Story!, 1, 0, capitulo, (int)substory!,
                        grupo, subgrupo, subsubgrupo, camadaseis, camadasete, camadaoito, camadanove);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/camadanove/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/{arr[5]}/{arr[6]}/{arr[7]}/{arr[8]}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    else
                        navigation!.NavigateTo($"/camadaoito/{capitulo}/1/1/1/1/1/1/1/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");

                }
                else
                {
                    navigation!
                   .NavigateTo($"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{proximo}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                }
            }
            else  if (camadaoito != null)
            {
                if(indice >= quantidadeLista || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray(repositoryPagina.filtros, Model!.Story!, 1, 0, capitulo, (int)substory!,
                        grupo, subgrupo, subsubgrupo, camadaseis, camadasete, camadaoito);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/camadaoito/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/{arr[5]}/{arr[6]}/{arr[7]}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    else
                        navigation!.NavigateTo($"/camadasete/{capitulo}/1/1/1/1/1/1/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");

                }
                else
                {
                    navigation!
                  .NavigateTo($"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{proximo}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                }
            }
            else  if (camadasete != null )
            {
                if(indice >= quantidadeLista || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray(repositoryPagina.filtros, Model!.Story!, 1, 0, capitulo, (int)substory!,
                        grupo, subgrupo, subsubgrupo, camadaseis, camadasete);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/camadasete/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/{arr[5]}/{arr[6]}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    else
                        navigation!.NavigateTo($"/camadaseis/{capitulo}/1/1/1/1/1/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");

                }
                else
                {
                    navigation!
                .NavigateTo($"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{proximo}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                }
            }
            else  if (camadaseis != null)
            {
                if(indice >= quantidadeLista || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray(repositoryPagina.filtros, Model!.Story!, 1, 0, capitulo, (int)substory!,
                        grupo, subgrupo, subsubgrupo, camadaseis);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/camadaseis/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/{arr[5]}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    else
                        navigation!.NavigateTo($"/subsubgrupo/{capitulo}/1/1/1/1/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");

                }
                else
                {
                    navigation!
               .NavigateTo($"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{proximo}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                }
            }
            else  if (subsubgrupo != null)
            {
                if(indice >= quantidadeLista || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray(repositoryPagina.filtros, Model!.Story!, 1, 0, capitulo, (int)substory!,
                        grupo, subgrupo, subsubgrupo);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/subsubgrupo/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    else
                        navigation!.NavigateTo($"/subgrupo/{capitulo}/1/1/1/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");

                }
                else
                {
                    navigation!
              .NavigateTo($"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{proximo}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                }
            }
            else if (subgrupo != null)
            {
                if(indice >= quantidadeLista || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray(repositoryPagina.filtros, Model!.Story!, 1, 0, capitulo, (int)substory!,
                        grupo, subgrupo);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/subgrupo/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    else
                        navigation!.NavigateTo($"/grupo/{capitulo}/1/1/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");

                }
                else
                {
                    navigation!
            .NavigateTo($"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{proximo}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                }
            }
            else if (grupo != null)
            {
                if(indice >= quantidadeLista || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray(repositoryPagina.filtros, Model!.Story!, 1, 0, capitulo, (int)substory!, grupo);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/grupo/{arr[0]}/{arr[1]}/{arr[2]}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    else
                        navigation!.NavigateTo($"/substory/{capitulo}/1/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");

                }
                else
                {
                    navigation!
           .NavigateTo($"/grupo/{capitulo}/{substory}/{grupo}/{proximo}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                }
            }
            else  if (substory != null)
            {
                if (indice >= quantidadeLista || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray(repositoryPagina.filtros, Model!.Story!, 1, 0, capitulo, (int)substory);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/substory/{arr[0]}/{arr[1]}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    else
                        navigation!.NavigateTo($"/renderizar/{capitulo}/1/{auto}/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}");

                }
                else
                {
                    navigation!
             .NavigateTo($"/substory/{capitulo}/{substory}/{proximo}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                }
            }
        }

        protected void buscarAnterior()
        {
            auto = 0;
            if (indice == 1 && capitulo != 0)
            {
                if(substory != null)
                {
                    voltarSubgrupos();
                }

                if (camadadez != null)
                navigation!.NavigateTo($"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/1/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                   
                else if (camadanove != null)
                navigation!.NavigateTo($"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/1/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                   
                else if (camadaoito != null)
                navigation!.NavigateTo($"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/1/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    
                else if (camadasete != null)
                navigation!.NavigateTo($"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/1/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    
                else if (camadaseis != null)
                navigation!.NavigateTo($"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/1/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                   
                else if (subsubgrupo != null)
                navigation!.NavigateTo($"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/1/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    
                else if (subgrupo != null)                
                    navigation!.NavigateTo($"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/1/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");    
                
                else if (grupo != null)
                navigation!.NavigateTo($"/grupo/{capitulo}/{substory}/{grupo}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/1/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");                    
                
                else if (substory != null)
                navigation!.NavigateTo($"/substory/{capitulo}/{substory}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/1/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                else
                    navigation!.NavigateTo($"/Renderizar/{capitulo - 1}/1/{auto}/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/1/{dominio}/{compartilhante}/{compartilhante2}");

            }
            if (indice != 1)
            {
                if (camadadez != null)
                    navigation!.NavigateTo($"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{anterior}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else if (camadanove != null)
                    navigation!.NavigateTo($"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{anterior}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else if (camadaoito != null)
                    navigation!.NavigateTo($"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{anterior}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else if (camadasete != null)
                    navigation!.NavigateTo($"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{anterior}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else if (camadaseis != null)
                    navigation!.NavigateTo($"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{anterior}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else if (subsubgrupo != null)
                    navigation!.NavigateTo($"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{anterior}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else if (subgrupo != null)
                    navigation!.NavigateTo($"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{anterior}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else if (grupo != null)
                    navigation!.NavigateTo($"/grupo/{capitulo}/{substory}/{grupo}/{anterior}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else if (substory != null)
                    navigation!.NavigateTo($"/substory/{capitulo}/{substory}/{anterior}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else
                    navigation!.NavigateTo($"/Renderizar/{capitulo}/{anterior}/{auto}/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{compartilhante2}");
            }
            

        }
      
        private void voltarSubgrupos()
        {
            if (camadadez != null && camadadez != 1)
                camadadez = 1;
            else if (camadanove != null && camadanove != 1)
                camadanove = 1;
            else if (camadaoito != null && camadaoito != 1)
                camadaoito = 1;
            else if (camadasete != null && camadasete != 1)
                camadasete = 1;
            else if (camadaseis != null && camadaseis != 1)
                camadaseis = 1;
            else if (subsubgrupo != null && subsubgrupo != 1)
                subsubgrupo = 1;
            else if (subgrupo != null && subgrupo != 1)
                subgrupo = 1;
            else if (grupo != null && grupo != 1)
                grupo = 1;
            else if (substory != null && substory != 1)
                substory = 1;
        }
    
        protected void desligarAuto()
        {
            if (auto == 1)
                desabilitarAuto();
        }

        protected async Task DarUmLike()
        {
            PageLiked pageLiked = new PageLiked
            {
                user = user.Identity!.Name!,
                 capitulo = capitulo,
                 indice = indice,
                 substory = (int) substory!,
                 grupo = grupo,
                 subgrupo = subgrupo,
                 subsubgrupo = subsubgrupo,
                 camadaSeis = camadaseis,
                 camadaSete = camadasete,
                 camadaOito = camadaoito,
                 camadaNove = camadanove,
                 camadaDez = camadadez,
                 verso =(int) vers!
            };
             await Context.AddAsync(pageLiked);
            await Context.SaveChangesAsync();
            repositoryPagina.paginasCurtidas.Add(pageLiked);

            var url = "";
            if (camadadez != null)
                url = $"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            else if (camadanove != null)
                url = $"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            else if (camadaoito != null)
                url = $"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            else if (camadasete != null)
                url = $"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            else if (camadaseis != null)
                url = $"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            else if (subsubgrupo != null)
                url = $"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            else if (subgrupo != null)
                url = $"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            else if (grupo != null)
                url = $"/grupo/{capitulo}/{substory}/{grupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            else if (substory != null)
                url = $"/substory/{capitulo}/{substory}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            navigation!.NavigateTo(url);
        }
        
        protected async Task Unlike()
        {
            PageLiked? page = await Context.PageLiked
                            .FirstOrDefaultAsync(p => p.capitulo == capitulo
                            && p.verso == vers
                            && p.user == user.Identity!.Name);
            if(page != null)
            {
                Context.Remove(page);
                await Context.SaveChangesAsync();
                repositoryPagina.paginasCurtidas.Remove(page);
            }
            var url = "";
            if (camadadez != null)
                url = $"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            else if (camadanove != null)
                url = $"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            else if (camadaoito != null)
                url = $"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            else if (camadasete != null)
                url = $"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            else if (camadaseis != null)
                url = $"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            else if (subsubgrupo != null)
                url = $"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            else if (subgrupo != null)
                url = $"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            else if (grupo != null)
                url = $"/grupo/{capitulo}/{substory}/{grupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            else if (substory != null)
                url = $"/substory/{capitulo}/{substory}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
            navigation!.NavigateTo(url);
        }
             
        protected void acessarPreferenciasUsuario(string usuario)
        {
            var pref = repositoryPagina!.preferencias!
                   .FirstOrDefault(u => u.user == usuario)!;
            usuarios!.Clear();
            var url = "";
            if (camadadez != null)
                url = $"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{usuario}/{pref.p1}/{pref.p2}/{pref.p3}/{pref.p4}/{pref.p5}/{pref.p6}/{pref.p7}/{pref.p8}/{pref.p9}/{pref.p10}";
            else if (camadanove != null)
                url = $"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{usuario}/{pref.p1}/{pref.p2}/{pref.p3}/{pref.p4}/{pref.p5}/{pref.p6}/{pref.p7}/{pref.p8}/{pref.p9}/{pref.p10}";
            else if (camadaoito != null)
                url = $"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{usuario}/{pref.p1}/{pref.p2}/{pref.p3}/{pref.p4}/{pref.p5}/{pref.p6}/{pref.p7}/{pref.p8}/{pref.p9}/{pref.p10}";
            else if (camadasete != null)
                url = $"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{usuario}/{pref.p1}/{pref.p2}/{pref.p3}/{pref.p4}/{pref.p5}/{pref.p6}/{pref.p7}/{pref.p8}/{pref.p9}/{pref.p10}";
            else if (camadaseis != null)
                url = $"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{usuario}/{pref.p1}/{pref.p2}/{pref.p3}/{pref.p4}/{pref.p5}/{pref.p6}/{pref.p7}/{pref.p8}/{pref.p9}/{pref.p10}";
            else if (subsubgrupo != null)
                url = $"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{usuario}/{pref.p1}/{pref.p2}/{pref.p3}/{pref.p4}/{pref.p5}/{pref.p6}/{pref.p7}/{pref.p8}/{pref.p9}/{pref.p10}";
            else if (subgrupo != null)
                url = $"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{usuario}/{pref.p1}/{pref.p2}/{pref.p3}/{pref.p4}/{pref.p5}/{pref.p6}/{pref.p7}/{pref.p8}/{pref.p9}/{pref.p10}";
            else if (grupo != null)
                url = $"/grupo/{capitulo}/{substory}/{grupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{usuario}/{pref.p1}/{pref.p2}/{pref.p3}/{pref.p4}/{pref.p5}/{pref.p6}/{pref.p7}/{pref.p8}/{pref.p9}/{pref.p10}";
            else if (substory != null)
                url = $"/substory/{capitulo}/{substory}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{usuario}/{pref.p1}/{pref.p2}/{pref.p3}/{pref.p4}/{pref.p5}/{pref.p6}/{pref.p7}/{pref.p8}/{pref.p9}/{pref.p10}";
            navigation!.NavigateTo(url);
        }

        protected void alterarQuery(ChangeEventArgs e)
        {
            opcional = e.Value!.ToString()!;

            try
            {
                var num = int.Parse(opcional);
            }
            catch(Exception ex)
            {
                usuarios = repositoryPagina!.preferencias.Where(p => p.pasta == indice_Filtro &&
               p.capitulo == capitulo &&
               p.user.Contains(opcional)).ToList();

                if (string.IsNullOrEmpty(opcional))
                {
                    usuarios.Clear();
                    var url = "";
                    if (camadadez != null)
                        url = $"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
                    else if (camadanove != null)
                        url = $"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
                    else if (camadaoito != null)
                        url = $"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
                    else if (camadasete != null)
                        url = $"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
                    else if (camadaseis != null)
                        url = $"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
                    else if (subsubgrupo != null)
                        url = $"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
                    else if (subgrupo != null)
                        url = $"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
                    else if (grupo != null)
                        url = $"/grupo/{capitulo}/{substory}/{grupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
                    else if (substory != null)
                        url = $"/substory/{capitulo}/{substory}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}";
                    navigation!.NavigateTo(url);
                }
            }
        }

        private int CountLikes( string conexao)
        {

            var _TotalRegistros = 0;
            try
            {
                using (var con = new SqlConnection(conexao))
                {
                    SqlCommand cmd = null;
                    if(substory == null)
                     cmd = new SqlCommand($"SELECT COUNT(*) FROM PageLiked as P  where P.capitulo={capitulo} and P.verso={indice}", con);
                    else
                     cmd = new SqlCommand($"SELECT COUNT(*) FROM PageLiked as P  where P.capitulo={capitulo} and P.verso={vers}", con);
                    
                    con.Open();
                    _TotalRegistros = int.Parse(cmd.ExecuteScalar().ToString());
                    con.Close();
                }
            }
            catch (Exception)
            {
                _TotalRegistros = 0;
            }
            return _TotalRegistros;
        }
            
    }
}
