using BlazorServerCms.Data;
using BlazorServerCms.servicos;
using business;
using business.business;
using business.Group;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Models;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Policy;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

namespace BlazorCms.Client.Pages
{
    public partial class RenderizarBase : ComponentBase
    {

        protected override async Task OnParametersSetAsync()
        {

            if (capitulo > stories!.Last().PaginaPadraoLink)
                capitulo = 1;

            await renderizar();

            bool testeNum = true;
            try
            {
                int num = int.Parse(opcional);
            }
            catch (Exception ex)
            {
                testeNum = false;
                opcional = "";
            }

            if (testeNum)
            {
                redirecionarParaVerso(int.Parse(opcional));
                opcional = "";
            }

            if(substory != null)
            if (resposta == null ||
                resposta != null && resposta.pasta != indice_Filtro ||
                resposta != null && compartilhante != resposta.user ||
                resposta != null && question != resposta.Pergunta ||
                marcador == null && marcacao  ||
                marcador != null && marcacao && question != indiceMarcador && indiceMarcador > 0)
            {
                usuario = await userManager.Users.FirstOrDefaultAsync(u => u.UserName == user.Identity!.Name)!;
                await marcarResponder();

            }

            if (auto == 1)
                StartTimer(Model!);

        }

        protected async Task marcarResponder()
        {
            numeros = "";
            if (!marcacao)
            {
                resposta = Context.UserResponse
                .FirstOrDefault(u => u.user == compartilhante && u.capitulo == capitulo &&
                u.pasta == indice_Filtro && u.Pergunta == question)!;
                int? pergu = null;

                if (resposta != null)
                {
                    pergu = resposta.Pergunta;
                    if (resposta.resposta1 != 0)
                    {
                        numeros += $"{resposta.resposta1}";
                    }
                    if (resposta.resposta2 != 0)
                    {
                        numeros += $", {resposta.resposta2}";
                    }
                    if (resposta.resposta3 != 0)
                    {
                        numeros += $", {resposta.resposta3}";
                    }
                    if (resposta.resposta4 != 0)
                    {
                        numeros += $", {resposta.resposta4}";
                    }
                    if (resposta.resposta5 != 0)
                    {
                        numeros += $", {resposta.resposta5}";
                    }
                    if (resposta.resposta6 != 0)
                    {
                        numeros += $", {resposta.resposta6}";
                    }
                    if (resposta.resposta7 != 0)
                    {
                        numeros += $", {resposta.resposta7}";
                    }
                    if (resposta.resposta8 != 0)
                    {
                        numeros += $", {resposta.resposta8}";
                    }
                    if (resposta.resposta9 != 0)
                    {
                        numeros += $", {resposta.resposta9}";
                    }
                    if (resposta.resposta10 != 0)
                    {
                        numeros += $", {resposta.resposta10}";
                    }
                }
                else
                {
                    numeros = "";
                    pergu = question;
                }

                var filtro = Context.Filtro.Include(u => u.Pergunta)
               .FirstOrDefault(u => u.Id == Model2.Id)!;
                pergunta = filtro.Pergunta.OrderBy(p => p.Id).Skip((int)pergu - 1).FirstOrDefault();
            }
            else
            {
                var marc = Context.highlighter.Where(hi => hi.capitulo == capitulo &&
                hi.pasta == indice_Filtro && hi.user == compartilhante).OrderBy(hi => hi.Id).ToList();
                marcador = marc.Skip(question - 1).FirstOrDefault()!;
                if (marcador != null)
                {
                    indiceMarcador = marc.IndexOf(marcador) + 1;
                    if(marcador.verso1 != 0)
                        {
                        numeros += $"{marcador.verso1}";
                    }
                    if (marcador.verso2 != 0)
                    {
                        numeros += $", {marcador.verso2}";
                    }
                    if (marcador.verso3 != 0)
                    {
                        numeros += $", {marcador.verso3}";
                    }
                    if (marcador.verso4 != 0)
                    {
                        numeros += $", {marcador.verso4}";
                    }
                    if (marcador.verso5 != 0)
                    {
                        numeros += $", {marcador.verso5}";
                    }
                    if (marcador.verso6 != 0)
                    {
                        numeros += $", {marcador.verso6}";
                    }
                    if (marcador.verso7 != 0)
                    {
                        numeros += $", {marcador.verso7}";
                    }
                    if (marcador.verso8 != 0)
                    {
                        numeros += $", {marcador.verso8}";
                    }
                    if (marcador.verso9 != 0)
                    {
                        numeros += $", {marcador.verso9}";
                    }
                    if (marcador.verso10 != 0)
                    {
                        numeros += $", {marcador.verso10}";
                    }
                }

                else
                {
                    numeros = "";
                    indiceMarcador = 0;
                }
                
            }
        }

