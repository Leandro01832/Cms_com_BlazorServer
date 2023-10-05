using BlazorServerCms.Data;
using BlazorServerCms.servicos;
using business;
using business.Group;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Models;
using System.Collections.Generic;

namespace BlazorCms.Client.Pages
{
    public class RenderizarBase : ComponentBase
    {
        [Inject] public RepositoryPagina? repositoryPagina { get; set; }
        [Inject] public NavigationManager? navigation { get; set; }
        [Inject] HttpClient? Http { get; set; }
        [Inject] BlazorTimer? Timer { get; set; }
        [Inject] IJSRuntime? js { get; set; }
        public ClassArray Arr = new ClassArray();
        private DemoContextFactory db = new DemoContextFactory();
        private ApplicationDbContext Context;

        protected MarkupString markup;
        protected ElementReference firstInput;
        protected string? Mensagem = null;
        protected string nameGroup = "";

        protected List<Story>? stories;
        protected Pagina? Model;
        protected Filtro? Model2;
        protected string[]? classificacoes = null;
        protected int opcional = 1;
        

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
        protected long ultimaPasta { get; set; }

        [Parameter] public int indiceLivro { get; set; } = 0;

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
            if(repositoryPagina!.paginas!.Where(p => p.Story!.PaginaPadraoLink == capitulo).ToList().Count == 0
                && capitulo > stories!.Last().PaginaPadraoLink)            
                capitulo = 1;
            

            if (repositoryPagina!.paginas!.Where(p => p.Story!.PaginaPadraoLink == capitulo).ToList().Count == 0)
            {
                Mensagem = "aguarde um momento...";
                var lista = await repositoryPagina.buscarCapitulo(capitulo);
                repositoryPagina.paginas!.AddRange(lista);

            }
            else
            {
                Mensagem = null;
                await renderizar();
            }


            if (repositoryPagina!.paginas!.Where(p => p.Story!.PaginaPadraoLink == capitulo).ToList().Count > 0
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
            if (repositoryPagina!.paginas == null || !repositoryPagina.aguarde &&
               repositoryPagina.paginas.Where(p => p.Story!.PaginaPadraoLink == capitulo).ToList().Count == 0)
            {

                repositoryPagina.aguarde = true;
                Mensagem = "aguarde um momento...";
                if (repositoryPagina.paginas == null)
                {
                    repositoryPagina.paginas = new List<Pagina>();
                    var lista = await repositoryPagina.buscarCapitulo(0);
                    repositoryPagina.paginas.AddRange(lista);
                    repositoryPagina.aguarde = false;
                    await repositoryPagina.buscarCapitulos();
                }
                else
                {
                    var lista = await repositoryPagina.buscarCapitulo(capitulo);
                    repositoryPagina.paginas.AddRange(lista);
                    repositoryPagina.aguarde = false;
                }
            }
            else if (repositoryPagina.paginas!.Where(p => p.Story!.PaginaPadraoLink == capitulo).ToList().Count == 0)
                Mensagem = "aguarde um momento...";
            stories = await Context.Story!.Where(st => st.Nome != "Padrao").OrderBy(st => st.Id).ToListAsync();

            

        }

