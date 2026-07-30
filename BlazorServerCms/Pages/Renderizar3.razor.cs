using System.Text.RegularExpressions;
using BlazorServerCms.servicos;
using business.business;
using business.business.conteudo;
using business.business.Group;
using business.business.sistema;
using business.business.relacionamento;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace BlazorCms.Client.Pages
{
    public partial class RenderizarBase : ComponentBase
    {
        #region  EventosPrincipais
        protected override async Task OnParametersSetAsync()
        {
            if (cap > RepositoryPagina.stories!.Last().Capitulo)
                capitulo = RepositoryPagina.stories!
                .OrderBy(str => str.Capitulo).Skip(1).ToList()[0].Capitulo;


            await renderizar();



            if (Filtro != null)
                adicionarPontos();

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && TipoClass == typeof(Streaming))
            {
                string token = "";
                // 1. Gera as credenciais da live no backend C#
                string urlServidorLiveKit = "wss://instagleo-rx9jiwj0.livekit.cloud"; // URL do seu LiveKit
                if (usuario != null)
                    token = LiveService.GerarTokenAcesso(Model!.Html!, usuario.Id, false);
                else
                {
                    string idAnonimo = $"anonimo_{Guid.NewGuid().ToString().Substring(0, 8)}";
                    token = LiveService.GerarTokenAcesso(Model!.Html!, idAnonimo, false);
                }


                // 2. Importa o script auxiliar usando JS Interop do Blazor
                moduloJs = await js!.InvokeAsync<IJSObjectReference>("import", "./livekit-helper.js");

                // 3. Inicializa o player passando o token gerado pelo C#
                await js.InvokeVoidAsync("window.livekitHelper.conectarNaLive",
                 urlServidorLiveKit, token, "playerLiveKit");

                carregandoStreaming = false;
                StateHasChanged();
            }

            if (QuantDiv != repositoryPagina!.QuantDiv)
            {
                QuantDiv = await marcarIndice(false);
                QuantDivCriterio = await marcarIndice(true);
                if (QuantDiv != repositoryPagina!.QuantDiv ||
                 QuantDivCriterio != repositoryPagina!.QuantDivCriterio)
                {
                    repositoryPagina!.QuantDiv = QuantDiv;
                    repositoryPagina!.QuantDivCriterio = QuantDivCriterio;
                }
            }


            if (id_video is not null && !AlterouCamada)
            {
                if (AlterouModel)
                    await js!.InvokeAsync<object>("zerar", "1");
                if (Filtro != null)
                    await js.InvokeVoidAsync("carregarVideo", id_video);




                id_video = null;
            }
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
                .Include(u => u.Relogio)
                .ThenInclude(u => u.Hashtag)
                .ThenInclude(u => u.HashtagContent)
                .FirstAsync(us => us.Id == u.Id);

                if (Compartilhou != null && Compartilhou != "comp")
                {
                    var c = Context.Users
                    .Include(u => u.Hashtag)
                    .ThenInclude(u => u.HashtagContent)
                    .Include(u => u.Time)
                    .ThenInclude(u => u.Time)
                    .ThenInclude(u => u.usuarios)
                    .ThenInclude(u => u.UserModel)
                    .Include(u => u.Relogio)
                    .ThenInclude(u => u.Content)
                    .FirstOrDefault(u => u.UserName == Compartilhou);
                    profile = c;

                    if (c.Compartilhar != null)
                    {
                        string padrao = @"\(([^)]*)\)";
                        Match = Regex.Match(c.Compartilhar, padrao);
                    }
                }

            }
            else
            {
                if (Compartilhou != null && Compartilhou != "comp")
                {
                    var c = Context.Users
                    .Include(u => u.Hashtag)
                    .ThenInclude(u => u.HashtagContent)
                    .Include(u => u.Time)
                    .ThenInclude(u => u.Time)
                    .ThenInclude(u => u.usuarios)
                    .ThenInclude(u => u.UserModel)
                    .Include(u => u.Relogio)
                    .ThenInclude(u => u.Content)
                    .FirstOrDefault(u => u.UserName == Compartilhou);
                    profile = c;
                }
                else
                    profile = null;
            }

            Marcacao.Marcados.Clear();
            Marcacao.resultado = "";

            // 1. Pega o "Assembly" (o seu programa/projeto executável)
            var assembly = typeof(Content).Assembly;

            // 2. Filtra todos os tipos que são subclasses de Animal
            tipos = assembly.GetTypes()
               .Where(t => t.IsSubclassOf(typeof(Content)) && !t.IsAbstract &&
               t.Namespace == "business.business.conteudo").ToList();

            Type itemParaMover = typeof(Pagina);
            Type itemParaMover2 = typeof(Chave);
            Type itemParaMover3 = typeof(ChangeContent);
            Type itemParaMover4 = typeof(Page);
            Type itemParaMover5 = typeof(Baralho);
            Type itemParaMover6 = typeof(BaralhoHashTag);

            // 1. Verifica se o item realmente existe na lista
            if (tipos.Contains(itemParaMover))
            {
                // 2. Remove o item de sua posição atual
                tipos.Remove(itemParaMover);
                tipos.Remove(itemParaMover2);
                tipos.Remove(itemParaMover3);
                tipos.Remove(itemParaMover4);
                // 3. Insere o item na primeira posição (índice 0)
                tipos.Insert(0, itemParaMover4);
                tipos.Insert(1, itemParaMover5);
                tipos.Insert(2, itemParaMover6);

            }

            Auto = 0;
            timeproduto = 11;

            if (Compartilhou == null) Compartilhou = "comp";

            if (Auto == 0 && Timer!.desligarAuto! != null
                && Timer!.desligarAuto!.Enabled == true)
            {

                Timer!.desligarAuto!.Elapsed -= desligarAuto_Elapsed;
                Timer!.desligarAuto!.Enabled = false;
                Timer.desligarAuto.Dispose();
            }

            if (nomeLivro != null)
                livro = await Context.Livro!.FirstOrDefaultAsync(l => l.Nome == nomeLivro);

            if (_story == null)
            {
                _story = RepositoryPagina.stories.Skip(1).ToList()[(int)capitulo! - 1];
            }

            camadas = await Context.Camada
           .Where(c => c.LivroId == (livro != null ? livro.Id : null))
           .ToListAsync();

            var result = await Context.SubFiltro!
               .Include(p => p.Criterio)!
               .ThenInclude(p => p.Content)!
               .Include(p => p.Criterio)!
               .ThenInclude(p => p.Filtro)!
               .Where(f => f.LivroId == (livro != null ? livro.Id : null) &&
               f.UltimaPasta &&
               f.StoryId == _story.Id)
               .ToListAsync();

            UltimasPastas = result
           .OrderBy(s => s.Criterio != null ? s.FiltroId : // s.FiltroId)           
           result.Where(f => f.FiltroId == s.FiltroId).LastOrDefault()!.Id)
           .ToList();


            var result2 = await Context.SubFiltro!
            .Include(p => p.Camada)!
            .Include(p => p.Criterio)!
            .ThenInclude(p => p.Content)!
            .Include(p => p.Criterio)!
            .ThenInclude(p => p.Filtro)!
            .Include(p => p.Pagina)!
            .ThenInclude(p => p.Content)!
            .Where(f => f.LivroId == (livro != null ? livro.Id : null) &&
            !f.UltimaPasta &&
            f.StoryId == _story.Id &&
            f.Pagina.Count > 0)
            .ToListAsync();

            listaFiltro = result2
           .OrderBy(s => s.Criterio != null ? s.FiltroId : //s.FiltroId)
           result2.Where(f => f.FiltroId == s.FiltroId).LastOrDefault()!.Id)
            .ToList();


            if (Versiculo == null)
            {
                var fil = listaFiltro.FirstOrDefault(f => f.Id == Filtro);
                Versiculo = retornarVerso(fil.Criterio.Content);
            }

            arrayContent = new long?[listaFiltro.Count][][];
            for (var i = 0; i < arrayContent.Length; i++)
                arrayContent[i] = new long?[tipos.Count][];



            var teste = await Context.SubFiltro
            .Include(s => s.Criterio)
            .ThenInclude(s => s.Content)
            .FirstOrDefaultAsync(f =>
            f.Criterio != null &&
            f.Criterio.Content is Chave &&
            ((Chave)f.Criterio.Content).Versiculo == versiculo);

            ultimaPasta = listaFiltro
           .FirstOrDefault(f => f.Id == teste.Id) == null;
            SubFiltro p = null;

            if (Indice != 0)
            {
                if (!ultimaPasta)
                    p = listaFiltro.Where(f => f.Criterio != null).FirstOrDefault(f =>
                retornarVerso(f.Criterio.Content) == Versiculo)!;
                else
                    p = UltimasPastas.Where(f => f.Criterio != null).FirstOrDefault(f =>
                    retornarVerso(f.Criterio.Content) == Versiculo)!;

                Filtro = listaFiltro.FirstOrDefault(f => f.Id == p.FiltroId)!.Id;
                alterarIndice(Indice);
            }
            else
            {

                long? fi = null;
                string numeros = string.Concat(Tipo.Where(char.IsDigit));
                fi = long.Parse(numeros);
                Tipo = Tipo.Replace(numeros, "");
                if (!ultimaPasta)
                    p = listaFiltro.FirstOrDefault(f => f.Id == fi)!;
                else
                    p = UltimasPastas.FirstOrDefault(f => f.Id == fi)!;
                Filtro = listaFiltro
                .FirstOrDefault(f => f.Id == p.Id)!.Id;
                if (profile != null)
                {
                    var rel = profile.Relogio.OrderBy(r => r.Data)
                                .FirstOrDefault(r => r.SubFiltro.CriterioId == null)!;
                    var c = await Context.Content.FirstAsync(c => c.Id == rel.ContentId);

                    var test = p.Pagina.FirstOrDefault(p => p.ContentId == c.Id);
                    Indice = p.Pagina
                   .Where(p => p.Content.GetType().Name.ToLower() == Tipo.ToLower())
                   .OrderBy(p => p.ContentId).ToList().IndexOf(test) + 1;
                }
                else
                {
                    var pages = p.Pagina.Where(p => p.Content is Page).ToList();
                    alterarIndice(repositoryPagina.random.Next(1, pages.Count));
                }

            }

            List<Chave> chaves = new List<Chave>();

            chaves = Context.Chave
            .Include(c => c.Criterio)!
             .ThenInclude(c => c.Filtro)
            .Include(c => c.Filtro)!
            .ThenInclude(c => c.Filtro)
            .ThenInclude(c => c.Criterio)
            .Where(c => c.StoryId == _story.Id
            && c.LivroId == (livro != null ? livro.Id : null))
            .OrderBy(c => c.Versiculo)
            .ToList();

            if (RepositoryPagina.Conteudo!
            .Where(c => c.GetType() == typeof(Chave)).ToList().Count == 0)
                RepositoryPagina.Conteudo!.UnionWith(chaves);

            if (profile != null && profile.Hashtag.Count > 0)
                HashtagId = profile.Hashtag.First().Id;

        }

        #endregion

        #region RenderizarConteudo

        private async Task renderizar()
        {

            // Pega o caminho relativo (ex: "videofilter/1/2/3")
            var relativePath = navigation.ToBaseRelativePath(navigation.Uri);
            //  int ind = 0;
            //  int ind2 = 0;
            var ti = Tipo.ToLower();
            if (ti != TipoClass.Name.ToLower())
                TipoClass = tipos
                .FirstOrDefault(t => t.Name.ToLower() == ti)!;

            contentAdd = RepositoryPagina.Conteudo!
            .Where(c => c.GetType() == TipoClass &&
             c.Filtro.FirstOrDefault(f => f.FiltroId == Filtro) != null)
            // .OrderBy(c => c.Id)
            .Distinct()
            .ToList();

            Model2 = listaFiltro.FirstOrDefault(f => f.Id == Filtro);

            Ind = listaFiltro.IndexOf(Model2);
            Ind2 = tipos.IndexOf(TipoClass);


            int count = 0;
            if (Filtro != null)
            {
                if (arrayContent[Ind][Ind2] != null)
                    count = arrayContent[Ind][Ind2].Length;
                else
                    count = CountPagesInFilterAsync((long)Filtro!, livro, TipoClass);

            }

            if (Filtro != null && TipoClass != typeof(Page) ||
              Filtro != null && contentAdd.Count == 0 ||
              Filtro != null && Ind >= 0 && arrayContent[Ind][Ind2][Indice - 1] == null)
            {
                bool teste = false;
                if (count > 0 && contentAdd.Count == 0 ||
                    count > 0 && arrayContent[Ind][Ind2][Indice - 1] == null)
                {
                    var re = RepositoryPagina.Conteudo!
                    .Where(c => c.Filtro.FirstOrDefault(f => f.FiltroId == Filtro) != null &&
                     c.GetType() == TipoClass)
                    .ToList();
                    if (re.Count == 0 || arrayContent[Ind][Ind2][Indice - 1] == null)
                    {
                        await preencher();

                    }
                }
                else
                    if (TipoClass != typeof(Page))
                        while (Filtro != null && contentAdd.Count == 0 ||
                        Filtro != null && arrayContent[Ind][Ind2][Indice - 1] == null)
                        {
                            teste = true;

                            if (TipoClass == typeof(Link) && listaFiltro
                            .Where(f => f.ComCriterio == Filtro).ToList().Count > 0)
                                break;

                            var t = tipos.First(ti => ti.Name == TipoClass.Name);
                            var indice = tipos.IndexOf(t);
                            TipoClass = tipos[indice - 1];

                            contentAdd.Clear();
                            contentAdd.AddRange(RepositoryPagina.Conteudo!.Where(c => c.GetType() == TipoClass)
                            // .OrderBy(c => c.Id)
                            .ToList());

                            if (arrayContent[Ind][Ind2] != null)
                                count = arrayContent[Ind][Ind2].Length;
                            else
                                count = CountPagesInFilterAsync((long)Filtro!, livro, TipoClass);
                            if (count == 0)
                            {
                                try
                                {
                                    await js!.InvokeAsync<object>("DarAlert", $"Não tem nenhum " +
                                    "item para este tipo de conteudo " +
                                    $"({Activator.CreateInstance(tipoClass)!.ToString()}).");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Um erro aconteceu: " + ex.Message);
                                }
                            }
                            else
                            {
                                Ind2 = tipos.IndexOf(TipoClass);
                                alterarIndice(1);
                                await preencher();
                            }
                        }
                if (teste)
                {
                    bool info = listaFiltro
                    .FirstOrDefault(f => f.CriterioId == null &&
                     f.ComCriterio == Model2.Id) != null;

                    if (TipoClass == typeof(Link) && info)
                    {
                        var l = listaFiltro
                        .Where(f => f.ComCriterio == Filtro).ToList();
                        count = l.Count;
                        contentAdd.Clear();
                        foreach (var item in l)
                        {
                            contentAdd.Add(new Link
                            {
                                Id = item.Id,
                                StoryId = item.StoryId,
                                LivroId = item.LivroId,
                                Criterio = null,
                                Html = $"<p> <a href='#' > {item.Nome} </a> </p>",
                                Data = DateTime.Now,
                                Filtro = new List<FiltroContent>
                                   {
                                     new FiltroContent
                                     {
                                         FiltroId = item.Id,
                                         Filtro = listaFiltro.First(f => f.Id == item.Id)
                                     }
                                   }
                            });
                        }

                    }

                    quantidadeLista = count;
                }
            }

            while (TipoClass == typeof(Baralho) && arrayContent[Ind][Ind2][Indice - 1] == null)
                await preencher();


            if (TipoClass != typeof(Link) && TipoClass != typeof(Chave))
                Model = contentAdd.FirstOrDefault(c => c.Id == arrayContent[Ind][Ind2][Indice - 1]);
            else if (TipoClass == typeof(Chave))
            {
                contentAdd = RepositoryPagina.Conteudo!
                .Where(c => c.GetType() == TipoClass)
                .OrderBy(c => ((Chave)c).Versiculo)
                .Distinct()
                .ToList();
                Model = contentAdd.Skip(Indice - 1).FirstOrDefault();
            }
            else
                Html = "";

            RepositoryPagina.Conteudo2.Clear();
            RepositoryPagina.Conteudo2
            .UnionWith(contentAdd);
            quantidadeLista = RepositoryPagina.Conteudo2.Count;

            // Lógica Inicial: Tratamento de exceção e chamadas JS iniciais
            await InicializarRenderizacao();

            // AQUI ESTÁ A CHAVE DA DIVISÃO
            if (Filtro == null)
            {
                await renderizarSemFiltro();
            }
            else // Filtro != null
            {
                await renderizarComFiltro();
            }

            // Lógica Final: Paginação, Renderização HTML e Finalização
            await FinalizarRenderizacao();
        }
        private async Task renderizarSemFiltro()
        {
            tellStory = false;
            if (Indice > quantidadeLista)
            {
                if (quantidadeLista != 0)
                    Mensagem = $"Por favor digite um numero menor que {quantidadeLista}.";
                else
                    Mensagem = "aguarde um momento...";
                return;
            }

            Versiculo = Indice;
            quantidadeLista = RepositoryPagina.Conteudo!
            .Where(c => c.GetType() == typeof(Chave)).ToList().Count;

        }

        private async Task renderizarComFiltro()
        {
            if (condicaoFiltro)
            {
               // var count = CountPagesInFilterAsync((long)Filtro, livro, TipoClass);
                quantidadeLista = arrayContent[Ind][Ind2].Length;
                var m = listaFiltro.FirstOrDefault(f => f.Id == Model2.FiltroId);
                NameGroup = Model2.Nome!;
                if (m != null)
                    nameGroup2 = m.Nome!;
                else nameGroup2 = "";

                var fils = listaFiltro
                .Where(f => f.FiltroId == Filtro)
                .ToList();
                var f = fils.FirstOrDefault(f => f.Id == Filtro);
                var p = fils.IndexOf(f);
                var q = await marcarIndice(true);
                if (q == 0) q = 1;
                slideAtualCriterio = p / q;
            }
        }

        private async Task InicializarRenderizacao()
        {
            try
            {
                if (AlterouModel && !AlterouCamada)
                    await js!.InvokeAsync<object>("zerar", "1");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            ultimaPasta = false;
            cap = RepositoryPagina.stories.First(st => st.Id == _story.Id).Capitulo;
            nameStory = RepositoryPagina.stories.First(st => st.Id == _story.Id).Nome;
            condicaoFiltro = CountFiltros();
        }

        private async Task FinalizarRenderizacao()
        {
            if (retroceder == 1)
                retroceder = 0;



            // Lógica de Renderização do HTML (Model.Html)
            if (cap != 0 && AlterouModel && !AlterouCamada)
                StartTimer(Model);

            // ... lógica de iframe/autoplay/renderizarPagina/Model.Html (muito grande, idealmente em outro método) ...
            await RenderizarModelHtml();

            // ... lógica de liked/usuário ...
            await VerificarUserLiked();

            // ... lógica de array, classCss e placeholders ...
            FinalizarVariaveisUI();

            try
            {
                await firstInput.FocusAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (!tellStory && Filtro != null)
            {
                quantidadeFiltro = listaFiltro
                    .Where(f => f.FiltroId == Model2.FiltroId)
                    .ToList().Count;
            }

            ultimaPasta =
           listaFiltro.FirstOrDefault(f => Filtro != null &&
           f.Criterio != null &&
           f.FiltroId == Model2.Id) == null;

            if (Filtro != null && ultimaPasta)
            {
                var f = UltimasPastas.FirstOrDefault(fil => fil.FiltroId == Model2.Id);
                if (f != null && f.Criterio != null)
                {
                    Versiculo = retornarVerso(f.Criterio.Content);

                }
                else
                {
                    var fi = listaFiltro.FirstOrDefault(fil => fil.Id == Model2.ComCriterio);
                    f = listaFiltro.FirstOrDefault(f => f.FiltroId == fi.Id);
                    Versiculo = retornarVerso(f.Criterio.Content);
                }
            }
            else if (Filtro != null)
            {
                Filtro f = null;
                if (Model2 != null && Model2.Criterio != null)
                {
                    f = listaFiltro.FirstOrDefault(f => f.FiltroId == Model2.Id)!;
                    Versiculo = retornarVerso(f.Criterio.Content);
                }

                else if (f.Criterio == null)
                {
                    f = listaFiltro.FirstOrDefault(f => f.Id == Model2.ComCriterio)!;
                    if (f != null && f.Criterio != null)
                        Versiculo = retornarVerso(f.Criterio.Content);
                }
            }
            else
                Versiculo = retornarVerso(Model);

            if (Filtro == null)
            {
                criterio = null;
                var fil = RepositoryPagina.Conteudo2!.FirstOrDefault(c => c is Chave &&
                 retornarVerso(c) == Versiculo)!;
                var m = ((SubFiltro)fil.Criterio!.Filtro.First()).FiltroId;
                var f = listaFiltro.FirstOrDefault(f => f.Id == m);
                if (f != null && f.Criterio != null)
                    criterio = f.Criterio;
            }

            PreencherProgresso();

            // Só executa se o ID realmente mudou,
            //  evitando rodar em re-renderizações bobas
            if (Model.Id != _ultimoIdProcessado && Indice != 1)
            {
                if (tipoClass != typeof(Link))
                    await AtualizarHashtagId();
                _ultimoIdProcessado = Model.Id;
                // Atualiza o último ID processado
            }

        }

        private async Task RenderizarModelHtml()
        {
            // Lógica 1: Processar e renderizar o HTML se o Model foi alterado
            if (Model != null && Model.Html != null && AlterouModel)
            {
                var conteudoHtml = Model.Html;

                // Aplica o AutoPlay se houver iframe/vídeo
                if (Model.Html.Contains("iframe"))
                {
                    conteudoHtml = colocarAutoPlay(conteudoHtml);
                }
                else
                {
                    // Se não for um iframe, reseta o id_video
                    id_video = null;
                }

                Model.Html = conteudoHtml;

                // Chama o método de renderização do repositório
                Html = await repositoryPagina!.renderizarPagina(Model);
                AlterouModel = false;
            }

            // Lógica 2: Exibir mensagens especiais de Chave/Filtro
            if (Model != null && Model.Html != null)
            {
                try
                {
                    if (Model.Titulo == "item" && Model.Html == "<p> Item </p>")
                    {
                        Model.Html = $"<p> Item do conteudo {Model2!.Nome} </p>";
                        Html = await repositoryPagina!.renderizarPagina(Model);
                        // Pode ser removido da lista de conteúdos, se necessário.
                        // Somente se todos os itens forem removidos, ficando apenas as chaves.
                        // as chaves não podem ser removidas. 
                    }


                    // Caso 2b: Página de Chave (Filtro Nulo)
                    if (Model is Chave && Model.Titulo == "chave" && Filtro == null)
                    {
                        var verso = retornarVerso(Model);
                        // Busca os Filtros associados a esta Chave
                        var fils = listaFiltro
                            .Where(f => f.Pagina!.FirstOrDefault(p => p.Content is Chave &&
                            retornarVerso(p.Content) == verso) != null).ToList();

                        Model.Html = $"<p> O versiculo {verso} é a chave que abre ";

                        // Formatação plural/singular
                        if (fils.Count == 1)
                            Model.Html += $"a sub-story (pasta): ";
                        else
                            Model.Html += $"as sub-stories (pastas): ";

                        // Lista os nomes dos Filtros
                        foreach (var item in fils)
                            Model.Html += item.Nome + ", ";

                        Model.Html += "</p>";

                        // Limpeza final da string
                        Model.Html = Model.Html.Replace(", </p>", "</p>");
                        Html = await repositoryPagina!.renderizarPagina(Model);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro na lógica de Chave/Filtro: " + ex.Message);
                }
            }
            // Lógica 3: Exibir capa (se o Model não tiver conteúdo)
            else if (Model != null && Model.Produto != null && Model.Produto.Count == 0)
            {
                if (livro != null)
                    Html = livro.Capa;
                else
                    Html = RepositoryPagina.Capa;
            }
        }

        private async Task VerificarUserLiked()
        {
            UserModelContent p = null;

            try
            {
                // Verifica se o usuário está autenticado
                if (user.Identity!.IsAuthenticated)
                {
                    // Tenta encontrar o registro de "like" para o conteúdo atual e o usuário logado
                    p = Context.UserModelPageLiked
                        .Include(umpl => umpl.Content)
                        .Include(umpl => umpl.UserModel)
                        .FirstOrDefault(p => p.ContentId == Model.Id &&
                        p.UserModel.UserName == user.Identity!.Name)!;
                }
            }
            catch (Exception)
            {
                // Em caso de erro (ex: Model nulo ou falha na query), assume que não houve "like"
                liked = false;
                // Não logamos a exceção aqui para evitar poluir o console com erros comuns de acesso
                // mas em um ambiente real, logging seria recomendado.
            }

            // Atualiza o estado da UI com base na pesquisa
            if (p != null)
                liked = true;
            else
                liked = false;
        }


        #endregion

        private async Task<int> marcarIndice(bool criterio)
        {
            try
            {
                string? num = await js.InvokeAsync<string>("retornarlargura", "url");
                int result = 0;

                var largura = int.Parse(num);
                if (!criterio)
                {
                    QuantDiv = ((19 * largura) / 1024);
                    result = QuantDiv;

                }
                else
                {
                    var calc = 0;
                    if (largura > 550)
                    {
                        calc = ((6 * largura) / 1280);
                        larg = true;
                    }
                    else
                    {
                        calc = ((6 * largura) / 375);
                        larg = false;
                    }
                    QuantDivCriterio = calc;
                    result = QuantDivCriterio;
                }
                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        protected int retornarVerso(Content c)
        {
            if (c != null && c is Pagina)
            {
                Pagina pag = (Pagina)c;
                return pag.Versiculo;

            }
            else
                return 0;
        }

        private void adicionarPontos()
        {
            if (profile != null)
            {
                int pts = 1;
                int multiplicador = 1;
                int times = 0;
                List<UserModel> lista = new List<UserModel>();
                foreach (var item in profile.Time)
                {
                    times++;
                    foreach (var item2 in item.Time.usuarios)
                        lista.Add(item2.UserModel);
                }

                for (var j = 0; j < lista.Count; j++)
                {


                    if (DateTime.Now.Date > lista[j].DataPontuacao.Date)
                    {
                        if (lista[j].PontosPorDia > lista[j].Recorde)
                        {

                            lista[j].Recorde = lista[j].PontosPorDia;
                            Context.Update(lista[j]);
                            Context.SaveChanges();
                        }
                        lista[j].PontosPorDia = 1;
                        lista[j].DataPontuacao = DateTime.Now;
                        Context.Update(lista[j]);
                        Context.SaveChanges();
                    }
                    else
                    {


                        var conteudos = Context.UserContent
                        .Include(c => c.UserModel)
                        .Where(c =>
                        c.UserModel.UserName == lista[j].UserName &&
                        c.Data.Date > DateTime.Now.AddDays(-7).Date)
                        .ToList();

                        if (lista.Count >= 100 && lista.Count < 200) multiplicador += 1;
                        else if (lista.Count >= 200 && lista.Count < 300) multiplicador += 2;
                        else if (lista.Count >= 300 && lista.Count < 400) multiplicador += 3;
                        else if (lista.Count >= 400 && lista.Count < 500) multiplicador += 4;
                        else if (lista.Count >= 500 && lista.Count < 600) multiplicador += 5;
                        else if (lista.Count >= 600 && lista.Count < 700) multiplicador += 6;
                        else if (lista.Count >= 700 && lista.Count < 800) multiplicador += 7;
                        else if (lista.Count >= 800 && lista.Count < 900) multiplicador += 8;
                        else if (lista.Count >= 900) multiplicador += 9;

                        var contentFiltro = conteudos.ToList();

                        multiplicador += conteudos.Count;

                        if (contentFiltro.Count > conteudos.Count / 2)
                            multiplicador += contentFiltro.Count;

                        if (times > 0)
                        {
                            multiplicador += 2 * profile.Time.Sum(ut => ut.Time.vendas);

                            int soma = 0;
                            List<UserModel> l = new List<UserModel>();

                            foreach (var t in profile.Time)
                                l.Add(t.UserModel);

                            soma += l.Sum(ut => ut.Recorde);

                            if (soma > repositoryPagina.metaTime)
                                multiplicador += 1;


                            var pontosGanhos = multiplicador * pts * Model2.Camada.Numero;
                            foreach (var UserModel in lista)
                            {

                                UserModel.PontosPorDia += pontosGanhos;
                                Context.Update(UserModel);
                                Context.SaveChanges();
                            }
                        }



                        multiplicador += conteudos.Count;
                        var pontosGanhos2 = multiplicador * pts * Model2.Camada.Numero;


                        usuarios[j].UserModel.PontosPorDia += pontosGanhos2;
                        Context.Update(usuarios[j]);
                        Context.SaveChanges();



                    }

                }
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

                        if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
                        {
                            var us = Context.Users.First(u => u.UserName == user.Identity.Name);

                            if (us.Compartilhar == null)
                            {
                                us.Compartilhar = "(" + Versiculo + ")";

                            }
                            else
                            {
                                if (us.Compartilhar.Contains(','))
                                {
                                    var arr = us.Compartilhar.Split(',');
                                    if (!arr.Contains(Versiculo.ToString()))
                                    {
                                        us.Compartilhar += "(" + Versiculo + ")";

                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro na mensaegem contar historia: " + ex.Message);
            }
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

        private async Task AtualizarHashtagId()
        {
            // Todos os usuarios vão ter a hashtag #Id 
            // que irá ajudar a compartilhar quando for apenas uma pagina
            // e não precisará agrupar

            if (Filtro != null)
            {
                if (user.Identity!.IsAuthenticated)
                {
                    var re = usuario.Relogio.FirstOrDefault(rel => rel.SubFiltroId == Model2.Id)!;
                    var p = TipoClass != typeof(Page);
                    if (re == null)
                    {
                        re = new Relogio
                        {
                            ContentId = Model.Id,
                            SubFiltroId = Model2.Id,
                            UserModelId = usuario.Id
                        };
                        Context.Add(re);
                        await Context.SaveChangesAsync();
                    }
                    else
                    {
                        re.Data = DateTime.UtcNow;
                        re.ContentId = Model.Id;
                        Context.Update(re);
                        await Context.SaveChangesAsync();
                        usuario = Context.Users
                        .Include(u => u.PageLiked)
                        .Include(u => u.Relogio)
                        .ThenInclude(u => u.Hashtag)
                        .ThenInclude(u => u.HashtagContent)
                        .FirstOrDefault(u => u.UserName == user.Identity!.Name)!;
                    }

                }
            }
        }

        private async Task<Relogio?> buscarRelogio(SubFiltro fil)
        {
            if (usuario != null)
            {
                var re = usuario.Relogio.FirstOrDefault(rel => rel.SubFiltroId == fil.Id &&
                rel.UserModelId == profile.Id)!;
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

        protected async Task SalvarConteudo(long Hashtag)
        {
            // 1. Validação de segurança para evitar NullReference Exception
            if (Model == null || Model.Id == 0)
                return;

            var hashyagContent = new HashtagContent
            {
                ContentId = Model.Id,
                HashtagId = Hashtag
            };

            // 2. Adiciona ao rastreador de forma síncrona
            Context.Add(hashyagContent);

            // 3. Salva de forma assíncrona
            await Context.SaveChangesAsync();
            if (HashtagId != null)
                HashtagId = hashtag.Id;

            showModal3 = false;
        }

        protected async void SalvarComentario()
        {
            if (usuario != null)
            {
                comment.ContentId = Model!.Id;
                comment.UserModelId = usuario.Id;
                Context.Add(comment);
                Context.SaveChanges();
                comment = new Comment();
                await js!.InvokeAsync<object>("DarAlert",
                 $"Comentário adicionado com sucesso!!!");
                showModal2 = false;

            }
        }

        protected async void Ocultar()
        {
            OcultarMenu = !OcultarMenu;      
             StateHasChanged();  
        }
    }

    public class Baralho : Content
    {
        public override string ToString()
        {
            return "Baralho";
        }
    }

    //Baralho filtrado: somente diminui a quantidade de itens
    public class BaralhoHashTag : Content
    {
        public override string ToString()
        {
            return "#Hashtag";
        }
    }

}