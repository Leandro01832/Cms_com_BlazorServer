using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using BlazorServerCms.servicos;
using business;
using business.business;
using business.business.Group;
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
        protected override async Task OnParametersSetAsync()
        {
            if (cap > RepositoryPagina.stories!.Last().Capitulo)
                capitulo = RepositoryPagina.stories!
                .OrderBy(str => str.Capitulo).Skip(1).ToList()[0].Capitulo;


            await renderizar();



            if (Model2 != null && Model2.usuariosDecorando != null && Model2.usuariosDecorando.Count > 0 && Filtro != null)
                adicionarPontos();

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (id_video is not null && !AlterouCamada)
            {
                if (AlterouModel)
                    await js!.InvokeAsync<object>("zerar", "1");
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
                .FirstAsync(us => us.Id == u.Id);

                if (Compartilhou != null && Compartilhou != "comp")
                {
                    var c = Context.Users
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
               .Where(t => t.IsSubclassOf(typeof(Content)) && !t.IsAbstract).ToList();

            Type itemParaMover = typeof(Pagina);
            Type itemParaMover2 = typeof(Chave);
            Type itemParaMover3 = typeof(ChangeContent);
            Type itemParaMover4 = typeof(Page);

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
           .OrderBy(s => s.Criterio?.Content != null && s.Criterio.Content is Chave ?
            ((Chave)s.Criterio.Content).Versiculo : 100000)
           .ToList();


            var result2 = await Context.SubFiltro!
            .Include(p => p.Camada)!
            .Include(p => p.Criterio)!
            .ThenInclude(p => p.Content)!
            .Include(p => p.Criterio)!
            .ThenInclude(p => p.Filtro)!
            .Include(p => p.Pagina)!
            .ThenInclude(p => p.Content)!
            .Include(p => p.usuariosDecorando)!
            .ThenInclude(p => p.UserModel)!
            .Where(f => f.LivroId == (livro != null ? livro.Id : null) &&
            !f.UltimaPasta &&
            f.StoryId == _story.Id &&
            f.Pagina.Count > 0)
            .ToListAsync();

            listaFiltro = result2
           .OrderBy(s => s.Criterio?.Content != null && s.Criterio.Content is Chave ?
               ((Chave)s.Criterio.Content).Versiculo : 100000)
            .ToList();


            if (Versiculo == null)
            {
                var fil = listaFiltro.FirstOrDefault(f => f.Id == Filtro);
                Versiculo = retornarVerso(fil.Criterio.Content);
            }

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

            if (!ultimaPasta)
                p = listaFiltro.Where(f => f.Criterio != null).FirstOrDefault(f =>
               retornarVerso(f.Criterio.Content) == Versiculo)!;
            else
                p = UltimasPastas.Where(f => f.Criterio != null).FirstOrDefault(f =>
                retornarVerso(f.Criterio.Content) == Versiculo)!;

            arrayContent = new long?[listaFiltro.Count][][];
            for (var i = 0; i < arrayContent.Length; i++)
                arrayContent[i] = new long?[tipos.Count][];



            Filtro = listaFiltro
            .FirstOrDefault(f => f.Id == p.FiltroId)!.Id;

            List<Chave> chaves = new List<Chave>();

            chaves = Context.Chave
            .Include(c => c.Criterio)!
             .ThenInclude(c => c.Filtro)
            .Include(c => c.Filtro)!
            .ThenInclude(c => c.Filtro)
            .ThenInclude(c => c.Criterio)
            .Where(c => c.StoryId == _story.Id && c.LivroId == (livro != null ? livro.Id : null))
            .OrderBy(c => c.Versiculo)
            .ToList();

            RepositoryPagina.Conteudo!.UnionWith(chaves);



        }

        private void instanciarTime(int camada)
        {
            SubFiltro[] fils = new SubFiltro[10];
            Time time = null;

            fils[0] = listaFiltro.FirstOrDefault(u => u.Id == Model2!.FiltroId)!;
            fils[1] = listaFiltro.FirstOrDefault(u => verificarFiltros(fils[0]) != null && u.Id == verificarFiltros(fils[0]).Id)!;
            fils[2] = listaFiltro.FirstOrDefault(u => verificarFiltros(fils[1]) != null && u.Id == verificarFiltros(fils[1]).Id)!;
            fils[3] = listaFiltro.FirstOrDefault(u => verificarFiltros(fils[2]) != null && u.Id == verificarFiltros(fils[2]).Id)!;
            fils[4] = listaFiltro.FirstOrDefault(u => verificarFiltros(fils[3]) != null && u.Id == verificarFiltros(fils[3]).Id)!;
            fils[5] = listaFiltro.FirstOrDefault(u => verificarFiltros(fils[4]) != null && u.Id == verificarFiltros(fils[4]).Id)!;
            fils[6] = listaFiltro.FirstOrDefault(u => verificarFiltros(fils[5]) != null && u.Id == verificarFiltros(fils[5]).Id)!;
            fils[7] = listaFiltro.FirstOrDefault(u => verificarFiltros(fils[6]) != null && u.Id == verificarFiltros(fils[6]).Id)!;
            fils[8] = listaFiltro.FirstOrDefault(u => verificarFiltros(fils[7]) != null && u.Id == verificarFiltros(fils[7]).Id)!;
            fils[9] = listaFiltro.FirstOrDefault(u => verificarFiltros(fils[8]) != null && u.Id == verificarFiltros(fils[8]).Id)!;

            if (fils[9] is not null)
            {
                time = Context.Time
               .Include(t => t.usuarios)
               .ThenInclude(t => t.UserModel)
               .FirstOrDefault(t =>
               t.usuarios.FirstOrDefault(u =>
               fils.FirstOrDefault(f =>
               f.usuariosDecorando.FirstOrDefault(us => us.UserModel.UserName
                == u.UserModel.UserName) != null) != null)! != null)!;
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

        private async Task<int> marcarIndice(bool criterio)
        {
            try
            {
                string? num = await js.InvokeAsync<string>("retornarlargura", "url");
                int result = 0;

                var largura = int.Parse(num);
                if (!criterio)
                {
                    quantDiv = ((19 * largura) / 1024);
                    result = quantDiv;

                }
                else
                {
                    var calc = 0;
                    if (largura > 550)
                        calc = ((18 * largura) / 1024);
                    else
                        calc = ((9 * largura) / 1024);
                    quantDivCriterio = calc;
                    result = quantDivCriterio;
                }
                return result;
            }
            catch (Exception ex)
            {
                return 10;
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

        private async Task renderizar()
        {
            // Pega o caminho relativo (ex: "videofilter/1/2/3")
            var relativePath = navigation.ToBaseRelativePath(navigation.Uri);
            int ind = 0;
            int ind2 = 0;
            var ti = relativePath.Split('/')[1].ToLower();
            if (ti != TipoClass.Name.ToLower())
                TipoClass = tipos
                .FirstOrDefault(t => t.Name.ToLower() == ti)!;

            var contentAdd = RepositoryPagina.Conteudo!
            .Where(c => c.GetType() == TipoClass &&
             c.Filtro.FirstOrDefault(f => f.FiltroId == Filtro) != null)
            .OrderBy(c => c.Id)
            .Distinct()
            .ToList();            

            Model2 = listaFiltro.FirstOrDefault(f => f.Id == Filtro);

            ind = listaFiltro.IndexOf(Model2);
            ind2 = tipos.IndexOf(TipoClass);

            if (Filtro != null)
            {
                var m = listaFiltro.FirstOrDefault(f => f.Id == Model2.FiltroId);


                nameGroup = Model2.Nome!;
                if (m != null)
                    nameGroup2 = m.Nome!;
                else nameGroup2 = "";
            }

            quantDiv = await marcarIndice(false);
            slideAtual = (Indice - 1) / quantDiv;
            int count = 0;
            if (Filtro != null)
                count = CountPagesInFilterAsync((long)Filtro!, livro, TipoClass);

            if (TipoClass != typeof(Page) && Filtro != null ||
               contentAdd.Count == 0 && Filtro != null ||
               arrayContent[ind][ind2][Indice - 1] == null && Filtro != null)
            {
                bool teste = false;
                if (contentAdd.Count == 0 && count > 0 ||
                    arrayContent[ind][ind2][Indice - 1] == null && count > 0)
                {
                    var r = RepositoryPagina.Conteudo!
                    .Where(c => c.Filtro.FirstOrDefault(f => f.FiltroId == Filtro) != null &&
                     c.GetType() == TipoClass)
                    .ToList();
                    if (r.Count == 0 || arrayContent[ind][ind2][Indice - 1] == null)
                    {
                        contentAdd.Clear();
                        var l = await PaginarFiltro((long)Filtro, quantDiv, slideAtual, livro, carregando);
                        contentAdd.AddRange(l.Select(li => li.Content)
                        .Where(c => c.GetType() == TipoClass)
                        .ToList()!);
                        quantidadeLista = contentAdd.Count;
                        var slide = slideAtual - 5;
                        if (slide < 0) slide = 0;
                        int posicaoInicial = slide * quantDiv;

                        for (var i = 0; i < contentAdd.Count; i++)
                        {
                            // 2. Acesse o índice real e contínuo dentro do seu array original
                            int indiceAlvo = posicaoInicial + i;

                            // 3. Opcional: Evite estouro de memória (IndexOutOfRangeException) conferindo o tamanho do array
                            if (indiceAlvo < arrayContent[ind][ind2].Length)
                            {
                                arrayContent[ind][ind2][indiceAlvo] = contentAdd[i].Id;
                            }
                        }

                    }
                }
                if (TipoClass != typeof(Page))
                    while (Filtro != null && contentAdd.Count == 0 ||
                    Filtro != null && arrayContent[ind][ind2][Indice - 1] == null)
                    {
                        teste = true;

                        if (TipoClass == typeof(Link) && listaFiltro
                        .Where(f => f.ComCriterio == Filtro).ToList().Count > 0)
                            break;

                        var t = tipos.First(ti => ti.Name == TipoClass.Name);
                        var indice = tipos.IndexOf(t);
                        TipoClass = tipos[indice - 1];
                        ind2 = tipos.IndexOf(TipoClass);
                        Indice = 1;
                        contentAdd.Clear();
                        contentAdd.AddRange(RepositoryPagina.Conteudo!.Where(c => c.GetType() == TipoClass)
                        .OrderBy(c => c.Id)
                        .ToList());
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

            Model = contentAdd.FirstOrDefault(c => c.Id == arrayContent[ind][ind2][Indice - 1]);

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

            var q = RepositoryPagina.Conteudo!
                    .LastOrDefault(c => c.StoryId == _story.Id && c is Chave && c.Html != null)!;
            if (q == null)
            {
                var pa = Context.Pagina!.OrderBy(p => p.Id)
                .LastOrDefault(c => c.StoryId == _story.Id && c is Chave && c.Html != null)!;
                RepositoryPagina.Conteudo2!.Add(pa);
                quantidadeLista = retornarVerso(pa);
            }
            else
                quantidadeLista = retornarVerso(q);

            if (Indice != 0)
            {
                if (livro == null)
                    Model = RepositoryPagina.Conteudo2!
                    .FirstOrDefault(p => p is Pagina &&
                    retornarVerso(p) == Indice
                    && p.StoryId == _story.Id
                    && p.LivroId == null);
                else
                    Model = RepositoryPagina.Conteudo2!
                    .FirstOrDefault(p => p is Pagina && retornarVerso(p) == Indice
                    && p.StoryId == _story.Id
                    && p.LivroId == livro.Id);

            }


            Model = RepositoryPagina.Conteudo2!
         .FirstOrDefault(p => p is Pagina && retornarVerso(p) == Indice && p.StoryId == _story.Id);

        }

        private async Task renderizarComFiltro()
        {
            if (condicaoFiltro || rotas != null)
            {
                carregando = 40;
                if (quantDiv == 0) quantDiv = 1;
                int slideAtivo = (Indice - 1) / quantDiv;
                slideAtual = slideAtivo;

                var count = CountPagesInFilterAsync((long)Filtro, livro, TipoClass);
                quantidadeLista = count;
                if (retroceder == 1)
                    retroceder = 0;

                var CountPages = CountPagesInFilterAsync((long)Filtro, livro, TipoClass);
                var CountPages2 = RepositoryPagina.Conteudo2!
                .Where(c => c.GetType() == TipoClass && c.Filtro != null &&
                c.Filtro!.FirstOrDefault(f => f.FiltroId == Filtro) != null).ToList().Count;

                if (CountPages2 == CountPages && CountPages2 != 0)
                {


                    if (Indice != 0)
                        Model = RepositoryPagina.Conteudo2!.Skip(Indice - 1).First();
                    else
                    {
                        if (Model != null)
                        {

                            Model = RepositoryPagina.Conteudo2!.FirstOrDefault(c => c.Id == Model!.Id);
                            Indice = RepositoryPagina.Conteudo2!.ToList().IndexOf(Model) + 1;
                        }
                    }
                }

            }

        }

        // Coloque no método 'renderizar()'
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
            var quantidadeFiltros = 0;

            int slideAtivo = 0;
            var fils = listaFiltro
                .Where(f => f.FiltroId == Filtro)
                .ToList();
            var f = fils.FirstOrDefault(f => f.Id == Filtro);
            var p = fils.IndexOf(f) + 1;
            slideAtualCriterio = (p - 1) / await marcarIndice(true);

            cap = RepositoryPagina.stories.First(st => st.Id == _story.Id).Capitulo;
            nameStory = RepositoryPagina.stories.First(st => st.Id == _story.Id).Nome;
            condicaoFiltro = CountFiltros();

        }

        private async Task FinalizarRenderizacao()
        {

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
            if (Model.Id != _ultimoIdProcessado)
            {
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
                html = await repositoryPagina!.renderizarPagina(Model);
                AlterouModel = false;
            }

            // Lógica 2: Exibir mensagens especiais de Chave/Filtro
            if (Model != null && Model.Html != null)
            {
                try
                {
                    if (Model.Titulo == "item" && Model.Html == "<p> Item </p>")
                    {
                        Model.Html = $"<p> Item {Indice} do conteudo {Model2!.Nome} </p>";
                        html = await repositoryPagina!.renderizarPagina(Model);
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
                        html = await repositoryPagina!.renderizarPagina(Model);
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
                    html = livro.Capa;
                else
                    html = RepositoryPagina.Capa;
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

        private async void FinalizarVariaveisUI()
        {
            // 1. Converte o HTML final em MarkupString para renderização no Blazor
            markup = new MarkupString(html);

            // 2. Prepara o array de Conteúdo para o componente de Paginação (se aplicável)

            array = new List<Content>[2];

            array2 = new List<Filtro>[2];


            // if (listaContent.Count != 0 && listaContent[0] != null)
            //   listaContent = listaContent.OrderBy(c => c.Id).ToList();

            if (array[0] == null)
                array[0] = new List<Content>();

            // Adiciona o primeiro slide/página de conteúdos ao array[0]
            if (RepositoryPagina.Conteudo2!.Count > quantDiv)
                array[0].AddRange(RepositoryPagina.Conteudo2!.Take(quantDiv).ToList());
            else
                array[0].AddRange(RepositoryPagina.Conteudo2!);


            if (array2[0] == null)
                array2[0] = new List<Filtro>();
            if (Filtro != null)
            {
                var fils = listaFiltro
                    .Where(f => f.FiltroId == Model2.FiltroId)
                    .ToList();

                // Adiciona o primeiro slide/página de conteúdos ao array[0]
                if (fils.Count > quantDiv)
                    array2[0].AddRange(fils.Skip(quantDiv * slideAtualCriterio)
                    .Take(quantDiv).ToList());
                else
                    array2[0].AddRange(fils);
            }



            // 3. Ajusta a classe CSS baseada no tamanho do número da página/verso

            int numeroParaVerificar = 0;
            if (Model2.Criterio != null)
                numeroParaVerificar = Filtro == null ? Indice : retornarVerso(Model2.Criterio.Content);
            else
                numeroParaVerificar = Indice;

            if (numeroParaVerificar < 100)
                classCss = "";
            else if (numeroParaVerificar >= 100 && numeroParaVerificar < 1000)
                classCss = " DivPagTam2";
            else if (numeroParaVerificar >= 1000 && numeroParaVerificar < 10000)
                classCss = " DivPagTam3";
            else if (numeroParaVerificar >= 10000 && numeroParaVerificar < 100000)
                classCss = " DivPagTam4";
            // Nota: O código original possui dois blocos IF/ELSE para Indice e vers. 
            // Foi consolidado usando uma variável temporária `numeroParaVerificar`.

            // 4. Define Placeholders e Classes CSS Dinâmicas
            if (!tellStory)
            {
                placeholder = "Nome";
                divPagina = "DivPagina";
                DivPag = "DivPag";
            }
            else
            {
                placeholder = "Nº do item";
                divPagina = "DivPagina2";
                DivPag = "DivPag2";
            }

        }

        private SubFiltro? verificarFiltros(SubFiltro f)
        {
            if (f == null)
                return f;
            else
            {
                return listaFiltro.FirstOrDefault(fil => fil.Id == f.FiltroId);
            }

        }

        private void adicionarPontos()
        {
            int pts = 0;
            int multiplicador = 1;
            Filtro[] fils = new Filtro[6];
            var us = Context.Users.FirstOrDefault(u => u.UserName == Compartilhou);
            var us2 = Context.SubFiltro.Include(f => f.usuariosDecorando).ThenInclude(f => f.UserModel)
                .FirstOrDefault(u => u.Id == Model2.Id);
            var us3 = Context.SubFiltro.Include(f => f.usuariosDecorando).ThenInclude(f => f.UserModel)
                .FirstOrDefault(u => u.Id == verificarFiltros(us2).Id);
            var us4 = Context.SubFiltro.Include(f => f.usuariosDecorando).ThenInclude(f => f.UserModel)
                .FirstOrDefault(u => u.Id == verificarFiltros(us3).Id);
            var us5 = Context.SubFiltro.Include(f => f.usuariosDecorando).ThenInclude(f => f.UserModel)
                .FirstOrDefault(u => u.Id == verificarFiltros(us4).Id);
            var us6 = Context.SubFiltro.Include(f => f.usuariosDecorando).ThenInclude(f => f.UserModel)
                .FirstOrDefault(u => u.Id == verificarFiltros(us5).Id);
            var us7 = Context.SubFiltro.Include(f => f.usuariosDecorando).ThenInclude(f => f.UserModel)
                .FirstOrDefault(u => u.Id == verificarFiltros(us6).Id);
            var users = Context.SubFiltro.ToList().Count;


            if (us2 != null) { pts = 2; fils[0] = us2; } else fils[0] = null;
            if (us3 != null) { pts = 3; fils[1] = us3; } else fils[1] = null;
            if (us4 != null) { pts = 4; fils[2] = us4; } else fils[2] = null;
            if (us5 != null) { pts = 5; fils[3] = us5; } else fils[3] = null;
            if (us6 != null) { pts = 6; fils[4] = us6; } else fils[4] = null;
            if (us7 != null) { pts = 7; fils[5] = us7; } else fils[5] = null;



            for (var i = 0; i < 6; i++)
            {
                var usuarios = fils[i].usuariosDecorando;

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
                                f.VersiculosDecorados != null);

                            var conteudos = Context.UserContent.Include(c => c.UserModel)
                                    .Where(c => c.UserModel.UserName == usuarios[j].UserModel.UserName &&
                                    c.Data.Date > DateTime.Now.AddDays(-7).Date)
                                    .ToList();

                            if (fil != null && i != 0)
                            {
                                Filtro condicao = listaFiltro.FirstOrDefault(f => f.usuariosDecorando
                                     .FirstOrDefault(us => us.UserModelId == usuarios[j].UserModel.Id) != null)!;

                                if (condicao != null)
                                {
                                    var UserModels = condicao.usuariosDecorando;
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
            foreach (var item in fil.usuariosDecorando)
                item.UserModel.IncluiTime(time);
            Context.SaveChanges();

        }

        private Filtro buscarProximoSubGrupo()
        {
            for (var i = 0; i < camadas.Count; i++)
            {
                if (Model2.Camada.Numero == camadas[i].Numero)
                {
                    var indice = returnList(true)
                    .Where(f => f.Camada.Numero == camadas[i].Numero)
                    .ToList().IndexOf(Model2);

                    if (indice + 1 == returnList(true).Where(f => f.Camada.Numero == camadas[i].Numero)
                    .ToList().Count)
                        return returnList(false, true).First();
                    else
                        return returnList(true)
                        .Where(f => f.Camada.Numero == camadas[i].Numero)
                        .ToList()[indice + 1];
                }
            }

            return null;
        }

        private Filtro voltarSubgrupos()
        {
            //  var tipo = Model2.GetType();

            for (var i = 10; i > 0; i--)
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

        private List<SubFiltro> returnList(bool todos, bool subir = false)
        {
            if (todos)
                return listaFiltro.Where(c => c.Pagina.Count > 0)
                .ToList();
            else
            {
                if (subir)
                    return listaFiltro.Where(c => c.Pagina.Count > 0 &&
                     c.Camada.Numero == Model2.Camada.Numero - 1)
                        .ToList();
                else
                    return listaFiltro.Where(c => c.Pagina.Count > 0 &&
                     c.Camada.Numero == Model2.Camada.Numero + 1)
                .ToList();

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

                            us.Compartilhar = "(" + name + ")";
                        }
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