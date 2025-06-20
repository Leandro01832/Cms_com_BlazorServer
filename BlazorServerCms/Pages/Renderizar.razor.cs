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
                if (p != null && p.Html != null  && outroHorizonte == 0)
                {
                    if (p.Html.Contains("iframe"))
                    {
                        var conteudoHtml = p.Html;
                        var arr = conteudoHtml!.Split("/");
                        var id_video = "";
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
                        var tempoVideo = await GetYouTubeVideo(id_video);
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

            Console.WriteLine("Timer Elapsed.");
            Timer!._timer!.Elapsed -= _timer_Elapsed;
        }

        protected void TeclaPressionada(KeyboardEventArgs args)
        {

            if (Filtro == null)
            {

                if (args.Key == "Enter" && cap == 0)
                {
                    storyid = RepositoryPagina.stories!
                    .OrderBy(str => str.Capitulo).Skip(1).ToList()[indice - 1].Id;
                    indice = 1;               
                }
                else if (args.Key == "Enter")
                {
                    storyid = RepositoryPagina.stories.First().Id;
                    var str = RepositoryPagina.stories.First(st => st.Id == _story.Id);
                    indice = RepositoryPagina.stories.IndexOf(str);         
                }
                automatico = false;
                acessar();
            }
            else if (args.Key == "Enter")
            {
                navegarSubgrupos(true);
            }

        }

        protected async void Casinha()
        {
            if(livro == null)
            acessar("/");
            else
            acessar($"/livro/{livro.Nome}");
        }

        protected async void Pesquisar()
        {
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
                indice = int.Parse(opcional);
                acessar();
            }
            else if (condicao)
            {
                if (Filtro != null && RepositoryPagina.Conteudo
                .Where(c => c is Pagina && c.Filtro != null &&
                c.Filtro!.FirstOrDefault(f => f.FiltroId == Filtro) != null).ToList().Count ==
                CountPagesInFilterAsync((long)Filtro, livro))
                {
                    var listainFilter = RepositoryPagina.Conteudo
                    .Where(c => c is Pagina && c.Filtro != null &&
                    c.Filtro!.FirstOrDefault(f => f.FiltroId == Filtro) != null).ToList();
                    var m = listainFilter.FirstOrDefault(c => retornarVerso(c) == int.Parse(opcional));
                    if (m != null)
                    {
                        indice = listainFilter.IndexOf(m) + 1;
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
                    .Where(c => c.StoryId == storyid &&  c.Versiculo == int.Parse(opcional) &&  c.Filtro != null &&
                    c.Filtro!.FirstOrDefault(f => f.FiltroId == Filtro) != null ).FirstOrDefault();
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
                         buscarIndice(c, countPages, teste);

                        acessar();
                    }
                }
            }
        }

        private async void buscarIndice(Content? c, int countPages, FiltroContentIndice? teste)
        {
            List<FiltroContent> resultados = null;
            if (countPages < 1000)
            {
                if (teste == null)
                {
                    resultados = await GetFiltroByIdAsync((long)Filtro, livro);
                    
                    var mo = resultados.FirstOrDefault(r => r.ContentId == c!.Id);
                    indice = resultados.IndexOf(mo) + 1;
                    foreach (var item in resultados)
                        if (RepositoryPagina.conteudoEmFiltro
                            .FirstOrDefault(cf => cf.conteudoEmFiltro.ContentId == item.ContentId &&
                            cf.conteudoEmFiltro.FiltroId == item.FiltroId) == null)
                            RepositoryPagina.conteudoEmFiltro.Add
                                (
                                    new BlazorServerCms.Data.FiltroContentIndice
                                    {
                                        conteudoEmFiltro = item,
                                        Indice = resultados.IndexOf(item) + 1
                                    }
                                );

                }
                else
                    indice = teste.Indice;
            }
            else
            {
                if (teste == null)
                {
                    var slide = 0;
                    while (resultados == null || resultados.FirstOrDefault(r => r.ContentId == c!.Id) == null)
                    {
                        resultados = await GetFiltroByIdAsync((long)Filtro!, livro, slide, 20);

                        slide++;
                        foreach (var item in resultados)
                            if (RepositoryPagina.conteudoEmFiltro
                                .FirstOrDefault(cf => cf.conteudoEmFiltro!.ContentId == item.ContentId &&
                                cf.conteudoEmFiltro.FiltroId == item.FiltroId) == null)
                                RepositoryPagina.conteudoEmFiltro.Add
                                    (
                                        new BlazorServerCms.Data.FiltroContentIndice
                                        {
                                            conteudoEmFiltro = item,
                                            Indice = resultados.IndexOf(item) + 1 + (slide * 20)
                                        }
                                    );
                    }
                    var mo = resultados.FirstOrDefault(r => r.ContentId == Model!.Id);
                    indice = resultados.IndexOf(mo) + 1 + (slide * 20);
                }
                else
                    indice = teste.Indice;
            }

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
            if (RepositoryPagina.Conteudo.FirstOrDefault(c => 
            listaFiltro!.FirstOrDefault(f => f.Id == Model2!.Id 
            && f.Pagina!.FirstOrDefault(p => p.ContentId == c.Id && p.Content is UserContent) != null) != null) == null)
            {
                Content = false;
                await js!.InvokeAsync<object>("DarAlert", $"Não existe mais conteudo para esta história: {Model2.Nome}.");
            }
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
            indice_Filtro = indice;

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
            if(livro != null)
            acessar($"/listar-pasta/{_story.Capitulo}/{livro.Id}");
            else
            acessar($"/listar-pasta/{_story.Capitulo}");
        }

        protected void acessarHorizontePastas(int? i)
        {
            automatico = false;
            outroHorizonte = 1;
            if (i != null)
                indice = (int)i;
            else
                indice = 1;
            acessar();
        }

        protected void acessarHorizonteVersos()
        {
            automatico = false;
            outroHorizonte = 0;
            indice = 1;
            acessar();
        }

        protected int buscarCamada(Filtro fil)
        {
            if (fil is SubStory)
                return 1;
            else if (fil is Grupo)
                return 2;
            else if (fil is SubGrupo)
                return 3;
            else if (fil is SubSubGrupo)
                return 4;
            else if (fil is CamadaSeis)
                return 5;
            else if (fil is CamadaSete)
                return 6;
            else if (fil is CamadaOito)
                return 7;
            else if (fil is CamadaNove)
                return 8;
            else if (fil is CamadaDez)
                return 9;

            return 0;
        }

        protected int? buscarPastaFiltrada(int camada)
        {
            long? IdGrupo = 0;
            Pagina pag = RepositoryPagina.Conteudo.OfType<Pagina>()
                    .FirstOrDefault(p => p.Filtro!.FirstOrDefault(f => f.FiltroId == Model2!.Id) != null)!;


            if (pag != null)
            {
                Filtro fil = pag.Filtro!.FirstOrDefault(f => f.FiltroId == Model2!.Id)!.Filtro!;

                if (camada != 0)
                {
                    Filtro fl = null;
                    var filtros = listaFiltro;
                    if (fil is SubStory)
                    {
                        SubStory gr = (SubStory)fil;
                        fl = filtros!.First(f => f.Id == gr.Id);
                    }
                    if (fil is Grupo)
                    {
                        Grupo gr = (Grupo)fil;
                        fl = filtros!.First(f => f.Id == gr.SubStoryId);
                    }
                    if (fil is SubGrupo)
                    {
                        SubGrupo gr = (SubGrupo)fil;
                        fl = filtros!.First(f => f.Id == gr.GrupoId);
                    }
                    if (fil is SubSubGrupo)
                    {
                        SubSubGrupo gr = (SubSubGrupo)fil;
                        fl = filtros!.First(f => f.Id == gr.SubGrupoId);
                    }
                    if (fil is CamadaSeis)
                    {
                        CamadaSeis gr = (CamadaSeis)fil;
                        fl = filtros!.First(f => f.Id == gr.SubSubGrupoId);
                    }
                    if (fil is CamadaSete)
                    {
                        CamadaSete gr = (CamadaSete)fil;
                        fl = filtros!.First(f => f.Id == gr.CamadaSeisId);
                    }
                    if (fil is CamadaOito)
                    {
                        CamadaOito gr = (CamadaOito)fil;
                        fl = filtros!.First(f => f.Id == gr.CamadaSeteId);
                    }
                    if (fil is CamadaNove)
                    {
                        CamadaNove gr = (CamadaNove)fil;
                        fl = filtros!.First(f => f.Id == gr.CamadaOitoId);
                    }
                    if (fil is CamadaDez)
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

        protected  async void buscarProximo()
        {
            Timer!._timer!.Elapsed -= _timer_Elapsed;

            long quant = 0;
            if (Filtro == null && outroHorizonte == 0)
                quant =  CountPaginas();
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

            var proximo = indice + 1;
            if (rotas != null)
            {

                if (proximo <= quant)
                {
                    indice = proximo;
                }
                else
                {
                    indice = 1;
                }
                
                acessar();
            }
            else
            if (proximo <= quant)
            {
                indice = proximo;
                acessar();
            }
            else if (Filtro != null)
                navegarSubgrupos(false);
            else
            {
                cap++;
                 storyid = RepositoryPagina.stories
                 .First(str => str.Capitulo == cap).Id;
                indice = 1;
                acessar();
            }              
        }

        private  async void navegarSubgrupos(bool somenteSubgrupos)
        {
            var quant = 0;
            if (Filtro == null)
                quant =  CountPaginas();
            else
            {
                var lista = await retornarListaFiltrada(null);
                quant = lista.Count;
            }           
            if (somenteSubgrupos)
            {
                Filtro proximoSubgrupo = buscarProximoSubGrupo();
                Filtro = proximoSubgrupo.Id;
                indice = 1;


            }
            else
                indice++;
            acessar();
        }
         
        protected void buscarAnterior()
        {
            Timer!._timer!.Elapsed -= _timer_Elapsed;

            if (rotas != null)
            {
                if (indice == 1)
                {
                    indice = 1;
                }                
                else
                {
                    indice--;
                }               
            }
            else
            if (indice == 1 && cap != 0)
            {
                if (Filtro != null)
                {
                   Filtro fi = voltarSubgrupos();
                    Filtro = fi.Id;
                }

                indice = 1;
                retroceder = 1;
                if (Filtro == null)
                {
                    cap--;
                    storyid = RepositoryPagina.stories
                    .First(str => str.Capitulo == cap).Id;
                }

            }
            if (indice != 1 && rotas == null)
            {
                var anterior = indice - 1;
                indice = anterior;
            }
                 acessar();
        }
        
        protected async Task DarUmLike()
        {            
            usuario.curtir(Model);
            await Context.SaveChangesAsync();
        }

        protected async Task Unlike()
        {          
            var page =  usuario.PageLiked
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
            else opcional = indice.ToString();

        }

        protected void acessarVerso()
        {
            indice_Filtro = 0;
            indice = (int) vers!;
            Filtro = null;
            ultimaPasta = false;
            acessar();
        }

        protected async void desabilitarTellStory()
        {
            tellStory = false;
            await js!.InvokeAsync<object>("DarAlert", $"A contagem e divisão da história {Model2.Nome} foi desativada!!!");
            acessar();
        }

        protected async void redirecionarMarcar()
        {
            int camada = repositoryPagina.buscarCamada();
            List<Filtro> fils = null;
            Filtro fi = null;

                if (Filtro == null)
                    opcional = indice.ToString();
                else
                {
                    opcional = vers.ToString();
                }  
                
                fils = listaFiltro.Where(f => f.Pagina != null).OrderBy(f => f.Id).ToList();                                               
                fi = fils.Where(f =>   f.Pagina
                .FirstOrDefault(p => retornarVerso(p.Content) == int.Parse(opcional!)) != null).LastOrDefault()!;           
           
            if (fi == null)
            {
                await js!.InvokeAsync<object>("DarAlert", "Marque um versiculo que esta em uma pasta!!!");
            }
            else
            {
                Filtro = fi.Id;
                indice = 0;
                acessar();            
            }
        }

        private int CountLikes()
        {
            if (Model != null)
                return CountLikesAsync(Model!.Id, livro);
            else
                return 0;
        }

        private bool CountFiltros()
        {
          return  HasFiltersAsync((long)storyid!, livro);
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
            return  CountPagesAsync((long)storyid!, livro);
        }

        private string colocarAutoPlay(string html)
        {
            var conteudoHtml = html;
            var arr = conteudoHtml!.Split("/");
            var id_video = "";
            for (var index = 0; index < arr.Length; index++)
            {
                if (arr[index] == "embed")
                {
                    var text = arr[index + 1];
                    var arr2 = text.Split('"');
                    id_video = arr2[0];
                    break;
                }
            }

                html = html.Replace(id_video, id_video + "?autoplay=1");
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
            if ( title == null || resumo == null)
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

                    if(Filtro != null)
                    {
                            if(user.Identity!.IsAuthenticated)                            
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
            indice = 1;
             acessar();
        }

        protected void acessarComentarios()
        {
            automatico = false;
            if(Filtro == null)
            {
                acessar($"/comentario/{Model.Id}");
            }
            else
            {
                acessar($"/comentario/{Model.Id}");

            }
        }

        protected void acessarPastas()
        {
            automatico = false;
            if (Filtro == null)
            {
                acessar($"/pastas/{storyid}/{indice}/{dominio}");
            }
            else
            {
                acessar($"/pastas/{storyid}/{vers}/{dominio}");

            }
        }

        protected void acessarCapitulos()
        {
            if(compartilhou != "comp")
                acessar($"/{compartilhou}/{_story.Capitulo}");
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
            indice = i;

            acessar();
        }

        protected void acessarStory()
        {
            outroHorizonte = 0;
            storyid = RepositoryPagina.stories!
                .OrderBy(str => str.Capitulo).Skip(1).ToList()[indice - 1].Id;
            automatico = false;
            indice = 1;

            acessar();
        }

        protected void AdicionarAoCarrinho(long ProdutoId)
        {
            criptografar = true;
            var url = $"/carrinho/{ProdutoId}/{Compartilhou}/{Model2!.Id}";
            criptografar = false;
            acessar(url);
        }

        protected void removerPreferencia()
        {
            preferencia = null;
        }

        private async void acessar(string url2 = null)
        {
            if (url2 != null) Auto = 0;

            if(url2 == null)
            {

                if (Content && conteudo == 0) indice = 1;
                conteudo = Convert.ToInt32(Content);
                criptografar = true;

                string url = null;
                if(rotas == null)
                {
                    if (Filtro != null)
                    {
                        if(livro != null)
                        url = $"/Renderizar/{livro}/{storyid}/{indice}/{Auto}/{timeproduto}/{outroHorizonte}/{retroceder}/{dominio}/{Compartilhou}/{Filtro}";               
                        else
                        url = $"/Renderizar/{storyid}/{indice}/{Auto}/{timeproduto}/{outroHorizonte}/{retroceder}/{dominio}/{Compartilhou}/{Filtro}";               
                    }
                    else
                    {
                        if (livro != null)
                        url = $"/Renderizar/{livro}/{storyid}/{indice}/{Auto}/{timeproduto}/{outroHorizonte}/{retroceder}/{dominio}/{Compartilhou}";
                        else
                        url = $"/Renderizar/{storyid}/{indice}/{Auto}/{timeproduto}/{outroHorizonte}/{retroceder}/{dominio}/{Compartilhou}";
                    }
                }
                else
                {                   
                    if (livro != null)
                        url = $"/Renderizar/{livro}/{storyid}/{indice}/{Auto}/{timeproduto}/{outroHorizonte}/{retroceder}/{dominio}/{Compartilhou}/{rotas}";
                    else
                        url = $"/Renderizar/{storyid}/{indice}/{Auto}/{timeproduto}/{outroHorizonte}/{retroceder}/{dominio}/{Compartilhou}/{rotas}";                
                    
                }

                criptografar = false;
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

        private  string Encrypt(string plainText)
        {
            string Hasheada = BCrypt.Net.BCrypt.HashPassword(plainText);

            if (!Hasheada.Contains("/"))
                return Hasheada;
            else return Encrypt(plainText);
        }

        private bool Decrypt(string cipherText, string usuario)
        {
            if (BCrypt.Net.BCrypt.Verify(usuario, cipherText))
                return true;
            else return false;
        }

        public Task<Story> GetStoryByIdAsync(long storyId)
        {
            return storyService.GetStoryByIdAsync(storyId);
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

        public int CountLikesAsync(long ContentId, Livro livro)
        {
            return storyService.CountLikesAsync(ContentId, livro);
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

        public Task<List<FiltroContent>> GetFiltroByIdAsync(long filtroId, Livro livro, int slide = 0, int quantDiv = 0)
        {
            return storyService.GetFiltroByIdAsync(filtroId, livro, slide, quantDiv);
        }
    }
    public class UserPreferencesImage
    {
        public string? user { get; set; }
        public UserModel UserModel { get; set; }
    }
}

            