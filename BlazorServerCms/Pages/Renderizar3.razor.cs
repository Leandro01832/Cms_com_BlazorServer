using BlazorServerCms.Data;
using BlazorServerCms.Pages;
using BlazorServerCms.servicos;
using business;
using business.business;
using business.Group;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Humanizer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Models;
using NuGet.Packaging;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Policy;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BlazorCms.Client.Pages
{
    public partial class RenderizarBase : ComponentBase
    {

        protected override async Task OnParametersSetAsync()
        {

            if (capitulo > repositoryPagina!.stories!.Last().PaginaPadraoLink)
                capitulo = 1;

            await renderizar();

            if (auto == 1)
                StartTimer(Model!);

        }
        
        protected override async Task OnInitializedAsync()
        {
            Context = db.CreateDbContext(null);

            Model = new Pagina
            {
                Story = new Story(),

            };
            vers = null;
            auto = 1;

            var authState = await AuthenticationStateProvider
                    .GetAuthenticationStateAsync();
            user = authState.User;


            if (compartilhante != "comp" && pontos != null)
            {
               var us = Context.Users.FirstOrDefault(u => u.UserName == compartilhante);

                if (us != null)
                {
                    if(DateTime.Now.Date > us.DataPontuacao.Date)
                    {
                        if (us.PontosPorDia > us.Recorde)
                        {
                            us.Recorde = us.PontosPorDia;
                            Context.Update(us);
                            Context.SaveChanges();
                        }
                        us.PontosPorDia = 1;
                        us.DataPontuacao = DateTime.Now;
                        Context.Update(us);
                        Context.SaveChanges();
                    }
                    else
                    {
                        us.PontosPorDia++;
                        Context.Update(us);
                        Context.SaveChanges();
                    }
                    
                }

                pontos = null;
            }

            if (compartilhante == null)
                compartilhante = "comp";
            if (compartilhante2 == null)
                compartilhante2 = "comp";
            if (dominio == null)
                dominio = "dominio";

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

            if(repositoryPagina.stories.Count == 0)
            {
                var str = await Context.Story!
                    .Include(p => p.Filtro)!
                   .ThenInclude(p => p.Pagina)!
                   .ThenInclude(p => p.Pagina)
                    .OrderBy(st => st.Id).ToListAsync();
                repositoryPagina.stories.AddRange(str);

            }
                       
            if(repositoryPagina.conteudos.Count == 0)
            {
                var str = await Context.Content!
                    .Include(p => p.Filtro)!
                   .ThenInclude(p => p.Pagina)!
                   .ThenInclude(p => p.Pagina)
                    .OrderBy(st => st.Id).ToListAsync();
            }

            if (repositoryPagina.paginas.Count == 0)
            {
                var pergs = await Context!.Pagina!.Include(p => p.Story).Include(p => p.Filtro).ToListAsync();
                repositoryPagina.paginas.AddRange(pergs);
            }

            if (repositoryPagina.perguntas.Count == 0)
            {
                var pergs = await Context.Pergunta
                    .Include(p => p.UserResponse)
                    .ThenInclude(p => p.Livro)
                    .Include(p => p.Filtro)
                    .ThenInclude(p => p.Story)
                    .ToListAsync();
                repositoryPagina.perguntas.AddRange(pergs);
            }
            
            if(repositoryPagina.paginasCurtidas.Count == 0)
            {
                var pergs = await Context.PageLiked.ToListAsync();
                repositoryPagina.paginasCurtidas.AddRange(pergs);
            }
            
            if(repositoryPagina.filtros.Count == 0)
            {
                var pergs = await Context.Filtro!.Include(f => f.Story).ToListAsync();
                repositoryPagina.filtros.AddRange(pergs);
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
            List<Pagina> listaFiltradaComConteudo = null;
          //  List<Filtro> filtros = new List<Filtro>();

            Model = repositoryPagina!.paginas
                    .FirstOrDefault(p => p.Versiculo == indice && p.Story!.PaginaPadraoLink == capitulo);

            

            cap = capitulo;

            if (Model == null)
            {
                Model = repositoryPagina.paginas
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
            else if (outroHorizonte == 2)
            {
                var pergs = repositoryPagina.perguntas.Where(p => p.Filtro.Story.PaginaPadraoLink == capitulo).OrderBy(p => p.Id).ToList();
                        Model3 = pergs.Skip((int)indice - 1).FirstOrDefault();
                        quantidadeLista = pergs.Count;

            }
            else if (outroHorizonte == 3)
            {
                marcadores = repositoryPagina.filtros.Where(p => p.user == compartilhante && p.Story.PaginaPadraoLink == capitulo).OrderBy(p => p.Id).ToList();
                        Model4 = marcadores.Skip((int)indice - 1).FirstOrDefault();
                        quantidadeLista = marcadores.Count;

                indiceAcesso = story.Filtro.IndexOf(Model4) + 1;

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

                    CapituloComentario = page!.Story!.PaginaPadraoLink;
                    VersoComentario = page.Versiculo;
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

                    if (retroceder == 1)
                    {
                        indice = listaFiltradaComConteudo!.Count;
                    }


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
                    auto = 0;

                    setarCamadas(arr);
                    acessar();
                }

            }

            // ultimaPasta = Model.UltimaPasta;
            



            if ( Model.Content.Contains("iframe"))
            {
                var conteudoHtml = "";

                if(Content)
                 conteudoHtml = pag.Content;
                else
                conteudoHtml = Model.Content;

                if (!conteudoHtml.Contains("?autoplay="))
                    conteudoHtml =  colocarAutoPlay(conteudoHtml);

                if (Content)
                    pag.Content = conteudoHtml;
                else
                    Model.Content = conteudoHtml;
            }
            if (!Content && Model.Content != null)
            {
                try
                {
                    if (Content)
                        html = await repositoryPagina!.renderizarPagina(pag);
                    else
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

            try
            {
                if (substory != null)
                    p = repositoryPagina.paginasCurtidas.FirstOrDefault(p => p.capitulo == capitulo
                       && p.verso == vers
                       && p.user == user.Identity!.Name)!;
                else
                    p = repositoryPagina.paginasCurtidas.FirstOrDefault(p => p.capitulo == capitulo
                        && p.verso == indice
                        && p.user == user.Identity!.Name)!;

            }
            catch (Exception)
            {
                liked = false;
            }

            if (p != null)
                liked = true;

            quantLiked = CountLikes(ApplicationDbContext._connectionString);
        }

        private void setarCamadas(int[] arr)
        {
            retroceder = 0;
            if (arr.Length == 10)
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
                camadadez = null;
            }
            else if (arr.Length == 8)
            {
                substory =   arr[1];
                grupo =  arr[2];
                subgrupo =  arr[3];
                subsubgrupo =  arr[4];
                camadaseis =  arr[5];
                camadasete =  arr[6];
                camadaoito =  arr[7];   
                camadanove = null;
                camadadez = null;
            }
            else if (arr.Length == 7)
            {
                substory =  arr[1];
                grupo =  arr[2];
                subgrupo =  arr[3];
                subsubgrupo =  arr[4];
                camadaseis =  arr[5];
                camadasete =  arr[6];
                camadaoito = null;
                    camadanove = null;
                    camadadez = null;

            }
            else if (arr.Length == 6)
            {
                substory =  arr[1];
                grupo =  arr[2];
                subgrupo =  arr[3];
                subsubgrupo =  arr[4];
                camadaseis =  arr[5];  
                camadasete = null;
                camadaoito = null;  
                camadanove = null;
                camadadez = null;
            }
            else if (arr.Length == 5)
            {
                substory =  arr[1];
                grupo =  arr[2];
                subgrupo =  arr[3];
                subsubgrupo =  arr[4];  
                camadaseis = null;
                camadasete = null;
                camadaoito = null;
                camadanove = null;
                camadadez = null;
            }
            else if (arr.Length == 4)
            {
                substory =  arr[1];
                grupo =  arr[2];
                subgrupo =  arr[3];  
                subsubgrupo = null;
                camadaseis = null;
                camadasete = null;
                camadaoito = null;
                camadanove = null;
                camadadez = null;
            }
            else if (arr.Length == 3)
            {
                substory =  arr[1];
                grupo =  arr[2];
                subgrupo = null;
                subsubgrupo = null;
                camadaseis = null;
                camadasete = null;
                camadaoito = null;
                camadanove = null;
                camadadez = null;
            }
            else if (arr.Length == 2)
            {
                substory =  arr[1];
                grupo = null;
                subgrupo = null;
                subsubgrupo = null;
                camadaseis = null;
                camadasete = null;
                camadaoito = null;
                camadanove = null;
                camadadez = null;
            }
            else if (arr == null)
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

        private int[] retornarArray(Filtro fi)
        {
            int[] arr = null;
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

        private List<Pagina> retornarListaFiltrada(string rota)
        {
            List<Pagina> listaFiltradaComConteudo = null;

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
                        listaFiltradaComConteudo = filtropa.Pagina!.Select(p => p.Pagina).ToList()!;
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
                            listaFiltradaComConteudo = filtropag.Pagina!.Select(p => p.Pagina).ToList()!;
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
                            listaFiltradaComConteudo = filtropag.Pagina!.Select(p => p.Pagina).ToList()!;
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
                            listaFiltradaComConteudo = filtropag.Pagina!.Select(p => p.Pagina).ToList()!;
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
                            listaFiltradaComConteudo = filtropag.Pagina!.Select(p => p.Pagina).ToList()!;
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
                            listaFiltradaComConteudo = filtropag.Pagina!.Select(p => p.Pagina).ToList()!;
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
                            listaFiltradaComConteudo = filtropag.Pagina!.Select(p => p.Pagina).ToList()!;
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
                            listaFiltradaComConteudo = filtropag.Pagina!.Select(p => p.Pagina).ToList()!;
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
                            listaFiltradaComConteudo = filtropag.Pagina!.Select(p => p.Pagina).ToList()!;
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
                    compartilhante = Model2.user;
                }  
                else if(Model2.user == null)
                    compartilhante = "comp";

                if (Content)
                    pag = listaFiltradaComConteudo!.OrderBy(p => p.Id).Skip((int)indice - 1).FirstOrDefault();
                else
                    pag = listaFiltradaComConteudo!.OrderBy(p => p.Id).Skip((int)indice - 1).FirstOrDefault();

                if (pag == null)                
                pag = repositoryPagina.paginas!.Where(p => p.Story.PaginaPadraoLink == capitulo)
                    .OrderBy(p => p.Id).Skip((int)indice - 1).FirstOrDefault();
                

                vers = pag.Versiculo;
                Model = repositoryPagina.includes()
                .FirstOrDefault(p => p.Versiculo == vers && p.Story.PaginaPadraoLink == capitulo);

                ultimaPasta = Model2.Id == story.Filtro
                    .Where(f => f.Pagina
                    .FirstOrDefault(p => p.Pagina!.Versiculo == Model.Versiculo) != null)
                        .LastOrDefault()!.Id;

                quantidadeLista = listaFiltradaComConteudo!.Count;
            }
            else if (outroHorizonte == 0 && substory != null && rota != null)
            {
                listaFiltradaComConteudo = new List<Pagina>();
                foreach (var item in story.Filtro)
                foreach(var item2 in item.Pagina)
                {
                        var rotas = item2.Pagina.Rotas.Split(",");
                        foreach (var rot in rotas)
                            if (rot.ToLower().TrimEnd().TrimStart() == rota.ToLower().TrimEnd().TrimStart())                             
                             if (!listaFiltradaComConteudo.Contains(item2.Pagina))                        
                            listaFiltradaComConteudo.Add(item2.Pagina);
                        
                }

                Pagina pag2 = listaFiltradaComConteudo!.OrderBy(p => p.Id).Skip((int)indice - 1).FirstOrDefault();

                var str = repositoryPagina.stories.First(st => st.Id == pag2.StoryId);
                cap = repositoryPagina.stories.IndexOf(str);

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

        private List<Pagina> listarConteudos(Filtro f)
        {
            List<Pagina> conteudos = new List<Pagina>();
            foreach (var p in repositoryPagina.conteudos.Where(p => p.FiltroId == f.Id).ToList()!)
                conteudos.Add(new Pagina { Content = p.Html });
            return conteudos;
        }

        


    }
}
