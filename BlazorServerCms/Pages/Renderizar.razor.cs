using BlazorServerCms.Data;
using BlazorServerCms.Pages;
using BlazorServerCms.servicos;
using business;
using business.Group;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Models;
using NVelocity.Runtime.Directive;
using System;

namespace BlazorCms.Client.Pages
{
    public class RenderizarBase : ComponentBase
    {
        

        [Inject] RepositoryPagina? repositoryPagina { get; set; }

        [Inject] public NavigationManager? navigation { get; set; }

        [Inject] HttpClient? Http { get; set; }
        [Inject] BlazorTimer? Timer { get; set; }
        [Inject] IJSRuntime? js { get; set; }

        
        public ClassArray Arr = new ClassArray();
        static string conexao = "Data Source=DESKTOP-7TI5J9C\\SQLEXPRESS;Initial Catalog=BlazorCms;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";
        ApplicationDbContext context = new ApplicationDbContext(conexao);
        protected MarkupString markup;
        protected ElementReference firstInput;
        protected string? Mensagem = null;
        protected Pagina? Model;
        protected string[]? classificacoes = null;
        protected int opcional = 1;
        protected string? html { get; set; } = "";
        protected int? filtro { get; set; } = null;
        protected int? vers { get; set; } = null;
        protected int? CapituloComentario { get; set; } = null;
        protected int? VersoComentario { get; set; } = null;
        protected int quantidadePaginas { get; set; }
        protected int anterior { get; set; }
        protected int proximo { get; set; }

        [Parameter] public int indice { get; set; } = 1; [Parameter] public int capitulo { get; set; } = 1;

        [Parameter] public int? substory { get; set; } = null; [Parameter] public int? grupo { get; set; } = null;

        [Parameter] public int? subgrupo { get; set; } = null; [Parameter] public int? subsubgrupo { get; set; } = null;

        [Parameter] public int? camadaseis { get; set; } = null; [Parameter] public int? camadasete { get; set; } = null;

        [Parameter] public int? camadaoito { get; set; } = null; [Parameter] public int? camadanove { get; set; } = null;

        [Parameter] public int? camadadez { get; set; } = null; [Parameter] public int auto { get; set; } = 1;

        [Parameter] public string? redirecionar { get; set; } = null; [Parameter] public string? compartilhante { get; set; } = "user";

        [Parameter] public string? filtrar { get; set; } = null;

        protected override void OnAfterRender(bool firstRender = false)
        {               
                            
        }

