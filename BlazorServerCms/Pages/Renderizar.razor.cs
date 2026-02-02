using System;
using BlazorServerCms.Data;
using BlazorServerCms.servicos;
using business;
using business.business;
using business.business.Book;
using business.Group;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace BlazorCms.Client.Pages
{
    public partial class RenderizarBase : ComponentBase, IStoryService
    {
        public RenderizarBase()
        {
        }

        private async void StartTimer(Content p)
        {
            try
            {
                if (p != null && p.Html != null && outroHorizonte == 0)
                {
                    if (p.Html.Contains("iframe"))
                    {
                        var conteudoHtml = p.Html;
                        var arr = conteudoHtml!.Split("/");
                        id_video = "";
                        for (var index = 0; index < arr.Length; index++)
                        {
                            if (arr[index] == "embed" && conteudoHtml.Contains("?autoplay="))
                            {
                                var text = arr[index + 1];
                                var arr2 = text.Split("?");
                                id_video = arr2[0];
                                break;
                            }
                            else if (arr[index] == "embed")
                            {
                                var text = arr[index + 1];
                                var arr2 = text.Split('"');
                                id_video = arr2[0];
                                break;
                            }
                        }
                         tempoVideo = await GetYouTubeVideo(id_video);
                         await js!.InvokeAsync<object>("PreencherProgressBar", tempoVideo + 3000);
                       
                        Timer!.SetTimer(tempoVideo + 3000);

                    }
                    else
                    {
                        await js!.InvokeAsync<object>("PreencherProgressBar", timeproduto * 1000);                        
                        Timer!.SetTimer(timeproduto * 1000);
                    }
                }



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Timer._timer!.Elapsed += _timer_Elapsed;
            Console.WriteLine("Timer Started.");

        }

        private void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (automatico)
                buscarProximo();
        }

        protected async void TeclaPressionada(KeyboardEventArgs args)
        {

            if (Filtro == null)
            {

                if (args.Key == "Enter" && cap == 0)
                {
                    storyid = RepositoryPagina.stories!
                    .OrderBy(str => str.Capitulo).Skip(1).ToList()[Indice - 1].Capitulo;
                    Indice = 1;
                }
                else if (args.Key == "Enter")
                {
                    storyid = RepositoryPagina.stories.First().Capitulo;
                    var str = RepositoryPagina.stories.First(st => st.Id == _story.Id);
                    Indice = RepositoryPagina.stories.IndexOf(str);
                }
                //  automatico = false;
                Timer!._timer!.Elapsed -= _timer_Elapsed;
                acessar();
            }
            else if (args.Key == "Enter")
            {
                AlterouModel = true;
                navegarSubgrupos(true);
            }
            else if (args.Key == "c")
            {
                perguntar(Model2!.Id);
            }
            
            

        }

       

        protected async void Casinha()
        {
            if (livro == null)
                acessar("/");
            else
                acessar($"/livro/{livro.Nome}");
        }

        protected async void Pesquisar()
        {
            AlterouModel = true;

            bool condicao = false;
            try
            {
                var n = int.Parse(opcional);
                automatico = false;
                condicao = true;
            }
            catch (Exception ex)
            {
                condicao = false;
            }

            if (Filtro == null && condicao || tellStory && condicao)
            {
                Indice = int.Parse(opcional);
                acessar();
            }
            else if (condicao)
            {
                if (Filtro != null && RepositoryPagina.Conteudo!
                .Where(c => c is Pagina && c.Filtro != null &&
                c.Filtro!.FirstOrDefault(f => f.FiltroId == Filtro) != null).ToList().Count ==
                CountPagesInFilterAsync((long)Filtro, livro))
                {
                    var listainFilter = RepositoryPagina.Conteudo!
                    .Where(c => c is Pagina && c.Filtro != null &&
                    c.Filtro!.FirstOrDefault(f => f.FiltroId == Filtro) != null).ToList();
                    var m = listainFilter.FirstOrDefault(c => retornarVerso(c) == int.Parse(opcional));
                    if (m != null)
                    {
                        Indice = listainFilter.IndexOf(m) + 1;
                        acessar();
                    }
                    else
                        await js!.InvokeAsync<object>("DarAlert",
                    $"Não foi encontrado o versiculo {int.Parse(opcional)} na pasta {indice_Filtro}." +
                    " O versiculo {int.Parse(opcional)} não é {Model2.Nome}.");
                }
                else
                {
                    var c = Context.Pagina!.Include(c => c.Filtro)
                    .Where(c => c.StoryId == storyid && c.Versiculo == int.Parse(opcional) && c.Filtro != null &&
                    c.Filtro!.FirstOrDefault(f => f.FiltroId == Filtro) != null).FirstOrDefault();
                    if (c == null)
                        await js!.InvokeAsync<object>("DarAlert",
                        $"Não foi encontrado o versiculo {int.Parse(opcional)} na pasta {indice_Filtro}." +
                        $" O versiculo {int.Parse(opcional)} não é {Model2.Nome}.");
                    else
                    {
                        List<FiltroContent> resultados = null;
                        int countPages = CountPagesInFilterAsync((long)Filtro, livro);
                        var teste = RepositoryPagina.conteudoEmFiltro
                         .FirstOrDefault(cf => cf.conteudoEmFiltro!.ContentId == c!.Id &&
                         cf.conteudoEmFiltro!.FiltroId == Filtro);
                        Indice = await buscarIndice(c, countPages, teste);

                        acessar();
                    }
                }
            }
        }

        private async Task<int> buscarIndice(Content? c, int countPages, FiltroContentIndice? teste)
        {
            List<Content> resultados = null;
            if (countPages < 1000)
            {
                if (teste == null)
                {
                    resultados = await GetFiltroByIdAsync((long)Filtro, livro);

                    var mo = resultados.FirstOrDefault(r => r.Id == c!.Id);
                    Indice = resultados.IndexOf(mo) + 1;

                }
                else
                    Indice = teste.Indice;
            }
            else
            {
                if (teste == null)
                {
                    var slide = 0;
                    while (resultados == null || resultados.FirstOrDefault(r => r.Id == c!.Id) == null)
                    {
                        resultados = await GetFiltroByIdAsync((long)Filtro!, livro, slide, 20);

                        slide++;

                    }
                    var mo = resultados.FirstOrDefault(r => r.Id == Model!.Id);
                    Indice = resultados.IndexOf(mo) + 1 + (slide * 20);
                }
                else
                    Indice = teste.Indice;
            }

            return Indice;
        }

        private void habilitarAuto()
        {
            Timer!.SetTimerAuto(repositoryPagina!.QuantMinutos);
            Timer!.desligarAuto!.Elapsed += desligarAuto_Elapsed;
        }

        protected void ativar()
        {
            Auto = Convert.ToInt32(!automatico);
        }

        protected async void ativarConteudo()
        {
            if (RepositoryPagina.Conteudo!.FirstOrDefault(c => c is UserContent &&
            c.Filtro!.FirstOrDefault(f => f.FiltroId == Model2!.Id) != null) != null) 
            {
                AlterouModel = true;
                Content = true;
                Indice = 1;
              var resultado = await retornarListaFiltrada(null);
              if(livro != null)
                {
                var lista = randomizar(resultado.OfType<UserContent>()
                .Where(c => c is UserContent && 
                c.LivroId == livro.Id && c.StoryId == _story.Id).ToList());
                listaContent.AddRange(lista);
                }
                else
                {
                    var lista = randomizar(resultado.OfType<UserContent>()
                .Where(c => c is UserContent && 
                c.LivroId is null && c.StoryId == _story.Id).ToList());
                listaContent.AddRange(lista);
                }

                Model = listaContent.Where(c => c is UserContent)
                .ToList()[Indice - 1];
                contentid = Model.Id;
            }
            else
            {
                contentid = null;
                Content = false;
                try
                {
                    await js!.InvokeAsync<object>("DarAlert", $"Não existe mais conteudo para esta história: {Model2!.Nome}.");                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                
            }

            acessar();
            
        }

        private void desabilitarAuto()
        {
            if (Timer!.desligarAuto != null)
            {
                Timer!.desligarAuto!.Enabled = false;
                Timer.desligarAuto.Dispose();
            }
        }

        private void desligarAuto_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("Timer Elapsed auto.");
            automatico = false;
            acessar("/");
        }

        private async Task<int> GetYouTubeVideo(string id_video)
        {
            return await GetYouTubeVideoDurationAsync(id_video);
        }

        protected async void acessarPasta()
        {
            indice_Filtro = Indice;

            if (Model2!.Pagina == null || Model2.Pagina.Count == 0)
            {
                automatico = false;
                await js!.InvokeAsync<object>("DarAlert", $"Esta pasta não possui versiculos");
                acessar();
            }
            else
            {
                Filtro = Model2.Id;
                acessar();
            }

        }

        protected void listarPasta()
        {
            acessar($"/lista-filtro/1/{Model2.Id}");
        }

        protected void listarPastas()
        {
            if (livro != null)
                acessar($"/listar-pasta/{_story.Capitulo}/{livro.Id}");
            else
                acessar($"/listar-pasta/{_story.Capitulo}");
        }

        protected void acessarHorizontePastas(int? i)
        {
            automatico = false;
            outroHorizonte = 1;
            if (i != null)
                Indice = (int)i;
            else
                Indice = 1;
            acessar();
        }

        protected void acessarHorizonteVersos()
        {
            automatico = false;
            outroHorizonte = 0;
            Indice = 1;
            acessar();
        }
       
        protected int? buscarPastaFiltrada(int camada)
        {
            if (camada != 0)
            {
                SubFiltro fl = null;
                var fil = listaFiltro.FirstOrDefault(f => f.Id == Model2!.Id);
                fl = listaFiltro!.First(f => f.Id == fil.Id);                    
                var pasta = listaFiltro!.IndexOf(fl) + 1;
                return pasta;
            }
            else
            return null;
        }

        protected async void buscarProximo()
        {
            AlterouModel = true;

            long quant = 0;
            if (Filtro == null && outroHorizonte == 0)
                quant = CountPaginas();
            else if (outroHorizonte == 1)
            {
                quant = QuantFiltros();
            }
            else
            {
                List<Content> lista = null;
                if (rotas != null)
                    lista = await retornarListaFiltrada(rotas);
                else
                    quant = quantidadeLista;
            }

            var proximo = Indice + 1;
            if (rotas != null)
            {

                if (proximo <= quant)
                {
                    Indice = proximo;
                }
                else
                {
                    Indice = 1;
                }

                acessar();
            }
            else
            if (proximo <= quant)
            {
                Indice = proximo;
                acessar();
            }
            else if (Filtro != null && !Content)
                navegarSubgrupos(true);
            else if(!Content)
            {
                cap++;
                storyid = RepositoryPagina.stories
                .First(str => str.Capitulo == cap).Capitulo;
                Indice = 1;
                acessar();
            }
        }

        private async void navegarSubgrupos(bool somenteSubgrupos)
        {
            var quant = 0;
            if (Filtro == null)
                quant = CountPaginas();
            else
            {
                var lista = await retornarListaFiltrada(null);
                quant = lista.Count;
            }
            if (somenteSubgrupos)
            {
                Filtro proximoSubgrupo = buscarProximoSubGrupo();
                Filtro = proximoSubgrupo.Id;
                Indice = 1;
            }
            else
                Indice++;
            acessar();
        }

        protected void buscarAnterior()
        {
            AlterouModel = true;

            bool alterouIdice = false;
            if (rotas != null || Content)
            {
                if (Indice == 1)
                {
                    Indice = 1;
                }
                else
                {
                    Indice--;
                }
            }
            else
            if (Indice == 1 && cap != 0)
            {
                if (Filtro != null)
                {
                    Filtro fi = voltarSubgrupos();
                    Filtro = fi.Id;
                    var count = CountPagesInFilterAsync((long)Filtro, livro);
                    Indice = count;
                    retroceder = 1;
                    alterouIdice = true;
                }

                if (Filtro == null)
                {
                    cap--;
                    storyid = RepositoryPagina.stories
                    .First(str => str.Capitulo == cap).Capitulo;
                }
                

            }
            if (Indice != 1 && rotas == null && !alterouIdice)
            {
                var anterior = Indice - 1;
                Indice = anterior;
            }
            acessar();
        }

        protected async Task DarUmLike()
        {
            Model!.QuantLiked++;
            Context.Update(Model);
            usuario.curtir(Model);
            await Context.SaveChangesAsync();
        }

        protected async Task Unlike()
        {
            Model!.QuantLiked--;
            Context.Update(Model);
            await Context.SaveChangesAsync();
            var page = usuario.PageLiked
            .FirstOrDefault(p => p.ContentId == Model.Id);

            if (page != null)
            {
                Context.Remove(page);
                await Context.SaveChangesAsync();
            }
        }

        protected async void acessarPreferenciasUsuario(string usu)
        {
            preferencia = usu;
        }

        protected void alterarQuery(ChangeEventArgs e)
        {
            if (!tellStory)
            {
                opcional = e.Value!.ToString()!;
                try
                {
                    var num = int.Parse(opcional);
                }
                catch (Exception ex)
                {
                    foreach (var item in Model2.Pagina.Select(p => p.Content)
                        .OfType<UserContent>().ToList())
                    {
                        var user = userManager.Users.First(u => u.Id == item.UserModelId);
                        usuarios.Add(new UserPreferencesImage { user = user.UserName, UserModel = user });
                    }

                    if (string.IsNullOrEmpty(opcional))
                    {
                        usuarios.Clear();
                    }
                }

            }
            else opcional = Indice.ToString();

        }

        protected void acessarVerso()
        {
            indice_Filtro = 0;
            Indice = (int)vers!;
            Filtro = null;
            ultimaPasta = false;
            acessar();
        }

        protected void acessarComCriterio()
        {
            AlterouModel = true;
            SubFiltro sub = listaFiltro.FirstOrDefault(s => s.Id == Model2!.Id);
            Indice = 1;
            Filtro = sub.ComCriterio;
            acessar();
        }

        protected void acessarSemCriterio(long filtro)
        {
            AlterouModel = true;
            Indice = 1;
            Filtro = filtro;
            showModal = false;
          //  chave = Versiculo;
            acessar();
        }
        
        protected void ativarModal()
        {
           showModal = true;
        }

        protected async void desabilitarTellStory()
        {
            tellStory = false;
            await js!.InvokeAsync<object>("DarAlert", $"A história {Model2.Nome} não esta sendo mais contada e dividida!!!");
            acessar();
        }

        protected async void redirecionarMarcar()
        {
            int camada = 0;
            List<SubFiltro> fils = null;
            SubFiltro fi = null;

            if (Filtro == null)
            {
                opcional = Indice.ToString();
                try
                {
                    camada = int.Parse(await js.InvokeAsync<string>("prompt", "marcar em qual camada?"));
                    if (camada > 10 || camada <= 1)
                        camada = 0;
                }
                catch (Exception ex)
                {
                    camada = 0;
                }

            }
            else
            {
                opcional = vers.ToString();
            }

            fils = listaFiltro.Where(f => f.Pagina != null)
            .OrderBy(f => f.FiltroId)
            .ThenBy(f => f.Id)
            .ToList();
            if (camada == 0)
            {
                fi = fils.Where(f => f.Pagina!
                .FirstOrDefault(p => retornarVerso(p.Content) == int.Parse(opcional!)) != null).LastOrDefault()!;

            }
            else
            {
                for(var i = 1; i <11; i++)
                {
                    if(camada == i)
                    {
                          fi = fils.Where(f => f.Camada.Numero==i && f.Pagina
                        .FirstOrDefault(p => retornarVerso(p.Content) == int.Parse(opcional!)) != null).LastOrDefault()!;
                        break;
                    }
                }
                

                if (fi == null)
                    fi = fils.Where(f => f.Pagina!
                    .FirstOrDefault(p => retornarVerso(p.Content) == int.Parse(opcional!)) != null).LastOrDefault()!;
            }


            if (fi == null)
            {
                await js!.InvokeAsync<object>("DarAlert", "Marque um versiculo que esta em uma pasta!!!");
            }
            else
            {
                Filtro = fi.Id;
                var name = listaFiltro.First(f => f.Id == Filtro).Nome;
                string? str = await js.InvokeAsync<string>("contarHistoria", name);

                if (str == "sim")
                    tellStory = true;
                else
                    tellStory = false;
                Indice = 0;
                indice_Filtro = listaFiltro
                .OrderBy(f => f.FiltroId)
                .ThenBy(f => f.Id)
                .ToList().IndexOf(fi) + 1;
                acessar(); 
            }
        }



        private bool CountFiltros()
        {
            return HasFiltersAsync((long)storyid!, livro);
        }

        private int QuantFiltros()
        {
            if (_story == null)
                _story = RepositoryPagina.stories!
               .First(p => p.Id == Model!.StoryId);

            return listaFiltro.Count;

        }

        private int CountPaginas()
        {
            return CountPagesAsync((long)storyid!, livro);
        }

        private string colocarAutoPlay(string html)
        {
            var conteudoHtml = html;
            var arr = conteudoHtml!.Split("/");
            
            
            // for (var index = 0; index < arr.Length; index++)
            // {
            //     if (arr[index] == "embed")
            //     {
            //         var text = arr[index + 1];
            //         var arr2 = text.Split('"');
            //         id_video = arr2[0];
            //         break;
            //     }
            // }

           // html = html.Replace(id_video, id_video + "?autoplay=1");
            // if (Auto == 0)
            //     html = html.Replace("?autoplay=1", "?autoplay=1" + "&loop=1" + "&playlist=" + id_video);
            html = html.Replace("<iframe", "<iframe" + " allow=' autoplay;' ");
            return html;
        }

        protected async void StartTour()
        {
            automatico = false;
            if (Filtro == null)
                await TourService.StartTour("FormGuidedTour1");
            else
                await TourService.StartTour("FormGuidedTour2");
        }

        protected async void share()
        {
            automatico = false;
            if (title == null || resumo == null)
            {
                if (title == null)
                {
                    title = Model.Titulo;
                }

                if (resumo == null)
                {
                    resumo = await js.InvokeAsync<string>("prompt", "Informe o resumo da pagina.");
                }
                await js!.InvokeAsync<object>("DarAlert", $"Agora Compartilhe!!!");

                if (Filtro != null)
                {
                    if (user.Identity!.IsAuthenticated)
                        Compartilhou = user.Identity.Name;
                    else
                        Compartilhou = "comp";
                }
                acessar();
            }

            else
            {
                try
                {
                    Model.QuantShared++;
                    Context.Update(Model);
                    await Context.SaveChangesAsync();

                    await js!.InvokeAsync<object>("share", $"{title} / {resumo}");
                    title = null;
                    resumo = null;
                }
                catch (Exception ex)
                {
                    title = null;
                    resumo = null;
                }

            }

        }

        protected void acessarItem()
        {
            Indice = 1;
            acessar();
        }

        protected void acessarComentarios()
        {
            showModal2 = true;
        }

        protected void SalvarComentario()
        {
            comment.ContentId = Model!.Id;
            comment.UserModelId = usuario.Id;
            comment.LivroId = livro?.Id;
            comment.StoryId = _story.Id;
            Context.Add(comment);
            Context.SaveChanges();
            comment = new Comment();
            showModal2 = false;
        }


        protected void acessarPastas()
        {
            automatico = false;
            if (Filtro == null)
            {
                acessar($"/pastas/{storyid}/{Indice}/{dominio}");
            }
            else
            {
                acessar($"/pastas/{storyid}/{vers}/{dominio}");

            }
        }

        protected void acessarCapitulos()
        {
            if (Compartilhou != "comp")
                acessar($"/{Compartilhou}/{_story.Capitulo}");
            else
            {
                if (livro == null)
                    acessar("/");
                else
                    acessar($"/livro/{livro.Nome}");
            }
        }

        protected void marcarPasta(int i)
        {
            outroHorizonte = 1;
            Indice = i;

            acessar();
        }

        protected void acessarStory()
        {
            outroHorizonte = 0;
            storyid = RepositoryPagina.stories!
            .OrderBy(str => str.Capitulo).Skip(1).ToList()[Indice - 1].Capitulo;
            automatico = false;
            Indice = 1;

            acessar();
        }

        protected void acessarChave()
        {
            Indice = indiceChave;
            acessar();
        }

        protected void AdicionarAoCarrinho(long ProdutoId)
        {
          //  criptografar = true;
            var url = $"/carrinho/{ProdutoId}/{Compartilhou}/{Model2!.Id}";
          //  criptografar = false;
            acessar(url);
        }

        protected void removerPreferencia()
        {
            preferencia = null;
        }

         protected async void OnClick(MouseEventArgs e) 
            {
                try
                {
                    var containerWidth = await js.InvokeAsync<string>("retornarLargura");
                    var percent = (e.OffsetX / int.Parse(containerWidth)) * 100;
                    Progress = Math.Round(percent);
                    var duracao = tempoVideo;
                    var segundos = duracao / 1000;
                    double seconds = (Progress / 100) * segundos;
                    await js.InvokeVoidAsync("seekToVideo", (int) seconds);
                    AlterouModel = true;
                    AlterouCamada = true;
                    AlterarCamada((int) seconds);                    
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

        private async void acessar(string url2 = null)
        {
            Timer!._timer!.Elapsed -= _timer_Elapsed;
            if (url2 != null) Auto = 0;

            if (url2 == null)
            {

                // if (Content && conteudo == 0) 
                // Indice = 1;
                conteudo = Convert.ToInt32(Content);
               // criptografar = true;

                string url = null;
                if (rotas == null)
                {
                    if (livro != null)
                        url = $"/renderizar/{livro.Nome}/{storyid}/{Indice}/{Compartilhou}/{Filtro}";
                        else
                        url = $"/renderizar/{storyid}/{Indice}/{Compartilhou}/{Filtro}";
                    
                }
                else
                {
                    if (livro != null)
                        url = $"/Renderizar/{livro.Nome}/{storyid}/{Indice}/{Compartilhou}/{rotas}";
                    else
                        url = $"/Renderizar/{storyid}/{Indice}/{Compartilhou}/{rotas}";

                }               

               // criptografar = false;
                navigation!.NavigateTo(url);
            }
            else
            {
                try
                {
                    navigation!.NavigateTo(url2);
                }
                catch (Exception) { throw; }

            }

        }        

        public Task<List<Content>> PaginarStory(long storyId, int quantidadeLista, int quantDiv, int slideAtual, Livro livro, int? carregando = null)
        {
            return storyService.PaginarStory(storyId, quantidadeLista, quantDiv, slideAtual, livro, carregando);
        }

        public int CountPagesAsync(long storyId, Livro livro)
        {
            return storyService.CountPagesAsync(storyId, livro);
        }

        public Task<int> GetYouTubeVideoDurationAsync(string videoId)
        {
            return storyService.GetYouTubeVideoDurationAsync(videoId);
        }

        public bool HasFiltersAsync(long storyId, Livro livro)
        {
            return storyService.HasFiltersAsync(storyId, livro);
        }

        public Task<List<FiltroContent>> PaginarFiltro(long filtroId, int quantidadeLista, int quantDiv, int slideAtual, Livro livro, int? carregando = null)
        {
            return storyService.PaginarFiltro(filtroId, quantidadeLista, quantDiv, slideAtual, livro, carregando);
        }

        public int CountPagesInFilterAsync(long filtroId, Livro livro)
        {
            return storyService.CountPagesInFilterAsync(filtroId, livro);
        }

        public Task<List<Content>> GetFiltroByIdAsync(long filtroId, Livro livro, int slide = 0, int quantDiv = 0)
        {
            return storyService.GetFiltroByIdAsync(filtroId, livro, slide, quantDiv);
        }

        private async void RemoverPlay()
        {
            try
            {
                await js.InvokeVoidAsync("removerVideo");

            }
            catch(Exception){

            }
        }        

            private List<UserContent> randomizar(List<UserContent> lista)
            {
                List<UserContent> retorno = new List<UserContent>();
                while(lista.Count != 0)
                {
                    var indice = repositoryPagina.random.Next(0, lista.Count - 1);
                    retorno.Add(lista[indice]);
                    lista.Remove(lista[indice]);
                }

                return retorno;
            }           

             private void AlterarCamada(int timeNumber)
        {
            if (Model is VideoFilter)
            {
                var marcacoes = Context.MarcacaoVideoFilter
                .Where(m => m.VideoFilterId == Model.Id)
                .OrderBy(m => m.Segundos)
                .ToList();
                foreach (var item in marcacoes)
                    porcentagens.Add(item.Segundos / tempoVideo);

                if (marcacoes.Count >= 9 && timeNumber > marcacoes[8].Segundos)
                    acessarCamada(10);
                else if (marcacoes.Count >= 8 && timeNumber > marcacoes[7].Segundos)
                    acessarCamada(9);
                else if (marcacoes.Count >= 7 && timeNumber > marcacoes[6].Segundos)
                    acessarCamada(8);
                else if (marcacoes.Count >= 6 && timeNumber > marcacoes[5].Segundos)
                    acessarCamada(7);
                else if (marcacoes.Count >= 5 && timeNumber > marcacoes[4].Segundos)
                    acessarCamada(6);
                else if (marcacoes.Count >= 4 && timeNumber > marcacoes[3].Segundos)
                    acessarCamada(5);
                else if (marcacoes.Count >= 3 && timeNumber > marcacoes[2].Segundos)
                    acessarCamada(4);
                else if (marcacoes.Count >= 2 && timeNumber > marcacoes[1].Segundos)
                    acessarCamada(3);
                else if (marcacoes.Count >= 1 && timeNumber > marcacoes[0].Segundos)
                    acessarCamada(2);

            }
            AlterouCamada = false;
        }

        private void acessarCamada(int camada)
        {
            if(Model2!.Camada.Numero != camada)            
            foreach(var item in listaFiltro.Where(l => l.Camada.Numero == camada).ToList())
            if(item.Pagina.FirstOrDefault(p => p.ContentId == Model.Id) != null)
            {
                Filtro = item.Id;
                var m = item.Pagina.FirstOrDefault(p => p.ContentId == Model.Id);
                Indice = item.Pagina.IndexOf(m) + 1;
                acessar();
            }
        }
    }
    public class UserPreferencesImage
    {
        public string? user { get; set; }
        public UserModel UserModel { get; set; }
    }
}