        protected override async Task OnInitializedAsync()
        {
            Context = db.CreateDbContext(null);

            Model = new Pagina
            {
                Story = new Story(),

            };
            vers = null;
            question = 1;

            if (repositoryPagina!.buscarEcommerce() == "yes")
                conteudo = await Context.Pagina.Include(p => p.Story).Where(p => p!.Content != null).ToListAsync();

            var authState = await AuthenticationStateProvider
                    .GetAuthenticationStateAsync();
            user = authState.User;

            if (compartilhante == null)
                compartilhante = "admin";
            if (dominio == null)
                dominio = "dominio";
            if (dominio == null)
                dominio = repositoryPagina!.buscarDominio();

            if (auto == 0 && Timer!.desligarAuto! != null
                && Timer!.desligarAuto!.Enabled == true)
            {
                Timer!.desligarAuto!.Elapsed -= desligarAuto_Elapsed;
                Timer!.desligarAuto!.Enabled = false;
                Timer.desligarAuto.Dispose();
            }
            if (indice == 0)
                indice = 1;
            if (timeproduto == 0)
                timeproduto = 11;


            stories = await Context.Story!
                .Include(p => p.Filtro)!
               .ThenInclude(p => p.Pagina)!
               .ThenInclude(p => p.Pagina)
                .OrderBy(st => st.Id).ToListAsync();

            if (dominio != repositoryPagina.buscarDominio() && dominio != "dominio")
            {
                var domi = await Context.Compartilhante!.FirstOrDefaultAsync(l => l.Livro == dominio);
                if (domi == null)
                {
                    var compartilhant = new business.Compartilhante
                    {
                        Livro = dominio,
                        Admin = compartilhante,
                        Data = DateTime.Now,
                        Comissao = 5
                    };
                    await Context.AddAsync(compartilhant);
                    await Context.SaveChangesAsync();
                }
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

            ultimaPasta = 0;
            var quantidadeFiltros = 0;
            var quantidadePaginas = 0;
            List<Pagina> listaFiltradaComConteudo = null;
            List<Filtro> filtros = new List<Filtro>();
            Story story = null;

            Model = repositoryPagina.includes()
                    .FirstOrDefault(p => p.Versiculo == indice && p.Story.PaginaPadraoLink == capitulo);

            if (Model == null)
            {
                Model = repositoryPagina.includes()
                .FirstOrDefault(p => p.Story.PaginaPadraoLink == capitulo);
            }
            if (outroHorizonte == 1)
            {
                if (Model != null)
                {
                    Pagina fils = null;
                    if (story == null)
                    {
                        fils = Context.Pagina.Include(p => p.Story).ThenInclude(p => p.Filtro).First(p => p.Id == Model.Id);
                        quantidadeFiltros = fils.Story.Filtro.Count;
                        Model2 = fils.Story.Filtro.OrderBy(f => f.Id).Skip((int)indice - 1).FirstOrDefault();
                    }
                    else
                    {
                        quantidadeFiltros = story.Filtro.Count;
                        quantidadeLista = quantidadeFiltros;
                        Model2 = story.Filtro.OrderBy(f => f.Id).Skip((int)indice - 1).FirstOrDefault();

                    }
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
                story = stories!
               .First(p => p.Id == Model!.StoryId);
            }

            if (filtros.Count == 0 && story.Filtro!.Count > 0)
                filtros.AddRange(story.Filtro);

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

                    CapituloComentario = page!.Story!.PaginaPadraoLink;
                    VersoComentario = page.Versiculo;
                }

            }

            else if (filtrar != null && condicaoFiltro || substory != null && condicaoFiltro)
            {
                if (substory != null)
                {
                    listaFiltradaComConteudo = retornarListaFiltrada();

                    if (retroceder == 1)
                    {
                        indice = listaFiltradaComConteudo!.Count;
                    }


                }

                if (filtrar != null)
                {
                    lista = 1;

                    preferencia = 0;
                    int[] arr = null;
                    var indiceFiltro = int.Parse(filtrar.Replace("pasta-", ""));
                    var fi = story!.Filtro!.OrderBy(f => f.Id).ToList()[indiceFiltro - 1];


                    if (fi is CamadaDez)
                        arr = Arr.RetornarArray(filtros, story!, 3, (long)fi.Id, capitulo, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                    else if (fi is CamadaNove)
                        arr = Arr.RetornarArray(filtros, story!, 3, (long)fi.Id, capitulo, 1, 1, 1, 1, 1, 1, 1, 1);
                    else if (fi is CamadaOito)
                        arr = Arr.RetornarArray(filtros, story!, 3, (long)fi.Id, capitulo, 1, 1, 1, 1, 1, 1, 1);
                    else if (fi is CamadaSete)
                        arr = Arr.RetornarArray(filtros, story!, 3, (long)fi.Id, capitulo, 1, 1, 1, 1, 1, 1);
                    else if (fi is CamadaSeis)
                        arr = Arr.RetornarArray(filtros, story!, 3, (long)fi.Id, capitulo, 1, 1, 1, 1, 1);
                    else if (fi is SubSubGrupo)
                        arr = Arr.RetornarArray(filtros, story!, 3, (long)fi.Id, capitulo, 1, 1, 1, 1);
                    else if (fi is SubGrupo)
                        arr = Arr.RetornarArray(filtros, story!, 3, (long)fi.Id, capitulo, 1, 1, 1);
                    else if (fi is Grupo)
                        arr = Arr.RetornarArray(filtros, story!, 3, (long)fi.Id, capitulo, 1, 1);
                    else if (fi is SubStory)
                        arr = Arr.RetornarArray(filtros, story!, 3, (long)fi.Id, capitulo, 1);
                    indice = 1;
                    if (compartilhante == null) compartilhante = "admin";
                    auto = 0;
                    if (arr.Length == 10)
                    {
                        substory = arr[1];
                        grupo = arr[2];
                        subgrupo = arr[3];
                        subsubgrupo = arr[4];
                        camadaseis = arr[5];
                        camadasete = arr[6];
                        camadaoito = arr[7];
                        camadanove = arr[8];
                        camadadez = arr[9];
                        navigation!.NavigateTo($"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{question}");
                    }
                    else if (arr.Length == 9)
                    {
                        substory = arr[1];
                        grupo = arr[2];
                        subgrupo = arr[3];
                        subsubgrupo = arr[4];
                        camadaseis = arr[5];
                        camadasete = arr[6];
                        camadaoito = arr[7];
                        camadanove = arr[8];
                        navigation!.NavigateTo($"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{question}");
                    }
                    else if (arr.Length == 8)
                    {
                        substory = arr[1];
                        grupo = arr[2];
                        subgrupo = arr[3];
                        subsubgrupo = arr[4];
                        camadaseis = arr[5];
                        camadasete = arr[6];
                        camadaoito = arr[7];
                        navigation!.NavigateTo($"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{question}");
                    }
                    else if (arr.Length == 7)
                    {
                        substory = arr[1];
                        grupo = arr[2];
                        subgrupo = arr[3];
                        subsubgrupo = arr[4];
                        camadaseis = arr[5];
                        camadasete = arr[6];
                        navigation!.NavigateTo($"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{question}");
                    }
                    else if (arr.Length == 6)
                    {
                        substory = arr[1];
                        grupo = arr[2];
                        subgrupo = arr[3];
                        subsubgrupo = arr[4];
                        camadaseis = arr[5];
                        navigation!.NavigateTo($"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{question}");
                    }
                    else if (arr.Length == 5)
                    {
                        substory = arr[1];
                        grupo = arr[2];
                        subgrupo = arr[3];
                        subsubgrupo = arr[4];
                        navigation!.NavigateTo($"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{question}");
                    }
                    else if (arr.Length == 4)
                    {
                        substory = arr[1];
                        grupo = arr[2];
                        subgrupo = arr[3];
                        navigation!.NavigateTo($"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{question}");
                    }
                    else if (arr.Length == 3)
                    {
                        substory = arr[1];
                        grupo = arr[2];
                        navigation!.NavigateTo($"/grupo/{capitulo}/{substory}/{grupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{question}");
                    }
                    else if (arr.Length == 2)
                    {
                        substory = arr[1];
                        navigation!.NavigateTo($"/substory/{capitulo}/{substory}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{question}");
                    }
                }

            }

            ultimaPasta = Model.UltimaPasta;

            if (Model.ContentUser != null && Model.ContentUser.Contains("iframe") ||
                Model.Content != null && Model.Content.Contains("iframe"))
            {
                var conteudoHtml = "";
                if (Model.ContentUser != null) conteudoHtml = Model.ContentUser;
                else conteudoHtml = Model.Content;

                if (!conteudoHtml.Contains("?autoplay="))
                    colocarAutoPlay(Model);
            }
            if (Model!.Content != null || Model.ContentUser != null)
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

            PageLiked p = null;
            if (substory != null)
                p = Context.PageLiked.FirstOrDefault(p => p.capitulo == capitulo
                   && p.verso == vers
                   && p.user == user.Identity!.Name)!;
            else
                p = Context.PageLiked.FirstOrDefault(p => p.capitulo == capitulo
                    && p.verso == indice
                    && p.user == user.Identity!.Name)!;

            if (p != null)
                liked = true;

            quantLiked = CountLikes(ApplicationDbContext._connectionString);
            filtros.Clear();
        }

        private List<Pagina> retornarListaFiltrada()
        {
            List<Pagina> listaFiltradaComConteudo = null;
            if (outroHorizonte == 0 && substory != null)
            {
                Story story = stories!.First(p => p.Id == Model!.StoryId);

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
                if (preferencia == 0)
                {
                    var filtropag = story.Filtro!.First(f => f.Id == group.Id);
                    if (lista == 1 && Model.ContentUser == null)
                        listaFiltradaComConteudo = retornarListaComConteudo(filtropag.Pagina!.Select(p => p.Pagina).ToList()!, (int)substory!);
                    else
                    {
                        listaFiltradaComConteudo = filtropag.Pagina!.Select(p => p.Pagina).ToList()!;
                    }
                }

                if (grupo != null)
                {
                    var fil1 = (SubStory)story.Filtro.First(f => f.Id == group.Id);
                    group2 = fil1.Grupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).Skip((int)grupo! - 1).First();
                    nameGroup = group2!.Nome!;
                    if (preferencia == 0)
                    {
                        var filtropag = story.Filtro!.First(f => f.Id == group2.Id);
                        if (lista == 1 && Model.ContentUser == null)
                            listaFiltradaComConteudo = retornarListaComConteudo(filtropag.Pagina!.Select(p => p.Pagina).ToList()!, (int)grupo!);
                        else
                        {
                            listaFiltradaComConteudo = filtropag.Pagina!.Select(p => p.Pagina).ToList()!;
                        }
                    }

                }

                if (subgrupo != null)
                {
                    var fil2 = (Grupo)story.Filtro.First(f => f.Id == group2.Id);
                    group3 = fil2.SubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).Skip((int)subgrupo! - 1).First();
                    nameGroup = group3!.Nome!;
                    if (preferencia == 0)
                    {
                        var filtropag = story.Filtro!.First(f => f.Id == group3.Id);
                        if (lista == 1 && Model.ContentUser == null)
                            listaFiltradaComConteudo = retornarListaComConteudo(filtropag.Pagina!.Select(p => p.Pagina).ToList()!, (int)subgrupo!);
                        else
                        {
                            listaFiltradaComConteudo = filtropag.Pagina!.Select(p => p.Pagina).ToList()!;
                        }
                    }

                }

                if (subsubgrupo != null)
                {
                    var fil3 = (SubGrupo)story.Filtro.First(f => f.Id == group3.Id);
                    group4 = fil3.SubSubGrupo.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).Skip((int)subsubgrupo! - 1).First();
                    nameGroup = group4!.Nome!;
                    if (preferencia == 0)
                    {
                        var filtropag = story.Filtro!.First(f => f.Id == group4.Id);
                        if (lista == 1 && Model.ContentUser == null)
                            listaFiltradaComConteudo = retornarListaComConteudo(filtropag.Pagina!.Select(p => p.Pagina).ToList()!, (int)subsubgrupo!);
                        else
                        {
                            listaFiltradaComConteudo = filtropag.Pagina!.Select(p => p.Pagina).ToList()!;
                        }
                    }

                }

