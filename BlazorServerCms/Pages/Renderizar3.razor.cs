using System.Diagnostics;
using System.Reflection;
using BlazorServerCms.servicos;
using business;
using business.business;
using business.business.Group;
using business.Group;
using Humanizer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.JSInterop;

namespace BlazorCms.Client.Pages
{
    public partial class RenderizarBase : ComponentBase
    {

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if ( id_video is not null && ! AlterouCamada)
        {
             if (AlterouModel)
                await js!.InvokeAsync<object>("zerar", "1");
                await js.InvokeVoidAsync("carregarVideo", id_video);           

                id_video = null;
        }
    }


        private async Task<int> marcarIndice()
        {
            try
            {
                string? num = await js.InvokeAsync<string>("retornarlargura", "url");

                var largura = int.Parse(num);
                if (largura > 500)
                    quantDiv = ((19 * largura) / 1024);
                else
                    quantDiv = ((13 * largura) / 344);

                return quantDiv;

            }
            catch (Exception ex)
            {
                return 10;
            }

        }

        protected override async Task OnParametersSetAsync()
        {
            if (cap > RepositoryPagina.stories!.Last().Capitulo)
                storyid = RepositoryPagina.stories!
                .OrderBy(str => str.Capitulo).Skip(1).ToList()[0].Capitulo;

           
                await renderizar();

            

            if (Model2 != null && Model2.usuarios != null && Model2.usuarios.Count > 0 && Filtro != null)
                adicionarPontos();

        }

        protected override async Task OnInitializedAsync()
        {
            Context = db.CreateDbContext(null);

            var authState = await AuthenticationStateProvider
               .GetAuthenticationStateAsync();
            user = authState.User;

            if (user.Identity!.IsAuthenticated)
            {
                var u = await userManager.GetUserAsync(user);
                usuario = await Context.Users
                    .Include(u => u.PageLiked)
                .FirstAsync(us => us.Id == u.Id);

            }

            Marcacao.Marcados.Clear();
            Marcacao.resultado = "";

            vers = null;
            Auto = 0;
            timeproduto = 11;

            if (Compartilhou == null) Compartilhou = "comp";

            if (dominio == null)
                dominio = "dominio";

            if (Auto == 0 && Timer!.desligarAuto! != null
                && Timer!.desligarAuto!.Enabled == true)
            {

                Timer!.desligarAuto!.Elapsed -= desligarAuto_Elapsed;
                Timer!.desligarAuto!.Enabled = false;
                Timer.desligarAuto.Dispose();
            }

            if (Indice == 0)
                Indice = 1;

            padroes = RepositoryPagina.stories.OfType<PatternStory>().ToList().Count - 1;

            


            if (nomeLivro != null)
                livro = await Context.Livro!.FirstOrDefaultAsync(l => l.Nome == nomeLivro);

            if (_story == null)
            {
                _story = RepositoryPagina.stories.Skip(1).ToList()[(int)storyid! - 1];
            }

            if (livro == null)
                listaFiltro = await Context.Filtro!
                     .Include(p => p.Camada)!
                     .Include(p => p.Criterio)!
                     .ThenInclude(p => p.Filtro)!
                     .Include(p => p.Pagina)!
                     .ThenInclude(p => p.Content)!
                     .ThenInclude(p => p.Comentario)!
                     .Include(p => p.usuarios)!
                     .ThenInclude(p => p.UserModel)!
                    .Where(f => f.LivroId == null && f.StoryId == _story.Id).ToListAsync();
            else
                listaFiltro = await Context.Filtro!
                     .Include(p => p.Camada)!
                    .Include(p => p.Criterio)!
                     .ThenInclude(p => p.Filtro)!
                    .Include(p => p.Pagina)!
                     .ThenInclude(p => p.Content)!
                     .ThenInclude(p => p.Comentario)!
                     .Include(p => p.usuarios)!
                     .ThenInclude(p => p.UserModel)!
                    .Where(f => f.LivroId == livro.Id && f.StoryId == _story.Id).ToListAsync();



            Indice = 0;    
            

            if (dominio != repositoryPagina!.buscarDominio() && dominio != "dominio")
            {
                var domi = await Context.Compartilhante!.FirstOrDefaultAsync(l => l.Livro == dominio);
                if (domi == null)
                {
                    var compartilhant = new business.Compartilhante
                    {
                        Livro = dominio,
                        Data = DateTime.Now,
                        Comissao = 5
                    };
                    await Context.AddAsync(compartilhant);
                    await Context.SaveChangesAsync();
                }
                dominio = "dominio";
            }

        }

