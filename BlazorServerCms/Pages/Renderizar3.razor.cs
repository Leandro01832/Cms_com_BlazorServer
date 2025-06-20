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
                .OrderBy(str => str.Capitulo).Skip(1).ToList()[0].Id;

            await renderizar();

            if (cap != 0)
                StartTimer(Model);

            if (Model2 != null && Model2.usuarios != null && Model2.usuarios.Count > 0  && Filtro != null)
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

            
            vers = null;
            Auto = 0;
            timeproduto = 11;

            if (compartilhou == null) compartilhou = "comp";
            
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

            padroes = RepositoryPagina.stories.OfType<PatternStory>().ToList().Count - 1;

            if (storyid == null)
            {
                storyid = RepositoryPagina.stories.OrderBy(str => str.Id).First().Id;
            }


            if (nomeLivro != null)            
                livro = await Context.Livro!.FirstOrDefaultAsync(l => l.Nome == nomeLivro);

            if (_story == null)
            {
                _story = await GetStoryByIdAsync((long)storyid!);
            }

            if (livro == null)
                listaFiltro = await Context.Filtro!
                     .Include(p => p.Pagina)!
                     .ThenInclude(p => p.Content)!
                     .Include(p => p.usuarios)!
                     .ThenInclude(p => p.UserModel)!
                    .Where(f => f.LivroId == null && f.StoryId == _story.Id).ToListAsync();
            else
                listaFiltro = await Context.Filtro!
                    .Include(p => p.Pagina)!
                     .ThenInclude(p => p.Content)!
                     .Include(p => p.usuarios)!
                     .ThenInclude(p => p.UserModel)!
                    .Where(f => f.LivroId == livro.Id && f.StoryId == _story.Id).ToListAsync();


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

            if(Filtro == null)
            listaContent.Clear();
            ultimaPasta = false;
            var quantidadeFiltros = 0;
            var quantidadePaginas = 0;
            

            if (outroHorizonte == 0)
            {
                var q = RepositoryPagina.Conteudo.OrderBy(c => c.Id)
                    .LastOrDefault(c => c.StoryId == storyid && c is Pagina && c.Html != null)!;
                if (q == null)
                {
                    var pa = Context.Pagina!.OrderBy(c => c.Id)
                    .LastOrDefault(c => c.StoryId == storyid && c is Pagina && c.Html != null)!;
                    RepositoryPagina.Conteudo.Add(pa);
                    quantidadeLista = retornarVerso(pa);
                }

                else
                    quantidadeLista = retornarVerso(q);
            }   


            if (Filtro == null)
                Model = RepositoryPagina.Conteudo
             .FirstOrDefault(p => p is Pagina && retornarVerso(p) == indice && p.StoryId == storyid);

            else if (Filtro != null && RepositoryPagina.Conteudo
                .Where(c => c is Pagina && c.Filtro != null &&
                c.Filtro!.FirstOrDefault(f => f.FiltroId == Filtro) != null).ToList().Count ==
                CountPagesInFilterAsync((long)Filtro, livro))
            {
                var listainFilter = RepositoryPagina.Conteudo.OrderBy(c => c.Id)
                .Where(c => c is Pagina && c.Filtro != null &&
                c.Filtro!.FirstOrDefault(f => f.FiltroId == Filtro) != null).ToList();

                if(indice != 0)
                Model = listainFilter[indice - 1];
                else
                {
                    var m = listainFilter.First(c => c.Id == Model!.Id);
                    indice = listainFilter.IndexOf(m) + 1;
                }                
            }
              

            if (indice > quantidadeLista)
                quantDiv = await marcarIndice();
            else
                quantDiv = await marcarIndice();
            int slideAtivo = (indice - 1) / quantDiv;
            slideAtual = slideAtivo;

            if (Filtro != null && RepositoryPagina.Conteudo
                .Where(c => c is Pagina && c.Filtro != null &&
                c.Filtro!.FirstOrDefault(f => f.FiltroId == Filtro) != null).ToList().Count ==
                CountPagesInFilterAsync((long)Filtro, livro))
                listaContent = RepositoryPagina.Conteudo
                .Where(c => c is Pagina && c.Filtro != null &&
                c.Filtro!.FirstOrDefault(f => f.FiltroId == Filtro) != null).OrderBy(p => p.Id)
                .Skip(quantDiv * slideAtual).Take(quantDiv)
                .ToList();


            if (Model == null && Filtro == null)
            {
                List<Content> conteudos = null;
                if(Filtro == null)
                {
                    conteudos = await PaginarStory((long)storyid!, quantidadeLista, quantDiv, slideAtual, livro, carregando);                                   
                    listaContent.AddRange(conteudos);

                    foreach (var item in listaContent)
                    if (RepositoryPagina.Conteudo.FirstOrDefault(c => c.Id == item.Id) == null)
                    RepositoryPagina.Conteudo.Add(item);

                    Model = RepositoryPagina.Conteudo
                    .FirstOrDefault(p => p is Pagina && retornarVerso(p) == indice && p.StoryId == storyid);
                }
            }

            
            cap = RepositoryPagina.stories.First(st => st.Id == storyid).Capitulo;
            nameStory = RepositoryPagina.stories.First(st => st.Id == storyid).Nome;

            if(Filtro == null)
            listaContent = RepositoryPagina.Conteudo.Where(c => c is Pagina).OrderBy(p => p.Id)
                .Skip(quantDiv * slideAtual).Take(quantDiv * 2)
                .ToList();

            if (outroHorizonte == 1)
            {                         
                    Model2 = listaFiltro.OrderBy(f => f.Id).Skip((int)indice - 1).FirstOrDefault();                    
                    quantidadeLista = listaFiltro.ToList().Count;                    
                    indiceAcesso = listaFiltro!.ToList().IndexOf(Model2) + 1;  
            }

            quantidadePaginas =  CountPaginas();

            // if (
            //     story is PatternStory && quantidadePaginas != 99999 && story.Id != repositoryPagina.stories!.First().Id ||
            //     story is SmallStory && quantidadePaginas != 9999 && story.Id != repositoryPagina.stories!.First().Id ||
            //     story is ShortStory && quantidadePaginas != 999 && story.Id != repositoryPagina.stories!.First().Id
            //     )
            //     repositoryPagina.erro = true;

            
                condicaoFiltro = CountFiltros();

            if (Filtro == null)
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
                        var c = RepositoryPagina.Conteudo.First(c => c.Id == pa.ContentId);
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
                    if (rotas == null)
                        listaContent = await retornarListaFiltrada(null);
                    else
                        listaContent = await retornarListaFiltrada(rotas);

                    if(listaContent.Count != 0 && listaContent[0] != null)
                    foreach(var item in listaContent)
                        if(RepositoryPagina.Conteudo.FirstOrDefault(c => c.Id == item.Id) == null)
                            RepositoryPagina.Conteudo.Add(item);

                    quantidadeLista = listaContent.Count;      

                    if(!Content)
                    listaContent = listaContent.Where(c => c is Pagina).OrderBy(p => p.Id)
                .Skip(quantDiv * slideAtual).Take(quantDiv * 2)
                .ToList();
                    else
                        listaContent = listaContent.Where(c => c is UserContent).OrderBy(p => p.Id)
                .Skip(quantDiv * slideAtual).Take(listaContent.Count)
                .ToList();

                }

            }

            // ultimaPasta = Model.UltimaPasta;

            if (Model != null && Model.Html != null && Model.Html.Contains("iframe") && alterouModel)
            {
                var conteudoHtml = Model.Html;
                conteudoHtml = colocarAutoPlay(conteudoHtml);
                Model.Html = conteudoHtml;
                alterouModel = false;
            }
            if (Model != null && Model.Html != null)
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
            else if (Model != null && Model.Produto != null && Model.Produto.Count == 0)
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
            if (listaContent.Count != 0 && listaContent[0] != null)
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
                Filtro Fil = listaFiltro.First(f => f.Id == Filtro);
                
                indice_Filtro = listaFiltro.OrderBy(f => f.Id).ToList().IndexOf(Fil) + 1;
                Model2 = Fil;
                nameGroup = Model2.Nome!;
                

                if (Fil.Pagina! != null && Model == null)
                    Model = Fil.Pagina!.OrderBy(p => p.ContentId).Select(p => p.Content)
                    .Where(c => c is Pagina).Skip((indice - 1) - (slideAtual * quantDiv)).FirstOrDefault();

                if (Model == null || indice == 0)
                {
                    int countPages = CountPagesInFilterAsync((long)Filtro, livro);
                    listaContent.Clear();
                   
                    if(indice == 0)
                    {
                        List<FiltroContent> resultados = null;
                        var teste = RepositoryPagina.conteudoEmFiltro
                         .FirstOrDefault(cf => cf.conteudoEmFiltro!.ContentId == Model!.Id &&
                         cf.conteudoEmFiltro!.FiltroId == Filtro);

                       buscarIndice(Model, countPages, teste);                        
                                              
                        int slideAtivo = (indice - 1) / quantDiv;
                        slideAtual = slideAtivo;
                    }

                    
                    Fil.Pagina = await PaginarFiltro((long)Filtro, countPages,
                            quantDiv, slideAtual, livro, carregando);

                    Model = Fil.Pagina!.OrderBy(p => p.ContentId).Select(p => p.Content)
                   .Where(c => c is Pagina)
                   .Skip((indice - 1) - (slideAtual * quantDiv)).FirstOrDefault();
                    listaContent.AddRange(Fil.Pagina.Select(c => c.Content!).ToList()!);  

                    if (carregando <= repositoryPagina!.quantSlidesCarregando - 30)
                        carregando += 10;
                    else
                        carregando = 40;
                }

                if(Content)
                if (listaContent.FirstOrDefault(c => c is UserContent) == null)
                    foreach (var item in RepositoryPagina.Conteudo
                        .Where(c => c.Filtro != null && c is UserContent &&
                        c.Filtro.FirstOrDefault(f => f.FiltroId == Filtro) != null).ToList())

                        if (listaContent.FirstOrDefault(c => c.Id == item.Id) == null)
                            listaContent.Add(item);

                if (Model2.usuarios != null && Model2.usuarios.Count != 0)
                {
                    int camada = repositoryPagina.buscarCamada();
                    instanciarTime(camada);

                }

                if (retroceder == 1)
                {
                    indice = listaContent!.Count;
                    retroceder = 0;
                }
                
                if (Model is Pagina)
                {
                    var p = (Pagina)Model;
                    vers = p.Versiculo;

                }
                else vers = 0;

                int cam = repositoryPagina.buscarCamada();
                Type t = retornarPasta(cam);
                ultimaPasta = Model2.GetType() == t;

                quantidadeLista = listaContent!.Count;
                
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
                    .OrderBy(p => p.Id).Skip((int)indice - 1).FirstOrDefault()!;

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

                quantidadeLista = listaContent!.Count;
                
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
           
            if ( fils[0] is not null && fils[1] is not null &&
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
            if (f is CamadaDez)
            {
                CamadaDez camada = (CamadaDez)f;
                return listaFiltro.First(fil => fil.Id == camada.CamadaNoveId);
            }
            else if (f is CamadaNove)
            {
                CamadaNove camada = (CamadaNove)f;
                return listaFiltro.First(fil => fil.Id == camada.CamadaOitoId);
            }
            else if (f is CamadaOito)
            {
                CamadaOito camada = (CamadaOito)f;
                return listaFiltro.First(fil => fil.Id == camada.CamadaSeteId);
            }
            else if (f is CamadaSete)
            {
                CamadaSete camada = (CamadaSete)f;
                return listaFiltro.First(fil => fil.Id == camada.CamadaSeisId);
            }
            else if (f is CamadaSeis)
            {
                CamadaSeis camada = (CamadaSeis)f;
                return listaFiltro.First(fil => fil.Id == camada.SubSubGrupoId);
            }
            else if (f is SubSubGrupo)
            {
                SubSubGrupo camada = (SubSubGrupo)f;
                return listaFiltro.First(fil => fil.Id == camada.SubGrupoId);
            }
            else if (f is SubGrupo)
            {
                SubGrupo camada = (SubGrupo)f;
                return listaFiltro.First(fil => fil.Id == camada.GrupoId);
            }
            else 
            {
                Grupo camada = (Grupo)f;
                return listaFiltro.First(fil => fil.Id == camada.SubStoryId);
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
            //  var properties = Model2!.GetType().GetProperties();

            if (Model2 is CamadaDez)
            {
                var indice = listaFiltro.Where(f => f.GetType() == Model2.GetType()).ToList().IndexOf(Model2);

                if (indice + 1 == listaFiltro!.OfType<CamadaDez>().ToList().Count)
                    return listaFiltro!.OfType<CamadaNove>().ToList().First();
                else
                    return returnList<CamadaDez>()[indice + 1];
            }
            else if (Model2 is CamadaNove)
            {
                var indice = listaFiltro.Where(f => f.GetType() == Model2.GetType()).ToList().IndexOf(Model2);

                if (indice + 1 == listaFiltro!.OfType<CamadaNove>().ToList().Count)
                    return listaFiltro!.OfType<CamadaOito>().ToList().First();
                else
                    return returnList<CamadaNove>()[indice + 1];
            }
            else if (Model2 is CamadaOito)
            {
                var indice = listaFiltro.Where(f => f.GetType() == Model2.GetType()).ToList().IndexOf(Model2);

                if (indice + 1 == listaFiltro!.OfType<CamadaOito>().ToList().Count)
                    return listaFiltro!.OfType<CamadaSete>().ToList().First();
                else
                    return returnList<CamadaOito>()[indice + 1];
            }
            else if (Model2 is CamadaSete)
            {
                var indice = listaFiltro.Where(f => f.GetType() == Model2.GetType()).ToList().IndexOf(Model2);

                if (indice + 1 == listaFiltro!.OfType<CamadaSete>().ToList().Count)
                    return listaFiltro!.OfType<CamadaSeis>().ToList().First();
                else
                    return returnList<CamadaSete>()[indice + 1];
            }
            else if (Model2 is CamadaSeis)
            {
                var indice = listaFiltro.Where(f => f.GetType() == Model2.GetType()).ToList().IndexOf(Model2);

                if (indice + 1 == listaFiltro!.OfType<CamadaSeis>().ToList().Count)
                    return listaFiltro!.OfType<SubSubGrupo>().ToList().First();
                else
                    return returnList<CamadaSeis>()[indice + 1];
            }
            else if (Model2 is SubSubGrupo)
            {
                var indice = listaFiltro.Where(f => f.GetType() == Model2.GetType()).ToList().IndexOf(Model2);

                if (indice + 1 == listaFiltro!.OfType<SubSubGrupo>().ToList().Count)
                    return listaFiltro!.OfType<SubGrupo>().ToList().First();
                else
                    return returnList<SubSubGrupo>()[indice + 1];
            }
            else if (Model2 is SubGrupo)
            {
                var indice = listaFiltro.Where(f => f.GetType() == Model2.GetType()).ToList().IndexOf(Model2);

                if (indice + 1 == listaFiltro!.OfType<SubGrupo>().ToList().Count)
                    return listaFiltro!.OfType<Grupo>().ToList().First();
                else
                    return returnList<SubGrupo>()[indice + 1];
            }
            else if (Model2 is Grupo)
            {
                var indice = listaFiltro.Where(f => f.GetType() == Model2.GetType()).ToList().IndexOf(Model2);

                if (indice + 1 == listaFiltro!.OfType<Grupo>().ToList().Count)
                    return listaFiltro!.OfType<SubStory>().ToList().First();
                else
                    return returnList<Grupo>()[indice + 1];
            }
            else if (Model2 is SubStory)
            {
                var indice = listaFiltro.Where(f => f.GetType() == Model2.GetType()).ToList().IndexOf(Model2);

                if (indice + 1 == listaFiltro!.OfType<SubStory>().ToList().Count)
                    return null;
                else
                    return returnList<SubStory>()[indice + 1];
            }

            return null;
        }

        private Filtro voltarSubgrupos()
        {
            //  var tipo = Model2.GetType();

            if (Model2 is CamadaDez)
            {
                var indice = listaFiltro.Where(f => f.GetType() == Model2.GetType()).ToList().IndexOf(Model2);

                if (indice == 0)
                    return null;
                else
                    return returnList<CamadaDez>()[indice - 1];
            }
            else if (Model2 is CamadaNove)
            {
                var indice = listaFiltro.Where(f => f.GetType() == Model2.GetType()).ToList().IndexOf(Model2);

                if (indice == 0 && HasValidFilters(listaFiltro!.OfType<CamadaDez>().ToList()))
                    return listaFiltro!.OfType<CamadaDez>().ToList().LastOrDefault()!;
                else
                    return returnList<CamadaNove>()[indice - 1];
            }
            else if (Model2 is CamadaOito)
            {
                var indice = listaFiltro.Where(f => f.GetType() == Model2.GetType()).ToList().IndexOf(Model2);

                if (indice == 0 && HasValidFilters(listaFiltro!.OfType<CamadaNove>().ToList()))
                    return listaFiltro!.OfType<CamadaNove>().ToList().LastOrDefault()!;
                else
                    return returnList<CamadaOito>()[indice - 1];
            }
            else if (Model2 is CamadaSete)
            {
                var indice = listaFiltro.Where(f => f.GetType() == Model2.GetType()).ToList().IndexOf(Model2);

                if (indice == 0 && HasValidFilters(listaFiltro!.OfType<CamadaOito>().ToList()))
                    return listaFiltro!.OfType<CamadaOito>().ToList().LastOrDefault()!;
                else
                    return returnList<CamadaSete>()[indice - 1];
            }
            else if (Model2 is CamadaSeis)
            {
                var indice = listaFiltro.Where(f => f.GetType() == Model2.GetType()).ToList().IndexOf(Model2);

                if (indice == 0 && HasValidFilters(listaFiltro!.OfType<CamadaSete>().ToList()))
                    return listaFiltro!.OfType<CamadaSete>().ToList().LastOrDefault()!;
                else
                    return returnList<CamadaSeis>()[indice - 1];
            }
            else if (Model2 is SubSubGrupo)
            {
                var indice = listaFiltro.Where(f => f.GetType() == Model2.GetType()).ToList().IndexOf(Model2);

                if (indice  == 0 && HasValidFilters(listaFiltro!.OfType<CamadaSeis>().ToList()))
                    return listaFiltro!.OfType<CamadaSeis>().ToList().LastOrDefault()!;
                else
                    return returnList<SubSubGrupo>()[indice - 1];
            }
            else if (Model2 is SubGrupo)
            {
                var indice = listaFiltro.Where(f => f.GetType() == Model2.GetType()).ToList().IndexOf(Model2);

                if (indice == 0 && HasValidFilters(listaFiltro!.OfType<SubSubGrupo>().ToList()))
                    return listaFiltro!.OfType<SubSubGrupo>().ToList().LastOrDefault()!;
                else
                    return returnList<SubGrupo>()[indice - 1];
            }
            else if (Model2 is Grupo)
            {
                var indice = listaFiltro.Where(f => f.GetType() == Model2.GetType()).ToList().IndexOf(Model2);

                if (indice == 0 && HasValidFilters(listaFiltro!.OfType<SubGrupo>().ToList()))
                    return listaFiltro!.OfType<SubGrupo>().ToList().LastOrDefault()!;
                else
                    return returnList<Grupo>()[indice - 1];
            }
            else if (Model2 is SubStory)
            {
                var indice = listaFiltro.Where(f => f.GetType() == Model2.GetType()).ToList().IndexOf(Model2);

                if (indice  == 0 && HasValidFilters(listaFiltro!.OfType<Grupo>().ToList()))
                    return listaFiltro!.OfType<Grupo>().ToList().LastOrDefault()!;
                else
                    return returnList<SubStory>()[indice - 1];
            }

            return null;
        }
               
        private bool HasValidFilters<T>(IEnumerable<T> collection)
        {
            return collection.ToList().Count > 0;
        }

        private List<T> returnList<T>()
        {
            return listaFiltro.OfType<T>().ToList();
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
