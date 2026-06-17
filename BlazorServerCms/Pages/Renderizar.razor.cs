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
                    Indice = 1;
                }
                else if (args.Key == "Enter")
                {
                    capitulo = RepositoryPagina.stories.First().Capitulo;
                    var str = RepositoryPagina.stories.First(st => st.Id == _story.Id);
                    Indice = RepositoryPagina.stories.IndexOf(str);
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
            else if (!tellStory && condicao)
            {
                Indice = 1;
                int p = int.Parse(opcional);
                Filtro =
                listaFiltro
                .Where(f => f.FiltroId == Model2.FiltroId)
                .ToList()[p - 1].Id;
                acessar();

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
                    Indice = proximo;
                }
                else
                {
                    Indice = 1;
                }

                acessar();
            }
            else
            {
                if (proximo <= quant)
                {
                    Indice = proximo;
                    acessar();
                }
                else if (TipoClass != typeof(Page))
                {
                    var t = tipos.FirstOrDefault(t => t.Name.ToLower() == TipoClass.Name.ToLower());
                    var i = tipos.IndexOf(t);
                    TipoClass = tipos[i - 1];
                    Indice = 1;
                    acessar();
                }
                else if (Filtro != null)
                    navegarSubgrupos(true);
                else
                {
                    cap++;
                    capitulo = RepositoryPagina.stories
                    .First(str => str.Capitulo == cap).Capitulo;
                    Indice = 1;
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
            if (rotas != null)
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
                        Indice = count;
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
                    Indice = anterior;
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

        protected void acessarVerso()
        {
            Indice = (int)Versiculo!;
            Filtro = null;
            ultimaPasta = false;
            acessar();
        }

        protected void acessarComCriterio()
        {
            SubFiltro sub = listaFiltro.FirstOrDefault(s => s.Id == Model2!.Id);
            Filtro = sub.ComCriterio;
            TipoClass = typeof(Link);
            Indice = 1;

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

        protected async void desabilitarTellStory()
        {
            tellStory = false;
            await js!.InvokeAsync<object>("DarAlert", $"A história {Model2.Nome} não esta sendo mais contada e dividida!!!");
            acessar();
        }

        protected async void redirecionarMarcar()
        {
            int Vers = 0;
            Filtro fi = null;
            try
            {
                if (Filtro != null)
                    Vers = int.Parse(await js.InvokeAsync<string>("prompt", "Informe o versículo."));
                else
                    Vers = (int)Versiculo!;
                if (Vers > arrayContent.Length)
                {
                    await js!.InvokeAsync<object>("DarAlert", $"Informe um versículo menor ou igual a {arrayContent.Length}.");
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
                    Filtro = fi?.Id;

                    var item = await buscarRelogio();
                    var filt = listaFiltro.First(f => f.Id == item.SubFiltroId);
                    var c = filt.Pagina.Select(p => p.Content)
                    .Where(p => p.GetType() == Type.GetType(item.Tipo)).Skip(Indice).FirstOrDefault();

                    if (item != null && c != null &&
                     fi.Pagina.FirstOrDefault(p => p.ContentId == c.Id) != null)
                        await AcessarHashtagId();
                    else
                    {
                        Indice = repositoryPagina.random.Next(1, fi.Pagina.Count);
                        acessar();
                    }
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

        private async Task<Relogio?> buscarRelogio()
        {
            if (profile != null)
            {
                var c = Context.Users
                .Include(u => u.Relogio)
                .FirstOrDefault(u => u.UserName == profile.UserName);
                var r = c.Relogio.FirstOrDefault(rel => rel.SubFiltroId == Model2.Id);
                if(r != null)
                return r;
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

                    var item = await buscarRelogio();
                    if (item != null)
                    {
                        var filt = listaFiltro.First(f => f.Id == item.SubFiltroId);
                        var co = filt.Pagina.Select(p => p.Content)
                        .FirstOrDefault(p => p.Id == item.ContentId);
                        var l = filt.Pagina.Select(p => p.Content)
                        .OrderBy(c => c.Id)
                        .Where(c => c.GetType() == co.GetType()).ToList();
                        var teste = l.First(c => c.Id == co.Id);
                        Indice = l.IndexOf(teste) + 1;
                        TipoClass = co.GetType();
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
            Relogio rel = null;
            Filtro = id;
            if(profile != null)
            {
                var rels = profile.Relogio;
                rel = rels.FirstOrDefault(r => r.SubFiltroId == id)!;                
            }
            if(rel != null)
            {
                tipoClass = rel.Content.GetType();
                var fil = listaFiltro.First(f => f.Id == rel.SubFiltroId);
                var teste = fil.Pagina.FirstOrDefault(p => p.ContentId == rel.ContentId);
                Indice = fil.Pagina
                .Where(p => p.Content.GetType() == tipoClass)
                .OrderBy(p => p.ContentId).ToList().IndexOf(teste) + 1;
                
            }
            else
            {
                TipoClass = typeof(Page);
                Indice = 1;                
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

        public Task<List<FiltroContent>> PaginarFiltro<T>( long filtroId, int quantDiv,
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
            if (Model2!.Camada.Numero != camada)
                foreach (var item in listaFiltro.Where(l => l.Camada.Numero == camada).ToList())
                    if (item.Pagina.FirstOrDefault(p => p.ContentId == Model.Id) != null)
                    {
                        Filtro = item.Id;
                        var m = item.Pagina.FirstOrDefault(p => p.ContentId == Model.Id);
                        Indice = item.Pagina.IndexOf(m) + 1;
                        acessar();
                    }
        }

        protected async void atualizarFiltro(ChangeEventArgs e)
        {
            var valor = e.Value!.ToString()!;
            TipoClass = tipos.FirstOrDefault(t => t.Name.ToLower() == valor.ToLower())!;
            var item = await buscarRelogio();
            if (item != null)
            {
                var filt = listaFiltro.First(f => f.Id == item.SubFiltroId);
                    var c = filt.Pagina.Select(p => p.Content)
                    .Where(p => p.GetType() == Type.GetType(item.Tipo)).Skip(Indice).FirstOrDefault();
                if (c != null &&  c.GetType().Name.ToLower() == valor.ToLower())
                    await AcessarHashtagId();
                else
                {
                    Indice = 1;
                    acessar();
                }
            }
            else
            {
                Indice = 1;
                acessar();
            }
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
   
    }

    public class UserPreferencesImage
    {
        public string? user { get; set; }
        public UserModel UserModel { get; set; }
    }
}

