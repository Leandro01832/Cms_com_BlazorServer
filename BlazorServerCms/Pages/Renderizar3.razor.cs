using BlazorServerCms.Data;
using BlazorServerCms.Pages;
using BlazorServerCms.servicos;
using business;
using business.business;
using business.business.Group;
using business.Group;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace BlazorCms.Client.Pages
{
    public partial class RenderizarBase : ComponentBase
    { 
        protected override async Task OnParametersSetAsync()
        {
            if (cap > repositoryPagina!.stories!.Last().PaginaPadraoLink)
                storyid = repositoryPagina!.stories!
                .OrderBy(str => str.PaginaPadraoLink).Skip(1).ToList()[0].Id;

            await renderizar();

            if(cap != 0)
            StartTimer(Model);



            if (Compartilhou != "comp" && pontos != null && substory != null  ||
                Compartilhante != 0 && pontos != null && substory != null)            
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

            if (substory != null)
                alterouPasta = true;

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

            var Stories = await Context.Story.OrderBy(s => s.Id).ToListAsync();
            var conteudos = await  Context!.Content!.ToListAsync();
            var UserModel =   Context!.Users!.ToList();
            var lista = await repositoryPagina.buscarPatternStory();

            if (Stories.Count == 4 || lista.Count !=  Stories.OfType<PatternStory>().ToList().Count )
            {
                if(Stories.Count == 4)
                {
                    foreach (var story in lista.OrderBy(str => str.PaginaPadraoLink).Skip(4).ToList())
                    {
                        var str = new PatternStory(story.Nome, Context.Story.ToList(), Stories.First());
                        Context.Add(str);
                        Context.SaveChanges();
                    }

                    var shortStory1 = new ShortStory("promoções 10% off", Context.Story.ToList(), Stories.First());
                    Context.Add(shortStory1);
                    var shortStory2 = new ShortStory("promoções 30% off", Context.Story.ToList(), Stories.First());
                    Context.Add(shortStory2);
                    var shortStory3 = new ShortStory("promoções 50% off", Context.Story.ToList(), Stories.First());
                    Context.Add(shortStory3);
                    var shortStory4 = new SmallStory("Logistica", Context.Story.ToList(), Stories.First());
                    Context.Add(shortStory4);
                    var shortStory5 = new SmallStory("Filiais", Context.Story.ToList(), Stories.First());
                    Context.Add(shortStory5);
                    var shortStory6 = new SmallStory("Entrega de produtos", Context.Story.ToList(), Stories.First());
                    Context.Add(shortStory6);

                    Context.SaveChanges();
                }
                else
                {
                    foreach(var item in lista)
                    {
                        if(Stories.OfType<PatternStory>().ToList()
                        .FirstOrDefault(l => l.PaginaPadraoLink == item.PaginaPadraoLink) == null)
                        {
                            Context.Add(item);
                            Context.SaveChanges();
                        }
                    }
                    var compartilharCapitulo = Context.Story.Where(str => !(str is  PatternStory))
                        .OrderBy(str => str.Id).ToList();
                    for(var i = 0; i < compartilharCapitulo.Count; i++)
                    {
                        compartilharCapitulo[i].PaginaPadraoLink = 
                        Context.PatternStory!
                        .OrderBy(ps => ps.Id)
                        .Last().PaginaPadraoLink + i + 1;
                        Context.Update(compartilharCapitulo[i]);
                        Context.SaveChanges();
                    }
                }
                
            
            }

            padroes = Stories.OfType<PatternStory>().ToList().Count - 1;

            if(storyid == null)
            {
                storyid = Stories.OrderBy(str => str.Id).First().Id;
            }

            if (repositoryPagina.stories.Count == 0 || 
                repositoryPagina.stories.Count != Stories.Count)
            {
                var str = await Context.Story!
                    .Include(p => p.Filtro)!
                   .ThenInclude(p => p.usuarios)!
                   .ThenInclude(p => p.UserModel)!
                    .Include(p => p.Filtro)!
                   .ThenInclude(p => p.Pagina)!
                   .ThenInclude(p => p.Content)
                    .OrderBy(st => st.Id).ToListAsync();
                repositoryPagina.stories.Clear();
                repositoryPagina.stories.AddRange(str);

            }
                          

            if (repositoryPagina.Conteudo.Count == 0 ||
                repositoryPagina.Conteudo.Count != conteudos.Count)
            {
                conteudos = Context!.Content!
                    .Include(c => c.Filtro)
                    .Include(c => c.Produto)
                    .ThenInclude(c => c.Produto)
                    .Where(c => c.StoryId == storyid)
                    .ToList();
                repositoryPagina.Conteudo.AddRange(conteudos);
            }

            if (repositoryPagina.UserModel.Count == 0 ||
                repositoryPagina.UserModel.Count != UserModel.Count)
            {
                repositoryPagina.UserModel.Clear();
                repositoryPagina.UserModel.AddRange(UserModel);
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

        private long retornarVerso(Content c)
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

            ultimaPasta = false;
            var quantidadeFiltros = 0;
            var quantidadePaginas = 0;
            List<Content> listaFiltradaComConteudo = null;

            Model = repositoryPagina!.Conteudo
                    .FirstOrDefault(p =>  p is Pagina && retornarVerso(p) == indice && p.StoryId == storyid );  

            if (Model == null)
            {
               var conteudos = Context!.Content!
                    .Include(c => c.Filtro)
                    .Include(c => c.Produto)
                    .ThenInclude(c => c.Produto)
                    .Where(c => c.StoryId == storyid)
                    .ToList();
                repositoryPagina.Conteudo.AddRange(conteudos);
                Model = repositoryPagina.Conteudo
                .FirstOrDefault(p => p is Pagina && p.StoryId == storyid);
            }
            
            if (outroHorizonte == 1)
            {
                if (Model != null)
                {
                    Model2 = story.Filtro.OrderBy(f => f.Id).Skip((int)indice - 1).FirstOrDefault(); 

                    quantidadeLista = story.Filtro.ToList().Count;
                    indiceAcesso = story.Filtro.IndexOf(Model2) + 1;
                }
            }
           

            if (story == null ||  story.Id != Model.StoryId)
            {
                story = repositoryPagina.stories!
               .First(p => p.Id == Model!.StoryId);
            }

            cap = story.PaginaPadraoLink;
           
               
            quantidadePaginas = CountPaginas(ApplicationDbContext._connectionString);

           // if (
           //     story is PatternStory && quantidadePaginas != 99999 && story.Id != repositoryPagina.stories!.First().Id ||
           //     story is SmallStory && quantidadePaginas != 9999 && story.Id != repositoryPagina.stories!.First().Id ||
           //     story is ShortStory && quantidadePaginas != 999 && story.Id != repositoryPagina.stories!.First().Id
           //     )
           //     repositoryPagina.erro = true;

            if (quantidadePaginas == 0 && outroHorizonte == 0)
                Mensagem = "aguarde um momento...";
            var proximo = indice + 1;
            var anterior = indice - 1;


            if (outroHorizonte == 0)
                quantidadeLista = 
                    retornarVerso(repositoryPagina.Conteudo.
                    Where(c => c.StoryId == story.Id && c is Pagina && c.Html != null)
                    .OrderBy(c => c.Id).LastOrDefault()!);

            if (Model != null)
                condicaoFiltro = CountFiltros(ApplicationDbContext._connectionString);

        

            if (filtrar == null && substory == null)
            {
                tellStory = false;
                if (Model == null)
                {
                    if (quantidadePaginas != 0)
                        Mensagem = $"Por favor digite um numero menor que {quantidadeLista}.";
                    else
                        Mensagem = "aguarde um momento...";
                    return;
                }

                nameStory = story.Nome;

                if (Model is Comment)
                {
                    Comment pa = (Comment)Model;
                    if(pa.ContentId != null)
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

            else if (filtrar != null && condicaoFiltro || substory != null && condicaoFiltro)
            {
                




                if (substory != null)
                {
                    if(rotas == null)
                    listaFiltradaComConteudo = retornarListaFiltrada(null);
                    else
                    listaFiltradaComConteudo = retornarListaFiltrada(rotas);

                   


                }

                if (filtrar != null)
                {

                    var indiceFiltro = int.Parse(filtrar.Replace("pasta-", ""));
                    Filtro fi = null;

                    var fils = story!.Filtro!.OrderBy(f => f.Id).ToList();

                    fi = fils[indiceFiltro - 1];

                    var arr =  retornarArray(fi);
                    indice = 1;

                    setarCamadas(arr);
                    acessar();
                }

            }

            // ultimaPasta = Model.UltimaPasta;

            if ( Model.Html.Contains("iframe"))
            {
                var conteudoHtml = Model.Html;

                if (!conteudoHtml.Contains("?autoplay="))
                    conteudoHtml =  colocarAutoPlay(conteudoHtml);

                    Model.Html = conteudoHtml;
            }
            if ( Model.Html != null)
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

                if (alterouPasta)
                {
                    try
                    {
                        string? str = await js.InvokeAsync<string>("contarHistoria", Model2.Nome);

                        if (str == "sim")
                            tellStory = true;
                        else
                            tellStory = false;
                        alterouPasta = false;
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine("Erro na mensaegem contar historia: " + ex.Message);
                    }
                }

            try
            {
                if (!tellStory)
                {

                    if (indice > quantidadeLista)
                        await js!.InvokeAsync<object>("MarcarIndice", "1");
                    else
                        await js!.InvokeAsync<object>("MarcarIndice", $"{indice}");                   
                }
                else
                {
                    if (indice > quantidadeLista)
                        await js!.InvokeAsync<object>("MarcarIndice2", "1");
                    else
                        await js!.InvokeAsync<object>("MarcarIndice2", $"{indice}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if(substory == null)
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


            quantLiked = CountLikes(ApplicationDbContext._connectionString);
        }

        private void setarCamadas(int?[] arr)
        {
            if (arr != null)
            {
                substory =  arr[1];
                grupo = arr[2];
                subgrupo = arr[3];
                subsubgrupo = arr[4];
                camadaseis = arr[5];
                camadasete = arr[6];
                camadaoito = arr[7];
                camadanove = arr[8];
                camadadez = arr[9];
                alterouPasta = true;
            }
            else
            {
                substory = null;
                grupo = null;
                subgrupo = null;
                subsubgrupo = null;
                camadaseis = null;
                camadasete = null;
                camadaoito = null;
                camadanove = null;
                camadadez = null;
            }
            
        }

        private  int?[] retornarArray(Filtro fi)
        {
            
            int?[] arr = null;
            if (fi is CamadaDez)
                arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, story.PaginaPadraoLink, 1, 1, 1, 1, 1, 1, 1, 1, 1);
            else if (fi is CamadaNove)
                arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, story.PaginaPadraoLink, 1, 1, 1, 1, 1, 1, 1, 1);
            else if (fi is CamadaOito)
                arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, story.PaginaPadraoLink, 1, 1, 1, 1, 1, 1, 1);
            else if (fi is CamadaSete)
                arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, story.PaginaPadraoLink, 1, 1, 1, 1, 1, 1);
            else if (fi is CamadaSeis)
                arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, story.PaginaPadraoLink, 1, 1, 1, 1, 1);
            else if (fi is SubSubGrupo)
                arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, story.PaginaPadraoLink, 1, 1, 1, 1);
            else if (fi is SubGrupo)
                arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, story.PaginaPadraoLink, 1, 1, 1);
            else if (fi is Grupo)
                arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, story.PaginaPadraoLink, 1, 1);
            else if (fi is SubStory)
                arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, story.PaginaPadraoLink, 1);

            


            return arr;
        }

        private List<Content> retornarListaFiltrada(string rota)
        {
            List<Content> listaFiltradaComConteudo = null;

            if (outroHorizonte == 0 && substory != null && rota == null)
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
                group = story!.Filtro!.Where(str => str is SubStory).OrderBy(str => str.Id).Skip((int)substory! - 1).First();
                nameGroup = group!.Nome!;
                
                    var filtropa = story.Filtro!.First(f => f.Id == group.Id);
                    if (Content)
                    {
                        listaFiltradaComConteudo = listarConteudos(filtropa);
                    }
                    else
                    {
                        listaFiltradaComConteudo = filtropa.Pagina!.Select(p => p.Content).ToList()!;
                    }
                

                if (grupo != null)
                {
                    var fil1 = (SubStory)story.Filtro.First(f => f.Id == group.Id);
                    group2 = fil1.Grupo.OrderBy(str => str.Id).Skip((int)grupo! - 1).First();
                    nameGroup = group2!.Nome!;
                    
                        var filtropag = story.Filtro!.First(f => f.Id == group2.Id);
                        if (Content)
                        {
                            listaFiltradaComConteudo = listarConteudos(filtropag);
                        }
                        else
                        {
                            listaFiltradaComConteudo = filtropag.Pagina!.Select(p => p.Content).ToList()!;
                        }
                    

                }

                if (subgrupo != null)
                {
                    var fil2 = (Grupo)story.Filtro.First(f => f.Id == group2.Id);
                    group3 = fil2.SubGrupo.OrderBy(str => str.Id).Skip((int)subgrupo! - 1).First();
                    nameGroup = group3!.Nome!;
                  
                        var filtropag = story.Filtro!.First(f => f.Id == group3.Id);
                        if (Content)
                        {
                            listaFiltradaComConteudo = listarConteudos(filtropag);
                        }
                        else
                        {
                            listaFiltradaComConteudo = filtropag.Pagina!.Select(p => p.Content).ToList()!;
                        }
                    

                }

                if (subsubgrupo != null)
                {
                    var fil3 = (SubGrupo)story.Filtro.First(f => f.Id == group3.Id);
                    group4 = fil3.SubSubGrupo.OrderBy(str => str.Id).Skip((int)subsubgrupo! - 1).First();
                    nameGroup = group4!.Nome!;
                   
                        var filtropag = story.Filtro!.First(f => f.Id == group4.Id);
                        if (Content)
                        {
                            listaFiltradaComConteudo = listarConteudos(filtropag);
                        }
                        else
                        {
                            listaFiltradaComConteudo = filtropag.Pagina!.Select(p => p.Content).ToList()!;
                        }
                    

                }

                if (camadaseis != null)
                {
                    var fil4 = (SubSubGrupo)story.Filtro.First(f => f.Id == group4.Id);
                    group5 = fil4.CamadaSeis.OrderBy(str => str.Id).Skip((int)camadaseis! - 1).First();
                    nameGroup = group5!.Nome!;
                   
                        var filtropag = story.Filtro!.First(f => f.Id == group5.Id);
                        if (Content)
                        {
                            listaFiltradaComConteudo = listarConteudos(filtropag);
                        }
                        else
                        {
                            listaFiltradaComConteudo = filtropag.Pagina!.Select(p => p.Content).ToList()!;
                        }
                    

                }

                if (camadasete != null)
                {
                    var fil5 = (CamadaSeis)story.Filtro.First(f => f.Id == group5.Id);
                    group6 = fil5.CamadaSete.OrderBy(str => str.Id).Skip((int)camadasete! - 1).First();
                    nameGroup = group6!.Nome!;
                    
                        var filtropag = story.Filtro!.First(f => f.Id == group6.Id);
                        if (Content)
                        {
                            listaFiltradaComConteudo = listarConteudos(filtropag);
                        }
                        else
                        {
                            listaFiltradaComConteudo = filtropag.Pagina!.Select(p => p.Content).ToList()!;
                        }
                    

                }

                if (camadaoito != null)
                {
                    var fil6 = (CamadaSete)story.Filtro.First(f => f.Id == group6.Id);
                    group7 = fil6.CamadaOito.OrderBy(str => str.Id).Skip((int)camadaoito! - 1).First();
                    nameGroup = group7!.Nome!;
                    
                        var filtropag = story.Filtro!.First(f => f.Id == group7.Id);
                        if (Content)
                        {
                            listaFiltradaComConteudo = listarConteudos(filtropag);
                        }
                        else
                        {
                            listaFiltradaComConteudo = filtropag.Pagina!.Select(p => p.Content).ToList()!;
                        }
                    

                }

                if (camadanove != null)
                {
                    var fil7 = (CamadaOito)story.Filtro.First(f => f.Id == group7.Id);
                    group8 = fil7.CamadaNove.OrderBy(str => str.Id).Skip((int)camadanove! - 1).First();
                    nameGroup = group8!.Nome!;
                    
                        var filtropag = story.Filtro!.First(f => f.Id == group8.Id);
                        if (Content)
                        {
                            listaFiltradaComConteudo = listarConteudos(filtropag);
                        }
                        else
                        {
                            listaFiltradaComConteudo = filtropag.Pagina!.Select(p => p.Content).ToList()!;
                        }
                    

                }

                if (camadadez != null)
                {
                    var fil8 = (CamadaNove)story.Filtro.First(f => f.Id == group8.Id);
                    group9 = fil8.CamadaDez.OrderBy(str => str.Id).Skip((int)camadadez! - 1).First();
                    nameGroup = group9!.Nome!;
                   
                        var filtropag = story.Filtro.First(f => f.Id == group.Id);                        
                        if (Content)
                        {
                            listaFiltradaComConteudo = listarConteudos(filtropag);
                        }
                        else
                        {
                            listaFiltradaComConteudo = filtropag.Pagina!.Select(p => p.Content).ToList()!;
                        }
                    

                }


                Filtro Filtro = null;
                if (group9 != null)
                    Filtro = story!.Filtro!.First(f => f.Id == group9!.Id);
               else if (group8 != null)
                    Filtro = story!.Filtro!.First(f => f.Id == group8!.Id);
                else if (group7 != null)
                    Filtro = story!.Filtro!.First(f => f.Id == group7!.Id);
                else if (group6 != null)
                    Filtro = story!.Filtro!.First(f => f.Id == group6!.Id);
                else if (group5 != null)
                    Filtro = story!.Filtro!.First(f => f.Id == group5!.Id);
                else if (group4 != null)
                    Filtro = story!.Filtro!.First(f => f.Id == group4!.Id);
                else if (group3 != null)
                    Filtro = story!.Filtro!.First(f => f.Id == group3!.Id);
                else if (group2 != null)
                    Filtro = story!.Filtro!.First(f => f.Id == group2!.Id);
                else if (group != null)
                    Filtro = story!.Filtro!.First(f => f.Id == group!.Id);

                               
                indice_Filtro = story.Filtro!.OrderBy(f => f.Id).ToList().IndexOf(Filtro) + 1;

                Model2 = Filtro;

                if (Model2.usuarios.Count != 0)
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
                    

                    if (fil.usuarios.Count != 0 )
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
                else if(Model2.usuarios.Count == 0)
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
                    indice = listaFiltradaComConteudo!.Count;
                    retroceder = 0;
                }

              //  if (Model.Id == listaFiltradaComConteudo.First().Id)
              //      indice = 1;

                Model = listaFiltradaComConteudo!
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

                quantidadeLista = listaFiltradaComConteudo!.Count;
            }
            else if (outroHorizonte == 0 && substory != null && rota != null)
            {
                listaFiltradaComConteudo = new List<Content>();
                foreach (var item in story.Filtro)
                foreach(var item2 in item.Pagina)
                {
                        var rotas = item2.Content.Rotas.Split(",");
                        foreach (var rot in rotas)
                            if (rot.ToLower().TrimEnd().TrimStart() == rota.ToLower().TrimEnd().TrimStart())                             
                             if (!listaFiltradaComConteudo.Contains(item2.Content))                        
                            listaFiltradaComConteudo.Add(item2.Content);
                        
                }

                Content pag2 = listaFiltradaComConteudo!
                    .OrderBy(p => p.Id).Skip((int)indice - 1).FirstOrDefault()!;

                var str = repositoryPagina.stories.First(st => st.Id == pag2.StoryId);
                cap = repositoryPagina.stories.IndexOf(str);

                if (pag2 == null)
                {
                    navigation.NavigateTo($"/renderizar/{storyid}/{indice_Filtro}/0/11/1/1/0/0/0/{dominio}/{Compartilhante}");
                }

                if(pag2 is Pagina)
                {
                    var p = (Pagina)pag2;
                vers = p.Versiculo;
                // Model = repositoryPagina.includes()
                  // .FirstOrDefault(p => p.Versiculo == vers && p.StoryId == storyid);
                }
                else vers = 0;

                quantidadeLista = listaFiltradaComConteudo!.Count;
            }
           

            return listaFiltradaComConteudo!.OrderBy(c => c.Id).ToList();
        }

        private void instanciarTime(int camada)
        {
            Filtro[] fils = new Filtro[10];
            Time time = null;
             fils[0] =  story.Filtro.FirstOrDefault(u => u.Id == Compartilhante)!;
             fils[1] =  story.Filtro.FirstOrDefault(u => u.Id == Compartilhante2)!;
             fils[2] =  story.Filtro.FirstOrDefault(u => u.Id == Compartilhante3)!;
             fils[3] =  story.Filtro.FirstOrDefault(u => u.Id == Compartilhante4)!;
             fils[4] =  story.Filtro.FirstOrDefault(u => u.Id == Compartilhante5)!;
             fils[5] =  story.Filtro.FirstOrDefault(u => u.Id == Compartilhante6)!;
             fils[6] =  story.Filtro.FirstOrDefault(u => u.Id == Compartilhante7)!;
             fils[7] =  story.Filtro.FirstOrDefault(u => u.Id == Compartilhante8)!;
             fils[8] =  story.Filtro.FirstOrDefault(u => u.Id == Compartilhante9)!;
             fils[9] =  story.Filtro.FirstOrDefault(u => u.Id == Compartilhante10)!;
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
                    fils[2] is not null && fils[3] is not null && fils[4] is not null )
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
                .FirstOrDefault(u => fils[2].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null) != null )!;

            }

            if (camada == 3 && fils[0] is not null && fils[1] is not null )
            {
                time = Context.Time
                .Include(t => t.usuarios)
                .ThenInclude(t => t.UserModel)
                .FirstOrDefault(t =>
                t.usuarios
                .FirstOrDefault(u => fils[0].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null) != null &&
                t.usuarios
                .FirstOrDefault(u => fils[1].usuarios.FirstOrDefault(us => us.UserModel.UserName == u.UserModel.UserName) != null) != null )!;

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
            if(preferencia == null)
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
            if(f is CamadaDez)
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
            var us =    Context.Users.FirstOrDefault(u =>  u.UserName == Compartilhou);
            var us2  =  Context.Filtro.Include(f => f.usuarios).ThenInclude(f => f.UserModel)
                .FirstOrDefault(u =>  u.Id == Compartilhante);
            var us3 =   Context.Filtro.Include(f => f.usuarios).ThenInclude(f => f.UserModel)
                .FirstOrDefault(u =>  u.Id == Compartilhante2);
            var us4 =   Context.Filtro.Include(f => f.usuarios).ThenInclude(f => f.UserModel)
                .FirstOrDefault(u =>  u.Id == Compartilhante3);
            var us5 =   Context.Filtro.Include(f => f.usuarios).ThenInclude(f => f.UserModel)
                .FirstOrDefault(u =>  u.Id == Compartilhante4);
            var us6 =   Context.Filtro.Include(f => f.usuarios).ThenInclude(f => f.UserModel)
                .FirstOrDefault(u =>  u.Id == Compartilhante5);
            var us7 =   Context.Filtro.Include(f => f.usuarios).ThenInclude(f => f.UserModel)
                .FirstOrDefault(u =>  u.Id == Compartilhante6);
            var users = Context.Filtro.ToList().Count;
            var filtros = story.Filtro.Count();

            if (us2 != null){ pts =  2; fils[0] = us2; } else fils[0] = null;
            if (us3 != null){ pts =  3; fils[1] = us3; } else fils[1] = null;
            if (us4 != null){ pts =  4; fils[2] = us4; } else fils[2] = null;
            if (us5 != null){ pts =  5; fils[3] = us5; } else fils[3] = null;
            if (us6 != null){ pts =  6; fils[4] = us6; } else fils[4] = null;
            if (us7 != null){ pts =  7; fils[5] = us7; } else fils[5] = null;

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
                             .FirstOrDefault(f => f.usuarios.FirstOrDefault(us => us.UserModelId == usuarios[j].UserModel.Id) != null );

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
                                    foreach(var UserModel in UserModels)
                                    {
                                        if(usuarios[0] is not null &&  UserModel.UserModel.Id == usuarios[0].UserModel.Id)
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
                    else if(i != 0) break;

                }

            }
            pontos = null;
        }
   
        private void verificarCompartilhante( Filtro fil, Time time)
        {
             foreach(var item in fil.usuarios)
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
        
    }
}
