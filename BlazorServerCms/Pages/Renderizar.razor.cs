using System;
using System.Text.RegularExpressions;
using BlazorServerCms.Data;
using BlazorServerCms.servicos;
using business;
using business.business;
using business.business.Book;
using business.business.Group;
using Humanizer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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
                if (p != null && p.Html != null)
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
                    capitulo = RepositoryPagina.stories!
                    .OrderBy(str => str.Capitulo).Skip(1).ToList()[Indice - 1].Capitulo;
                    alterarIndice(1);
                }
                else if (args.Key == "Enter")
                {
                    capitulo = RepositoryPagina.stories.First().Capitulo;
                    var str = RepositoryPagina.stories.First(st => st.Id == _story.Id);
                    alterarIndice(RepositoryPagina.stories.IndexOf(str));
                }

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
                if (tellStory)
                    condicao = int.Parse(opcional) <=
                     Model2.Pagina.Select(p => p.Content)
                    .Where(c => c.GetType() == TipoClass).ToList().Count;
                else
                    condicao = int.Parse(opcional) <=
                    RepositoryPagina.Conteudo!
                    .Where(c => c.GetType() == typeof(Chave)).ToList().Count;
            }
            catch (Exception ex)
            {
                condicao = false;
            }

            if (tellStory)
            {
                alterarIndice(int.Parse(opcional));
                acessar();
            }
            else if (condicao)
            {
                Versiculo = int.Parse(opcional);
                redirecionarMarcar();

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

        protected void listarPastas()
        {
            if (livro != null)
                acessar($"/listar-pasta/{_story.Capitulo}/{livro.Id}");
            else
                acessar($"/listar-pasta/{_story.Capitulo}");
        }

        protected void buscarProximo()
        {
            AlterouModel = true;

            long quant = 0;
            if (Filtro == null)
                quant = quantidadeLista;
            else
            {
                List<Content> lista = null;
                if (rotas != null)
                    lista = null;
                else
                    quant = quantidadeLista;
            }

            var proximo = Indice + 1;


            if (rotas != null)
            {
                if (proximo <= quant)
                {
                    alterarIndice(proximo);
                }
                else
                {
                    alterarIndice(1);
                }

                acessar();
            }
            else
            {
                if (proximo <= quant)
                {
                    alterarIndice(proximo);
                    acessar();
                }
                else if (TipoClass != typeof(Page))
                {
                    var t = tipos.FirstOrDefault(t => t.Name.ToLower() == TipoClass.Name.ToLower());
                    var i = tipos.IndexOf(t);
                    TipoClass = tipos[i - 1];
                    alterarIndice(1);
                    acessar();
                }
                else if (Filtro != null)
                    navegarSubgrupos(true);
                else
                {
                    cap++;
                    capitulo = RepositoryPagina.stories
                    .First(str => str.Capitulo == cap).Capitulo;
                    alterarIndice(1);
                    acessar();
                }
            }
        }

        private async void navegarSubgrupos(bool somenteSubgrupos)
        {
            if (somenteSubgrupos)
            {
                Filtro proximoSubgrupo = buscarProximoSubGrupo();
                Filtro = proximoSubgrupo.Id;
                alterarIndice(1);
            }
            else
            {
                alterarIndice(Indice + 1);
            }
            acessar();
        }

        protected void buscarAnterior()
        {
            AlterouModel = true;

            bool alterouIdice = false;
            if (rotas != null)
            {
                if (Indice == 1)
                {
                    alterarIndice(1);
                }
                else
                {
                    alterarIndice(Indice - 1);
                }
            }
            else
            {
                if (Indice == 1 && cap != 0)
                {
                    if (Filtro != null)
                    {
                        if (TipoClass != typeof(UserContent))
                        {
                            var t = tipos.FirstOrDefault(t => t.Name.ToLower() == TipoClass.Name.ToLower());
                            var i = tipos.IndexOf(t);
                            TipoClass = tipos[i + 1];
                        }
                        Filtro fi = voltarSubgrupos();
                        Filtro = fi.Id;
                        var count = CountPagesInFilterAsync((long)Filtro, livro, TipoClass);
                        alterarIndice(count);
                        retroceder = 1;
                        alterouIdice = true;
                    }

                    if (Filtro == null)
                    {
                        cap--;
                        capitulo = RepositoryPagina.stories
                        .First(str => str.Capitulo == cap).Capitulo;
                    }


                }
                if (Indice != 1 && rotas == null && !alterouIdice)
                {
                    var anterior = Indice - 1;
                    alterarIndice(anterior);
                }
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

        protected async Task acessarVerso()
        {
            if(!tellStory)
            {
                alterarIndice((int)Versiculo!);
                Filtro = null;
                ultimaPasta = false;
                TipoClass = typeof(Chave);
                acessar();                
            }
            else
            {
                tellStory = false;
                await js!.InvokeAsync<object>("DarAlert", $"A história {Model2.Nome} não esta sendo mais contada e dividida!!!");

            }
        }

        protected void acessarComCriterio()
        {
            SubFiltro sub = listaFiltro.FirstOrDefault(s => s.Id == Model2!.Id);
            Filtro = sub.ComCriterio;
            TipoClass = typeof(Link);
            alterarIndice(1);

            acessar();
        }

        protected void ativarModal()
        {
            showModal = true;
        }

        protected void Agrupar()
        {
            AlterouModel = false;
            var cam = Model2!.Camada.Numero;
            acessarCamada(cam - 1);
        }

        protected void Filtrar()
        {
            AlterouModel = false;
            var cam = Model2!.Camada.Numero;
            acessarCamada(cam + 1);
        }

       

        protected async void redirecionarMarcar()
        {
            int Vers = 0;
            Filtro fi = null;
            try
            {
                var quantVersiculos = RepositoryPagina.Conteudo!
                .Where(c => c.GetType() == typeof(Chave)).ToList().Count;

                Vers = (int)Versiculo!;
                if (Indice == 1)
                {
                    await js!.InvokeAsync<object>("DarAlert", $"Seja bem vindo a Story {nameStory}!!! Marque um versículo entre 1 e {quantVersiculos}.");
                    return;
                }

                if (Vers > quantVersiculos)
                {
                    await js!.InvokeAsync<object>("DarAlert", $"Informe um versículo menor ou igual a {quantVersiculos}.");
                    return;
                }
                var fil = listaFiltro.FirstOrDefault(f => f.Criterio.Content is Chave
                && retornarVerso(f.Criterio.Content) == Vers);
                if (fil == null)
                {
                    await js!.InvokeAsync<object>("DarAlert", $"Não tem nenhum item para este versículo {Vers}.");
                    return;
                }
                else if (listaFiltro.FirstOrDefault(f => f.Id == fil.FiltroId).Pagina.Count == 0)
                {
                    await js!.InvokeAsync<object>("DarAlert", $"Não tem nenhum item para este versículo {Vers}.");
                    return;
                }
                else
                {
                    fi = listaFiltro.FirstOrDefault(f => f.Id == fil.FiltroId);
                    TipoClass = typeof(Page);
                    Filtro = fi?.Id;
                    acessar();

                    // var item = await buscarRelogio();
                    // SubFiltro filt = null;
                    // Content c = null;
                    // if (item != null)
                    //     filt = listaFiltro.First(f => f.Id == item.SubFiltroId);
                    // if (filt != null)
                    //     c = filt.Pagina.Select(p => p.Content)
                    //    .Where(p => p.GetType() == Type.GetType(item.Tipo))
                    //    .Skip(Indice).FirstOrDefault()!;

                    // if (item != null && c != null &&
                    //  fi.Pagina.FirstOrDefault(p => p.ContentId == c.Id) != null)
                    //     await AcessarHashtagId();
                    // else
                    // {
                    //     Indice = repositoryPagina.random.Next(1, fi.Pagina.Count);
                    // }
                }
            }
            catch (Exception ex)
            {
                await js!.InvokeAsync<object>("DarAlert", $"Não tem nenhum item para este versículo {Vers}.");
                return;
            }
        }

        private bool CountFiltros()
        {
            return HasFiltersAsync((long)capitulo!, livro);
        }

        private string colocarAutoPlay(string html)
        {
            var conteudoHtml = html;
            var arr = conteudoHtml!.Split("/");

            html = html.Replace("<iframe", "<iframe" + " allow=' autoplay;' ");
            html = html.Replace("controls=0", "");
            return html;
        }

        private async Task AtualizarHashtagId()
        {
            // Todos os usuarios vão ter a hashtag #Id 
            // que irá ajudar a compartilhar quando for apenas uma pagina
            // e não precisará agrupar

            if (Filtro != null)
            {
                if (user.Identity!.IsAuthenticated)
                {
                    //atualizar #Id
                    UserModel? c = Context.Users
                    .Include(u => u.Relogio)
                    .FirstOrDefault(u => u.UserName == user.Identity!.Name);
                    var r = c.Relogio.FirstOrDefault(rel => rel.SubFiltroId == Model2.Id);
                    var p = TipoClass != typeof(Page);
                    if (r == null)
                    {
                        r = new Relogio
                        {
                            ContentId = Model.Id,
                            SubFiltroId = Model2.Id,
                            UserModelId = c.Id
                        };
                        Context.Add(r);
                        await Context.SaveChangesAsync();
                    }
                    else
                    {
                        r.Data = DateTime.UtcNow;
                        r.ContentId = Model.Id;
                        Context.Update(r);
                        await Context.SaveChangesAsync();
                    }

                }


            }
        }

        private async Task<Relogio?> buscarRelogio(SubFiltro fil)
        {
            if (profile != null)
            {
                var re = Context.Relogio.FirstOrDefault(rel => rel.SubFiltroId == fil.Id &&
                rel.UserModelId == profile.Id);
                var filt = listaFiltro.First(f => f.Id == re.SubFiltroId);
                var co = filt.Pagina.Select(p => p.Content)
                .FirstOrDefault(p => p.Id == re.ContentId);
                TipoClass = co.GetType();
                if (re != null)
                {
                    Filtro = re.SubFiltroId;
                    var fi = listaFiltro.FirstOrDefault(f => f.Id == Filtro);
                    if (arrayContent[Ind][Ind2] != null &&
                    arrayContent[Ind][Ind2].Contains(re.ContentId) && fi.Embaralhar)
                    {

                        arrayContent[Ind][Ind2] = repositoryPagina.embaralhar(arrayContent[Ind][Ind2].ToList()).ToArray();
                        alterarIndice(arrayContent[Ind][Ind2].ToList().IndexOf(re.ContentId) + 1);

                    }
                    else
                    {

                        var l = filt.Pagina.Select(p => p.Content)
                        // .OrderBy(c => c.Id)
                        .Where(c => c.GetType() == co.GetType()).ToList();
                        var teste = l.First(c => c.Id == co.Id);
                        alterarIndice(l.IndexOf(teste) + 1);
                        await preencher();
                    }
                    return re;
                }
                else
                    return null;

            }
            return null;
        }

        protected async Task AcessarHashtagId()
        {
            // Todos os usuarios vão ter a hashtag #Id 
            // que irá ajudar a compartilhar quando for apenas uma pagina
            // e não precisará agrupar

            if (Filtro != null)
            {
                if (profile != null)
                {
                    // aceessar hashtag #Id

                    var item = await buscarRelogio(Model2);
                    if (item != null)
                    {
                        var filt = listaFiltro.First(f => f.Id == item.SubFiltroId);
                        var co = filt.Pagina.Select(p => p.Content)
                        .FirstOrDefault(p => p.Id == item.ContentId);
                        TipoClass = co.GetType();
                        var l = filt.Pagina.Select(p => p.Content)
                        // .OrderBy(c => c.Id)
                        .Where(c => c.GetType() == co.GetType()).ToList();
                        var teste = l.First(c => c.Id == co.Id);
                        if (arrayContent[Ind][Ind2] != null)
                        {
                            if (arrayContent[Ind][Ind2].Contains(co.Id))
                                alterarIndice(arrayContent[Ind][Ind2].ToList().IndexOf(co.Id) + 1);
                            else
                            {
                                alterarIndice(l.IndexOf(teste) + 1);
                                await preencher();
                            }
                        }
                        acessar();
                    }
                    else
                    {
                        try
                        {
                            await js!.InvokeAsync<object>("DarAlert",
                             $"Hashtag #Id não encontrada. Marque o versículo correto.");

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }

            }
        }

        protected async void StartTour()
        {
            automatico = false;
            try
            {
                await js!.InvokeAsync<object>("ExibirOpcoesTour");
                await js!.InvokeAsync<object>("ExibirOpcoesTour");
                await TourService.StartTour("FormGuidedTour");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected async void ativarOption()
        {
            try
            {
                await js!.InvokeAsync<object>("ExibirOpcoesTour");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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

        protected void acessarComentarios()
        {
            showModal2 = true;
        }

        protected async void SalvarComentario()
        {
            if (usuario != null)
            {
                comment.ContentId = Model!.Id;
                comment.UserModelId = usuario.Id;
                comment.LivroId = livro?.Id;
                comment.StoryId = _story.Id;
                Context.Add(comment);
                Context.SaveChanges();
                var FiltroContent = new FiltroContent { ContentId = comment.Id, FiltroId = Model2.Id };
                Context.Add(FiltroContent);
                Context.SaveChanges();
                RepositoryPagina.Conteudo!.Add(comment);
                comment = new Comment();
                await js!.InvokeAsync<object>("DarAlert",
                 $"Comentário adicionado com sucesso!!!");
                showModal2 = false;

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

        protected void AdicionarAoCarrinho(long ProdutoId)
        {
            var url = $"/carrinho/{ProdutoId}/{Compartilhou}/{Model2!.Id}";
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
                await js.InvokeVoidAsync("seekToVideo", (int)seconds);
                AlterouModel = true;
                AlterouCamada = true;
                AlterarCamada((int)seconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected async void AcessarComLink(long id)
        {
            Relogio? rel = null;
            Filtro = id;
            if (profile != null)
            {
                var fil = listaFiltro.FirstOrDefault(f => f.Id == id);
                rel = await buscarRelogio(fil);
            }
            if (rel != null)
            {
                tipoClass = rel.Content.GetType();
                var fil = listaFiltro.First(f => f.Id == rel.SubFiltroId);
                var teste = fil.Pagina.FirstOrDefault(p => p.ContentId == rel.ContentId);
                alterarIndice(fil.Pagina
                .Where(p => p.Content.GetType() == tipoClass)
                // .OrderBy(p => p.ContentId)
                .ToList().IndexOf(teste) + 1);


            }
            else
            {
                TipoClass = typeof(Page);
                alterarIndice(1);
            }

            acessar();
        }

        private async void acessar(string url2 = null)
        {
            Timer!._timer!.Elapsed -= _timer_Elapsed;
            if (url2 != null) Auto = 0;
            Tipo = TipoClass.Name.ToLower();

            if (url2 == null)
            {
                string url = null;
                if (rotas == null)
                {
                    if (livro != null)
                        url = $"/renderizar/{Tipo}/{livro.Nome}/{capitulo}/{Versiculo}/{Indice}/{Compartilhou}";
                    else
                        url = $"/renderizar/{Tipo}/{capitulo}/{Versiculo}/{Indice}/{Compartilhou}";

                }
                else
                {
                    if (livro != null)
                        url = $"/renderizar/{Tipo}/{livro.Nome}/{capitulo}/{Versiculo}/{Indice}/{Compartilhou}/{rotas}";
                    else
                        url = $"/renderizar/{Tipo}/{capitulo}/{Versiculo}/{Indice}/{Compartilhou}/{rotas}";

                }

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

        public Task<int> GetYouTubeVideoDurationAsync(string videoId)
        {
            return storyService.GetYouTubeVideoDurationAsync(videoId);
        }

        public bool HasFiltersAsync(long storyId, Livro livro)
        {
            return storyService.HasFiltersAsync(storyId, livro);
        }

        public Task<List<FiltroContent>> PaginarFiltro<T>(long filtroId, int quantDiv,
         int slideAtual, Livro livro, int? carregando = null) where T : class
        {
            return storyService.PaginarFiltro<T>(filtroId, quantDiv, slideAtual, livro, carregando);
        }

        public int CountPagesInFilterAsync(long filtroId, Livro livro, Type type)
        {
            return storyService.CountPagesInFilterAsync(filtroId, livro, type);
        }

        private async void RemoverPlay()
        {
            try
            {
                await js.InvokeVoidAsync("removerVideo");

            }
            catch (Exception)
            {

            }
        }

        private void AlterarCamada(int timeNumber)
        {
            if (Model is VideoFilter)
            {
                var marcacoes = Context.MarcacaoVideoFilter
                .Where(m => m.ContentId == Model.Id)
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
            if (Model2!.Camada.Numero != camada)
                foreach (var item in listaFiltro.Where(l => l.Camada.Numero == camada).ToList())
                    if (item.Pagina.FirstOrDefault(p => p.ContentId == Model.Id) != null)
                    {
                        Filtro = item.Id;
                        var m = item.Pagina.FirstOrDefault(p => p.ContentId == Model.Id);
                        alterarIndice(item.Pagina
                        .Where(p => p.Content.GetType() == TipoClass).ToList().IndexOf(m) + 1);
                        acessar();
                    }
        }

        protected async void atualizarFiltro(ChangeEventArgs e)
        {
            var valor = e.Value!.ToString()!;
            TipoClass = tipos.FirstOrDefault(t => t.Name.ToLower() == valor.ToLower())!;            
            acessar();
        }

        protected void TratarToqueInicio(TouchEventArgs e)
        {
            // Pega a coordenada X do primeiro dedo que encostou na tela
            toqueInicioX = e.TargetTouches[0].ClientX;
        }

        protected void TratarToqueFim(TouchEventArgs e)
        {
            // Pega a coordenada X de onde o dedo saiu da tela
            toqueFimX = e.ChangedTouches[0].ClientX;

            // Calcula a direção do movimento
            AnalisarGestoSwipe();
        }

        private void AnalisarGestoSwipe()
        {
            double diferencaX = toqueInicioX - toqueFimX;

            // Verifica se o movimento foi longo o suficiente
            if (Math.Abs(diferencaX) > DistanciaMinimaParaSwipe)
            {
                if (diferencaX > 0)
                {
                    // O dedo moveu da direita para a esquerda -> Avança o carrossel
                    buscarProximo();
                }
                else
                {
                    // O dedo moveu da esquerda para a direita -> Voltar o carrossel
                    buscarAnterior();
                }
            }
        }

        public async Task preencher()
        {
            if (TipoClass != typeof(Baralho))
            {
                var l = await preencherLista(Ind, Ind2, Indice, TipoClass);              
                
            }
            else
            {
                List<Content>[] arr2 = null;
                string[] arr = null;
                int count = 0;
                if (usuario != null)
                {
                    arr = usuario.TipoBaralho!.Split(',');
                    arr2 = new List<Content>[arr.Length + 2];
                }
                else
                    arr2 = new List<Content>[2];

                if (usuario != null)
                {
                    var assemblyDoProjeto = typeof(Content).Assembly;
                    for (var i = 0; i < arr.Length; i++)
                    {
                       Type tip = assemblyDoProjeto.GetType(arr[i].Trim())!;
                        var lista = await buscarLista(tip);
                        arr2[i] = lista;
                        count += lista.Count;
                    }
                }


                var lista2 = await buscarLista(typeof(Page));
                if (usuario != null)
                    arr2[arr.Length] = lista2;
                else
                    arr2[0] = lista2;
                count += lista2.Count;

                var lista3 = await buscarLista(typeof(ProductContent));
                if (usuario != null)
                    arr2[arr.Length + 1] = lista3;
                else
                    arr2[1] = lista3;
                count += lista3.Count;

                int mi = 0;

                // 1. Criamos uma lista com todos os índices válidos do seu array (ex: 0, 1, 2, 3...)
                List<int> indicesDisponiveis = Enumerable
                .Range(0, count).ToList();

                // O loop agora roda baseando-se estritamente nas vagas restantes
                while (indicesDisponiveis.Count > 0)
                {
                    var k = repositoryPagina.random.Next(0, arr2.Length);
                    if (arr2[k] == null || arr2[k].Count == 0) continue;

                    var l = repositoryPagina.random.Next(0, arr2[k].Count);

                    // 2. Em vez de sortear o array inteiro, sorteamos uma POSIÇÃO da lista de vagas restantes!
                    var indexNaListaDeVagas = repositoryPagina.random.Next(0, indicesDisponiveis.Count);
                    var indiceAlvo = indicesDisponiveis[indexNaListaDeVagas];

                    // O 'indiceAlvo' aqui é GARANTIDO que está nulo/vazio, eliminando tentativas repetidas
                    if (!arrayContent[ind][ind2].Contains(arr2[k][l].Id))
                    {
                        arrayContent[ind][ind2][indiceAlvo] = arr2[k][l].Id;
                        arr2[k].Remove(arr2[k][l]);
                        mi++;

                        // 3. Como essa vaga foi preenchida, removemos ela da nossa lista de disponíveis!
                        indicesDisponiveis.RemoveAt(indexNaListaDeVagas);
                    }

                }
                alterarIndice(1);
                Console.WriteLine("Total de itens inseridos de forma ultra rápida: " + mi);
            }
        }

        private async Task<List<Content>> preencherLista(int ind, int ind2, int ind3, Type t)
        {
            List<Content> lista = new List<Content>();
            carregou = false;
            lista.Clear();
            List<FiltroContent> l = null;

            // 1. Pegamos os metadados do método "PaginarFiltro" da classe onde ele está declarado
            // Se o método estiver na mesma classe atual, use 'this.GetType()'
            var metodoInfo = this.GetType().GetMethod("PaginarFiltro");

            if (metodoInfo != null)
            {
                // 2. Transformamos o método genérico aberto no tipo específico que está na sua variável 'tipoClass'
                var metodoGenerico = metodoInfo.MakeGenericMethod(t);

                // 3. Invocamos o método passando os parâmetros dentro de um array de objetos na ordem exata
                var task = (Task)metodoGenerico.Invoke(this, new object[] { (long)Filtro!, QuantDiv, SlideAtual, livro, carregando });

                // 4. Como o método é assíncrono (await), aguardamos o término e pegamos o resultado
                await task;
                l = (List<FiltroContent>)((dynamic)task).Result;
            }

            lista.AddRange(l.Select(li => li.Content)
            .Where(c => c.GetType() == t)
            .ToList()!);

            contentAdd.AddRange(lista);

            int posicaoInicial = SlideAtual * QuantDiv;

            if (Model2.Embaralhar)
                lista = repositoryPagina.embaralhar(lista).ToList();
           
            var value = Model2.Pagina.Where(p => p.Content.GetType() == t)
            .Select(p => p.Content).ToList()[ind3 - 1];

            if (TipoClass != typeof(Baralho))
                for (var i = 0; i < lista.Count; i++)
                {
                    // 2. Acesse o índice real e contínuo dentro do seu array original
                    int indiceAlvo = posicaoInicial + i;

                    // 3. Opcional: Evite estouro de memória (IndexOutOfRangeException) conferindo o tamanho do array
                    if (indiceAlvo < arrayContent[ind][ind2].Length)
                    {
                        if (!arrayContent[ind][ind2].Contains(lista[i].Id))
                            arrayContent[ind][ind2][indiceAlvo] = lista[i].Id;
                    }
                }

            var li = arrayContent[ind][ind2].ToList();
            if (Model2.Embaralhar && TipoClass != typeof(Baralho) && Indice != 1)
             alterarIndice(li.IndexOf(value.Id)  + 1);
                
            
            carregou = true;
            return lista;
        }

        private async Task<List<Content>> buscarLista(Type t)
        {
            var num = 0;
            List<Content> lista = new List<Content>();
            int total = CountPagesInFilterAsync((long)Filtro!, livro, t);
            if(total > num)
            num = repositoryPagina.random.Next(1, total);

            var condicao = total > QuantDiv * carregando * 2;

            if(num > 0)
            while (lista.Count != QuantDiv * carregando)
            {
                var li = await preencherLista(Ind, Ind2, num, t);
                if (!condicao)
                {
                    lista = new List<Content>(li);
                    break;
                }
                else
                    if (lista.Count != QuantDiv * carregando)
                    {
                        num = repositoryPagina.random.Next(1, total);
                    }
            }
            
            return lista;
        }
    }

    public class UserPreferencesImage
    {
        public string? user { get; set; }
        public UserModel UserModel { get; set; }
    }



}