                if (camadaseis != null)
                {
                    var fil4 = (SubSubGrupo)story.Filtro.First(f => f.Id == group4.Id);
                    group5 = fil4.CamadaSeis.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).Skip((int)camadaseis! - 1).First();
                    nameGroup = group5!.Nome!;
                    if (preferencia == 0)
                    {
                        var filtropag = story.Filtro!.First(f => f.Id == group5.Id);
                        if (lista == 1 && Model.ContentUser == null)
                            listaFiltradaComConteudo = retornarListaComConteudo(filtropag.Pagina!.Select(p => p.Pagina).ToList()!, (int)camadaseis!);
                        else
                        {
                            listaFiltradaComConteudo = filtropag.Pagina!.Select(p => p.Pagina).ToList()!;
                        }
                    }

                }

                if (camadasete != null)
                {
                    var fil5 = (CamadaSeis)story.Filtro.First(f => f.Id == group5.Id);
                    group6 = fil5.CamadaSete.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).Skip((int)camadasete! - 1).First();
                    nameGroup = group6!.Nome!;
                    if (preferencia == 0)
                    {
                        var filtropag = story.Filtro!.First(f => f.Id == group6.Id);
                        if (lista == 1 && Model.ContentUser == null)
                            listaFiltradaComConteudo = retornarListaComConteudo(filtropag.Pagina!.Select(p => p.Pagina).ToList()!, (int)camadasete!);
                        else
                        {
                            listaFiltradaComConteudo = filtropag.Pagina!.Select(p => p.Pagina).ToList()!;
                        }
                    }

                }

                if (camadaoito != null)
                {
                    var fil6 = (CamadaSete)story.Filtro.First(f => f.Id == group6.Id);
                    group7 = fil6.CamadaOito.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).Skip((int)camadaoito! - 1).First();
                    nameGroup = group7!.Nome!;
                    if (preferencia == 0)
                    {
                        var filtropag = story.Filtro!.First(f => f.Id == group7.Id);
                        if (lista == 1 && Model.ContentUser == null)
                            listaFiltradaComConteudo = retornarListaComConteudo(filtropag.Pagina!.Select(p => p.Pagina).ToList()!, (int)camadaoito!);
                        else
                        {
                            listaFiltradaComConteudo = filtropag.Pagina!.Select(p => p.Pagina).ToList()!;
                        }
                    }

                }

                if (camadanove != null)
                {
                    var fil7 = (CamadaOito)story.Filtro.First(f => f.Id == group7.Id);
                    group8 = fil7.CamadaNove.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).Skip((int)camadanove! - 1).First();
                    nameGroup = group8!.Nome!;
                    if (preferencia == 0)
                    {
                        var filtropag = story.Filtro!.First(f => f.Id == group8.Id);
                        if (lista == 1 && Model.ContentUser == null)
                            listaFiltradaComConteudo = retornarListaComConteudo(filtropag.Pagina!.Select(p => p.Pagina).ToList()!, (int)camadanove!);
                        else
                        {
                            listaFiltradaComConteudo = filtropag.Pagina!.Select(p => p.Pagina).ToList()!;
                        }
                    }

                }

                if (camadadez != null)
                {
                    var fil8 = (CamadaNove)story.Filtro.First(f => f.Id == group8.Id);
                    group9 = fil8.CamadaDez.Where(str => str.Pagina != null && str.Pagina!.Count > 0).OrderBy(str => str.Id).Skip((int)camadadez! - 1).First();
                    nameGroup = group9!.Nome!;
                    if (preferencia == 0)
                    {
                        var filtropag = story.Filtro.First(f => f.Id == group.Id);
                        if (lista == 1 && Model.ContentUser == null)
                            listaFiltradaComConteudo = retornarListaComConteudo(filtropag.Pagina!.Select(p => p.Pagina).ToList()!, (int)camadadez!);
                        else
                        {
                            listaFiltradaComConteudo = filtropag.Pagina!.Select(p => p.Pagina).ToList()!;
                        }
                    }

                }

                if (preferencia == 1) listaFiltradaComConteudo = retornarListaPreferencial();

                Filtro Filtro = null;
                if (group9 != null)
                    Filtro = story!.Filtro!.First(f => f.Id == group9!.Id);
                if (group8 != null)
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

                Pagina pag2 = listaFiltradaComConteudo!.OrderBy(p => p.Id).Skip((int)indice - 1).FirstOrDefault();

                if (pag2 == null)
                {
                    navigation.NavigateTo($"/renderizar/{capitulo}/{indice_Filtro}/0/11/1/1/0/0/0/{dominio}/{compartilhante}");
                }

                vers = pag2.Versiculo;
                Model = repositoryPagina.includes()
                .FirstOrDefault(p => p.Versiculo == vers && p.Story.PaginaPadraoLink == capitulo);

                quantidadeLista = listaFiltradaComConteudo!.Count;
            }

            return listaFiltradaComConteudo;
        }

    }
}
