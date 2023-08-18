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

namespace BlazorCms.Client.Pages
{
    public class RenderizarBase : ComponentBase
    {
        [Inject] public RepositoryPagina? repositoryPagina { get; set; }
        [Inject] public NavigationManager? navigation { get; set; }
        [Inject] PareamentServices? servico { get; set; }
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
        
        protected Pagina? Model;
        protected string[]? classificacoes = null;
        protected int opcional = 1;
        
        protected string? html { get; set; } = "";
        protected int? indice_Filtro { get; set; } = null;
        protected int? vers { get; set; } = null;
        protected int? CapituloComentario { get; set; } = null;
        protected int? VersoComentario { get; set; } = null;
        protected int quantidadePaginas { get; set; }
        protected int anterior { get; set; }
        protected int proximo { get; set; }

        [Parameter] public int lista { get; set; } = 1;
        [Parameter] public int timeproduto { get; set; } = 1; [Parameter] public int desconto { get; set; } = 1;
        [Parameter] public int indice { get; set; } = 1; [Parameter] public int capitulo { get; set; } = 1;

        [Parameter] public int? substory { get; set; } = null; [Parameter] public int? grupo { get; set; } = null;

        [Parameter] public int? subgrupo { get; set; } = null; [Parameter] public int? subsubgrupo { get; set; } = null;

        [Parameter] public int? camadaseis { get; set; } = null; [Parameter] public int? camadasete { get; set; } = null;

        [Parameter] public int? camadaoito { get; set; } = null; [Parameter] public int? camadanove { get; set; } = null;

        [Parameter] public int? camadadez { get; set; } = null; [Parameter] public int auto { get; set; } = 1;

        [Parameter] public string? redirecionar { get; set; } = null; [Parameter] public string? compartilhante { get; set; } = "user";

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
                if (indice >= quantidadePaginas)
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
            if (compartilhante == null)
            {
                compartilhante = repositoryPagina!.buscarDominio();
                desconto = 0;
            }
            if (auto == 0 && Timer!.desligarAuto! != null 
                && Timer!.desligarAuto!.Enabled == true)
            {
                Timer!.desligarAuto!.Elapsed -= desligarAuto_Elapsed;
                Timer!.desligarAuto!.Enabled = false;
                Timer.desligarAuto.Dispose();
            }
            if (indice == 0)
                indice = 1;
            if (compartilhante == null)
                compartilhante = "user";
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
                    var lista = await repositoryPagina.buscarCapitulo(capitulo);
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


            //try
            //{
            //    await js!.InvokeAsync<object>("FullScreen", "1");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}

        }