        protected override async Task OnParametersSetAsync()
        {
            if (repositoryPagina!.paginas!.Where(p => p.Story!.PaginaPadraoLink == capitulo).ToList().Count == 0)
                Mensagem = "aguarde um momento...";
            else
                await renderizar();

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
            if (indice == 0)
                indice = 1;
            if(compartilhante == null)
                compartilhante = "user";
            if (repositoryPagina!.paginas == null || !repositoryPagina.aguarde &&
               repositoryPagina.paginas.Where(p => p.Story!.PaginaPadraoLink == capitulo).ToList().Count == 0)
            {

                repositoryPagina.aguarde = true;
                Mensagem = "aguarde um momento...";
                if (repositoryPagina.paginas == null)
                {
                    repositoryPagina.paginas = new List<Pagina>();
                    var lista = await repositoryPagina.buscarCapitulo(context, capitulo);
                    repositoryPagina.paginas.AddRange(lista);
                    repositoryPagina.aguarde = false;
                    await repositoryPagina.buscarCapitulos(context);
                }
                else
                {
                    var lista = await repositoryPagina.buscarCapitulo(context, capitulo);
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
            var lista = repositoryPagina!.paginas!.Where(p => p.Story!.PaginaPadraoLink == capitulo && !p.Layout).ToList();
            Pagina pag = lista.First();
            

            if (filtrar == null && substory == null)
            {
                await Verificar(capitulo);
                var lst = repositoryPagina.paginas!.Where(p => p.Story!.PaginaPadraoLink == capitulo && !p.Layout).ToList();
                Model = lst.Skip((int)indice - 1).FirstOrDefault();


                if (Model == null)
                {
                    Mensagem = "Por favor digite um numero menor";
                    return;
                }

                quantidadePaginas = lista.Count();
                //  ViewBag.story = pagina.Story.Nome;
                string html = "";
                if (Model!.Div!.Count > 0)
                {
                    if (!string.IsNullOrEmpty(Model.Sobreescrita))
                        html = Model.Sobreescrita;
                    else
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
                        page = await context.Pagina!.FirstAsync(p => p.Id == Model.Comentario);
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
                    var livro = await context.Livro!.FirstOrDefaultAsync(l => l.Compartilhando);
                    if (livro != null && redirecionar == null)
                        navigation!.NavigateTo($"{livro.url}/{livro.Capitulo}/{filtrar}/1");

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
                    if (group!.Pagina!.Where(p => !p.Layout && p.Produto != null).ToList().Count > 0)
                        listaComConteudo = retornarListaComConteudo(lista,
                     group.Pagina!.Where(p => !p.Layout).ToList(), (int)camadadez!);
                    else listaComConteudo = group.Pagina!.Where(p => !p.Layout).ToList();


                    if (grupo != null)
                    {
                        group2 = group!.Grupo!.Where(str => str.Pagina!.Count > 0).Skip((int)grupo! - 1).First();
                        if (group2!.Pagina!.Where(p => !p.Layout && p.Produto != null).ToList().Count > 0)
                            listaComConteudo = retornarListaComConteudo(lista,
                         group2.Pagina!.Where(p => !p.Layout).ToList(), (int)camadadez!);
                        else listaComConteudo = group2.Pagina!.Where(p => !p.Layout).ToList();
                    }

                    if (subgrupo != null)
                    {
                        group3 = group2!.SubGrupo!.Where(str => str.Pagina!.Count > 0).Skip((int)subgrupo! - 1).First();
                        if (group3!.Pagina!.Where(p => !p.Layout && p.Produto != null).ToList().Count > 0)
                            listaComConteudo = retornarListaComConteudo(lista,
                         group3.Pagina!.Where(p => !p.Layout).ToList(), (int)camadadez!);
                        else listaComConteudo = group3.Pagina!.Where(p => !p.Layout).ToList();
                    }

                    if (subsubgrupo != null)
                    {
                        group4 = group3!.SubSubGrupo!.Where(str => str.Pagina!.Count > 0).Skip((int)subsubgrupo! - 1).First();
                        if (group4!.Pagina!.Where(p => !p.Layout && p.Produto != null).ToList().Count > 0)
                            listaComConteudo = retornarListaComConteudo(lista,
                         group4.Pagina!.Where(p => !p.Layout).ToList(), (int)camadadez!);
                        else listaComConteudo = group4.Pagina!.Where(p => !p.Layout).ToList();
                    }

                    if (camadaseis != null)
                    {
                        group5 = group4!.CamadaSeis!.Where(str => str.Pagina!.Count > 0).Skip((int)camadaseis! - 1).First();
                        if (group5!.Pagina!.Where(p => !p.Layout && p.Produto != null).ToList().Count > 0)
                            listaComConteudo = retornarListaComConteudo(lista,
                         group5.Pagina!.Where(p => !p.Layout).ToList(), (int)camadadez!);
                        else listaComConteudo = group5.Pagina!.Where(p => !p.Layout).ToList();
                    }

                    if (camadasete != null)
                    {
                        group6 = group5!.CamadaSete!.Where(str => str.Pagina!.Count > 0).Skip((int)camadasete! - 1).First();
                        if (group6!.Pagina!.Where(p => !p.Layout && p.Produto != null).ToList().Count > 0)
                            listaComConteudo = retornarListaComConteudo(lista,
                         group6.Pagina!.Where(p => !p.Layout).ToList(), (int)camadadez!);
                        else listaComConteudo = group6.Pagina!.Where(p => !p.Layout).ToList();
                    }

                    if (camadaoito != null)
                    {
                        group7 = group6!.CamadaOito!.Where(str => str.Pagina!.Count > 0).Skip((int)camadaoito! - 1).First();
                        if (group7!.Pagina!.Where(p => !p.Layout && p.Produto != null).ToList().Count > 0)
                            listaComConteudo = retornarListaComConteudo(lista,
                         group7.Pagina!.Where(p => !p.Layout).ToList(), (int)camadadez!);
                        else listaComConteudo = group7.Pagina!.Where(p => !p.Layout).ToList();
                    }

                    if (camadanove != null)
                    {
                        group8 = group7!.CamadaNove!.Where(str => str.Pagina!.Count > 0).Skip((int)camadanove! - 1).First();
                        if (group8!.Pagina!.Where(p => !p.Layout && p.Produto != null).ToList().Count > 0)
                            listaComConteudo = retornarListaComConteudo(lista,
                         group8.Pagina!.Where(p => !p.Layout).ToList(), (int)camadadez!);
                        else listaComConteudo = group8.Pagina!.Where(p => !p.Layout).ToList();
                    }

                    if (camadadez != null)
                    {
                        group9 = group8!.CamadaDez!.Where(str => str.Pagina!.Count > 0).Skip((int)camadadez! - 1).First();
                        if (group9!.Pagina!.Where(p => !p.Layout && p.Produto != null).ToList().Count > 0)
                            listaComConteudo = retornarListaComConteudo(lista,
                         group9.Pagina!.Where(p => !p.Layout).ToList(), (int)camadadez!);
                        else listaComConteudo = group9.Pagina!.Where(p => !p.Layout).ToList();
                    }

                    Pagina pag2 = listaComConteudo!.Where(p => !p.Layout).Skip((int)indice - 1).First();

                    Model = lista.First(p => p.Id == pag2.Id);
                    vers = lista.IndexOf(Model) + 1;

                    quantidadePaginas = listaComConteudo!.Count(p => !p.Layout);
                    //  ViewBag.group = group9;

                    if (!string.IsNullOrEmpty(Model.Sobreescrita))
                        html = Model.Sobreescrita;
                    else
                        html = await repositoryPagina!.renderizarPagina(Model);

                    proximo = indice + 1;
                    anterior = indice - 1;                    

                    var Filtro = pag.Story!.Filtro!.First(f => f.CamadaNoveId == group8!.Id);
                    filtro = pag.Story.Filtro!.OrderBy(f => f.Id).ToList().IndexOf(Filtro) + 1;
                }               
            }

           // if(capitulo == 0)
                try
                {
                    await firstInput.FocusAsync();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

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


        private async Task Verificar(int? capitulo)
        {
            var quantidadeCap = repositoryPagina!.paginas!.Where(p => p.Story!.PaginaPadraoLink ==
               capitulo && !p.Layout).ToList().Count;
            var quant = 0;
            if (repositoryPagina.paginas!.Count != 0 &&
            repositoryPagina.paginas!.Where(p => p.Story!.PaginaPadraoLink ==
            capitulo).ToList().Count != 0 &&
            repositoryPagina.paginas!.FirstOrDefault(p => p.Story!.PaginaPadraoLink ==
            capitulo)!.Story!.Quantidade == 0)
            {
                quant = buscarCount( conexao, capitulo);
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
                comentarios = CountComentarios(null!, null!, new Story().GetType(), conexao);
                repositoryPagina.paginas!.First()!.Story!.QuantComentario = comentarios;
            }
            else if (repositoryPagina.paginas!.Count != 0)
                comentarios = repositoryPagina.paginas!.FirstOrDefault()!.Story!.QuantComentario;

            if (quantidadeCap == 0 || quant != quantidadeCap)
            {
                repositoryPagina.paginas!.RemoveAll(p => p.Story!.PaginaPadraoLink == capitulo);
                repositoryPagina.paginas
                .AddRange(await repositoryPagina.buscarCapitulo(context, (int) capitulo!));
            }

            if (repositoryPagina.paginas!.Where(p => p.Story!.Comentario).ToList().Count != comentarios)
            {
                repositoryPagina.paginas!.RemoveAll(p => p.Story!.Comentario);
                repositoryPagina.paginas
                .AddRange(await repositoryPagina.includes(context)
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
            List<Pagina> conteudo = content.Where(p => p.Div!.Count > 0).ToList();
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
                Timer!.SetTimer(p.Tempo);
                Timer._timer!.Elapsed += _timer_Elapsed;
                try
                {
                        await js!.InvokeAsync<object>("PreencherProgressBar", p.Tempo);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("Timer Started.");
            }            
        }

        private void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if(auto == 1)
            {
                if(substory == null)
                {
                    if (capitulo == 0 && indice >= quantidadePaginas)
                        navigation!.NavigateTo($"/Renderizar/{Model!.Story!.PaginaPadraoLink}/1/1/{compartilhante}");
                    else if (capitulo != 0 && indice >= quantidadePaginas)
                        navigation!.NavigateTo($"/Renderizar/{Model!.Story!.PaginaPadraoLink + 1}/1/1/{compartilhante}");
                    else
                        navigation!.NavigateTo($"/Renderizar/{Model!.Story!.PaginaPadraoLink}/{proximo}/1/{compartilhante}");
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
            if (camadadez != null && indice >= quantidadePaginas)
            {
                var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory!,
                    grupo, subgrupo, subsubgrupo, camadaseis, camadasete, camadaoito, camadanove, camadadez);
                if (arr != null)
                    navigation!
                 .NavigateTo($"/camadadez/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/{arr[5]}/{arr[6]}/{arr[7]}/{arr[8]}/{arr[9]}/1/1/{compartilhante}");
                else
                    navigation!.NavigateTo($"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/1/1/{compartilhante}");
            }
            else
                                if (camadanove != null && indice >= quantidadePaginas)
            {
                var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory!,
                    grupo, subgrupo, subsubgrupo, camadaseis, camadasete, camadaoito, camadanove);
                if (arr != null)
                    navigation!
                 .NavigateTo($"/camadanove/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/{arr[5]}/{arr[6]}/{arr[7]}/{arr[8]}/1/1/{compartilhante}");
                else
                    navigation!.NavigateTo($"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/1/1/{compartilhante}");
            }
            else
                                if (camadaoito != null && indice >= quantidadePaginas)
            {
                var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory!,
                    grupo, subgrupo, subsubgrupo, camadaseis, camadasete, camadaoito);
                if (arr != null)
                    navigation!
                 .NavigateTo($"/camadaoito/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/{arr[5]}/{arr[6]}/{arr[7]}/1/1/{compartilhante}");
                else
                    navigation!.NavigateTo($"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/1/1/{compartilhante}");
            }
            else
                                if (camadasete != null && indice >= quantidadePaginas)
            {
                var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory!,
                    grupo, subgrupo, subsubgrupo, camadaseis, camadasete);
                if (arr != null)
                    navigation!
                 .NavigateTo($"/camadasete/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/{arr[5]}/{arr[6]}/1/1/{compartilhante}");
                else
                    navigation!.NavigateTo($"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/1/1/{compartilhante}");
            }
            else
                                if (camadaseis != null && indice >= quantidadePaginas)
            {
                var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory!,
                    grupo, subgrupo, subsubgrupo, camadaseis);
                if (arr != null)
                    navigation!
                 .NavigateTo($"/camadaseis/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/{arr[5]}/1/1/{compartilhante}");
                else
                    navigation!.NavigateTo($"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/1/1/{compartilhante}");
            }
            else
                                if (subsubgrupo != null && indice >= quantidadePaginas)
            {
                var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory!,
                    grupo, subgrupo, subsubgrupo);
                if (arr != null)
                    navigation!
                 .NavigateTo($"/subsubgrupo/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/1/1/{compartilhante}");
                else
                    navigation!.NavigateTo($"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/1/1/{compartilhante}");
            }
            else
                                if (subgrupo != null && indice >= quantidadePaginas)
            {
                var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory!,
                    grupo, subgrupo);
                if (arr != null)
                    navigation!
                 .NavigateTo($"/subgrupo/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/1/1/{compartilhante}");
                else
                    navigation!.NavigateTo($"/grupo/{capitulo}/{substory}/{grupo}/1/1/{compartilhante}");
            }
            else
                                if (grupo != null && indice >= quantidadePaginas)
            {
                var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory!, grupo);
                if (arr != null)
                    navigation!
                 .NavigateTo($"/grupo/{arr[0]}/{arr[1]}/{arr[2]}/1/1/{compartilhante}");
                else
                    navigation!.NavigateTo($"/substory/{capitulo}/{substory}/1/1/{compartilhante}");
            }
            else
                                if (substory != null && indice >= quantidadePaginas)
            {
                var arr = Arr.RetornarArray(Model!.Story!, true, 0, capitulo, (int)substory);
                if (arr != null)
                    navigation!
                 .NavigateTo($"/substory/{arr[0]}/{arr[1]}/1/1/{compartilhante}");
                else
                    navigation!.NavigateTo($"/renderizar/{capitulo}/1/1/{compartilhante}");
            }
        }


        protected void TeclaPressionada(KeyboardEventArgs args)
        {
            if(substory == null)
            {
                if (args.Key == "Enter" && capitulo == 0)
                {
                    Console.WriteLine("foi dado Enter");
                    navigation!.NavigateTo($"/Renderizar/{indice}/1/1/{compartilhante}");
                }
                else if (args.Key == "Enter")
                {
                    Console.WriteLine("foi dado Enter em outro capitulo");
                    navigation!.NavigateTo($"/Renderizar/0/{Model!.Story!.PaginaPadraoLink}/1/{compartilhante}");
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
            navigation!.NavigateTo("/");
        }

        protected async void Pesquisar()
        {
            auto = 0;
            var url = $"/Renderizar/{Model!.Story!.PaginaPadraoLink}/{opcional}/0/{compartilhante}";
            navigation!.NavigateTo(url);
        }

        protected async void Listar()
        {
            auto = 0;
            var url = $"/paginacao/1/capitulo/1/30/81/{compartilhante}";
            navigation!.NavigateTo(url);

            
        }

    }
}
