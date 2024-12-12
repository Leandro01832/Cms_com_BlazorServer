using BlazorServerCms.Data;
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
            if (capitulo > repositoryPagina!.stories!.Last().PaginaPadraoLink)
                capitulo = 1;

            await renderizar();

            if (Auto == 1)
                StartTimer(Model!);

            if (compartilhou != "comp" && pontos != null ||
                compartilhante != "comp" && pontos != null)            
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
            Auto = 1;
          

            if (compartilhante == null)
            {
                compartilhou = "comp";
                compartilhante = "comp";
                compartilhante2 = "comp";
                compartilhante3 = "comp";
                compartilhante4 = "comp";
                compartilhante5 = "comp";
                compartilhante6 = "comp";
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

            if(Context.Story.ToList().Count == 2)
            {
                var lista = await repositoryPagina.buscarPatternStory();

                foreach (var story in lista.OrderBy(str => str.PaginaPadraoLink).Skip(3).ToList())
                    Context.Add(story);
                Context.SaveChanges();

                var shortStory1 = new ShortStory(Context.Story.ToList(), "story promoções 10% off");
                var shortStory2 = new ShortStory(Context.Story.ToList(), "story promoções 30% off");
                var shortStory3 = new ShortStory(Context.Story.ToList(), "story promoções 50% off");
                Context.Add(shortStory1);
                Context.Add(shortStory2);
                Context.Add(shortStory3);
                Context.SaveChanges();

                foreach (var item in Context.Story.OrderBy(str => str.Id).Skip(1).ToList())
                {
                    var strPadrao = await Context.Story.Include(str => str.Pagina).FirstAsync(str => str.Id == 1);
                    var pagPadrao = new Pagina(strPadrao)
                    {
                        Comentario = 0,
                        Html = $"<a href=''#'' class=''LinkPadrao''> <h1> {item.Nome} </h1> </a>",
                        Titulo = "capitulos",
                    };
                    Context.Add(pagPadrao);
                    Context.SaveChanges();
                }
            }

            if(repositoryPagina.stories.Count == 0)
            {
                var str = await Context.Story!
                    .Include(p => p.Filtro)!
                   .ThenInclude(p => p.UserModel)!
                    .Include(p => p.Filtro)!
                   .ThenInclude(p => p.Pagina)!
                   .ThenInclude(p => p.Content)
                    .OrderBy(st => st.Id).ToListAsync();
                repositoryPagina.stories.AddRange(str);

            }
                       
            

            if (repositoryPagina.Conteudo.Count == 0)
            {
                var pergs = await Context!.Content!.Include(p => p.Story).Include(p => p.Filtro).ToListAsync();
                repositoryPagina.Conteudo.AddRange(pergs);
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

            Model = repositoryPagina!.Conteudo.OfType<Pagina>()
                    .FirstOrDefault(p => p.Versiculo == indice && p.Story!.PaginaPadraoLink == capitulo);

            

            cap = capitulo;

            if (Model == null)
            {
                Model = repositoryPagina.Conteudo.OfType<Pagina>()
                .FirstOrDefault(p => p.Story.PaginaPadraoLink == capitulo);
            }
            
            if (outroHorizonte == 1)
            {
                if (Model != null)
                {
                    Model2 = story.Filtro.Where(f => f.user == null).OrderBy(f => f.Id).Skip((int)indice - 1).FirstOrDefault(); 

                    quantidadeLista = story.Filtro.Where(f => f.user == null).ToList().Count;
                    indiceAcesso = story.Filtro.IndexOf(Model2) + 1;
                }
            }
           

           
               
            quantidadePaginas = CountPaginas(ApplicationDbContext._connectionString);

            if (quantidadePaginas == 0 && outroHorizonte == 0)
                Mensagem = "aguarde um momento...";
            var proximo = indice + 1;
            var anterior = indice - 1;


            if (outroHorizonte == 0)
                quantidadeLista = quantidadePaginas;

            if (Model != null)
                condicaoFiltro = CountFiltros(ApplicationDbContext._connectionString);

            if (story == null || story.Id != Model.StoryId)
            {
                story = repositoryPagina.stories!
               .First(p => p.Id == Model!.StoryId);
            }

        

            if (filtrar == null && substory == null)
            {
                if (Model == null)
                {
                    if (quantidadePaginas != 0)
                        Mensagem = $"Por favor digite um numero menor que {quantidadeLista}.";
                    else
                        Mensagem = "aguarde um momento...";
                    return;
                }

                nameStory = story.Nome;

                if (Model != null && Model.Comentario != 0 && Model.Comentario != null)
                {
                    var page = story.Pagina!.FirstOrDefault(p => p.Id == Model.Comentario);
                    Pagina pa = (Pagina)page;
                    CapituloComentario = pa!.Story!.PaginaPadraoLink;
                    VersoComentario = pa.Versiculo;
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

                    var arr = retornarArray(fi);
                    indice = 1;
                    if (compartilhante == null) compartilhante = "comp";
                    if (compartilhante2 == null) compartilhante2 = "comp";
                    Auto = 0;

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
            if (!Content && Model.Html != null)
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
            else if (Model.Produto == null)
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

        private int?[] retornarArray(Filtro fi)
        {
            int?[] arr = null;
            if (fi is CamadaDez)
                arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, capitulo, 1, 1, 1, 1, 1, 1, 1, 1, 1);
            else if (fi is CamadaNove)
                arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, capitulo, 1, 1, 1, 1, 1, 1, 1, 1);
            else if (fi is CamadaOito)
                arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, capitulo, 1, 1, 1, 1, 1, 1, 1);
            else if (fi is CamadaSete)
                arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, capitulo, 1, 1, 1, 1, 1, 1);
            else if (fi is CamadaSeis)
                arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, capitulo, 1, 1, 1, 1, 1);
            else if (fi is SubSubGrupo)
                arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, capitulo, 1, 1, 1, 1);
            else if (fi is SubGrupo)
                arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, capitulo, 1, 1, 1);
            else if (fi is Grupo)
                arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, capitulo, 1, 1);
            else if (fi is SubStory)
                arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, capitulo, 1);
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
                group = story!.Filtro!.Where(str => str is SubStory && str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).Skip((int)substory! - 1).First();
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
                    group2 = fil1.Grupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).Skip((int)grupo! - 1).First();
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
                    group3 = fil2.SubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).Skip((int)subgrupo! - 1).First();
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
                    group4 = fil3.SubSubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).Skip((int)subsubgrupo! - 1).First();
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
                    group5 = fil4.CamadaSeis.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).Skip((int)camadaseis! - 1).First();
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
                    group6 = fil5.CamadaSete.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).Skip((int)camadasete! - 1).First();
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
                    group7 = fil6.CamadaOito.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).Skip((int)camadaoito! - 1).First();
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
                    group8 = fil7.CamadaNove.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).Skip((int)camadanove! - 1).First();
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
                    group9 = fil8.CamadaDez.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).Skip((int)camadadez! - 1).First();
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

                if (Filtro.user != null)                
                indice_Filtro = story.Filtro!.Where(f => f.user == Filtro.user).OrderBy(f => f.Id).ToList().IndexOf(Filtro) + 1;
                else
                indice_Filtro = story.Filtro!.Where(f => f.user == null).OrderBy(f => f.Id).ToList().IndexOf(Filtro) + 1;

                Model2 = Filtro;

                if (Model2.user != null)
                {
                    var fil = verificarFiltros(Model2);
                    compartilhante = Model2.user;
                    compartilhante2 = "comp";
                    compartilhante3 = "comp";
                    compartilhante4 = "comp";
                    compartilhante5 = "comp";
                    compartilhante6 = "comp";
                    Filtro fil2 = null;
                    Filtro fil3 = null;
                    Filtro fil4 = null;
                    Filtro fil5 = null;

                    if (fil.user != null )
                    {
                        //compartilhante ganha 2 pts
                        compartilhante = fil.user;
                        compartilhante2 = Model2.user;
                        compartilhante3 = "comp";
                        compartilhante4 = "comp";
                        compartilhante5 = "comp";
                        compartilhante6 = "comp";

                         fil2 = verificarFiltros(fil);
                        if (fil2.user != null)
                        {
                            //compartilhante ganha 3 pts
                            compartilhante = fil2.user;
                            compartilhante2 = fil.user;
                            compartilhante3 = Model2.user;
                            compartilhante4 = "comp";
                            compartilhante5 = "comp";
                            compartilhante6 = "comp";

                            fil3 = verificarFiltros(fil2);
                            if (fil3.user != null)
                            {
                                //compartilhante ganha 4 pts
                                compartilhante = fil3.user;
                                compartilhante2 = fil2.user;
                                compartilhante3 = fil.user;
                                compartilhante4 = Model2.user;
                                compartilhante5 = "comp";
                                compartilhante6 = "comp";

                                fil4 = verificarFiltros(fil3);
                                if (fil4.user != null)
                                {
                                    //compartilhante ganha 5 pts
                                    compartilhante = fil4.user;
                                    compartilhante2 = fil3.user;
                                    compartilhante3 = fil2.user;
                                    compartilhante4 = fil.user;
                                    compartilhante5 = Model2.user;
                                    compartilhante6 = "comp";

                                    fil5 = verificarFiltros(fil4);
                                    if (fil5.user != null)
                                    {
                                        //compartilhante ganha 6 pts
                                        compartilhante = fil5.user;
                                        compartilhante2 = fil4.user;
                                        compartilhante3 = fil3.user;
                                        compartilhante4 = fil2.user;
                                        compartilhante5 = fil.user;
                                        compartilhante6 = Model2.user;                
                                       
                                    }

                                }

                            }

                        }

                    }

                    if (fil.user != null)
                    {
                        int camada = repositoryPagina.buscarCamada();
                        instanciarTime(camada);

                    }
               
                }  
                else if(Model2.user == null)
                {
                    compartilhante = "comp";
                    compartilhante2 = "comp";
                    compartilhante3 = "comp";
                    compartilhante4 = "comp";
                    compartilhante5 = "comp";
                    compartilhante6 = "comp";

                }



                if (retroceder == 1)
                {
                    indice = listaFiltradaComConteudo!.Count;
                    retroceder = 0;
                }


                Model = listaFiltradaComConteudo!.OrderBy(p => p.Id).Skip((int)indice - 1).FirstOrDefault();

                
                Model = repositoryPagina.Conteudo!.Where(p => p.Story.PaginaPadraoLink == capitulo)
                    .OrderBy(p => p.Id).Skip((int)indice - 1).FirstOrDefault();

                if (Model is Pagina)
                {
                    var p = (Pagina)Model;
                    vers = p.Versiculo;
                    Model = repositoryPagina.includes()
                    .FirstOrDefault(p => p.Versiculo == vers && p.Story.PaginaPadraoLink == capitulo);

                }
                else vers = 0;

                ultimaPasta = Model2.Id == story.Filtro
                    .Where(f => f.Pagina
                    .FirstOrDefault(p => p.Content!.Id == Model.Id) != null)
                        .LastOrDefault()!.Id;

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

                Content pag2 = listaFiltradaComConteudo!.OrderBy(p => p.Id).Skip((int)indice - 1).FirstOrDefault();

                var str = repositoryPagina.stories.First(st => st.Id == pag2.StoryId);
                cap = repositoryPagina.stories.IndexOf(str);

                if (pag2 == null)
                {
                    navigation.NavigateTo($"/renderizar/{capitulo}/{indice_Filtro}/0/11/1/1/0/0/0/{dominio}/{compartilhante}");
                }

                if(pag2 is Pagina)
                {
                    var p = (Pagina)pag2;
                vers = p.Versiculo;
                Model = repositoryPagina.includes()
                   .FirstOrDefault(p => p.Versiculo == vers && p.Story.PaginaPadraoLink == capitulo);
                }
                else vers = 0;

                quantidadeLista = listaFiltradaComConteudo!.Count;
            }
           

            return listaFiltradaComConteudo;
        }

        private void instanciarTime(int camada)
        {
            UserModel[] users = new UserModel[6];
            Time time = null;
            users[0] = Context.Users
               .FirstOrDefault(u => u.UserName == compartilhante)!;
            users[1] = Context.Users
            .FirstOrDefault(u => u.UserName == compartilhante2)!;
            users[2] = Context.Users
            .FirstOrDefault(u => u.UserName == compartilhante3)!;
            users[3] = Context.Users
            .FirstOrDefault(u => u.UserName == compartilhante4)!;
            users[4] = Context.Users
            .FirstOrDefault(u => u.UserName == compartilhante5)!;
            users[5] = Context.Users
            .FirstOrDefault(u => u.UserName == compartilhante6)!;
            if (camada == 7 && users[0] is not null && users[1] is not null &&
                    users[2] is not null && users[3] is not null && users[4] is not null &&
                     users[5] is not null)
            {
                 time = Context.Time
                .Include(t => t.usuarios)
                .ThenInclude(t => t.UserModel)
                .FirstOrDefault(t =>
                t.usuarios
                .FirstOrDefault(u => u.UserModel.UserName == users[0].UserName) != null &&
                t.usuarios
                .FirstOrDefault(u => u.UserModel.UserName == users[1].UserName) != null &&
                t.usuarios                                   
                .FirstOrDefault(u => u.UserModel.UserName == users[2].UserName) != null &&
                t.usuarios                                   
                .FirstOrDefault(u => u.UserModel.UserName == users[3].UserName) != null &&
                t.usuarios                                   
                .FirstOrDefault(u => u.UserModel.UserName == users[4].UserName) != null &&
                t.usuarios                                   
                .FirstOrDefault(u => u.UserModel.UserName == users[5].UserName) != null)!;
                
            }

            if (camada == 6 && users[0] is not null && users[1] is not null &&
                    users[2] is not null && users[3] is not null && users[4] is not null )
            {
                time = Context.Time
                .Include(t => t.usuarios)
                .ThenInclude(t => t.UserModel)
                .FirstOrDefault(t =>
                t.usuarios
                .FirstOrDefault(u => u.UserModel.UserName == users[0].UserName) != null &&
                t.usuarios
                .FirstOrDefault(u => u.UserModel.UserName == users[1].UserName) != null &&
                t.usuarios
                .FirstOrDefault(u => u.UserModel.UserName == users[2].UserName) != null &&
                t.usuarios
                .FirstOrDefault(u => u.UserModel.UserName == users[3].UserName) != null &&
                t.usuarios
                .FirstOrDefault(u => u.UserModel.UserName == users[4].UserName) != null)!;

            }

            if (camada == 5 && users[0] is not null && users[1] is not null &&
                    users[2] is not null && users[3] is not null)
            {
                time = Context.Time
                .Include(t => t.usuarios)
                .ThenInclude(t => t.UserModel)
                .FirstOrDefault(t =>
                t.usuarios
                .FirstOrDefault(u => u.UserModel.UserName == users[0].UserName) != null &&
                t.usuarios
                .FirstOrDefault(u => u.UserModel.UserName == users[1].UserName) != null &&
                t.usuarios
                .FirstOrDefault(u => u.UserModel.UserName == users[2].UserName) != null &&
                t.usuarios
                .FirstOrDefault(u => u.UserModel.UserName == users[3].UserName) != null)!;

            }

            if (camada == 4 && users[0] is not null && users[1] is not null &&
                   users[2] is not null)
            {
                time = Context.Time
                .Include(t => t.usuarios)
                .ThenInclude(t => t.UserModel)
                .FirstOrDefault(t =>
                t.usuarios
                .FirstOrDefault(u => u.UserModel.UserName == users[0].UserName) != null &&
                t.usuarios
                .FirstOrDefault(u => u.UserModel.UserName == users[1].UserName) != null &&
                t.usuarios
                .FirstOrDefault(u => u.UserModel.UserName == users[2].UserName) != null )!;

            }

            if (camada == 3 && users[0] is not null && users[1] is not null )
            {
                time = Context.Time
                .Include(t => t.usuarios)
                .ThenInclude(t => t.UserModel)
                .FirstOrDefault(t =>
                t.usuarios
                .FirstOrDefault(u => u.UserModel.UserName == users[0].UserName) != null &&
                t.usuarios
                .FirstOrDefault(u => u.UserModel.UserName == users[1].UserName) != null )!;

            }

            if (time is null)
            {
                var times = Context.Time.ToList().Count + 1;
                time = new Time()
                { nome = $"Time - {times}" };
                Context.Add(time);
                Context.SaveChanges();

                for (var i = 0; i < camada - 1; i++)
                    verificarCompartilhante(users[i], time);

            }
        }

        private List<Content> listarConteudos(Filtro f)
        {
            List<Content> conteudos = new List<Content>();

            return conteudos;
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
            UserModel[] usuarios = new UserModel[7];
            var us = Context.Users.FirstOrDefault(u =>   u.UserName == compartilhou);
            var us2  = Context.Users.FirstOrDefault(u => u.UserName == compartilhante);
            var us3 = Context.Users.FirstOrDefault(u =>  u.UserName == compartilhante2);
            var us4 = Context.Users.FirstOrDefault(u =>  u.UserName == compartilhante3);
            var us5 = Context.Users.FirstOrDefault(u =>  u.UserName == compartilhante4);
            var us6 = Context.Users.FirstOrDefault(u =>  u.UserName == compartilhante5);
            var us7 = Context.Users.FirstOrDefault(u =>  u.UserName == compartilhante6);
            var users = Context.Users.ToList().Count;
            var filtros = story.Filtro.Count();

            if (us  != null){ pts  = 1; usuarios[0] = us;  } else usuarios[0] = null;
            if (us2 != null){ pts =  2; usuarios[1] = us2; } else usuarios[1] = null;
            if (us3 != null){ pts =  3; usuarios[2] = us3; } else usuarios[2] = null;
            if (us4 != null){ pts =  4; usuarios[3] = us4; } else usuarios[3] = null;
            if (us5 != null){ pts =  5; usuarios[4] = us5; } else usuarios[4] = null;
            if (us6 != null){ pts =  6; usuarios[5] = us6; } else usuarios[5] = null;
            if (us7 != null){ pts =  7; usuarios[6] = us7; } else usuarios[6] = null;

            if (filtros > repositoryPagina.meta1) pts++;
            if (filtros > repositoryPagina.meta2) pts++;
            if (filtros > repositoryPagina.meta3) pts++;
            if (filtros > repositoryPagina.meta4) pts++;
            if (filtros > repositoryPagina.meta5) pts++;
            if (filtros > repositoryPagina.meta6) pts++;
            if (filtros > repositoryPagina.meta7) pts++;
            if (filtros > repositoryPagina.meta8) pts++;              

            for (var i = 0; i < 7; i++)
            {
                if (usuarios[i] != null)
                {

                    if (DateTime.Now.Date > usuarios[i].DataPontuacao.Date)
                    {
                        if (usuarios[i].PontosPorDia > usuarios[i].Recorde)
                        {
                            usuarios[i].Recorde = usuarios[i].PontosPorDia;
                            Context.Update(usuarios[i]);
                            Context.SaveChanges();
                        }
                        usuarios[i].PontosPorDia = 1;
                        usuarios[i].DataPontuacao = DateTime.Now;
                        Context.Update(usuarios[i]);
                        Context.SaveChanges();
                    }
                    else
                    {
                        var fil = Context.Users!
                            .FirstOrDefault(f => f.UserName == usuarios[i].UserName &&
                            f.FiltroId != null);

                        var conteudos = Context.ContentUser.Include(c => c.UserModel)
                                .Where(c => c.UserModel.UserName == usuarios[i].UserName &&
                                c.Data.Date > DateTime.Now.AddDays(-7).Date)
                                .ToList();

                        if (fil != null && i != 0)
                        {
                            var condicao = story.Filtro
                             .FirstOrDefault(f => f.user == usuarios[i].UserName);

                            if (condicao != null)
                            {
                                var UserModels = condicao.UserModel;
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
                                    .Where(ut => ut.UserModel.UserName == usuarios[i].UserName).ToList();

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
                                        if(usuarios[0] is not null &&  UserModel.Id == usuarios[0].Id)
                                        UserModel.PontosPorDia += 1000;
                                        UserModel.PontosPorDia += pontosGanhos;
                                        Context.Update(usuarios[i]);
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


                            usuarios[i].PontosPorDia += pontosGanhos;
                            Context.Update(usuarios[i]);
                            Context.SaveChanges();
                        }

                       
                    }

                }
                else if(i != 0) break;
            }
            pontos = null;
        }
   
        private void verificarCompartilhante( UserModel usuario, Time time)
        {
            
                usuario.IncluiTime(time);
                Context.SaveChanges();
            
        }    
    }
}