        protected async Task renderizar()
        {
            await Verificar(capitulo);
            var list = repositoryPagina!.paginas!.Where(p => p.Story!.PaginaPadraoLink == capitulo).ToList();
            Pagina pag = list.First();


            if (filtrar == null && substory == null)
            {
                await Verificar(capitulo);
                var lst = repositoryPagina.paginas!.Where(p => p.Story!.PaginaPadraoLink == capitulo).ToList();
                Model = lst.Skip((int)indice - 1).FirstOrDefault();


                if (Model == null)
                {
                    Mensagem = "Por favor digite um numero menor.";
                    return;
                }

                quantidadePaginas = list.Count();
                //  ViewBag.story = pagina.Story.Nome;
                string html = "";
                if (Model!.Content != null || Model.ContentUser != null)
                {
                    try
                    {
                        html = await repositoryPagina!.renderizarPagina(Model);
                        markup = new MarkupString(html);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erro: " + ex.Message);
                    }
                }
                else if (Model.Produto == null)
                    html = RepositoryPagina.Capa;

                proximo = indice + 1;
                anterior = indice - 1;

                if (Model.Comentario != null)
                {
                    var page = repositoryPagina.paginas!.FirstOrDefault(p => p.Id == Model.Comentario);
                    if (page == null)
                    {
                        page = await repositoryPagina.Context.Pagina!.FirstAsync(p => p.Id == Model.Comentario);
                        await Verificar(page!.Story!.PaginaPadraoLink);
                        page = repositoryPagina.paginas!.FirstOrDefault(p => p.Id == Model.Comentario);
                    }
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
                    var livro = await repositoryPagina.Context.Livro!.FirstOrDefaultAsync(l => l.Compartilhando);
                    if (livro != null && redirecionar == null)
                        navigation!.NavigateTo($"{livro.url}/filtro/{livro.Capitulo}/{filtrar}/{compartilhante}/{livro.DescontoDoCompartilhante}/0/0/0/0/0/0/0/0/0/0/1");

                    removePreferencia();

                    var indiceFiltro = int.Parse(filtrar.Replace("pasta-", ""));
                    var fi = pag.Story!.Filtro!.OrderBy(f => f.Id).ToList()[indiceFiltro];
                    var arr = Arr.RetornarArray(pag.Story, false, (long)fi.CamadaDezId!, capitulo, 1);
                    indice = 1;
                    compartilhante = "user";
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
                    }
                    else if (arr.Length == 7)
                    {
                        substory = arr[1];
                        grupo = arr[2];
                        subgrupo = arr[3];
                        subsubgrupo = arr[4];
                        camadaseis = arr[5];
                        camadasete = arr[6];
                    }
                    else if (arr.Length == 6)
                    {
                        substory = arr[1];
                        grupo = arr[2];
                        subgrupo = arr[3];
                        subsubgrupo = arr[4];
                        camadaseis = arr[5];
                    }
                    else if (arr.Length == 5)
                    {
                        substory = arr[1];
                        grupo = arr[2];
                        subgrupo = arr[3];
                        subsubgrupo = arr[4];
                    }
                    else if (arr.Length == 4)
                    {
                        substory = arr[1];
                        grupo = arr[2];
                        subgrupo = arr[3];
                    }
                    else if (arr.Length == 3)
                    {
                        substory = arr[1];
                        grupo = arr[2];
                    }
                    else if (arr.Length == 2)
                    {
                        substory = arr[1];
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

                    group = pag.Story!.SubStory!.Where(str => str.Pagina!.Count > 0).Skip((int)substory! - 1).First();
                    nameGroup = group.Nome!;
                    if (group!.Pagina!.Where(p => p.Produto != null).ToList().Count > 0 && lista == 1 && pag.ContentUser == null)
                        listaComConteudo = retornarListaComConteudo(list,
                     group.Pagina!.ToList(), (int)camadadez!);
                    else listaComConteudo = group.Pagina!.ToList();


                    if (grupo != null)
                    {
                        group2 = group!.Grupo!.Where(str => str.Pagina!.Count > 0).Skip((int)grupo! - 1).First();
                        nameGroup = group2.Nome!;
                        if (group2!.Pagina!.Where(p => p.Produto != null).ToList().Count > 0 && lista == 1 && pag.ContentUser == null)
                            listaComConteudo = retornarListaComConteudo(list,
                         group2.Pagina!.ToList(), (int)camadadez!);
                        else listaComConteudo = group2.Pagina!.ToList();
                    }

                    if (subgrupo != null)
                    {
                        group3 = group2!.SubGrupo!.Where(str => str.Pagina!.Count > 0).Skip((int)subgrupo! - 1).First();
                        nameGroup = group3.Nome!;
                        if (group3!.Pagina!.Where(p => p.Produto != null).ToList().Count > 0 && lista == 1 && pag.ContentUser == null)
                            listaComConteudo = retornarListaComConteudo(list,
                         group3.Pagina!.ToList(), (int)camadadez!);
                        else listaComConteudo = group3.Pagina!.ToList();
                    }

                    if (subsubgrupo != null)
                    {
                        group4 = group3!.SubSubGrupo!.Where(str => str.Pagina!.Count > 0).Skip((int)subsubgrupo! - 1).First();
                        nameGroup = group4.Nome!;
                        if (group4!.Pagina!.Where(p => p.Produto != null).ToList().Count > 0 && lista == 1 && pag.ContentUser == null)
                            listaComConteudo = retornarListaComConteudo(list,
                         group4.Pagina!.ToList(), (int)camadadez!);
                        else listaComConteudo = group4.Pagina!.ToList();
                    }

                    if (camadaseis != null)
                    {
                        group5 = group4!.CamadaSeis!.Where(str => str.Pagina!.Count > 0).Skip((int)camadaseis! - 1).First();
                        nameGroup = group5.Nome!;
                        if (group5!.Pagina!.Where(p => p.Produto != null).ToList().Count > 0 && lista == 1 && pag.ContentUser == null)
                            listaComConteudo = retornarListaComConteudo(list,
                         group5.Pagina!.ToList(), (int)camadadez!);
                        else listaComConteudo = group5.Pagina!.ToList();
                    }

                    if (camadasete != null)
                    {
                        group6 = group5!.CamadaSete!.Where(str => str.Pagina!.Count > 0).Skip((int)camadasete! - 1).First();
                        nameGroup = group6.Nome!;
                        if (group6!.Pagina!.Where(p => p.Produto != null).ToList().Count > 0 && lista == 1 && pag.ContentUser == null)
                            listaComConteudo = retornarListaComConteudo(list,
                         group6.Pagina!.ToList(), (int)camadadez!);
                        else listaComConteudo = group6.Pagina!.ToList();
                    }

                    if (camadaoito != null)
                    {
                        group7 = group6!.CamadaOito!.Where(str => str.Pagina!.Count > 0).Skip((int)camadaoito! - 1).First();
                        nameGroup = group7.Nome!;
                        if (group7!.Pagina!.Where(p => p.Produto != null).ToList().Count > 0 && lista == 1 && pag.ContentUser == null)
                            listaComConteudo = retornarListaComConteudo(list,
                         group7.Pagina!.ToList(), (int)camadadez!);
                        else listaComConteudo = group7.Pagina!.ToList();
                    }

                    if (camadanove != null)
                    {
                        group8 = group7!.CamadaNove!.Where(str => str.Pagina!.Count > 0).Skip((int)camadanove! - 1).First();
                        nameGroup = group8.Nome!;
                        if (group8!.Pagina!.Where(p => p.Produto != null).ToList().Count > 0 && lista == 1 && pag.ContentUser == null)
                            listaComConteudo = retornarListaComConteudo(list,
                         group8.Pagina!.ToList(), (int)camadadez!);
                        else listaComConteudo = group8.Pagina!.ToList();
                    }

                    if (camadadez != null)
                    {
                        group9 = group8!.CamadaDez!.Where(str => str.Pagina!.Count > 0).Skip((int)camadadez! - 1).First();
                        nameGroup = group9.Nome!;
                        if (group9!.Pagina!.Where(p => p.Produto != null).ToList().Count > 0 && lista == 1 && pag.ContentUser == null)
                            listaComConteudo = retornarListaComConteudo(list,
                         group9.Pagina!.ToList(), (int)camadadez!);
                        else listaComConteudo = group9.Pagina!.ToList();
                    }

                    Pagina pag2 = listaComConteudo!.Skip((int)indice - 1).First();

                    Model = list.First(p => p.Id == pag2.Id);
                    vers = list.IndexOf(Model) + 1;

                    quantidadePaginas = listaComConteudo!.Count;
                    html = await repositoryPagina!.renderizarPagina(Model);
                    proximo = indice + 1;
                    anterior = indice - 1;

                    Filtro Filtro = null;
                    if(group8 != null)
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
                if (indice > quantidadePaginas)
                    await js!.InvokeAsync<object>("MarcarIndice", "1");
                else
                    await js!.InvokeAsync<object>("MarcarIndice", $"{indice}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

           

        }

        private async Task Verificar(int? capitulo)
        {
            var quantidadeCap = repositoryPagina!.paginas!
                .Where(p => p.Story!.PaginaPadraoLink == capitulo).ToList().Count;
            var quant = 0;
            if (repositoryPagina.paginas!.Count != 0 &&
            repositoryPagina.paginas!.Where(p => p.Story!.PaginaPadraoLink ==
            capitulo).ToList().Count != 0 &&
            repositoryPagina.paginas!.FirstOrDefault(p => p.Story!.PaginaPadraoLink ==
            capitulo)!.Story!.Quantidade == 0)
            {
                quant = buscarCount(ApplicationDbContext._connectionString, capitulo);
                repositoryPagina.paginas!.First(p => p.Story!.PaginaPadraoLink ==
                 capitulo)!.Story!.Quantidade = quant;

            }
            else if (repositoryPagina.paginas!.Count != 0 &&
            repositoryPagina.paginas!.Where(p => p.Story!.PaginaPadraoLink ==
               capitulo).ToList().Count != 0)
            {
                quant = repositoryPagina.paginas!.First(p => p.Story!.PaginaPadraoLink ==
                        capitulo)!.Story!.Quantidade;
            }

            var comentarios = 0;
            if (repositoryPagina.paginas!.Count != 0 &&
             repositoryPagina.paginas!.FirstOrDefault()!.Story!.QuantComentario == 0)
            {
                comentarios = CountComentarios(null!, null!, new Story().GetType(), ApplicationDbContext._connectionString);
                repositoryPagina.paginas!.First()!.Story!.QuantComentario = comentarios;
            }
            else if (repositoryPagina.paginas!.Count != 0)
                comentarios = repositoryPagina.paginas!.FirstOrDefault()!.Story!.QuantComentario;

            if (quantidadeCap == 0 || quant != quantidadeCap)
            {
                repositoryPagina.paginas!.RemoveAll(p => p.Story!.PaginaPadraoLink == capitulo);
                repositoryPagina.paginas
                .AddRange(await repositoryPagina.buscarCapitulo((int)capitulo!));
            }

            if (repositoryPagina.paginas!.Where(p => p.Story!.Comentario).ToList().Count != comentarios)
            {
                repositoryPagina.paginas!.RemoveAll(p => p.Story!.Comentario);
                repositoryPagina.paginas
                .AddRange(await repositoryPagina.includes()
                .Where(p => p.Story!.Comentario).ToListAsync());
            }


        }

        private static int buscarCount(string conexao, int? capitulo)
        {
            SqlConnection con;
            SqlCommand cmd;
            var _TotalRegistros = 0;
            try
            {
                using (con = new SqlConnection(conexao))
                {
                    cmd = new SqlCommand($"SELECT COUNT(*) FROM Pagina as P inner join Story as st on P.StoryId = st.Id where st.PaginaPadraoLink={capitulo}", con);
                    con.Open();
                    _TotalRegistros = int.Parse(cmd.ExecuteScalar().ToString()!);
                    con.Close();
                }
            }
            catch (Exception)
            {
                _TotalRegistros = 0;
            }
            return _TotalRegistros;
        }

        private static int CountComentarios(SqlConnection con, SqlCommand cmd, Type item, string conexao)
        {

            var _TotalRegistros = 0;
            try
            {
                using (con = new SqlConnection(conexao))
                {
                    cmd = new SqlCommand($"SELECT COUNT(*) FROM Pagina as P inner join Story as S on P.StoryId=S.Id  where S.Comentario='True'", con);
                    con.Open();
                    _TotalRegistros = int.Parse(cmd.ExecuteScalar().ToString()!);
                    con.Close();
                }
            }
            catch (Exception)
            {
                _TotalRegistros = 0;
            }
            return _TotalRegistros;
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
                    if (capitulo == 0 && indice >= quantidadePaginas)
                        navigation!.NavigateTo($"/Renderizar/{Model!.Story!.PaginaPadraoLink}/1/1/{timeproduto}/{lista}/{compartilhante}/{desconto}");
                    else if (capitulo != 0 && indice >= quantidadePaginas)
                        navigation!.NavigateTo($"/Renderizar/{Model!.Story!.PaginaPadraoLink + 1}/1/1/{timeproduto}/{lista}/{compartilhante}/{desconto}");
                    else
                        navigation!.NavigateTo($"/Renderizar/{Model!.Story!.PaginaPadraoLink}/{proximo}/1/{timeproduto}/{lista}/{compartilhante}/{desconto}");
                }
                else
                {
                    navegarSubgrupos();
                }
            }

            Console.WriteLine("Timer Elapsed.");
            Timer!._timer!.Elapsed -= _timer_Elapsed;
        }

        private void navegarSubgrupos()
        {
            removePreferencia();
            if (camadadez != null && indice >= quantidadePaginas)
            {
                var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory!,
                    grupo, subgrupo, subsubgrupo, camadaseis, camadasete, camadaoito, camadanove, camadadez);
                if (arr != null)
                    navigation!
                 .NavigateTo($"/camadadez/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/{arr[5]}/{arr[6]}/{arr[7]}/{arr[8]}/{arr[9]}/1/1/{timeproduto}/{lista}/{compartilhante}/{desconto}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else
                    navigation!.NavigateTo($"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/1/1/{timeproduto}/{lista}/{compartilhante}/{desconto}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
            }
            else
                                if (camadanove != null && indice >= quantidadePaginas)
            {
                var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory!,
                    grupo, subgrupo, subsubgrupo, camadaseis, camadasete, camadaoito, camadanove);
                if (arr != null)
                    navigation!
                 .NavigateTo($"/camadanove/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/{arr[5]}/{arr[6]}/{arr[7]}/{arr[8]}/1/1/{timeproduto}/{lista}/{compartilhante}/{desconto}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else
                    navigation!.NavigateTo($"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/1/1/{timeproduto}/{lista}/{compartilhante}/{desconto}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
            }
            else
                                if (camadaoito != null && indice >= quantidadePaginas)
            {
                var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory!,
                    grupo, subgrupo, subsubgrupo, camadaseis, camadasete, camadaoito);
                if (arr != null)
                    navigation!
                 .NavigateTo($"/camadaoito/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/{arr[5]}/{arr[6]}/{arr[7]}/1/1/{timeproduto}/{lista}/{compartilhante}/{desconto}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else
                    navigation!.NavigateTo($"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/1/1/{timeproduto}/{lista}/{compartilhante}/{desconto}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
            }
            else
                                if (camadasete != null && indice >= quantidadePaginas)
            {
                var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory!,
                    grupo, subgrupo, subsubgrupo, camadaseis, camadasete);
                if (arr != null)
                    navigation!
                 .NavigateTo($"/camadasete/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/{arr[5]}/{arr[6]}/1/1/{timeproduto}/{lista}/{compartilhante}/{desconto}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else
                    navigation!.NavigateTo($"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/1/1/{timeproduto}/{lista}/{compartilhante}/{desconto}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
            }
            else
                                if (camadaseis != null && indice >= quantidadePaginas)
            {
                var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory!,
                    grupo, subgrupo, subsubgrupo, camadaseis);
                if (arr != null)
                    navigation!
                 .NavigateTo($"/camadaseis/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/{arr[5]}/1/1/{timeproduto}/{lista}/{compartilhante}/{desconto}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else
                    navigation!.NavigateTo($"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/1/1/{timeproduto}/{lista}/{compartilhante}/{desconto}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
            }
            else
                                if (subsubgrupo != null && indice >= quantidadePaginas)
            {
                var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory!,
                    grupo, subgrupo, subsubgrupo);
                if (arr != null)
                    navigation!
                 .NavigateTo($"/subsubgrupo/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/1/1/{timeproduto}/{lista}/{compartilhante}/{desconto}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else
                    navigation!.NavigateTo($"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/1/1/{timeproduto}/{lista}/{compartilhante}/{desconto}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
            }
            else
                                if (subgrupo != null && indice >= quantidadePaginas)
            {
                var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory!,
                    grupo, subgrupo);
                if (arr != null)
                    navigation!
                 .NavigateTo($"/subgrupo/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/1/1/{timeproduto}/{lista}/{compartilhante}/{desconto}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else
                    navigation!.NavigateTo($"/grupo/{capitulo}/{substory}/{grupo}/1/1/{timeproduto}/{lista}/{compartilhante}/{desconto}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
            }
            else
                                if (grupo != null && indice >= quantidadePaginas)
            {
                var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory!, grupo);
                if (arr != null)
                    navigation!
                 .NavigateTo($"/grupo/{arr[0]}/{arr[1]}/{arr[2]}/1/1/{timeproduto}/{lista}/{compartilhante}/{desconto}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else
                    navigation!.NavigateTo($"/substory/{capitulo}/{substory}/1/1/{timeproduto}/{lista}/{compartilhante}/{desconto}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
            }
            else
                                if (substory != null && indice >= quantidadePaginas)
            {
                var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory);
                if (arr != null)
                    navigation!
                 .NavigateTo($"/substory/{arr[0]}/{arr[1]}/1/1/{timeproduto}/{lista}/{compartilhante}/{desconto}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}");
                else
                    navigation!.NavigateTo($"/renderizar/{capitulo}/1/1/{timeproduto}/{lista}/{compartilhante}/{desconto}");
            }
        }

        protected void TeclaPressionada(KeyboardEventArgs args)
        {
            if (substory == null)
            {
                if(auto == 0)
                {
                    Timer!.SetTimerAuto();
                    Timer!.desligarAuto!.Elapsed += desligarAuto_Elapsed;
                }
                if (args.Key == "Enter" && capitulo == 0)
                {
                    Console.WriteLine("foi dado Enter");
                    navigation!.NavigateTo($"/Renderizar/{indice}/1/1/{timeproduto}/{lista}/{lista}/{compartilhante}/{desconto}");
                }
                else if (args.Key == "Enter")
                {
                    Console.WriteLine("foi dado Enter em outro capitulo");
                    navigation!.NavigateTo($"/Renderizar/0/{Model!.Story!.PaginaPadraoLink}/1/{timeproduto}/{lista}/{lista}/{compartilhante}/{desconto}");
                }
            }
            else if (args.Key == "Enter")
            {
                navegarSubgrupos();
            }

        }

        protected void Casinha()
        {
            auto = 0;
            navigation!.NavigateTo($"/info/{compartilhante}/{desconto}");
        }

        protected async void Pesquisar()
        {
            auto = 0;
            var url = $"/Renderizar/{Model!.Story!.PaginaPadraoLink}/{opcional}/0/{timeproduto}/{lista}/{lista}/{compartilhante}/{desconto}";
            navigation!.NavigateTo(url);
        }

        protected void habilitarAuto()
        {
            Timer!.SetTimerAuto();
            Timer!.desligarAuto!.Elapsed += desligarAuto_Elapsed;
            navigation!.NavigateTo($"/Renderizar/{Model!.Story!.PaginaPadraoLink}/{indice}/1/{timeproduto}/{lista}/{lista}/{compartilhante}/{desconto}");
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
            navigation!.NavigateTo($"/comentario/{Model!.Story!.PaginaPadraoLink}/{indice}/{compartilhante}/{desconto}");
        }

        protected async void configurar()
        {
            if (auto == 1)
                desabilitarAuto();
            navigation!.NavigateTo($"/configuracoes/{Model!.Story!.PaginaPadraoLink}/{indice}/{compartilhante}/{desconto}");
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
   
        protected async void Parear()
        {
             DemoContextFactory db = new DemoContextFactory();
         ApplicationDbContext Contexto = db.CreateDbContext(null);
        var livros = await Contexto.Livro!.ToListAsync();
           var compartilhantes = await Contexto.Compartilhante!.ToListAsync();

            foreach (var item in livros)
                await Http.GetAsync($"{item.url}/api/pareamento?dominio={"https://" +repositoryPagina.buscarDominio()}&capitulo={capitulo}&indiceFiltro={indice_Filtro}&preferencia={indice}");
            foreach (var item in compartilhantes)
                await Http.GetAsync($"{item.Livro}/api/pareamento?dominio={"https://" + repositoryPagina.buscarDominio()}&capitulo={capitulo}&indiceFiltro={indice_Filtro}&preferencia={indice}");
            // await http.GetAsync($"https://localhost:7224/api/pareamento?dominio={"https://" + repositoryPagina.buscarDominio()}&capitulo={capitulo}&indiceFiltro={indice_Filtro}&preferencia={indice}");
        }

       protected void getPastas()
        {
            if (auto == 1)
                desabilitarAuto();
            navigation!.NavigateTo($"/pastas/{Model!.Story!.PaginaPadraoLink}/{indice}/{compartilhante}/{desconto}");
        }
    
        private void removePreferencia()
        {
            p1 = 0; p2 = 0; p3 = 0; p4 = 0; p5 = 0;
            p6 = 0; p7 = 0; p8 = 0; p9 = 0; p10 = 0;
        }
    }
}
