using System;
using System.Linq;
using System.Text.RegularExpressions;
using BlazorServerCms.Data;
using BlazorServerCms.Pages;
using BlazorServerCms.servicos;
using business;
using business.business;
using business.business.Group;
using business.Group;
using Humanizer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Identity;
using NuGet.Packaging;

namespace BlazorCms.Client.Pages
{
    public partial class RenderizarBase : ComponentBase
    {
        private async Task<int> marcarIndice(int ind)
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
                return 0;
            }

        }

        protected override async Task OnParametersSetAsync()
        {
            if (cap > repositoryPagina!.stories!.Last().PaginaPadraoLink)
                storyid = repositoryPagina!.stories!
                .OrderBy(str => str.PaginaPadraoLink).Skip(1).ToList()[0].Id;

            await renderizar();

            if (cap != 0)
                StartTimer(Model);



            if (Compartilhou != "comp" && pontos != null && Filtro != null ||
                Compartilhante != 0 && pontos != null && Filtro != null)
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

            Model = new Pagina
            {
                Story = new Story(),

            };
            vers = null;
            Auto = 0;
            timeproduto = 11;

            if (compartilhou == null) compartilhou = "comp";
            if (Compartilhante == null)
            {

                Compartilhante = 0;
                Compartilhante2 = 0;
                Compartilhante3 = 0;
                Compartilhante4 = 0;
                Compartilhante5 = 0;
                Compartilhante6 = 0;
                Compartilhante7 = 0;
                Compartilhante8 = 0;
                Compartilhante9 = 0;
                Compartilhante10 = 0;
            }
            if (dominio == null)
                dominio = "dominio";

            if (Auto == 0 && Timer!.desligarAuto! != null
                && Timer!.desligarAuto!.Enabled == true)
            {

                Timer!.desligarAuto!.Elapsed -= desligarAuto_Elapsed;
                Timer!.desligarAuto!.Enabled = false;
                Timer.desligarAuto.Dispose();
            }

            if (indice == 0)
                indice = 1;

            padroes = repositoryPagina.stories.OfType<PatternStory>().ToList().Count - 1;

            if (storyid == null)
            {
                storyid = repositoryPagina.stories.OrderBy(str => str.Id).First().Id;
            }


            if (dominio != repositoryPagina.buscarDominio() && dominio != "dominio")
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
            Pagina pag = (Pagina)c;
            return pag.Versiculo;
        }

        protected async Task renderizar()
        {
            try
            {
                await js!.InvokeAsync<object>("zerar", "1");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            listaContent.Clear();
            ultimaPasta = false;
            var quantidadeFiltros = 0;
            var quantidadePaginas = 0;

            if (outroHorizonte == 0)
            {
                var q = repositoryPagina.Conteudo.OrderBy(c => c.Id)
                    .LastOrDefault(c => c.StoryId == storyid && c is Pagina && c.Html != null)!;
                if (q == null)
                {
                    var pa = Context.Pagina!.OrderBy(c => c.Id)
                    .LastOrDefault(c => c.StoryId == storyid && c is Pagina && c.Html != null)!;
                    repositoryPagina.Conteudo.Add(pa);
                    quantidadeLista = retornarVerso(pa);
                }

                else
                    quantidadeLista = retornarVerso(q);
            }

             if (indice > quantidadeLista)
                quantDiv = await marcarIndice(1);
             else
                quantDiv = await marcarIndice(indice);
            int slideAtivo = (indice - 1) / quantDiv;
            slideAtual = slideAtivo;

            Model = repositoryPagina!.Conteudo
             .FirstOrDefault(p => p is Pagina && retornarVerso(p) == indice && p.StoryId == storyid);


            if (Model == null)
            {
                List<Content> conteudos = null;
                if(filtro == null)
                {
                    conteudos = await GetContentsByStoryIdAsync((long)storyid!, quantidadeLista, quantDiv, slideAtual, carregando);
                                   
                    carregando = 0;
                    listaContent.AddRange(conteudos);

                    foreach (var item in listaContent)
                    if (repositoryPagina.Conteudo.FirstOrDefault(c => c.Id == item.Id) == null)
                    repositoryPagina.Conteudo.Add(item);

                    Model = repositoryPagina.Conteudo
                    .FirstOrDefault(p => p is Pagina && retornarVerso(p) == indice && p.StoryId == storyid);
                }
            }

            if (story == null )
            {
                story = await GetStoryByIdAsync((long)storyid!);
            }
            cap = repositoryPagina.stories.First(st => st.Id == storyid).PaginaPadraoLink;
            nameStory = repositoryPagina.stories.First(st => st.Id == storyid).Nome;

            listaContent = repositoryPagina.Conteudo.Where(c => c is Pagina).OrderBy(p => p.Id)
                .Skip(quantDiv * slideAtual).Take(quantDiv * 2)
                .ToList();

            if (outroHorizonte == 1)
            {
                
                    Model2 = story.Filtro.OrderBy(f => f.Id).Skip((int)indice - 1).FirstOrDefault();
                    quantidadeLista = story.Filtro.ToList().Count;
                    indiceAcesso = story.Filtro.IndexOf(Model2) + 1;
                
            }

            quantidadePaginas =  CountPaginas();

            // if (
            //     story is PatternStory && quantidadePaginas != 99999 && story.Id != repositoryPagina.stories!.First().Id ||
            //     story is SmallStory && quantidadePaginas != 9999 && story.Id != repositoryPagina.stories!.First().Id ||
            //     story is ShortStory && quantidadePaginas != 999 && story.Id != repositoryPagina.stories!.First().Id
            //     )
            //     repositoryPagina.erro = true;

            
                condicaoFiltro = CountFiltros();

            if (filtrar == null && Filtro == null)
            {
                tellStory = false;
                if (indice > quantidadeLista)
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
                        var c = repositoryPagina.Conteudo.First(c => c.Id == pa.ContentId);
                        var s = repositoryPagina.stories.First(s => s.Id == c.StoryId);
                        CapituloComentario = s.PaginaPadraoLink;
                        VersoComentario = repositoryPagina.Conteudo
                        .Where(con => con.StoryId == s.Id).OrderBy(con => con.Id)
                        .ToList().IndexOf(c) + 1;
                    }
                }

            }          

            else if (filtrar != null && condicaoFiltro || Filtro != null && condicaoFiltro)
            {
                listaContent.Clear();
                if (Filtro != null)
                {
                    if (rotas == null)
                        listaContent = await retornarListaFiltrada(null);
                    else
                        listaContent = await retornarListaFiltrada(rotas);

                    foreach(var item in listaContent)
                        if(repositoryPagina.Conteudo.FirstOrDefault(c => c.Id == item.Id) == null)
                            repositoryPagina.Conteudo.Add(item);

                    quantidadeLista = listaContent.Count;                    
                        
                    if (indice > quantidadeLista)
                        quantDiv = await marcarIndice(1);
                    else
                        quantDiv = await marcarIndice(indice);
                    int slideAtivo2 = (indice - 1) / quantDiv;
                    slideAtual = slideAtivo2;

                    if(listaContent.Count >= quantDiv)
                    listaContent = listaContent.Where(c => c is Pagina).OrderBy(p => p.Id)
                .Skip(quantDiv * slideAtual).Take(quantDiv * 2)
                .ToList();
                    else
                        listaContent = listaContent.Where(c => c is Pagina).OrderBy(p => p.Id)
                .Skip(quantDiv * slideAtual).Take(listaContent.Count)
                .ToList();

                }

                if (filtrar != null)
                {

                    var indiceFiltro = int.Parse(filtrar.Replace("pasta-", ""));
                    Filtro fi = null;

                    var fils = story!.Filtro!.OrderBy(f => f.Id).ToList();

                    fi = fils[indiceFiltro - 1];

                    Filtro = fi.Id;
                    indice = 1;

                    acessar();
                }

            }

            // ultimaPasta = Model.UltimaPasta;

            if (Model.Html.Contains("iframe"))
            {
                var conteudoHtml = Model.Html;

              //  if (!conteudoHtml.Contains("?autoplay="))
                    conteudoHtml = colocarAutoPlay(conteudoHtml);

                Model.Html = conteudoHtml;
            }
            if (Model.Html != null)
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
            else if (Model.Produto.Count == 0)
                html = RepositoryPagina.Capa;

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
            listaContent = listaContent.OrderBy(c => c.Id).ToList();
             if (array[0] == null)
             array[0] = new List<Content>();
             if(listaContent.Count > quantDiv)
             array[0].AddRange(listaContent.Take(quantDiv).ToList());
             else
             array[0].AddRange(listaContent);
             
            if (Filtro == null)
            {
                if (indice < 100) classCss = "";
                else if (indice > 99 && indice < 1000) classCss = " DivPagTam2";
                else if (indice > 999 && indice < 10000) classCss = " DivPagTam3";
                else if (indice > 9999 && indice < 100000) classCss = " DivPagTam4";
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

            if (!tellStory) placeholder = "digita nome";
            else placeholder = "Nº do item";

            if (!tellStory) divPagina = "DivPagina";
            else divPagina = "DivPagina2";

            if (!tellStory) DivPag = "DivPag";
            else DivPag = "DivPag2";

            perguntar();
            quantLiked = CountLikes();
        }
        
        private async Task<List<Content>> retornarListaFiltrada(string rota)
        {


            if (outroHorizonte == 0 && Filtro != null && rota == null)
            {
                Filtro Fil = story!.Filtro!.First(f => f.Id == Filtro);
                indice_Filtro = story.Filtro!.OrderBy(f => f.Id).ToList().IndexOf(Fil) + 1;
                Model2 = Fil;
                
                if(repositoryPagina.filtros.FirstOrDefault(f => f.Id == this.Filtro) == null)
                {
                    listaContent.Clear();
                    var conteudos = await GetContentsByFiltroIdAsync((long)Filtro, quantidadeLista, quantDiv, slideAtual, carregando);
                    Fil.Pagina = await GetContentByStoryIdAsync((long)this.Filtro);
                    listaContent.AddRange(Fil.Pagina.Select<FiltroContent, Content>(c => c.Content!).ToList()!);   

                }
                else
                {
                    listaContent.Clear();
                    listaContent.AddRange(Fil.Pagina!.Select<FiltroContent, Content>(c => c.Content!).ToList()!);
                }

               
                    
                

                if (Model2.usuarios != null && Model2.usuarios.Count != 0)
                {
                    var fil = verificarFiltros(Model2);
                    Compartilhante = Model2.Id;
                    Compartilhante2 = 0;
                    Compartilhante3 = 0;
                    Compartilhante4 = 0;
                    Compartilhante5 = 0;
                    Compartilhante6 = 0;
                    Compartilhante7 = 0;
                    Compartilhante8 = 0;
                    Compartilhante9 = 0;
                    Compartilhante10 = 0;


                    if (fil.usuarios.Count != 0)
                    {
                        //compartilhante ganha 2 pts                        
                        Compartilhante = fil.Id;
                        Compartilhante2 = Model2.Id;
                        Compartilhante3 = 0;
                        Compartilhante4 = 0;
                        Compartilhante5 = 0;
                        Compartilhante6 = 0;
                        Compartilhante7 = 0;
                        Compartilhante8 = 0;
                        Compartilhante9 = 0;
                        Compartilhante10 = 0;

                        fil2 = verificarFiltros(fil);
                        if (fil2.usuarios.Count != 0)
                        {
                            //compartilhante ganha 3 pts                            
                            Compartilhante = fil2.Id;
                            Compartilhante2 = fil.Id;
                            Compartilhante3 = Model2.Id;
                            Compartilhante4 = 0;
                            Compartilhante5 = 0;
                            Compartilhante6 = 0;
                            Compartilhante7 = 0;
                            Compartilhante8 = 0;
                            Compartilhante9 = 0;
                            Compartilhante10 = 0;

                            fil3 = verificarFiltros(fil2);
                            if (fil3.usuarios.Count != 0)
                            {
                                //compartilhante ganha 4 pts                                
                                Compartilhante = fil3.Id;
                                Compartilhante2 = fil2.Id;
                                Compartilhante3 = fil.Id;
                                Compartilhante4 = Model2.Id;
                                Compartilhante5 = 0;
                                Compartilhante6 = 0;
                                Compartilhante7 = 0;
                                Compartilhante8 = 0;
                                Compartilhante9 = 0;
                                Compartilhante10 = 0;

                                fil4 = verificarFiltros(fil3);
                                if (fil4.usuarios.Count != 0)
                                {
                                    //compartilhante ganha 5 pts                                    
                                    Compartilhante = fil4.Id;
                                    Compartilhante2 = fil3.Id;
                                    Compartilhante3 = fil2.Id;
                                    Compartilhante4 = fil.Id;
                                    Compartilhante5 = Model2.Id;
                                    Compartilhante6 = 0;
                                    Compartilhante7 = 0;
                                    Compartilhante8 = 0;
                                    Compartilhante9 = 0;
                                    Compartilhante10 = 0;

                                    fil5 = verificarFiltros(fil4);
                                    if (fil5.usuarios.Count != 0)
                                    {
                                        //compartilhante ganha 6 pts                                        
                                        Compartilhante = fil5.Id;
                                        Compartilhante2 = fil4.Id;
                                        Compartilhante3 = fil3.Id;
                                        Compartilhante4 = fil2.Id;
                                        Compartilhante5 = fil.Id;
                                        Compartilhante6 = Model2.Id;
                                        Compartilhante7 = 0;
                                        Compartilhante8 = 0;
                                        Compartilhante9 = 0;
                                        Compartilhante10 = 0;

                                    }

                                }

                            }

                        }

                    }

                    if (fil.usuarios.Count > 0)
                    {
                        int camada = repositoryPagina.buscarCamada();
                        instanciarTime(camada);

                    }

                }
                else if (Model2.usuarios.Count == 0)
                {
                    Compartilhante = 0;
                    Compartilhante2 = 0;
                    Compartilhante3 = 0;
                    Compartilhante4 = 0;
                    Compartilhante5 = 0;
                    Compartilhante6 = 0;
                    Compartilhante7 = 0;
                    Compartilhante8 = 0;
                    Compartilhante9 = 0;
                    Compartilhante10 = 0;
                }

                if (retroceder == 1)
                {
                    indice = listaContent!.Count;
                    retroceder = 0;
                }


                Model = listaContent!
                    .OrderBy(p => p.Id).Skip((int)indice - 1).FirstOrDefault();



                if (Model is Pagina)
                {
                    var p = (Pagina)Model;
                    vers = p.Versiculo;

                }
                else vers = 0;

                //ultimaPasta = Model2.Id == story.Filtro
                //    .Where(f => f.Pagina
                //    .FirstOrDefault(p => p.Content!.Id == Model.Id) != null)
                //        .LastOrDefault()!.Id;

                int cam = repositoryPagina.buscarCamada();
                Type t = retornarPasta(cam);
                ultimaPasta = Model2.GetType() == t;

                quantidadeLista = listaContent!.Count;
            }
            else if (outroHorizonte == 0 && Filtro != null && rota != null)
            {
                listaContent = new List<Content>();
                foreach (var item in story.Filtro)
                    foreach (var item2 in item.Pagina)
                    {
                        var rotas = item2.Content.Rotas.Split(",");
                        foreach (var rot in rotas)
                            if (rot.ToLower().TrimEnd().TrimStart() == rota.ToLower().TrimEnd().TrimStart())
                                if (!listaContent.Contains(item2.Content))
                                    listaContent.Add(item2.Content);

                    }

                Content pag2 = listaContent!
                    .OrderBy(p => p.Id).Skip((int)indice - 1).FirstOrDefault()!;

                var str = repositoryPagina.stories.First(st => st.Id == pag2.StoryId);
                cap = repositoryPagina.stories.IndexOf(str);

                if (pag2 == null)
                {
                    navigation.NavigateTo($"/renderizar/{storyid}/{indice_Filtro}/0/11/1/1/0/0/0/{dominio}/{Compartilhante}");
                }

                if (pag2 is Pagina)
                {
                    var p = (Pagina)pag2;
                    vers = p.Versiculo;
                    // Model = repositoryPagina.includes()
                    // .FirstOrDefault(p => p.Versiculo == vers && p.StoryId == storyid);
                }
                else vers = 0;

                quantidadeLista = listaContent!.Count;
            }


            return listaContent!.OrderBy(c => c.Id).ToList();
        }

        private void instanciarTime(int camada)
        {
            Filtro[] fils = new Filtro[10];
            Time time = null;
            fils[0] = story.Filtro.FirstOrDefault(u => u.Id == Compartilhante)!;
            fils[1] = story.Filtro.FirstOrDefault(u => u.Id == Compartilhante2)!;
            fils[2] = story.Filtro.FirstOrDefault(u => u.Id == Compartilhante3)!;
            fils[3] = story.Filtro.FirstOrDefault(u => u.Id == Compartilhante4)!;
            fils[4] = story.Filtro.FirstOrDefault(u => u.Id == Compartilhante5)!;
            fils[5] = story.Filtro.FirstOrDefault(u => u.Id == Compartilhante6)!;
            fils[6] = story.Filtro.FirstOrDefault(u => u.Id == Compartilhante7)!;
            fils[7] = story.Filtro.FirstOrDefault(u => u.Id == Compartilhante8)!;
            fils[8] = story.Filtro.FirstOrDefault(u => u.Id == Compartilhante9)!;
            fils[9] = story.Filtro.FirstOrDefault(u => u.Id == Compartilhante10)!;
            if (camada == 7 && fils[0] is not null && fils[1] is not null &&
                    fils[2] is not null && fils[3] is not null && fils[4] is not null &&
                     fils[5] is not null)
            {
                time = Context.Time
               .Include(t => t.usuarios)
               .ThenInclude(t => t.UserModel)
               .FirstOrDefault(t =>
               t.usuarios
               .FirstOrDefault(u => fils[0].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null) != null &&
               t.usuarios
               .FirstOrDefault(u => fils[1].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null) != null &&
               t.usuarios
               .FirstOrDefault(u => fils[2].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null) != null &&
               t.usuarios
               .FirstOrDefault(u => fils[3].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null) != null &&
               t.usuarios
               .FirstOrDefault(u => fils[4].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null) != null &&
               t.usuarios
               .FirstOrDefault(u => fils[5].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null) != null)!;

            }

            if (camada == 6 && fils[0] is not null && fils[1] is not null &&
                    fils[2] is not null && fils[3] is not null && fils[4] is not null)
            {
                time = Context.Time
                .Include(t => t.usuarios)
                .ThenInclude(t => t.UserModel)
                .FirstOrDefault(t =>
                t.usuarios
                .FirstOrDefault(u => fils[0].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null) != null &&
                t.usuarios
                .FirstOrDefault(u => fils[1].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null) != null &&
                t.usuarios
                .FirstOrDefault(u => fils[2].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null) != null &&
                t.usuarios
                .FirstOrDefault(u => fils[3].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null) != null &&
                t.usuarios
                .FirstOrDefault(u => fils[4].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null) != null)!;

            }

            if (camada == 5 && fils[0] is not null && fils[1] is not null &&
                    fils[2] is not null && fils[3] is not null)
            {
                time = Context.Time
                .Include(t => t.usuarios)
                .ThenInclude(t => t.UserModel)
                .FirstOrDefault(t =>
                t.usuarios
                .FirstOrDefault(u => fils[0].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null) != null &&
                t.usuarios
                .FirstOrDefault(u => fils[1].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null) != null &&
                t.usuarios
                .FirstOrDefault(u => fils[2].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null) != null &&
                t.usuarios
                .FirstOrDefault(u => fils[3].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null) != null)!;

            }

            if (camada == 4 && fils[0] is not null && fils[1] is not null &&
                   fils[2] is not null)
            {
                time = Context.Time
                .Include(t => t.usuarios)
                .ThenInclude(t => t.UserModel)
                .FirstOrDefault(t =>
                t.usuarios
                .FirstOrDefault(u => fils[0].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null) != null &&
                t.usuarios
                .FirstOrDefault(u => fils[1].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null) != null &&
                t.usuarios
                .FirstOrDefault(u => fils[2].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null) != null)!;

            }

            if (camada == 3 && fils[0] is not null && fils[1] is not null)
            {
                time = Context.Time
                .Include(t => t.usuarios)
                .ThenInclude(t => t.UserModel)
                .FirstOrDefault(t =>
                t.usuarios
                .FirstOrDefault(u => fils[0].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null) != null &&
                t.usuarios
                .FirstOrDefault(u => fils[1].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null) != null)!;

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

        private string retornarUserContent(Content c)
        {
            UserContent u = (UserContent)c;
            return u.UserModelId;
        }

        private List<Content> listarConteudos(Filtro f)
        {
            var fil = story.Filtro.FirstOrDefault(fi => fi.Id == f.Id);
            if (preferencia == null)
                return fil.Pagina!.Select(p => p.Content).ToList()!;
            else
            {
                var user = userManager.Users.First(u => u.UserName == preferencia);
                return fil.Pagina!.Select(p => p.Content)
                    .Where(c => c is UserContent && retornarUserContent(c) == user.Id).ToList()!;
            }
        }

        private Filtro verificarFiltros(Filtro f)
        {
            if (f is CamadaDez)
            {
                CamadaDez camada = (CamadaDez)f;
                return story.Filtro.First(fil => fil.Id == camada.CamadaNoveId);
            }
            else if (f is CamadaNove)
            {
                CamadaNove camada = (CamadaNove)f;
                return story.Filtro.First(fil => fil.Id == camada.CamadaOitoId);
            }
            else if (f is CamadaOito)
            {
                CamadaOito camada = (CamadaOito)f;
                return story.Filtro.First(fil => fil.Id == camada.CamadaSeteId);
            }
            else if (f is CamadaSete)
            {
                CamadaSete camada = (CamadaSete)f;
                return story.Filtro.First(fil => fil.Id == camada.CamadaSeisId);
            }
            else if (f is CamadaSeis)
            {
                CamadaSeis camada = (CamadaSeis)f;
                return story.Filtro.First(fil => fil.Id == camada.SubSubGrupoId);
            }
            else if (f is SubSubGrupo)
            {
                SubSubGrupo camada = (SubSubGrupo)f;
                return story.Filtro.First(fil => fil.Id == camada.SubGrupoId);
            }
            else if (f is SubGrupo)
            {
                SubGrupo camada = (SubGrupo)f;
                return story.Filtro.First(fil => fil.Id == camada.GrupoId);
            }
            else if (f is Grupo)
            {
                Grupo camada = (Grupo)f;
                return story.Filtro.First(fil => fil.Id == camada.SubStoryId);
            }
            else
            {
                return f;
            }
        }

        private void adicionarPontos()
        {
            int pts = 0;
            int multiplicador = 1;
            Filtro[] fils = new Filtro[6];
            var us = Context.Users.FirstOrDefault(u => u.UserName == Compartilhou);
            var us2 = Context.Filtro.Include(f => f.usuarios).ThenInclude(f => f.UserModel)
                .FirstOrDefault(u => u.Id == Compartilhante);
            var us3 = Context.Filtro.Include(f => f.usuarios).ThenInclude(f => f.UserModel)
                .FirstOrDefault(u => u.Id == Compartilhante2);
            var us4 = Context.Filtro.Include(f => f.usuarios).ThenInclude(f => f.UserModel)
                .FirstOrDefault(u => u.Id == Compartilhante3);
            var us5 = Context.Filtro.Include(f => f.usuarios).ThenInclude(f => f.UserModel)
                .FirstOrDefault(u => u.Id == Compartilhante4);
            var us6 = Context.Filtro.Include(f => f.usuarios).ThenInclude(f => f.UserModel)
                .FirstOrDefault(u => u.Id == Compartilhante5);
            var us7 = Context.Filtro.Include(f => f.usuarios).ThenInclude(f => f.UserModel)
                .FirstOrDefault(u => u.Id == Compartilhante6);
            var users = Context.Filtro.ToList().Count;
            var filtros = story.Filtro.Count();

            if (us2 != null) { pts = 2; fils[0] = us2; } else fils[0] = null;
            if (us3 != null) { pts = 3; fils[1] = us3; } else fils[1] = null;
            if (us4 != null) { pts = 4; fils[2] = us4; } else fils[2] = null;
            if (us5 != null) { pts = 5; fils[3] = us5; } else fils[3] = null;
            if (us6 != null) { pts = 6; fils[4] = us6; } else fils[4] = null;
            if (us7 != null) { pts = 7; fils[5] = us7; } else fils[5] = null;

            if (filtros > repositoryPagina.meta1) pts++;
            if (filtros > repositoryPagina.meta2) pts++;
            if (filtros > repositoryPagina.meta3) pts++;
            if (filtros > repositoryPagina.meta4) pts++;
            if (filtros > repositoryPagina.meta5) pts++;
            if (filtros > repositoryPagina.meta6) pts++;
            if (filtros > repositoryPagina.meta7) pts++;
            if (filtros > repositoryPagina.meta8) pts++;

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
                                var condicao = story.Filtro
                                 .FirstOrDefault(f => f.usuarios.FirstOrDefault(us => us.UserModelId == usuarios[j].UserModel.Id) != null);

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
            pontos = null;
        }

        private void verificarCompartilhante(Filtro fil, Time time)
        {
            foreach (var item in fil.usuarios)
                item.UserModel.IncluiTime(time);
            Context.SaveChanges();

        }

        private Type retornarPasta(int camada)
        {
            if (camada == 10)
                return new CamadaDez().GetType();
            if (camada == 9)
                return new CamadaNove().GetType();
            if (camada == 8)
                return new CamadaOito().GetType();
            if (camada == 7)
                return new CamadaSete().GetType();
            if (camada == 6)
                return new CamadaSeis().GetType();
            if (camada == 5)
                return new SubSubGrupo().GetType();
            if (camada == 4)
                return new SubGrupo().GetType();
            if (camada == 3)
                return new Grupo().GetType();
            if (camada == 2)
                return new SubStory().GetType();
            return null;
        }

        private Filtro buscarProximoSubGrupo()
        {
            if (Model2 is CamadaDez)
            {
                var indice = GetIndex(Model2);

                if (indice + 1 == story.Filtro!.OfType<CamadaDez>().Where(HasPages).ToList().Count)
                    return story.Filtro!.OfType<CamadaNove>().Where(HasPages).ToList().First();
                else
                    return returnList(Model2)[indice + 1];
            }
            else if (Model2 is CamadaNove)
            {
                var indice = GetIndex(Model2);

                if (indice + 1 == story.Filtro!.OfType<CamadaNove>().Where(HasPages).ToList().Count)
                    return story.Filtro!.OfType<CamadaOito>().Where(HasPages).ToList().First();
                else
                    return returnList(Model2)[indice + 1];
            }
            else if (Model2 is CamadaOito)
            {
                var indice = GetIndex(Model2);

                if (indice + 1 == story.Filtro!.OfType<CamadaOito>().Where(HasPages).ToList().Count)
                    return story.Filtro!.OfType<CamadaSete>().Where(HasPages).ToList().First();
                else
                    return returnList(Model2)[indice + 1];
            }
            else if (Model2 is CamadaSete)
            {
                var indice = GetIndex(Model2);

                if (indice + 1 == story.Filtro!.OfType<CamadaSete>().Where(HasPages).ToList().Count)
                    return story.Filtro!.OfType<CamadaSeis>().Where(HasPages).ToList().First();
                else
                    return returnList(Model2)[indice + 1];
            }
            else if (Model2 is CamadaSeis)
            {
                var indice = GetIndex(Model2);

                if (indice + 1 == story.Filtro!.OfType<CamadaSeis>().Where(HasPages).ToList().Count)
                    return story.Filtro!.OfType<SubSubGrupo>().Where(HasPages).ToList().First();
                else
                    return returnList(Model2)[indice + 1];
            }
            else if (Model2 is SubSubGrupo)
            {
                var indice = GetIndex(Model2);

                if (indice + 1 == story.Filtro!.OfType<SubSubGrupo>().Where(HasPages).ToList().Count)
                    return story.Filtro!.OfType<SubGrupo>().Where(HasPages).ToList().First();
                else
                    return returnList(Model2)[indice + 1];
            }
            else if (Model2 is SubGrupo)
            {
                var indice = GetIndex(Model2);

                if (indice + 1 == story.Filtro!.OfType<SubGrupo>().Where(HasPages).ToList().Count)
                    return story.Filtro!.OfType<Grupo>().Where(HasPages).ToList().First();
                else
                    return returnList(Model2)[indice + 1];
            }
            else if (Model2 is Grupo)
            {
                var indice = GetIndex(Model2);

                if (indice + 1 == story.Filtro!.OfType<Grupo>().Where(HasPages).ToList().Count)
                    return story.Filtro!.OfType<SubStory>().Where(HasPages).ToList().First();
                else
                    return returnList(Model2)[indice + 1];
            }
            else if (Model2 is SubStory)
            {
                var indice = GetIndex(Model2);

                if (indice + 1 == story.Filtro!.OfType<SubStory>().Where(HasPages).ToList().Count)
                    return null;
                else
                    return returnList(Model2)[indice + 1];
            }

            return null;
        }

        private Filtro voltarSubgrupos()
        {
            if (Model2 is CamadaDez)
            {
                var indice = GetIndex(Model2);

                if (indice == 0)
                    return null;
                else
                    return returnList(Model2)[indice - 1];
            }
            else if (Model2 is CamadaNove)
            {
                var indice = GetIndex(Model2);

                if (indice == 0 && HasValidFilters(story.Filtro!.OfType<CamadaDez>().Where(HasPages).ToList()))
                    return story.Filtro!.OfType<CamadaDez>().Where(HasPages).ToList().LastOrDefault()!;
                else
                    return returnList(Model2)[indice - 1];
            }
            else if (Model2 is CamadaOito)
            {
                var indice = GetIndex(Model2);

                if (indice == 0 && HasValidFilters(story.Filtro!.OfType<CamadaNove>().Where(HasPages).ToList()))
                    return story.Filtro!.OfType<CamadaNove>().Where(HasPages).ToList().LastOrDefault()!;
                else
                    return returnList(Model2)[indice - 1];
            }
            else if (Model2 is CamadaSete)
            {
                var indice = GetIndex(Model2);

                if (indice == 0 && HasValidFilters(story.Filtro!.OfType<CamadaOito>().Where(HasPages).ToList()))
                    return story.Filtro!.OfType<CamadaOito>().Where(HasPages).ToList().LastOrDefault()!;
                else
                    return returnList(Model2)[indice - 1];
            }
            else if (Model2 is CamadaSeis)
            {
                var indice = GetIndex(Model2);

                if (indice == 0 && HasValidFilters(story.Filtro!.OfType<CamadaSete>().Where(HasPages).ToList()))
                    return story.Filtro!.OfType<CamadaSete>().Where(HasPages).ToList().LastOrDefault()!;
                else
                    return returnList(Model2)[indice - 1];
            }
            else if (Model2 is SubSubGrupo)
            {
                var indice = GetIndex(Model2);

                if (indice  == 0 && HasValidFilters(story.Filtro!.OfType<CamadaSeis>().Where(HasPages).ToList()))
                    return story.Filtro!.OfType<CamadaSeis>().Where(HasPages).ToList().LastOrDefault()!;
                else
                    return returnList(Model2)[indice - 1];
            }
            else if (Model2 is SubGrupo)
            {
                var indice = GetIndex(Model2);

                if (indice == 0 && HasValidFilters(story.Filtro!.OfType<SubSubGrupo>().Where(HasPages).ToList()))
                    return story.Filtro!.OfType<SubSubGrupo>().Where(HasPages).ToList().LastOrDefault()!;
                else
                    return returnList(Model2)[indice - 1];
            }
            else if (Model2 is Grupo)
            {
                var indice = GetIndex(Model2);

                if (indice == 0 && HasValidFilters(story.Filtro!.OfType<SubGrupo>().Where(HasPages).ToList()))
                    return story.Filtro!.OfType<SubGrupo>().Where(HasPages).ToList().LastOrDefault()!;
                else
                    return returnList(Model2)[indice - 1];
            }
            else if (Model2 is SubStory)
            {
                var indice = GetIndex(Model2);

                if (indice  == 0 && HasValidFilters(story.Filtro!.OfType<Grupo>().Where(HasPages).ToList()))
                    return story.Filtro!.OfType<Grupo>().Where(HasPages).ToList().LastOrDefault()!;
                else
                    return returnList(Model2)[indice - 1];
            }

            return null;
        }

        private bool HasPages(Filtro filtro)
        {
            return filtro.Pagina != null && filtro.Pagina.Count > 0;
        }
        
        private bool HasValidFilters<T>(IEnumerable<T> collection)
        {
            return collection.ToList().Count > 0;
        }

        private List<T> returnList<T>(T item)
        {
            var lista = story.Filtro!.Where(HasPages).ToList();
            return lista.OfType<T>().ToList();
        }

        private int GetIndex<T>(T item)
        {
            var lista = story.Filtro!.Where(HasPages).ToList();
            return lista.OfType<T>().ToList().IndexOf(item);
        }
   
        private async void perguntar()
        {
            try
            {
                if(alterouPasta && Model2 != null)
                {
                    string? str = await js.InvokeAsync<string>("contarHistoria", Model2.Nome);

                    if (str == "sim")
                        tellStory = true;
                    else
                        tellStory = false;
                    alterouPasta = false;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro na mensaegem contar historia: " + ex.Message);
            }
        }
    }
}
