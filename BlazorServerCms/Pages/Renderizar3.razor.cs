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
using NVelocity.Runtime.Directive;
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

            if (Auto == 1)
                StartTimer(Model!);

            if (compartilhante != "comp" && pontos != null)
            {
                adicionarPontos(compartilhante);
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
            Auto = 1;

            var authState = await AuthenticationStateProvider
                    .GetAuthenticationStateAsync();
            user = authState.User;           

            if (compartilhante == null)
            {
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
                        
            if(repositoryPagina.paginasCurtidas.Count == 0)
            {
                var pergs = await Context.PageLiked.ToListAsync();
                repositoryPagina.paginasCurtidas.AddRange(pergs);
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

            }
            else if (outroHorizonte == 3)
            {
                marcadores = story.Filtro.Where(p => p.user == compartilhante ).OrderBy(p => p.Id).ToList();
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
                    var fil = verificarFiltros(Model2);
                    Filtro fil2 = null;
                    Filtro fil3 = null;
                    Filtro fil4 = null;
                    Filtro fil5 = null;

                    if (fil.user != null)
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

        private void adicionarPontos(string username)
        {
            int pts = 0;
            int multiplicador = 10;
            var us = Context.Users.FirstOrDefault(u => u.UserName == username);
            var us2 = Context.Users.FirstOrDefault(u => u.UserName == compartilhante2);
            var us3 = Context.Users.FirstOrDefault(u => u.UserName == compartilhante3);
            var us4 = Context.Users.FirstOrDefault(u => u.UserName == compartilhante4);
            var us5 = Context.Users.FirstOrDefault(u => u.UserName == compartilhante5);
            var us6 = Context.Users.FirstOrDefault(u => u.UserName == compartilhante6);
            UserModel[] usuarios = new UserModel[6];

            if (us != null) { pts  = 1; usuarios[0] = us;  } else usuarios[0] = null;
            if (us2 != null){ pts =  2; usuarios[1] = us2; } else usuarios[1] = null;
            if (us3 != null){ pts =  3; usuarios[2] = us3; } else usuarios[2] = null;
            if (us4 != null){ pts =  4; usuarios[3] = us4; } else usuarios[3] = null;
            if (us5 != null){ pts =  5; usuarios[4] = us5; } else usuarios[4] = null;
            if (us6 != null){ pts =  6; usuarios[5] = us6; } else usuarios[5] = null;

            if (story.Filtro.Count() > repositoryPagina.meta1) pts += 1;
            if (story.Filtro.Count() > repositoryPagina.meta2) pts += 1;
            if (story.Filtro.Count() > repositoryPagina.meta3) pts += 1;
            if (story.Filtro.Count() > repositoryPagina.meta4) pts += 1;
            if (story.Filtro.Count() > repositoryPagina.meta5) pts += 1;
            if (story.Filtro.Count() > repositoryPagina.meta6) pts += 1;
            if (story.Filtro.Count() > repositoryPagina.meta7) pts += 1;
            if (story.Filtro.Count() > repositoryPagina.meta8) pts += 1;

            for (var i = 0; i < 6; i++)
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
                        var quantFiltros = Context.Filtro
                            .Where(f => f.user == usuarios[i].UserName).ToList().Count;

                        multiplicador -= quantFiltros;

                        if (multiplicador < 1) multiplicador = 1;

                        var pontosGanhos = multiplicador * (pts - i);

                        usuarios[i].PontosPorDia += pontosGanhos;
                        Context.Update(usuarios[i]);
                        Context.SaveChanges();
                    }

                }
            }
            pontos = null;
        }
    
    }
}