        protected async Task renderizar()
        {
            var list = repositoryPagina!.paginas!.Where(p => p.Story!.PaginaPadraoLink == capitulo).ToList();
            var filtros = repositoryPagina!.paginas!.Where(p => p.Story!.PaginaPadraoLink == capitulo)
                   .First().Story!.Filtro!.OrderBy(f => f.Id).ToList();
            Pagina pag = list.First();
            proximo = indice + 1;
            anterior = indice - 1;

            if (filtrar == null && substory == null)
            {        
                
                 Model = list.Skip((int)indice - 1).FirstOrDefault();


                if (Model == null)
                    nameStory = list.First().Story!.Nome;
                else
                    nameStory = Model.Story.Nome;
                              
                if(outroHorizonte == 1)
                 Model2 = filtros.Skip((int)indice - 1).FirstOrDefault();

                if(Model != null)
                {
                    long id_ultimo = 0;
                    if (Model.CamadaDezId != null) id_ultimo = (long) Model.CamadaDezId;
                    else if (Model.CamadaNoveId != null) id_ultimo = (long)Model.CamadaNoveId;
                    else if (Model.CamadaOitoId != null) id_ultimo = (long)Model.CamadaOitoId;
                    else if (Model.CamadaSeteId != null) id_ultimo = (long)Model.CamadaSeteId;
                    else if (Model.CamadaSeisId != null) id_ultimo = (long)Model.CamadaSeisId;
                    else if (Model.SubSubGrupoId != null) id_ultimo = (long)Model.SubSubGrupoId;
                    else if (Model.SubGrupoId != null) id_ultimo = (long)Model.SubGrupoId;
                    else if (Model.GrupoId != null) id_ultimo = (long)Model.GrupoId;
                    else if (Model.SubStoryId != null) id_ultimo = (long)Model.SubStoryId;
                    if(id_ultimo != 0)
                    {
                        var fil = filtros.First(f => f.Id == id_ultimo);
                        ultimaPasta = filtros.IndexOf(fil) + 1;
                    }
                    else ultimaPasta = 0;
                }
                

                if (Model == null && Model2 == null)
                {
                    Mensagem = "Por favor digite um numero menor.";
                    return;
                }

                quantidadePaginas = list.Count();
                quantidadeFiltros = filtros.Count;

                if (outroHorizonte == 0)
                    quantidadeLista = quantidadePaginas;
                else quantidadeLista = quantidadeFiltros;

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
                    var page = repositoryPagina.paginas!.FirstOrDefault(p => p.Id == Model.Comentario);
                    
                    CapituloComentario = page!.Story!.PaginaPadraoLink;
                    var paginas = repositoryPagina.paginas!
                    .Where(p => p.Story!.PaginaPadraoLink == page.Story.PaginaPadraoLink)
                    .OrderBy(p => p.Id)
                    .ToList();
                    VersoComentario = paginas.IndexOf(page) + 1;

                    if (Model.SubStory != null)
                    {
                        if (Model.CamadaDez != null)
                        {
                            classificacoes = new string[9];
                            classificacoes[0] = Model.SubStory.Nome!;
                            classificacoes[1] = Model.Grupo!.Nome!;
                            classificacoes[2] = Model.SubGrupo!.Nome!;
                            classificacoes[3] = Model.SubSubGrupo!.Nome!;
                            classificacoes[4] = Model.CamadaSeis!.Nome!;
                            classificacoes[5] = Model.CamadaSete!.Nome!;
                            classificacoes[6] = Model.CamadaOito!.Nome!;
                            classificacoes[7] = Model.CamadaNove!.Nome!;
                            classificacoes[8] = Model.CamadaDez!.Nome!;
                        }
                        else
                        if (Model.CamadaNove != null)
                        {
                            classificacoes = new string[8];
                            classificacoes[0] = Model.SubStory!.Nome!;
                            classificacoes[1] = Model.Grupo!.Nome!;
                            classificacoes[2] = Model.SubGrupo!.Nome!;
                            classificacoes[3] = Model.SubSubGrupo!.Nome!;
                            classificacoes[4] = Model.CamadaSeis!.Nome!;
                            classificacoes[5] = Model.CamadaSete!.Nome!;
                            classificacoes[6] = Model.CamadaOito!.Nome!;
                            classificacoes[7] = Model.CamadaNove!.Nome!;
                        }
                        else
                        if (Model.CamadaOito != null)
                        {
                            classificacoes = new string[7];
                            classificacoes[0] = Model.SubStory!.Nome!;
                            classificacoes[1] = Model.Grupo!.Nome!;
                            classificacoes[2] = Model.SubGrupo!.Nome!;
                            classificacoes[3] = Model.SubSubGrupo!.Nome!;
                            classificacoes[4] = Model.CamadaSeis!.Nome!;
                            classificacoes[5] = Model.CamadaSete!.Nome!;
                            classificacoes[6] = Model.CamadaOito!.Nome!;
                        }
                        else
                        if (Model.CamadaSete != null)
                        {
                            classificacoes = new string[6];
                            classificacoes[0] = Model.SubStory!.Nome!;
                            classificacoes[1] = Model.Grupo!.Nome!;
                            classificacoes[2] = Model.SubGrupo!.Nome!;
                            classificacoes[3] = Model.SubSubGrupo!.Nome!;
                            classificacoes[4] = Model.CamadaSeis!.Nome!;
                            classificacoes[5] = Model.CamadaSete!.Nome!;
                        }
                        else
                        if (Model.CamadaSeis != null)
                        {
                            classificacoes = new string[5];
                            classificacoes[0] = Model.SubStory!.Nome!;
                            classificacoes[1] = Model.Grupo!.Nome!;
                            classificacoes[2] = Model.SubGrupo!.Nome!;
                            classificacoes[3] = Model.SubSubGrupo!.Nome!;
                            classificacoes[4] = Model.CamadaSeis!.Nome!;
                        }
                        else
                        if (Model.SubSubGrupo != null)
                        {
                            classificacoes = new string[4];
                            classificacoes[0] = Model.SubStory!.Nome!;
                            classificacoes[1] = Model.Grupo!.Nome!;
                            classificacoes[2] = Model.SubGrupo!.Nome!;
                            classificacoes[3] = Model.SubSubGrupo!.Nome!;
                        }
                        else
                        if (Model.SubGrupo != null)
                        {
                            classificacoes = new string[3];
                            classificacoes[0] = Model.SubStory!.Nome!;
                            classificacoes[1] = Model.Grupo!.Nome!;
                            classificacoes[2] = Model.SubGrupo!.Nome!;
                        }
                        else
                        if (Model.Grupo != null)
                        {
                            classificacoes = new string[2];
                            classificacoes[0] = Model.SubStory!.Nome!;
                            classificacoes[1] = Model.Grupo!.Nome!;
                        }
                        else
                        if (Model.SubStory != null)
                        {
                            classificacoes = new string[1];
                            classificacoes[0] = Model.SubStory!.Nome!;
                        }
                    }

                }

            }
            else
            {
                List<Pagina>? listaComConteudo = null;
                if (filtrar != null)
                {
                    lista = 1;
                    var livro = await Context.Livro!.FirstOrDefaultAsync(l => l.Compartilhando);
                    if (livro != null && redirecionar == null)
                        navigation!.NavigateTo($"{livro.url}/filtro/{livro.Capitulo}/{filtrar}/0/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0/1");

                    removePreferencia();
                    preferencia = 0;
                    int[] arr = null;
                    var indiceFiltro = int.Parse(filtrar.Replace("pasta-", ""));
                    var fi = pag.Story!.Filtro!.OrderBy(f => f.Id).ToList()[indiceFiltro - 1];
                    if(fi.CamadaDezId != null)
                    arr = Arr.RetornarArray(pag.Story, false, (long)fi.CamadaDezId, capitulo, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                    else if (fi.CamadaNoveId != null)
                        arr = Arr.RetornarArray(pag.Story, false, (long)fi.CamadaNoveId, capitulo, 1, 1, 1, 1, 1, 1, 1, 1);
                    else if (fi.CamadaOitoId != null)
                        arr = Arr.RetornarArray(pag.Story, false, (long)fi.CamadaOitoId, capitulo, 1, 1, 1, 1, 1, 1, 1);
                    else if (fi.CamadaSeteId != null)
                        arr = Arr.RetornarArray(pag.Story, false, (long)fi.CamadaSeteId, capitulo, 1, 1, 1, 1, 1, 1);
                    else if (fi.CamadaSeisId != null)
                        arr = Arr.RetornarArray(pag.Story, false, (long)fi.CamadaSeisId, capitulo, 1, 1, 1, 1, 1);
                    else if (fi.SubSubGrupoId != null)
                        arr = Arr.RetornarArray(pag.Story, false, (long)fi.SubSubGrupoId, capitulo, 1, 1, 1,  1);
                    else if (fi.SubGrupoId != null)
                        arr = Arr.RetornarArray(pag.Story, false, (long)fi.SubGrupoId, capitulo, 1, 1,  1);
                    else if (fi.GrupoId != null)
                        arr = Arr.RetornarArray(pag.Story, false, (long)fi.GrupoId, capitulo, 1, 1);
                    else if (fi.SubStoryId != null)
                        arr = Arr.RetornarArray(pag.Story, false, (long)fi.SubStoryId, capitulo, 1);
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
                        navigation!.NavigateTo($"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
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
                        navigation!.NavigateTo($"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
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
                        navigation!.NavigateTo($"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                    }
                    else if (arr.Length == 7)
                    {
                        substory = arr[1];
                        grupo = arr[2];
                        subgrupo = arr[3];
                        subsubgrupo = arr[4];
                        camadaseis = arr[5];
                        camadasete = arr[6];
                        navigation!.NavigateTo($"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                    }
                    else if (arr.Length == 6)
                    {
                        substory = arr[1];
                        grupo = arr[2];
                        subgrupo = arr[3];
                        subsubgrupo = arr[4];
                        camadaseis = arr[5];
                        navigation!.NavigateTo($"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                    }
                    else if (arr.Length == 5)
                    {
                        substory = arr[1];
                        grupo = arr[2];
                        subgrupo = arr[3];
                        subsubgrupo = arr[4];
                        navigation!.NavigateTo($"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                    }
                    else if (arr.Length == 4)
                    {
                        substory = arr[1];
                        grupo = arr[2];
                        subgrupo = arr[3];
                        navigation!.NavigateTo($"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                    }
                    else if (arr.Length == 3)
                    {
                        substory = arr[1];
                        grupo = arr[2];
                        navigation!.NavigateTo($"/grupo/{capitulo}/{substory}/{grupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                    }
                    else if (arr.Length == 2)
                    {
                        substory = arr[1];
                        navigation!.NavigateTo($"/substory/{capitulo}/{substory}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                    }
                }
                else if (substory != null)
                {
                    SubStory? group = null;
                    Grupo? group2 = null;
                    SubGrupo? group3 = null;
                    SubSubGrupo? group4 = null;
                    CamadaSeis? group5 = null;
                    CamadaSete? group6 = null;
                    CamadaOito? group7 = null;
                    CamadaNove? group8 = null;
                    CamadaDez? group9 = null;

                    group = pag.Story!.SubStory!.Where(str => str.Pagina != null && str.Pagina!.Count > 0).Skip((int)substory! - 1).First();
                    nameGroup = group.Nome!;
                    if (preferencia == 0)
                    {
                        if (group!.Pagina!.Where(p => p.Produto != null).ToList().Count > 0 && lista == 1 && pag.ContentUser == null)
                            listaComConteudo = retornarListaComConteudo(list,
                         group.Pagina!.ToList(), (int)camadadez!);
                        else listaComConteudo = group.Pagina!.ToList();
                    }

                    if (grupo != null )
                    {
                            group2 = group!.Grupo!.Where(str => str.Pagina != null && str.Pagina!.Count > 0).Skip((int)grupo! - 1).First();
                            nameGroup = group2.Nome!;
                            if(preferencia == 0)
                        {
                            if (group2!.Pagina!.Where(p => p.Produto != null).ToList().Count > 0 && lista == 1 && pag.ContentUser == null)
                                listaComConteudo = retornarListaComConteudo(list,
                             group2.Pagina!.ToList(), (int)camadadez!);
                            else listaComConteudo = group2.Pagina!.ToList();
                        }
                            
                    }

                    if (subgrupo != null )
                    {
                            group3 = group2!.SubGrupo!.Where(str => str.Pagina != null && str.Pagina!.Count > 0).Skip((int)subgrupo! - 1).First();
                            nameGroup = group3.Nome!;
                        if (preferencia == 0)
                        {
                            if (group3!.Pagina!.Where(p => p.Produto != null).ToList().Count > 0 && lista == 1 && pag.ContentUser == null)
                                listaComConteudo = retornarListaComConteudo(list,
                             group3.Pagina!.ToList(), (int)camadadez!);
                            else listaComConteudo = group3.Pagina!.ToList();
                        }
                            
                    }

                    if (subsubgrupo != null )
                    {
                            group4 = group3!.SubSubGrupo!.Where(str => str.Pagina != null && str.Pagina!.Count > 0).Skip((int)subsubgrupo! - 1).First();
                            nameGroup = group4.Nome!;
                        if (preferencia == 0)
                        {
                            if (group4!.Pagina!.Where(p => p.Produto != null).ToList().Count > 0 && lista == 1 && pag.ContentUser == null)
                                listaComConteudo = retornarListaComConteudo(list,
                             group4.Pagina!.ToList(), (int)camadadez!);
                            else listaComConteudo = group4.Pagina!.ToList();
                        }
                            
                    }

                    if (camadaseis != null )
                    {
                            group5 = group4!.CamadaSeis!.Where(str => str.Pagina != null && str.Pagina!.Count > 0).Skip((int)camadaseis! - 1).First();
                            nameGroup = group5.Nome!;
                        if (preferencia == 0)
                        {
                            if (group5!.Pagina!.Where(p => p.Produto != null).ToList().Count > 0 && lista == 1 && pag.ContentUser == null)
                                listaComConteudo = retornarListaComConteudo(list,
                             group5.Pagina!.ToList(), (int)camadadez!);
                            else listaComConteudo = group5.Pagina!.ToList();
                        }
                                                   
                    }

                    if (camadasete != null )
                    {
                            group6 = group5!.CamadaSete!.Where(str => str.Pagina != null && str.Pagina!.Count > 0).Skip((int)camadasete! - 1).First();
                            nameGroup = group6.Nome!;
                        if (preferencia == 0)
                        {
                            if (group6!.Pagina!.Where(p => p.Produto != null).ToList().Count > 0 && lista == 1 && pag.ContentUser == null)
                                listaComConteudo = retornarListaComConteudo(list,
                             group6.Pagina!.ToList(), (int)camadadez!);
                            else listaComConteudo = group6.Pagina!.ToList();
                        }
                                                  
                    }

                    if (camadaoito != null )
                    {
                            group7 = group6!.CamadaOito!.Where(str => str.Pagina != null && str.Pagina!.Count > 0).Skip((int)camadaoito! - 1).First();
                            nameGroup = group7.Nome!;
                        if (preferencia == 0)
                        {
                            if (group7!.Pagina!.Where(p => p.Produto != null).ToList().Count > 0 && lista == 1 && pag.ContentUser == null)
                                listaComConteudo = retornarListaComConteudo(list,
                             group7.Pagina!.ToList(), (int)camadadez!);
                            else listaComConteudo = group7.Pagina!.ToList();
                        }
                            
                    }

                    if (camadanove != null )
                    {
                            group8 = group7!.CamadaNove!.Where(str => str.Pagina != null && str.Pagina!.Count > 0).Skip((int)camadanove! - 1).First();
                            nameGroup = group8.Nome!;
                        if (preferencia == 0)
                        {
                            if (group8!.Pagina!.Where(p => p.Produto != null).ToList().Count > 0 && lista == 1 && pag.ContentUser == null)
                                listaComConteudo = retornarListaComConteudo(list,
                             group8.Pagina!.ToList(), (int)camadadez!);
                            else listaComConteudo = group8.Pagina!.ToList();
                        }
                                                   
                    }

                    if (camadadez != null )
                    {
                            group9 = group8!.CamadaDez!.Where(str => str.Pagina != null && str.Pagina!.Count > 0).Skip((int)camadadez! - 1).First();
                            nameGroup = group9.Nome!;
                        if (preferencia == 0)
                        {
                            if (group9!.Pagina!.Where(p => p.Produto != null).ToList().Count > 0 && lista == 1 && pag.ContentUser == null)
                                listaComConteudo = retornarListaComConteudo(list,
                             group9.Pagina!.ToList(), (int)camadadez!);
                            else listaComConteudo = group9.Pagina!.ToList();
                        }
                                                 
                    }

                    if(preferencia == 1) listaComConteudo = retornarListaPreferencial();

                    Pagina pag2 = listaComConteudo!.OrderBy(p => p.Id).Skip((int)indice - 1).First();

                    Model = list.First(p => p.Id == pag2.Id);
                    vers = list.IndexOf(Model) + 1;

                    quantidadeLista = listaComConteudo!.Count;
                    html = await repositoryPagina!.renderizarPagina(Model);
                    markup = new MarkupString(html);
                    proximo = indice + 1;
                    anterior = indice - 1;

                    if (Model != null)
                    {
                        long id_ultimo = 0;
                        if (Model.CamadaDezId != null) id_ultimo = (long)Model.CamadaDezId;
                        else if (Model.CamadaNoveId != null) id_ultimo = (long)Model.CamadaNoveId;
                        else if (Model.CamadaOitoId != null) id_ultimo = (long)Model.CamadaOitoId;
                        else if (Model.CamadaSeteId != null) id_ultimo = (long)Model.CamadaSeteId;
                        else if (Model.CamadaSeisId != null) id_ultimo = (long)Model.CamadaSeisId;
                        else if (Model.SubSubGrupoId != null) id_ultimo = (long)Model.SubSubGrupoId;
                        else if (Model.SubGrupoId != null) id_ultimo = (long)Model.SubGrupoId;
                        else if (Model.GrupoId != null) id_ultimo = (long)Model.GrupoId;
                        else if (Model.SubStoryId != null) id_ultimo = (long)Model.SubStoryId;
                        if (id_ultimo != 0)
                        {
                            var fil = filtros.First(f => f.Id == id_ultimo);
                            ultimaPasta = filtros.IndexOf(fil) + 1;
                        }
                        else ultimaPasta = 0;
                    }

                    Filtro Filtro = null;
                    if (group8 != null)
                        Filtro = pag.Story!.Filtro!.First(f => f.CamadaNoveId == group8!.Id);
                    else if (group7 != null)
                        Filtro = pag.Story!.Filtro!.First(f => f.CamadaOitoId == group7!.Id);
                    else if (group6 != null)
                        Filtro = pag.Story!.Filtro!.First(f => f.CamadaSeteId == group6!.Id);
                    else if (group5 != null)
                        Filtro = pag.Story!.Filtro!.First(f => f.CamadaSeisId == group5!.Id);
                    else if (group4 != null)
                        Filtro = pag.Story!.Filtro!.First(f => f.SubSubGrupoId == group4!.Id);
                    else if (group3 != null)
                        Filtro = pag.Story!.Filtro!.First(f => f.SubGrupoId == group3!.Id);
                    else if (group2 != null)
                        Filtro = pag.Story!.Filtro!.First(f => f.GrupoId == group2!.Id);
                    else if (group != null)
                        Filtro = pag.Story!.Filtro!.First(f => f.SubStoryId == group!.Id);
                    indice_Filtro = pag.Story.Filtro!.OrderBy(f => f.Id).ToList().IndexOf(Filtro) + 1;
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
            var list = repositoryPagina!.paginas!
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
                    if (p.Content != null && p.Content.Contains("iframe"))
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
                        navigation!.NavigateTo($"/Renderizar/{capitulo}/1/1/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}");
                   
                    else if (capitulo != 0 && indice >= quantidadeLista && outroHorizonte == 0)
                        navigation!.NavigateTo($"/Renderizar/{capitulo + 1}/1/1/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}");
                    else if (capitulo != 0 && indice >= quantidadeLista && outroHorizonte == 1)
                        navigation!.NavigateTo($"/Renderizar/{capitulo}/1/1/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}");
                    else
                        navigation!.NavigateTo($"/Renderizar/{capitulo}/{proximo}/1/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}");
                }
                else
                {
                    navegarSubgrupos(false);
                }
            }

            Console.WriteLine("Timer Elapsed.");
            Timer!._timer!.Elapsed -= _timer_Elapsed;
        }

        private void navegarSubgrupos(bool somenteSubgrupos)
        {         
            if (camadadez != null)
            {
                if(indice >= quantidadeLista || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory!,
                        grupo, subgrupo, subsubgrupo, camadaseis, camadasete, camadaoito, camadanove, camadadez);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/camadadez/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/{arr[5]}/{arr[6]}/{arr[7]}/{arr[8]}/{arr[9]}/1/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    else
                        navigation!.NavigateTo($"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/1/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");

                }
                else
                {
                    navigation!
                    .NavigateTo($"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{proximo}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                }
            }
            else  if (camadanove != null)
            {
                if(indice >= quantidadeLista || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory!,
                        grupo, subgrupo, subsubgrupo, camadaseis, camadasete, camadaoito, camadanove);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/camadanove/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/{arr[5]}/{arr[6]}/{arr[7]}/{arr[8]}/1/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    else
                        navigation!.NavigateTo($"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/1/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");

                }
                else
                {
                    navigation!
                   .NavigateTo($"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{proximo}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                }
            }
            else  if (camadaoito != null)
            {
                if(indice >= quantidadeLista || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory!,
                        grupo, subgrupo, subsubgrupo, camadaseis, camadasete, camadaoito);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/camadaoito/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/{arr[5]}/{arr[6]}/{arr[7]}/1/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    else
                        navigation!.NavigateTo($"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/1/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");

                }
                else
                {
                    navigation!
                  .NavigateTo($"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{proximo}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                }
            }
            else  if (camadasete != null )
            {
                if(indice >= quantidadeLista || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory!,
                        grupo, subgrupo, subsubgrupo, camadaseis, camadasete);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/camadasete/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/{arr[5]}/{arr[6]}/1/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    else
                        navigation!.NavigateTo($"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/1/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");

                }
                else
                {
                    navigation!
                .NavigateTo($"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{proximo}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                }
            }
            else  if (camadaseis != null)
            {
                if(indice >= quantidadeLista || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory!,
                        grupo, subgrupo, subsubgrupo, camadaseis);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/camadaseis/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/{arr[5]}/1/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    else
                        navigation!.NavigateTo($"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/1/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");

                }
                else
                {
                    navigation!
               .NavigateTo($"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{proximo}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                }
            }
            else  if (subsubgrupo != null)
            {
                if(indice >= quantidadeLista || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory!,
                        grupo, subgrupo, subsubgrupo);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/subsubgrupo/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/1/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    else
                        navigation!.NavigateTo($"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/1/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");

                }
                else
                {
                    navigation!
              .NavigateTo($"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{proximo}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                }
            }
            else if (subgrupo != null)
            {
                if(indice >= quantidadeLista || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory!,
                        grupo, subgrupo);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/subgrupo/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/1/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    else
                        navigation!.NavigateTo($"/grupo/{capitulo}/{substory}/{grupo}/1/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");

                }
                else
                {
                    navigation!
            .NavigateTo($"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{proximo}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                }
            }
            else if (grupo != null)
            {
                if(indice >= quantidadeLista || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory!, grupo);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/grupo/{arr[0]}/{arr[1]}/{arr[2]}/1/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    else
                        navigation!.NavigateTo($"/substory/{capitulo}/{substory}/1/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");

                }
                else
                {
                    navigation!
           .NavigateTo($"/grupo/{capitulo}/{substory}/{grupo}/{proximo}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                }
            }
            else  if (substory != null)
            {
                if (indice >= quantidadeLista || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/substory/{arr[0]}/{arr[1]}/1/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                    else
                        navigation!.NavigateTo($"/renderizar/{capitulo}/1/1/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}");

                }
                else
                {
                    navigation!
             .NavigateTo($"/substory/{capitulo}/{substory}/{proximo}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                }
            }
        }

        protected void TeclaPressionada(KeyboardEventArgs args)
        {
            
            if (substory == null)
            {
                
                if (args.Key == "Enter" && capitulo == 0)
                {
                    navigation!.NavigateTo($"/Renderizar/{indice}/1/1/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}");
                }
                else if (args.Key == "Enter")
                {
                    navigation!.NavigateTo($"/Renderizar/0/{capitulo}/1/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}");
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
            auto = 0;            
            var url = $"/Renderizar/{capitulo}/{opcional}/{auto}/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}";
            navigation!.NavigateTo(url);

        }

        protected void habilitarAuto()
        {
            Timer!.SetTimerAuto();
            Timer!.desligarAuto!.Elapsed += desligarAuto_Elapsed;
            navigation!.NavigateTo($"/Renderizar/{capitulo}/{indice}/1/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}");
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
       
        private void removePreferencia()
        {
            p1 = 0; p2 = 0; p3 = 0; p4 = 0; p5 = 0;
            p6 = 0; p7 = 0; p8 = 0; p9 = 0; p10 = 0;
        }

        protected async void marcarPreferencia(int Versiculo)
        {
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

            if(camadadez != null)
            navigation!
                    .NavigateTo($"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
            else if (camadanove != null)
                navigation!
                        .NavigateTo($"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");

            else  if (camadaoito != null)
                navigation!
                        .NavigateTo($"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");

            else if (camadasete != null)
                navigation!
                        .NavigateTo($"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");

            else if (camadaseis != null)
                navigation!
                        .NavigateTo($"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
               
            else if (subsubgrupo != null)
                navigation!
                        .NavigateTo($"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");

            else if (subgrupo != null)
                navigation!
                        .NavigateTo($"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");

            else if (grupo != null)
                navigation!
                        .NavigateTo($"/grupo/{capitulo}/{substory}/{grupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");

            else if (substory != null)
                navigation!
                        .NavigateTo($"/substory/{capitulo}/{substory}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
        }

        protected async void acessarPasta()
        {
                indice_Filtro = indice;
                var fil = repositoryPagina!.paginas!.FirstOrDefault(p =>  p.SubStoryId    == Model2!.Id);
                var fil2 = repositoryPagina!.paginas!.FirstOrDefault(p => p.GrupoId       == Model2!.Id);
                var fil3 = repositoryPagina!.paginas!.FirstOrDefault(p => p.SubGrupoId    == Model2!.Id);
                var fil4 = repositoryPagina!.paginas!.FirstOrDefault(p => p.SubSubGrupoId == Model2!.Id);
                var fil5 = repositoryPagina!.paginas!.FirstOrDefault(p => p.CamadaSeisId  == Model2!.Id);
                var fil6 = repositoryPagina!.paginas!.FirstOrDefault(p => p.CamadaSeteId  == Model2!.Id);
                var fil7 = repositoryPagina!.paginas!.FirstOrDefault(p => p.CamadaOitoId  == Model2!.Id);
                var fil8 = repositoryPagina!.paginas!.FirstOrDefault(p => p.CamadaNoveId  == Model2!.Id);
                var fil9 = repositoryPagina!.paginas!.FirstOrDefault(p => p.CamadaDezId   == Model2!.Id);

                if(fil == null && fil2 == null && fil3 == null &&  fil4 == null && fil5 == null && fil6 == null
                    && fil7 == null && fil8 == null && fil9 == null)
                {
                    await js!.InvokeAsync<object>("DarAlert", $"Esta pasta não possui versiculos");
                navigation!.NavigateTo($"/renderizar/{capitulo}/{indice}/{auto}/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}");
                }
                else
                {
                if (auto == 1)
                 desabilitarAuto();
                navigation!.NavigateTo($"/filtro/{capitulo}/pasta-{indice_Filtro}/{preferencia}/0/{dominio}/{compartilhante}/{compartilhante2}/0/0/0/0/0/0/0/0/0/0");
                        
                }
            
        }
        protected void listarPasta()
        {
            if (auto == 1)
                desabilitarAuto();
            navigation!.NavigateTo($"/lista-filtro/1/teste/1/11/20/{dominio}/{compartilhante}/{compartilhante2}/{capitulo}/{indice_Filtro}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
        }

        protected void acessarHorizontePastas()
        {
            if (auto == 1)
                desabilitarAuto();
            outroHorizonte = 1;
            navigation!.NavigateTo($"/renderizar/{capitulo}/1/0/11/1/{outroHorizonte}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}");
        }

        protected void acessarHorizonteVersos()
        {
            if (auto == 1)
                desabilitarAuto();
            outroHorizonte = 0;
            navigation!.NavigateTo($"/renderizar/{capitulo}/1/0/11/1/{outroHorizonte}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}");
        }
           
        protected int buscarCamada(Filtro fil)
        {
            if (fil.SubStoryId != null)
                return 1;
          else  if (fil.GrupoId != null)
               return 2;
          else  if (fil.SubGrupoId != null)
               return 3;
          else  if (fil.SubSubGrupoId != null)
               return 4;
          else  if (fil.CamadaSeisId != null)
               return 5;
          else  if (fil.CamadaSeteId != null)
               return 6;
          else  if (fil.CamadaOitoId != null)
                return 7;
          else  if (fil.CamadaNoveId != null)
                return 8;
          else  if (fil.CamadaDezId != null)
                return 9;

            return 0;
        }    
   
        protected int? buscarPastaFiltrada(int camada)
        {
            long? IdGrupo = 0;
            Pagina pag = null;
            if (camada == 1)
                pag = repositoryPagina!.paginas!.FirstOrDefault(p => p.SubStoryId == Model2!.SubStoryId);
           else if (camada == 2)
                pag = repositoryPagina!.paginas!.FirstOrDefault(p => p.GrupoId == Model2!.GrupoId);
            else if (camada == 3)
                pag = repositoryPagina!.paginas!.FirstOrDefault(p => p.SubGrupoId == Model2!.SubGrupoId);
            else if (camada == 4)
                pag = repositoryPagina!.paginas!.FirstOrDefault(p => p.SubSubGrupoId == Model2!.SubSubGrupoId);
            else if (camada == 5)
                pag = repositoryPagina!.paginas!.FirstOrDefault(p => p.CamadaSeisId == Model2!.CamadaSeisId);
            else if (camada == 6)
                pag = repositoryPagina!.paginas!.FirstOrDefault(p => p.CamadaSeteId == Model2!.CamadaSeteId);
            else if (camada == 7)
                pag = repositoryPagina!.paginas!.FirstOrDefault(p => p.CamadaOitoId == Model2!.CamadaOitoId);
            else if (camada == 8)
                pag = repositoryPagina!.paginas!.FirstOrDefault(p => p.CamadaNoveId == Model2!.CamadaNoveId);
            else if (camada == 9)
                pag = repositoryPagina!.paginas!.FirstOrDefault(p => p.CamadaDezId == Model2!.CamadaDezId);

            if (pag != null)
            {
                if (camada == 2)
                    IdGrupo = pag!.SubStoryId;
                else if (camada == 3)
                    IdGrupo = pag!.GrupoId;
                else if (camada == 4)
                    IdGrupo = pag!.SubGrupoId;
                else if (camada == 5)
                    IdGrupo = pag!.SubSubGrupoId;
                else if (camada == 6)
                    IdGrupo = pag!.CamadaSeisId;
                else if (camada == 7)
                    IdGrupo = pag!.CamadaSeteId;
                else if (camada == 8)
                    IdGrupo = pag!.CamadaOitoId;
                else if (camada == 9)
                    IdGrupo = pag!.CamadaNoveId;

                if(camada != 1)
                {
                    var filtros = repositoryPagina!.paginas!.Where(p => p.Story!.PaginaPadraoLink == capitulo)
                       .First().Story!.Filtro!.OrderBy(f => f.Id).ToList();
                    var fil = filtros!.First(f => f.Id == IdGrupo);
                    var pasta = filtros!.IndexOf(fil) + 1;

                    return pasta;
                }
                return 0;                
            }
            else
                return null;            
        }
   
        protected void buscarProximo()
        {
            if (proximo <= quantidadeLista)
            {
                if (camadadez != null)
                    navigation!.NavigateTo($"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{proximo}/0/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else if (camadanove != null)
                    navigation!.NavigateTo($"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{proximo}/0/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else if (camadaoito != null)
                    navigation!.NavigateTo($"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{proximo}/0/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else if (camadasete != null)
                    navigation!.NavigateTo($"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{proximo}/0/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else if (camadaseis != null)
                    navigation!.NavigateTo($"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{proximo}/0/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else if (subsubgrupo != null)
                    navigation!.NavigateTo($"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{proximo}/0/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else if (subgrupo != null)
                    navigation!.NavigateTo($"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{proximo}/0/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else if (grupo != null)
                    navigation!.NavigateTo($"/grupo/{capitulo}/{substory}/{grupo}/{proximo}/0/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else if (substory != null)
                    navigation!.NavigateTo($"/substory/{capitulo}/{substory}/{proximo}/0/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else
                    navigation!.NavigateTo($"/Renderizar/{capitulo}/{proximo}/0/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}");
            }
            else if(substory != null)
                navegarSubgrupos(false);
            else
                navigation!.NavigateTo($"/Renderizar/{capitulo + 1}/1/0/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}");


        }

        protected void buscarAnterior()
        {
            
            if (camadadez != null)
                navigation!.NavigateTo($"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{anterior}/0/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
            else if (camadanove != null)
                navigation!.NavigateTo($"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{anterior}/0/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
            else if (camadaoito != null)
                navigation!.NavigateTo($"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{anterior}/0/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
            else if (camadasete != null)
                navigation!.NavigateTo($"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{anterior}/0/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
            else if (camadaseis != null)
                navigation!.NavigateTo($"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{anterior}/0/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
            else if (subsubgrupo != null)
                navigation!.NavigateTo($"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{anterior}/0/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
            else if (subgrupo != null)
                navigation!.NavigateTo($"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{anterior}/0/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
            else if (grupo != null)
                navigation!.NavigateTo($"/grupo/{capitulo}/{substory}/{grupo}/{anterior}/0/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
            else if (substory != null)
                navigation!.NavigateTo($"/substory/{capitulo}/{substory}/{anterior}/0/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
            else
                navigation!.NavigateTo($"/Renderizar/{capitulo}/{anterior}/0/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/{dominio}/{compartilhante}/{compartilhante2}");

        }
    
        protected void desligarAuto()
        {
            if (auto == 1)
                desabilitarAuto();
        }
    }
}