        protected int retornarVerso(Content c)
        {
            if (c != null)
            {
                Pagina pag = (Pagina)c;
                return pag.Versiculo;

            }
            else
                return 0;
        }

        protected async Task renderizar()
        {
            try
            {               

                if (AlterouModel && ! AlterouCamada)
                await js!.InvokeAsync<object>("zerar", "1");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }              
            

            if (Filtro == null)
                listaContent.Clear();
            ultimaPasta = false;
            var quantidadeFiltros = 0;
            var quantidadePaginas = 0;


            if (outroHorizonte == 0)
            {
                if (Filtro != null)
                {
                    var count = CountPagesInFilterAsync((long)Filtro, livro);
                    quantidadeLista = count;
                    if (retroceder == 1)                    
                        retroceder = 0;
                    
                }
                else
                {
                    var q = RepositoryPagina.Conteudo!
                        .LastOrDefault(c => c.StoryId == storyid && c is Chave && c.Html != null)!;
                    if (q == null)
                    {
                        var pa = Context.Pagina!.Include(p => p.Comentario).OrderBy(p => p.Id)
                        .LastOrDefault(c => c.StoryId == storyid && c is Chave && c.Html != null)!;
                        RepositoryPagina.Conteudo!.Add(pa);
                        quantidadeLista = retornarVerso(pa);
                    }
                    else
                        quantidadeLista = retornarVerso(q);
                }
            }


            if (Filtro == null)
                Model = RepositoryPagina.Conteudo!
             .FirstOrDefault(p => p is Pagina && retornarVerso(p) == Indice && p.StoryId == storyid);

            else if (Filtro != null)
            {
                var CountPages = CountPagesInFilterAsync((long)Filtro, livro);
                var CountPages2 = RepositoryPagina.Conteudo!.Where(c => c is Pagina && c.Filtro != null &&
                c.Filtro!.FirstOrDefault(f => f.FiltroId == Filtro) != null).ToList().Count;

                if (CountPages2 == CountPages && CountPages2 != 0)
                {
                    listaContent.Clear();
                    listaContent.AddRange(RepositoryPagina.Conteudo!
                    .Where(c => c is Pagina && c.Filtro != null &&
                    c.Filtro!.FirstOrDefault(f => f.FiltroId == Filtro) != null).ToList());

                    Model2 = listaFiltro.First(f => f.Id == Filtro);

                    nameGroup = Model2.Nome!;

                    InfoSemCriterio = listaFiltro.OfType<SubFiltro>()
                .FirstOrDefault(f => f.CriterioId == null && f.FiltroId == Model2.Id) != null; // mesma camada de com criterio

                    if (Indice != 0)
                        Model = listaContent[Indice - 1];
                    else
                    {
                        if (Model != null)
                        {
                            
                            Model = listaContent.FirstOrDefault(c => c.Id == Model!.Id);
                            Indice = listaContent.IndexOf(Model) + 1;
                        }
                    }
                    if (Model is Pagina)
                        vers = ((Pagina)Model).Versiculo;
                }
                else
                    Model = null;
            }


            if (Indice > quantidadeLista)
                quantDiv = await marcarIndice();
            else
                quantDiv = await marcarIndice();
            int slideAtivo = (Indice - 1) / quantDiv;
            slideAtual = slideAtivo;

            if (Filtro != null && RepositoryPagina.Conteudo!
                .Where(c => c is Pagina && c.Filtro != null &&
                c.Filtro!.FirstOrDefault(f => f.FiltroId == Filtro) != null).ToList().Count ==
                CountPagesInFilterAsync((long)Filtro, livro))
                listaContent = RepositoryPagina.Conteudo!
                .Where(c => c is Pagina && c.Filtro != null &&
                c.Filtro!.FirstOrDefault(f => f.FiltroId == Filtro) != null).OrderBy(p => p.Id)
                .Skip(quantDiv * slideAtual).Take(quantDiv)
                .ToList();

            if (Filtro == null)
            {
                List<Content> conteudos = null;
                conteudos = await PaginarStory((long)storyid!, quantidadeLista, quantDiv, slideAtual, livro, carregando);
                listaContent.AddRange(conteudos);

                foreach (var item in listaContent)
                    if (RepositoryPagina.Conteudo!.FirstOrDefault(c => c.Id == item.Id) == null)
                        RepositoryPagina.Conteudo!.Add(item);

                if (Indice != 0)
                {
                    if (livro == null)
                        Model = RepositoryPagina.Conteudo!
                        .FirstOrDefault(p => p is Pagina &&
                        retornarVerso(p) == Indice
                        && p.StoryId == storyid);
                    else
                        Model = RepositoryPagina.Conteudo!
                        .FirstOrDefault(p => p is Pagina && retornarVerso(p) == Indice
                        && p.StoryId == storyid
                        && p.LivroId == livro.Id);

                }
            }





            cap = RepositoryPagina.stories.First(st => st.Id == _story.Id).Capitulo;
            nameStory = RepositoryPagina.stories.First(st => st.Id == _story.Id).Nome;

            if (Filtro == null)
                listaContent = RepositoryPagina.Conteudo!.Where(c => c is Chave).OrderBy(p => p.Id)
                    .Skip(quantDiv * slideAtual).Take(quantDiv * 2)
                    .ToList();

            if (outroHorizonte == 1)
            {
                Model2 = listaFiltro.OrderBy(f => f.Id).Skip((int)Indice - 1).FirstOrDefault();
                quantidadeLista = listaFiltro.ToList().Count;
                indiceAcesso = listaFiltro!.ToList().IndexOf(Model2) + 1;
            }

            quantidadePaginas = CountPaginas();
            condicaoFiltro = CountFiltros();

            if (Filtro == null)
            {
                tellStory = false;
                if (Indice > quantidadeLista)
                {
                    if (quantidadePaginas != 0)
                        Mensagem = $"Por favor digite um numero menor que {quantidadeLista}.";
                    else
                        Mensagem = "aguarde um momento...";
                    return;
                }



                if (Model != null && Model is Comment)
                {
                    Comment pa = (Comment)Model;
                    if (pa.ContentId != null)
                    {
                        var c = RepositoryPagina.Conteudo!.First(c => c.Id == pa.ContentId);
                        var s = RepositoryPagina.stories.First(s => s.Id == c.StoryId);
                        CapituloComentario = s.Capitulo;
                        VersoComentario = RepositoryPagina.Conteudo
                        .Where(con => con.StoryId == s.Id).OrderBy(con => con.Id)
                        .ToList().IndexOf(c) + 1;
                    }
                }

            }

            else if (Filtro != null && condicaoFiltro || rotas != null)
            {
                if (Filtro != null)
                {
                    if (AlterouModel && Model == null)
                    {
                        if (rotas == null)
                            listaContent = await retornarListaFiltrada(null);
                        else
                            listaContent = await retornarListaFiltrada(rotas);
                    }

                    if (listaContent.Count != 0 && listaContent[0] != null)
                        foreach (var item in listaContent)
                            if (RepositoryPagina.Conteudo!.FirstOrDefault(c => c.Id == item.Id) == null)
                                RepositoryPagina.Conteudo!.Add(item);

                    if (Content)
                        listaContent = listaContent.Where(c => c is UserContent).OrderBy(p => p.Id)
                    .Skip(quantDiv * slideAtual).Take(listaContent.Count)
                    .ToList();

                }

            }

            // ultimaPasta = Model.UltimaPasta;
            if (cap != 0 && AlterouModel && !AlterouCamada)
                StartTimer(Model);

            if (Model != null && Model.Html != null && AlterouModel)
            {
                var conteudoHtml = Model.Html;
                if (Model.Html.Contains("iframe"))
                    conteudoHtml = colocarAutoPlay(conteudoHtml);
                else
                    id_video = null;
                    Model.Html = conteudoHtml;
                html = await repositoryPagina!.renderizarPagina(Model);
                AlterouModel = false;
            }
            if (Model != null && Model.Html != null)
            {
                try
                {
                    if (retornarVerso(Model) == chave && Filtro != null)
                    {
                        Model.Html = $"<p> Seja bem-vindo a sub-story {Model2!.Nome} </p>";
                        html = await repositoryPagina!.renderizarPagina(Model);
                    }
                    else if (Model is Chave && Model.Titulo == "chave" && Filtro == null)
                    {
                        var verso = retornarVerso(Model);
                        var fils = listaFiltro
                        .Where(f => f.Pagina!.FirstOrDefault(p => p.Content is Chave &&
                        retornarVerso(p.Content) == verso) != null).ToList();

                        Model.Html = $"<p> O versiculo {verso} é a chave que abre ";
                        if (fils.Count == 1)
                            Model.Html = $"a sub-story (pasta): ";
                        else
                            Model.Html = $"as sub-stories (pastas): ";
                        foreach (var item in fils)
                            Model.Html += item.Nome + ", ";

                        Model.Html += "</p>";

                        Model.Html = Model.Html.Replace(", </p>", "</p>");
                        html = await repositoryPagina!.renderizarPagina(Model);
                    }


                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro: " + ex.Message);
                }
            }
            else if (Model != null && Model.Produto != null && Model.Produto.Count == 0)
            {
                if (livro != null)
                    html = livro.Capa;
                else
                    html = RepositoryPagina.Capa;
            }

            markup = new MarkupString(html);

            try
            {
                await firstInput.FocusAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            array = new List<Content>[2];
            if (listaContent.Count != 0 && listaContent[0] != null)
                listaContent = listaContent.OrderBy(c => c.Id).ToList();
            if (array[0] == null)
                array[0] = new List<Content>();
            if (listaContent.Count > quantDiv)
                array[0].AddRange(listaContent.Take(quantDiv).ToList());
            else
                array[0].AddRange(listaContent);

            if (Filtro == null)
            {
                if (Indice < 100) classCss = "";
                else if (Indice > 99 && Indice < 1000) classCss = " DivPagTam2";
                else if (Indice > 999 && Indice < 10000) classCss = " DivPagTam3";
                else if (Indice > 9999 && Indice < 100000) classCss = " DivPagTam4";
            }
            else
            {
                if (vers < 100) classCss = "";
                else if (vers > 99 && vers < 1000) classCss = " DivPagTam2";
                else if (vers > 999 && vers < 10000) classCss = " DivPagTam3";
                else if (vers > 9999 && vers < 100000) classCss = " DivPagTam4";
            }

            UserModelContent p = null;

            try
            {
                if (user.Identity!.IsAuthenticated)
                {
                    p = Context.UserModelPageLiked
                        .Include(umpl => umpl.Content)
                        .Include(umpl => umpl.UserModel)
                        .FirstOrDefault(p => p.ContentId == Model.Id &&
                        p.UserModel.UserName == user.Identity!.Name)!;

                }


            }
            catch (Exception)
            {
                liked = false;
            }

            if (p != null)
                liked = true;
            else
                liked = false;

            if (!tellStory) placeholder = "digita nome";
            else placeholder = "Nº do item";

            if (!tellStory) divPagina = "DivPagina";
            else divPagina = "DivPagina2";

            if (!tellStory) DivPag = "DivPag";
            else DivPag = "DivPag2";


            var ultimoVerso = RepositoryPagina.Conteudo!
            .OfType<Chave>().LastOrDefault(p => p.StoryId == _story.Id).Versiculo;
            if (Model is Pagina && Filtro != null && vers <= ultimoVerso)
                Versiculo = (int)vers!;
            else if (Model is Chave && Filtro == null)
                Versiculo = retornarVerso(Model);
            else
                Versiculo = chave;

           if(Content)
                inputs = "inputs";
            else
                inputs = "inputs2";

        }

        private async Task<List<Content>> retornarListaFiltrada(string rota)
        {
            if (outroHorizonte == 0 && Filtro != null && rota == null)
            {
                Filtro Fil = listaFiltro.First(f => f.Id == Filtro);
                var lista = Fil.Pagina.Select(p => p.Content).ToList();

                var cha = lista.OfType<Chave>().LastOrDefault(p => p is Chave);
                chave = retornarVerso(cha);
                indiceChave = lista.IndexOf(cha) + 1;
                quantidadeLista = lista.Count;
              //  cha.Ordenar = (int)lista[cha.Posicao]!.Id;

                indice_Filtro = listaFiltro.OrderBy(f => f.Id).ToList().IndexOf(Fil) + 1;
                Model2 = Fil;
                nameGroup = Model2.Nome!;
                InfoSemCriterio = listaFiltro.OfType<SubFiltro>()
                .FirstOrDefault(f => f.CriterioId == null && f.FiltroId == Model2.Id) != null; // mesma camada de com criterio


                if (Fil.Pagina! != null && Model == null)
                    Model = Fil.Pagina!.OrderBy(p => p.ContentId).Select(p => p.Content)
                    .Where(c => c is Pagina).Skip((Indice - 1) - (slideAtual * quantDiv))
                    .FirstOrDefault();

                listaContent.Clear();
                if (Model == null || Indice == 0)
                {
                    int countPages = CountPagesInFilterAsync((long)Filtro, livro);

                    if (Indice == 0)
                    {
                        List<FiltroContent> resultados = null;
                        var teste = RepositoryPagina.conteudoEmFiltro
                         .FirstOrDefault(cf => cf.conteudoEmFiltro!.ContentId == Model!.Id &&
                         cf.conteudoEmFiltro!.FiltroId == Filtro);

                        Indice = await buscarIndice(Model, countPages, teste);

                        int slideAtivo = (Indice - 1) / quantDiv;
                        slideAtual = slideAtivo;
                    }

                    result.Clear();
                    var l = await PaginarFiltro((long)Filtro, countPages,
                            quantDiv, slideAtual, livro, carregando);
                    result.AddRange(l);

                    Model = result.OrderBy(p => p.ContentId).Select(p => p.Content)
                   .Where(c => c is Pagina)
                   .Skip((Indice - 1) - (slideAtual * quantDiv))
                   // .Skip( (slideAtual * quantDiv))
                   .FirstOrDefault();
                    listaContent.AddRange(result.Select(c => c.Content!).ToList()!);

                    if (carregando <= repositoryPagina!.quantSlidesCarregando - 30)
                        carregando += 10;
                    else
                        carregando = 40;
                }
                else
                {
                    int slideAtivo = (Indice - 1) / quantDiv;
                    slideAtual = slideAtivo;
                    if (alterouPasta)
                    {
                        int countPages = CountPagesInFilterAsync((long)Filtro, livro);
                        result.Clear();
                        var l = await PaginarFiltro((long)Filtro, countPages,
                                quantDiv, slideAtual, livro, carregando);
                        result.AddRange(l);
                        alterouPasta = false;
                    }
                    Model = result.OrderBy(p => p.ContentId).Select(p => p.Content)
                   .Where(c => c is Pagina)
                   .Skip((Indice - 1) - (slideAtual * quantDiv))
                   // .Skip( (slideAtual * quantDiv))
                   .FirstOrDefault();
                    listaContent.AddRange(result.Select(c => c.Content!).ToList()!);
                }
                
                if (Content)
                    if (listaContent.FirstOrDefault(c => c is UserContent) == null)
                        foreach (var item in RepositoryPagina.Conteudo!
                            .Where(c => c.Filtro != null && c is UserContent &&
                            c.Filtro.FirstOrDefault(f => f.FiltroId == Filtro) != null).ToList())

                            if (listaContent.FirstOrDefault(c => c.Id == item.Id) == null)
                                listaContent.Add(item);

                if (Model2.usuarios != null && Model2.usuarios.Count != 0)
                {
                    int camada = repositoryPagina.buscarCamada();
                    instanciarTime(camada);

                }



                if (Model is Pagina)
                {
                    var p = (Pagina)Model;
                    vers = p.Versiculo;

                }
                else vers = 0;

                int cam = repositoryPagina.buscarCamada();
                ultimaPasta = Model2.Camada.Numero == cam;

                // quantidadeLista = listaContent!.Count;

                return listaContent.ToList();
            }
            else if (outroHorizonte == 0 && Filtro != null && rota != null)
            {
                listaContent = new List<Content>();
                foreach (var item in listaFiltro)
                    foreach (var item2 in item.Pagina)
                    {
                        var rotas = item2.Content.Rotas.Split(",");
                        foreach (var rot in rotas)
                            if (rot.ToLower().TrimEnd().TrimStart() == rota.ToLower().TrimEnd().TrimStart())
                                if (!listaContent.Contains(item2.Content))
                                    listaContent.Add(item2.Content);

                    }

                Content pag2 = listaContent!
                    .OrderBy(p => p.Id).Skip((int)Indice - 1).FirstOrDefault()!;

                var str = RepositoryPagina.stories.First(st => st.Id == pag2.StoryId);
                cap = RepositoryPagina.stories.IndexOf(str);

                if (pag2 == null)
                {
                    navigation.NavigateTo($"/renderizar/{storyid}/{indice_Filtro}/0/11/1/1/0/0/0/{dominio}/");
                }

                if (pag2 is Pagina)
                {
                    var p = (Pagina)pag2;
                    vers = p.Versiculo;
                }
                else vers = 0;

                // quantidadeLista = listaContent!.Count;

                return listaContent.ToList();
            }

            return listaContent.ToList();
        }

        private void instanciarTime(int camada)
        {
            Filtro[] fils = new Filtro[10];
            Time time = null;

            fils[0] = listaFiltro.FirstOrDefault(u => u.Id == Model2!.Id)!;
            fils[1] = listaFiltro.FirstOrDefault(u => u.Id == verificarFiltros(fils[0]).Id)!;
            fils[2] = listaFiltro.FirstOrDefault(u => u.Id == verificarFiltros(fils[1]).Id)!;
            fils[3] = listaFiltro.FirstOrDefault(u => u.Id == verificarFiltros(fils[2]).Id)!;
            fils[4] = listaFiltro.FirstOrDefault(u => u.Id == verificarFiltros(fils[3]).Id)!;
            fils[5] = listaFiltro.FirstOrDefault(u => u.Id == verificarFiltros(fils[4]).Id)!;
            fils[6] = listaFiltro.FirstOrDefault(u => u.Id == verificarFiltros(fils[5]).Id)!;
            fils[7] = listaFiltro.FirstOrDefault(u => u.Id == verificarFiltros(fils[6]).Id)!;
            fils[8] = listaFiltro.FirstOrDefault(u => u.Id == verificarFiltros(fils[7]).Id)!;
            fils[9] = listaFiltro.FirstOrDefault(u => u.Id == verificarFiltros(fils[8]).Id)!;

            if (fils[0] is not null && fils[1] is not null &&
                    fils[2] is not null && fils[3] is not null && fils[4] is not null &&
                     fils[5] is not null && fils[6] is not null && fils[7] is not null &&
                     fils[8] is not null && fils[9] is not null)
            {
                time = Context.Time
               .Include(t => t.usuarios)
               .ThenInclude(t => t.UserModel)
               .FirstOrDefault(t =>
               t.usuarios
               .FirstOrDefault(u =>
               fils[0].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null &&
               fils[1].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null &&
               fils[2].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null &&
               fils[3].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null &&
               fils[4].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null &&
               fils[5].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null &&
               fils[6].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null &&
               fils[7].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null &&
               fils[8].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null &&
               fils[9].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null) != null)!;
            }


            if (time is null)
            {
                var times = Context.Time.ToList().Count + 1;
                time = new Time()
                { nome = $"Time - {times}" };
                Context.Add(time);
                Context.SaveChanges();

                for (var i = 0; i < camada - 1; i++)
                    verificarCompartilhante(fils[i], time);

            }
        }

        private Filtro verificarFiltros(Filtro f)
        {
            if (f == null)
                return f;
            else
            {
                SubFiltro camada = (SubFiltro)f;
                return listaFiltro.First(fil => fil.Id == camada.FiltroId);
            }

        }

        private void adicionarPontos()
        {
            int pts = 0;
            int multiplicador = 1;
            Filtro[] fils = new Filtro[6];
            var us = Context.Users.FirstOrDefault(u => u.UserName == Compartilhou);
            var us2 = Context.Filtro.Include(f => f.usuarios).ThenInclude(f => f.UserModel)
                .FirstOrDefault(u => u.Id == Model2.Id);
            var us3 = Context.Filtro.Include(f => f.usuarios).ThenInclude(f => f.UserModel)
                .FirstOrDefault(u => u.Id == verificarFiltros(us2).Id);
            var us4 = Context.Filtro.Include(f => f.usuarios).ThenInclude(f => f.UserModel)
                .FirstOrDefault(u => u.Id == verificarFiltros(us3).Id);
            var us5 = Context.Filtro.Include(f => f.usuarios).ThenInclude(f => f.UserModel)
                .FirstOrDefault(u => u.Id == verificarFiltros(us4).Id);
            var us6 = Context.Filtro.Include(f => f.usuarios).ThenInclude(f => f.UserModel)
                .FirstOrDefault(u => u.Id == verificarFiltros(us5).Id);
            var us7 = Context.Filtro.Include(f => f.usuarios).ThenInclude(f => f.UserModel)
                .FirstOrDefault(u => u.Id == verificarFiltros(us6).Id);
            var users = Context.Filtro.ToList().Count;


            if (us2 != null) { pts = 2; fils[0] = us2; } else fils[0] = null;
            if (us3 != null) { pts = 3; fils[1] = us3; } else fils[1] = null;
            if (us4 != null) { pts = 4; fils[2] = us4; } else fils[2] = null;
            if (us5 != null) { pts = 5; fils[3] = us5; } else fils[3] = null;
            if (us6 != null) { pts = 6; fils[4] = us6; } else fils[4] = null;
            if (us7 != null) { pts = 7; fils[5] = us7; } else fils[5] = null;



            for (var i = 0; i < 6; i++)
            {
                var usuarios = fils[i].usuarios;

                for (var j = 0; j < 7; j++)
                {
                    if (usuarios[j].UserModel != null)
                    {

                        if (DateTime.Now.Date > usuarios[j].UserModel.DataPontuacao.Date)
                        {
                            if (usuarios[j].UserModel.PontosPorDia > usuarios[j].UserModel.Recorde)
                            {
                                usuarios[j].UserModel.Recorde = usuarios[j].UserModel.PontosPorDia;
                                Context.Update(usuarios[j].UserModel);
                                Context.SaveChanges();
                            }
                            usuarios[j].UserModel.PontosPorDia = 1;
                            usuarios[j].UserModel.DataPontuacao = DateTime.Now;
                            Context.Update(usuarios[j].UserModel);
                            Context.SaveChanges();
                        }
                        else
                        {
                            var fil = Context.Users!
                                .FirstOrDefault(f => f.UserName == usuarios[j].UserModel.UserName &&
                                f.Pastas != null);

                            var conteudos = Context.UserContent.Include(c => c.UserModel)
                                    .Where(c => c.UserModel.UserName == usuarios[j].UserModel.UserName &&
                                    c.Data.Date > DateTime.Now.AddDays(-7).Date)
                                    .ToList();

                            if (fil != null && i != 0)
                            {
                                Filtro condicao = listaFiltro.FirstOrDefault(f => f.usuarios
                                     .FirstOrDefault(us => us.UserModelId == usuarios[j].UserModel.Id) != null)!;

                                if (condicao != null)
                                {
                                    var UserModels = condicao.usuarios;
                                    if (users >= 100000 && users < 200000) multiplicador += 1;
                                    else if (users >= 200000 && users < 300000) multiplicador += 2;
                                    else if (users >= 300000 && users < 400000) multiplicador += 3;
                                    else if (users >= 400000 && users < 500000) multiplicador += 4;
                                    else if (users >= 500000 && users < 600000) multiplicador += 5;
                                    else if (users >= 600000 && users < 700000) multiplicador += 6;
                                    else if (users >= 700000 && users < 800000) multiplicador += 7;
                                    else if (users >= 800000 && users < 900000) multiplicador += 8;
                                    else if (users >= 900000) multiplicador += 9;

                                    if (UserModels.Count >= 100 && UserModels.Count < 200) multiplicador += 1;
                                    else if (UserModels.Count >= 200 && UserModels.Count < 300) multiplicador += 2;
                                    else if (UserModels.Count >= 300 && UserModels.Count < 400) multiplicador += 3;
                                    else if (UserModels.Count >= 400 && UserModels.Count < 500) multiplicador += 4;
                                    else if (UserModels.Count >= 500 && UserModels.Count < 600) multiplicador += 5;
                                    else if (UserModels.Count >= 600 && UserModels.Count < 700) multiplicador += 6;
                                    else if (UserModels.Count >= 700 && UserModels.Count < 800) multiplicador += 7;
                                    else if (UserModels.Count >= 800 && UserModels.Count < 900) multiplicador += 8;
                                    else if (UserModels.Count >= 900) multiplicador += 9;


                                    var contentFiltro = conteudos.ToList();
                                    var userTime = Context.UserModelTime
                                        .Include(ut => ut.UserModel)
                                        .Include(ut => ut.Time)
                                        .Where(ut => ut.UserModel.UserName == usuarios[j].UserModel.UserName).ToList();

                                    multiplicador += conteudos.Count;

                                    if (contentFiltro.Count > conteudos.Count / 2)
                                        multiplicador += contentFiltro.Count;

                                    if (userTime.Count > 0)
                                    {
                                        multiplicador += 1;
                                        multiplicador += 2 * userTime.Sum(ut => ut.Time.vendas);

                                        int soma = 0;
                                        List<UserModel> l = new List<UserModel>();

                                        foreach (var t in userTime)
                                            l.Add(Context.Users
                                            .First(u => u.UserName == t.UserModel.UserName));

                                        soma += l.Sum(ut => ut.Recorde);

                                        if (soma > repositoryPagina.metaTime)
                                            multiplicador += 1;


                                        var pontosGanhos = multiplicador * (pts - i);
                                        foreach (var UserModel in UserModels)
                                        {
                                            if (usuarios[0] is not null && UserModel.UserModel.Id == usuarios[0].UserModel.Id)
                                                UserModel.UserModel.PontosPorDia += 1000;
                                            UserModel.UserModel.PontosPorDia += pontosGanhos;
                                            Context.Update(usuarios[j].UserModel);
                                            Context.SaveChanges();
                                        }
                                    }

                                }
                            }
                            else
                            if (i == 0)
                            {
                                multiplicador += conteudos.Count;
                                var pontosGanhos = multiplicador * (pts - i);


                                usuarios[j].UserModel.PontosPorDia += pontosGanhos;
                                Context.Update(usuarios[j]);
                                Context.SaveChanges();
                            }


                        }

                    }
                    else if (i != 0) break;

                }

            }
        }

        private void verificarCompartilhante(Filtro fil, Time time)
        {
            foreach (var item in fil.usuarios)
                item.UserModel.IncluiTime(time);
            Context.SaveChanges();

        }        

        private Filtro buscarProximoSubGrupo()
        {
            for(var i = 1; i < 11; i++)
            {
                 if (Model2.Camada.Numero == i)
                {
                    var indice = returnList(true)
                    .Where(f => f.Camada.Numero == i).OrderBy(f => f.Id).ToList().IndexOf(Model2);

                    if (indice + 1 == returnList(true).Where(f => f.Camada.Numero == i)
                    .OrderBy(f => f.Id).ToList().Count)
                        return returnList(false, true).First();
                    else
                        return returnList(true)
                        .Where(f => f.Camada.Numero == i).OrderBy(f => f.Id).ToList()[indice + 1];
                }
            }           

            return null;
        }

        private Filtro voltarSubgrupos()
        {
            //  var tipo = Model2.GetType();

            for(var i = 10; i > 0; i--)
            {
                if (Model2.Camada.Numero == i)
                {
                    var indice = returnList(true)
                    .Where(f => f.Camada.Numero == i).ToList().IndexOf(Model2);

                    if (indice == 0)
                        return returnList(false, false)!.ToList().LastOrDefault()!;
                    else
                        return returnList(true)
                        .Where(f => f.Camada.Numero == i).ToList()[indice - 1];
                }
            }   
            return null;
        }

        

        private List<Filtro> returnList(bool todos, bool subir = false)
        {
            if(todos)
                return listaFiltro.Where(c => c.Pagina.Count > 0)
                .OrderBy(c => c.Id).ToList();
            else
            {
                if(subir)
                return listaFiltro.Where(c => c.Pagina.Count > 0 && c.Camada.Numero == Model2.Camada.Numero - 1)
                    .OrderBy(c => c.Id).ToList();
                else
                    return listaFiltro.Where(c => c.Pagina.Count > 0 && c.Camada.Numero == Model2.Camada.Numero + 1)
                .OrderBy(c => c.Id).ToList();
                
            }

        }

        private async void perguntar(long pasta)
        {
            try
            {
                if (Filtro != null)
                {
                    var name = listaFiltro.First(f => f.Id == pasta).Nome;

                    if (RepositoryPagina.Perguntar)
                    {
                        string? str = await js.InvokeAsync<string>("contarHistoria", name);

                        if (str == "sim")
                            tellStory = true;
                        else
                            tellStory = false;

                    }



                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro na mensaegem contar historia: " + ex.Message);
            }
        }
               
    }

}
