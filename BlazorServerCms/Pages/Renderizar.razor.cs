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
using PSC.Blazor.Components.Tours.Services;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Policy;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

namespace BlazorCms.Client.Pages
{
    public partial class RenderizarBase : ComponentBase
    {

        private List<Pagina> retornarListaComConteudo( List<Pagina> produtos, int grupo)
        {
            int pular = conteudo.Where(p => p.Story.PaginaPadraoLink == capitulo).ToList().Count / grupo;
            List<Pagina> conteudoPorGrupo = conteudo.Skip((grupo - 1) * pular).Take(pular).ToList();
            List<Pagina> listaComConteudo = new List<Pagina>();
            int interacao = 0;

            while (produtos.Skip(interacao * 2).ToList().Count >= 2)
            {
                listaComConteudo.AddRange(produtos.Skip(interacao * 2).Take(2).ToList());
                if (conteudoPorGrupo.Skip(interacao).FirstOrDefault() != null)
                    listaComConteudo.Add(conteudoPorGrupo.Skip(interacao).First());

                interacao++;
            }

            if (listaComConteudo.Count == 0) return produtos;
            if (!listaComConteudo.Contains(produtos.Last()))
                listaComConteudo.Add(produtos.Last());

            return listaComConteudo;
        }

        private List<Pagina> retornarListaPreferencial()
        {
            List<Pagina> retorno = new List<Pagina>();

            return retorno;
        }

        private async void StartTimer(Pagina p)
        {
            try
            {
                if (p != null && p.Content != null && p.Content.Contains("iframe") && outroHorizonte == 0 ||
                    p != null && p.ContentUser != null && p.ContentUser.Contains("iframe") && outroHorizonte == 0)
                {
                    var conteudoHtml = "";
                    if (p.Content != null) conteudoHtml = p.Content;
                    else conteudoHtml = p.ContentUser;
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
                    await js!.InvokeAsync<object>("PreencherProgressBar", tempoVideo);
                    Timer!.SetTimer(tempoVideo);
                }

                else
                {
                    if(Id == 0)
                    {
                    await js!.InvokeAsync<object>("PreencherProgressBar", timeproduto * 1000);
                    Timer!.SetTimer(timeproduto * 1000);

                    }
                    else
                    {
                        await js!.InvokeAsync<object>("PreencherProgressBar", 11000);
                        Timer!.SetTimer(11000);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Timer._timer!.Elapsed += _timer_Elapsed;
            Console.WriteLine("Timer Started.");
            if (Timer!.desligarAuto! == null || Timer!.desligarAuto!.Enabled == false)
            {
                Timer!.SetTimerAuto();
                Timer!.desligarAuto!.Elapsed += desligarAuto_Elapsed;
            }
        }
        
        private void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            var quant = 0;
            if (substory == null)
                quant = CountPaginas(ApplicationDbContext._connectionString);
            else
            {
                var lista = retornarListaFiltrada(null);
                quant = lista.Count;
            }
            if (auto == 1 && Id == 0)
            {
                if (substory == null)
                {
                    if (capitulo == 0 && indice >= quant)
                        navigation!.NavigateTo($"/Renderizar/{capitulo}/1/1/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                   
                    else if (capitulo != 0 && indice >= quant && outroHorizonte == 0)
                        navigation!.NavigateTo($"/Renderizar/{capitulo + 1}/1/1/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                    else if (capitulo != 0 && indice >= quant && outroHorizonte == 1)
                        navigation!.NavigateTo($"/Renderizar/{capitulo}/1/1/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                    else
                        navigation!.NavigateTo($"/Renderizar/{capitulo}/{indice + 1}/1/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                }
                else
                {
                    navegarSubgrupos(false);
                }
            }
            else if (auto == 1 && Id != 0)
            {
                buscarProximo();
            }


                Console.WriteLine("Timer Elapsed.");
            Timer!._timer!.Elapsed -= _timer_Elapsed;
        }

        protected void TeclaPressionada(KeyboardEventArgs args)
        {
            
            if (substory == null)
            {
                
                if (args.Key == "Enter" && capitulo == 0)
                {
                    navigation!.NavigateTo($"/Renderizar/{indice}/1/1/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                }
                else if (args.Key == "Enter")
                {
                    navigation!.NavigateTo($"/Renderizar/0/{capitulo}/1/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                }
            }
            else if (args.Key == "Enter")
            {
                navegarSubgrupos(true);
            }

        }

        protected async void Casinha()
        {
            auto = 0;
            if (compartilhante == null) compartilhante = "comp";
            if (dominio == null) dominio = repositoryPagina!.buscarDominio();
            navigation!.NavigateTo($"/info/{dominio}/{compartilhante}");
        }

        protected async void Pesquisar()
        {
            bool condicao = false;
            try
            {
                var n = int.Parse(opcional);
                auto = 0;
                condicao = true;

            }
            catch(Exception ex)
            {
                condicao = false;
            }
           
                if(substory == null && condicao)
                {
                    var url = $"/Renderizar/{capitulo}/{opcional}/{auto}/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}";
                    navigation!.NavigateTo(url);

                }
                else if (condicao)
                {

                redirecionarParaVerso(int.Parse(opcional));

                }
                   
        }

        protected void habilitarAuto()
        {
            Timer!.SetTimerAuto();
            Timer!.desligarAuto!.Elapsed += desligarAuto_Elapsed;

            var url = "";
            if(Id == 0)
            {
                if (camadadez != null)
                    url = $"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{indice}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                else if (camadanove != null)
                    url = $"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{indice}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                else if (camadaoito != null)
                    url = $"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{indice}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                else if (camadasete != null)
                    url = $"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{indice}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                else if (camadaseis != null)
                    url = $"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{indice}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                else if (subsubgrupo != null)
                    url = $"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{indice}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                else if (subgrupo != null)
                    url = $"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{indice}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                else if (grupo != null)
                    url = $"/grupo/{capitulo}/{substory}/{grupo}/{indice}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                else if (substory != null)
                    url = $"/substory/{capitulo}/{substory}/{indice}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                else
                url = $"/Renderizar/{capitulo}/{indice}/1/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}";

            }
            else
            {
                url = $"/marcador/{Id}/{indice}/{compartilhante}/1";
            }
            navigation!.NavigateTo(url);
        }

        protected void desabilitarAuto()
        {
            if(Timer!.desligarAuto != null)
            {
                Timer!.desligarAuto!.Elapsed -= desligarAuto_Elapsed;
                Timer!.desligarAuto!.Enabled = false;
                Timer.desligarAuto.Dispose();
                auto = 0;
            }            
        }
        
        private void desligarAuto_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (auto == 1)
                auto = 0;

            Console.WriteLine("Timer Elapsed auto.");
            Timer!.desligarAuto!.Elapsed -= desligarAuto_Elapsed;
            Timer.desligarAuto.Dispose();
            navigation!.NavigateTo("/");
        }

        private async Task<int> GetYouTubeVideo(string id_video)
        {
            int calculo = 0;
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "",
                ApplicationName = this.GetType().ToString()
            });
            var searchListRequest = youtubeService.Videos.List("snippet,contentDetails,statistics,status");
            searchListRequest.Id = id_video;
            var search = await searchListRequest.ExecuteAsync();
            var duration = search.Items[0].ContentDetails.Duration;
            if (duration.Contains("M"))
            {
                duration = duration.Replace("PT", "");
                var minutos = int.Parse(duration.Split('M')[0]);
                var segundos = int.Parse(duration.Replace(minutos.ToString() + "M", "").Replace("S", ""));
                var minutosEmMileSegundos = minutos * 60 * 1000;
                var segundosEmMileSegundos = segundos * 1000;
                calculo = minutosEmMileSegundos + segundosEmMileSegundos;
            }
            else
                calculo = int.Parse(duration.Replace("PT", "").Replace("S", "")) * 1000;


            return calculo;
        }        
       
        protected async void acessarPasta()
        {
            indice_Filtro = indice;

            if (Model2!.Pagina == null || Model2.Pagina.Count == 0)
            {
                await js!.InvokeAsync<object>("DarAlert", $"Esta pasta não possui versiculos");
                navigation!.NavigateTo($"/renderizar/{capitulo}/{indice}/{auto}/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
            }
            else
            {
                navigation!.NavigateTo($"/filtro/{capitulo}/pasta-{indice_Filtro}/{preferencia}/0/0/{dominio}/{compartilhante}");

            }

        }
        
        protected async void acessarPergunta()
        {
            indice_Filtro = indice;


            if (Model3!.UserResponse != null)
            {
                var dominio = "";
                if (Model3.UserResponse.Livro == null)
                    dominio = repositoryPagina.buscarDominio();
                else
                    dominio = Model3.UserResponse.Livro.url;

                var fils = repositoryPagina.filtros.Where(f => f.StoryId == Model3.Filtro!.StoryId).OrderBy(f => f.Id).ToList();
                var fil = repositoryPagina.filtros.First(f => f.StoryId == Model3.Filtro!.Id);
                var i = fils.IndexOf(fil) + 1;

                navigation!.NavigateTo($"{dominio}/lista-filtro/1/{compartilhante}/{capitulo}/{i}/{Model3.Id}");
            }
            else
            {
                navigation.NavigateTo($"/responderpergunta/{Model3.Id}");
            }

        }

        protected void listarPasta()
        {
            var lista = retornarListaFiltrada(null);
            int tamanho = 0;

            if (dominio == null) dominio = "dominio";

            if (lista.FirstOrDefault(p => p.ContentUser != null) != null)
                tamanho = 5;
            else
                tamanho = 20;

            if (auto == 1)
                desabilitarAuto();
            navigation!.NavigateTo($"/lista-filtro/1/{compartilhante}/{capitulo}/{indice_Filtro}/0");
        }

        protected void acessarHorizontePastas()
        {
            auto = 0;
            outroHorizonte = 1;
            navigation!.NavigateTo($"/renderizar/{capitulo}/1/{auto}/11/1/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
        }

        protected void acessarHorizonteVersos()
        {
            if (auto == 1)
                desabilitarAuto();
            outroHorizonte = 0;
            navigation!.NavigateTo($"/renderizar/{capitulo}/1/0/11/1/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
        }

        protected void acessarHorizontePerguntas()
        {
            if (auto == 1)
                desabilitarAuto();
            outroHorizonte = 2;
            navigation!.NavigateTo($"/renderizar/{capitulo}/1/0/11/1/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
        }

        protected void acessarHorizonteMarcadores()
        {
            if (auto == 1)
                desabilitarAuto();
            outroHorizonte = 3;
            navigation!.NavigateTo($"/renderizar/{capitulo}/1/0/11/1/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
        }

        protected int buscarCamada(Filtro fil)
        {
            if (fil is SubStory)
                return 1;
          else  if (fil is Grupo)
               return 2;
          else  if (fil is SubGrupo)
               return 3;
          else  if (fil is SubSubGrupo)
               return 4;
          else  if (fil is CamadaSeis)
               return 5;
          else  if (fil is CamadaSete)
               return 6;
          else  if (fil is CamadaOito)
                return 7;
          else  if (fil is CamadaNove)
                return 8;
          else  if (fil is CamadaDez)
                return 9;

            return 0;
        }    
   
        protected int? buscarPastaFiltrada(int camada)
        {
            long? IdGrupo = 0;
            Pagina pag = repositoryPagina.paginas
                    .FirstOrDefault(p => p.Filtro!.FirstOrDefault(f => f.FiltroId == Model2!.Id) != null)!;
           

            if (pag != null)
            {
                Filtro fil = pag.Filtro!.FirstOrDefault(f => f.FiltroId == Model2!.Id)!.Filtro!;

                if (camada != 0)
                {
                    Filtro fl = null;
                    var filtros = repositoryPagina.filtros
                        .Where(p => p.Story!.PaginaPadraoLink == capitulo).ToList();
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

        protected void buscarProximo()
        {
            var quant = 0;
            if (substory == null && outroHorizonte == 0)
                quant = CountPaginas(ApplicationDbContext._connectionString);
            else if (outroHorizonte == 1)
            {
                quant = QuantFiltros();
            }
            else
            {
                List<Pagina> lista = null;
                if (rotas != null)
                    lista = retornarListaFiltrada(rotas);
                else
                quant = quantidadeLista;
            }
            
            var proximo = indice + 1;
            if(Id != 0)
            {
                if (proximo > quantidadeLista)
                {
                    var indice = marcadores.IndexOf(Model4) + 1;
                    if (indice < marcadores.Count)
                        navigation.NavigateTo($"/marcador/{marcadores[indice].Id}/1/{compartilhante}/{auto}");
                    else
                        navigation.NavigateTo($"/marcador/{marcadores[0].Id}/1/{compartilhante}/{auto}");

                }
                else
                    navigation.NavigateTo($"/marcador/{Model4.Id}/{proximo}/{compartilhante}/{auto}");
            }
            else
            if(rotas != null)
            {
                
                 if (proximo <= quant)
                    navigation!.NavigateTo($"/substory/{capitulo}/{substory}/{proximo}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{rotas}");
                else
                    navigation!.NavigateTo($"/substory/{capitulo}/{substory}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{rotas}");
            }
            else
            if (proximo <= quant)
            {
                if (camadadez != null)
                    navigation!.NavigateTo($"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{proximo}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                else if (camadanove != null)
                    navigation!.NavigateTo($"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{proximo}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                else if (camadaoito != null)
                    navigation!.NavigateTo($"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{proximo}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                else if (camadasete != null)
                    navigation!.NavigateTo($"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{proximo}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                else if (camadaseis != null)
                    navigation!.NavigateTo($"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{proximo}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                else if (subsubgrupo != null)
                    navigation!.NavigateTo($"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{proximo}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                else if (subgrupo != null)
                    navigation!.NavigateTo($"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{proximo}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                else if (grupo != null)
                    navigation!.NavigateTo($"/grupo/{capitulo}/{substory}/{grupo}/{proximo}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                else if (substory != null)
                    navigation!.NavigateTo($"/substory/{capitulo}/{substory}/{proximo}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                else
                    navigation!.NavigateTo($"/Renderizar/{capitulo}/{proximo}/{auto}/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
            }
            else if(substory != null)
                navegarSubgrupos(false);
            else
                navigation!.NavigateTo($"/Renderizar/{capitulo + 1}/1/{auto}/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");


        }
      
        private void navegarSubgrupos(bool somenteSubgrupos)
        {
            var quant = 0;
                Story story = repositoryPagina!.stories!.First(p => p.Id == Model!.StoryId);
            if (substory == null)
                quant = CountPaginas(ApplicationDbContext._connectionString);
            else
            {
                var lista = retornarListaFiltrada(null);
                quant = lista.Count;
            }

            var proximo = indice + 1;
            if (somenteSubgrupos) auto = 0;
            if (camadadez != null)
            {
                if(indice >= quant || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray( story.Filtro, Model!.Story!, 1, 0, capitulo, (int)substory!,
                        grupo, subgrupo, subsubgrupo, camadaseis, camadasete, camadaoito, camadanove, camadadez);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/camadadez/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/{arr[5]}/{arr[6]}/{arr[7]}/{arr[8]}/{arr[9]}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                    else
                        navigation!.NavigateTo($"/camadanove/{capitulo}/1/1/1/1/1/1/1/1/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");

                }
                else
                {
                    navigation!
                    .NavigateTo($"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{proximo}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                }
            }
            else  if (camadanove != null)
            {
                if(indice >= quant || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray(story.Filtro, Model!.Story!, 1, 0, capitulo, (int)substory!,
                        grupo, subgrupo, subsubgrupo, camadaseis, camadasete, camadaoito, camadanove);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/camadanove/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/{arr[5]}/{arr[6]}/{arr[7]}/{arr[8]}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                    else
                        navigation!.NavigateTo($"/camadaoito/{capitulo}/1/1/1/1/1/1/1/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");

                }
                else
                {
                    navigation!
                   .NavigateTo($"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{proximo}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                }
            }
            else  if (camadaoito != null)
            {
                if(indice >= quant || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray(story.Filtro, Model!.Story!, 1, 0, capitulo, (int)substory!,
                        grupo, subgrupo, subsubgrupo, camadaseis, camadasete, camadaoito);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/camadaoito/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/{arr[5]}/{arr[6]}/{arr[7]}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                    else
                        navigation!.NavigateTo($"/camadasete/{capitulo}/1/1/1/1/1/1/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");

                }
                else
                {
                    navigation!
                  .NavigateTo($"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{proximo}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                }
            }
            else  if (camadasete != null )
            {
                if(indice >= quant || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray(story.Filtro, Model!.Story!, 1, 0, capitulo, (int)substory!,
                        grupo, subgrupo, subsubgrupo, camadaseis, camadasete);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/camadasete/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/{arr[5]}/{arr[6]}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                    else
                        navigation!.NavigateTo($"/camadaseis/{capitulo}/1/1/1/1/1/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");

                }
                else
                {
                    navigation!
                .NavigateTo($"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{proximo}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                }
            }
            else  if (camadaseis != null)
            {
                if(indice >= quant || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray(story.Filtro, Model!.Story!, 1, 0, capitulo, (int)substory!,
                        grupo, subgrupo, subsubgrupo, camadaseis);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/camadaseis/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/{arr[5]}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                    else
                        navigation!.NavigateTo($"/subsubgrupo/{capitulo}/1/1/1/1/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");

                }
                else
                {
                    navigation!
               .NavigateTo($"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{proximo}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                }
            }
            else  if (subsubgrupo != null)
            {
                if(indice >= quant || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray(story.Filtro, Model!.Story!, 1, 0, capitulo, (int)substory!,
                        grupo, subgrupo, subsubgrupo);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/subsubgrupo/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/{arr[4]}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                    else
                        navigation!.NavigateTo($"/subgrupo/{capitulo}/1/1/1/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");

                }
                else
                {
                    navigation!
              .NavigateTo($"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{proximo}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                }
            }
            else if (subgrupo != null)
            {
                if(indice >= quant || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray(story.Filtro, Model!.Story!, 1, 0, capitulo, (int)substory!,
                        grupo, subgrupo);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/subgrupo/{arr[0]}/{arr[1]}/{arr[2]}/{arr[3]}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                    else
                        navigation!.NavigateTo($"/grupo/{capitulo}/1/1/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");

                }
                else
                {
                    navigation!
            .NavigateTo($"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{proximo}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                }
            }
            else if (grupo != null)
            {
                if(indice >= quant || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray(story.Filtro, Model!.Story!, 1, 0, capitulo, (int)substory!, grupo);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/grupo/{arr[0]}/{arr[1]}/{arr[2]}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                    else
                        navigation!.NavigateTo($"/substory/{capitulo}/1/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");

                }
                else
                {
                    navigation!
           .NavigateTo($"/grupo/{capitulo}/{substory}/{grupo}/{proximo}/1/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                }
            }
            else  if (substory != null)
            {
                if (indice >= quant || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray(story.Filtro, Model!.Story!, 1, 0, capitulo, (int)substory);
                    if (arr != null)
                        navigation!
                     .NavigateTo($"/substory/{arr[0]}/{arr[1]}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                    else
                        navigation!.NavigateTo($"/renderizar/{capitulo}/1/{auto}/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");

                }
                else
                {
                    navigation!
             .NavigateTo($"/substory/{capitulo}/{substory}/{proximo}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                }
            }
        }

        protected void buscarAnterior()
        {
            auto = 0;
            if(Id != 0)
            {
                if (indice == 1)
                {
                    if (Model4.Id != marcadores.First().Id)
                    {

                        var indice = marcadores.IndexOf(Model4) - 1;
                        quantidadeLista = marcadores[indice].Pagina.Count + marcadores[indice].Content.Count;
                        navigation.NavigateTo($"/marcador/{marcadores[indice].Id}/{quantidadeLista}/{compartilhante}/{auto}");
                    }
                    else
                    {
                        quantidadeLista = marcadores[marcadores.Count - 1].Pagina.Count + marcadores[marcadores.Count - 1].Content.Count;
                        navigation.NavigateTo($"/marcador/{marcadores[marcadores.Count - 1].Id}/{quantidadeLista}/{compartilhante}/{auto}");

                    }

                }
                else
                {
                    var anterior = indice - 1;
                    navigation.NavigateTo($"/marcador/{Model4.Id}/{anterior}/{compartilhante}/{auto}");

                }
            }
            else
            if(rotas != null)
            {
                if (indice == 1)
                    navigation!.NavigateTo($"/substory/{capitulo}/{substory}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{rotas}");
                else
                    navigation!.NavigateTo($"/substory/{capitulo}/{substory}/{indice - 1}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}/{rotas}");
            }
            else
            if (indice == 1 && capitulo != 0)
            {
                if(substory != null)
                {
                    voltarSubgrupos();
                }

                if (camadadez != null)
                navigation!.NavigateTo($"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/1/{dominio}/{compartilhante}");
                   
                else if (camadanove != null)
                navigation!.NavigateTo($"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/1/{dominio}/{compartilhante}");
                   
                else if (camadaoito != null)
                navigation!.NavigateTo($"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/1/{dominio}/{compartilhante}");
                    
                else if (camadasete != null)
                navigation!.NavigateTo($"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/1/{dominio}/{compartilhante}");
                    
                else if (camadaseis != null)
                navigation!.NavigateTo($"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/1/{dominio}/{compartilhante}");
                   
                else if (subsubgrupo != null)
                navigation!.NavigateTo($"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/1/{dominio}/{compartilhante}");
                    
                else if (subgrupo != null)                
                    navigation!.NavigateTo($"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/1/{dominio}/{compartilhante}");    
                
                else if (grupo != null)
                navigation!.NavigateTo($"/grupo/{capitulo}/{substory}/{grupo}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/1/{dominio}/{compartilhante}");                    
                
                else if (substory != null)
                navigation!.NavigateTo($"/substory/{capitulo}/{substory}/1/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/1/{dominio}/{compartilhante}");
                else
                    navigation!.NavigateTo($"/Renderizar/{capitulo - 1}/1/{auto}/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/1/{dominio}/{compartilhante}");

            }
            if (indice != 1 && rotas == null && Id == 0)
            {
                var anterior = indice - 1;
                if (camadadez != null)
                    navigation!.NavigateTo($"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{anterior}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                else if (camadanove != null)
                    navigation!.NavigateTo($"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{anterior}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                else if (camadaoito != null)
                    navigation!.NavigateTo($"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{anterior}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                else if (camadasete != null)
                    navigation!.NavigateTo($"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{anterior}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                else if (camadaseis != null)
                    navigation!.NavigateTo($"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{anterior}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                else if (subsubgrupo != null)
                    navigation!.NavigateTo($"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{anterior}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                else if (subgrupo != null)
                    navigation!.NavigateTo($"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{anterior}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                else if (grupo != null)
                    navigation!.NavigateTo($"/grupo/{capitulo}/{substory}/{grupo}/{anterior}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                else if (substory != null)
                    navigation!.NavigateTo($"/substory/{capitulo}/{substory}/{anterior}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
                else
                    navigation!.NavigateTo($"/Renderizar/{capitulo}/{anterior}/{auto}/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/0/{dominio}/{compartilhante}");
            }
            

        }
      
        private void voltarSubgrupos()
        {
            if (camadadez != null && camadadez != 1)
                camadadez = 1;
            else if (camadanove != null && camadanove != 1)
                camadanove = 1;
            else if (camadaoito != null && camadaoito != 1)
                camadaoito = 1;
            else if (camadasete != null && camadasete != 1)
                camadasete = 1;
            else if (camadaseis != null && camadaseis != 1)
                camadaseis = 1;
            else if (subsubgrupo != null && subsubgrupo != 1)
                subsubgrupo = 1;
            else if (subgrupo != null && subgrupo != 1)
                subgrupo = 1;
            else if (grupo != null && grupo != 1)
                grupo = 1;
            else if (substory != null && substory != 1)
                substory = 1;
        }
    
        protected void desligarAuto()
        {
            if (auto == 1)
                desabilitarAuto();
        }

        protected async Task DarUmLike()
        {
            PageLiked pageLiked = new PageLiked
            {
                user = user.Identity!.Name!,
                 capitulo = capitulo,
                 indice = indice,
                 substory = (int) substory!,
                 grupo = grupo,
                 subgrupo = subgrupo,
                 subsubgrupo = subsubgrupo,
                 camadaSeis = camadaseis,
                 camadaSete = camadasete,
                 camadaOito = camadaoito,
                 camadaNove = camadanove,
                 camadaDez = camadadez,
                 verso =(int)vers!
            };
             await Context.AddAsync(pageLiked);
            await Context.SaveChangesAsync();
            repositoryPagina.paginasCurtidas.Add(pageLiked);

            var url = "";
            if (camadadez != null)
                url = $"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
            else if (camadanove != null)
                url = $"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
            else if (camadaoito != null)
                url = $"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
            else if (camadasete != null)
                url = $"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
            else if (camadaseis != null)
                url = $"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
            else if (subsubgrupo != null)
                url = $"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
            else if (subgrupo != null)
                url = $"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
            else if (grupo != null)
                url = $"/grupo/{capitulo}/{substory}/{grupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
            else if (substory != null)
                url = $"/substory/{capitulo}/{substory}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
            navigation!.NavigateTo(url);
        }
        
        protected async Task Unlike()
        {
            PageLiked? page = await Context.PageLiked
                            .FirstOrDefaultAsync(p => p.capitulo == capitulo
                            && p.verso == vers
                            && p.user == user.Identity!.Name);
            if(page != null)
            {
                Context.Remove(page);
                await Context.SaveChangesAsync();
                repositoryPagina.paginasCurtidas.Remove(page);
            }
            var url = "";
            if (camadadez != null)
                url = $"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
            else if (camadanove != null)
                url = $"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
            else if (camadaoito != null)
                url = $"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
            else if (camadasete != null)
                url = $"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
            else if (camadaseis != null)
                url = $"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
            else if (subsubgrupo != null)
                url = $"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
            else if (subgrupo != null)
                url = $"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
            else if (grupo != null)
                url = $"/grupo/{capitulo}/{substory}/{grupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
            else if (substory != null)
                url = $"/substory/{capitulo}/{substory}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
            navigation!.NavigateTo(url);
        }
             
        protected async void acessarPreferenciasUsuario(string usu)
        {
            var pref = Context.UserResponse
                   .FirstOrDefault(u => u.user == usu)!;
            usuarios!.Clear();
            var url = "";
            if (camadadez != null)
                url = $"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{usu}";
            else if (camadanove != null)
                url = $"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{usu}";
            else if (camadaoito != null)
                url = $"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{usu}";
            else if (camadasete != null)
                url = $"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{usu}";
            else if (camadaseis != null)
                url = $"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{usu}";
            else if (subsubgrupo != null)
                url = $"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{usu}";
            else if (subgrupo != null)
                url = $"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{usu}";
            else if (grupo != null)
                url = $"/grupo/{capitulo}/{substory}/{grupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{usu}";
            else if (substory != null)
                url = $"/substory/{capitulo}/{substory}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{usu}";
            else
                url = $"/renderizar/{capitulo}/{indice}/{auto}/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{usu}";
                navigation!.NavigateTo(url);
        }

        protected void alterarQuery(ChangeEventArgs e)
        {
            opcional = e.Value!.ToString()!;

            try
            {
                var num = int.Parse(opcional);
            }
            catch(Exception ex)
            {
                    foreach (var item in userManager.Users)
                    usuarios.Add(new UserPreferencesImage { user = item.UserName, UserModel = item });

                if (string.IsNullOrEmpty(opcional))
                {
                    usuarios.Clear();
                    var url = "";
                    if (camadadez != null)
                        url = $"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                    else if (camadanove != null)
                        url = $"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                    else if (camadaoito != null)
                        url = $"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                    else if (camadasete != null)
                        url = $"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                    else if (camadaseis != null)
                        url = $"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                    else if (subsubgrupo != null)
                        url = $"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                    else if (subgrupo != null)
                        url = $"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                    else if (grupo != null)
                        url = $"/grupo/{capitulo}/{substory}/{grupo}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                    else if (substory != null)
                        url = $"/substory/{capitulo}/{substory}/{indice}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                    else
                        url = $"/renderizar/{capitulo}/{indice}/{auto}/{timeproduto}/{lista}/{outroHorizonte}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                    navigation!.NavigateTo(url);
                }
            }
        }

       protected void acessarVerso()
        {
            indice_Filtro = 0;
            navigation!.NavigateTo($"/Renderizar/{capitulo}/{vers}/0/{timeproduto}/{lista}/0/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}");
            
        }
        
      protected void redirecionarMarcar()
        {
            if (substory == null)
                opcional = indice.ToString();
            else
            {
                 var lista = retornarListaFiltrada(null);
                opcional = vers.ToString();
            }
            substory = 1;
            navigation!.NavigateTo($"/filtro/{capitulo}/pasta-{ultimaPasta}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}");
            
            
        }
              

        private async void redirecionarParaVerso(int verso)
        {
            if(outroHorizonte == 0)
            {
                var list = retornarListaFiltrada(null);
                if (list == null)
                {
                    navigation!.NavigateTo($"/renderizar/{capitulo}/{verso}/{auto}/{timeproduto}/{lista}/0/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}");
                    return;
                }
                opcional = verso.ToString();
                int indiceListaFiltrada = 0;
                foreach (var item in list)
                {
                    var p = repositoryPagina!.paginas!.First(p => p.Id == item.Id);
                    if (int.Parse(opcional) == p.Versiculo)
                    {
                        indiceListaFiltrada = list.IndexOf(item) + 1;
                        break;
                    }
                }

                if (indiceListaFiltrada == 0)
                {
                    indiceListaFiltrada = indice;
                    await js!.InvokeAsync<object>("DarAlert", $"Não foi encontrado o versiculo {verso} na pasta {indice_Filtro}");
                }
                else
                {
                    var url = "";
                    if (camadadez != null)
                        url = $"/camadadez/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{indiceListaFiltrada}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                    else if (camadanove != null)
                        url = $"/camadanove/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{indiceListaFiltrada}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                    else if (camadaoito != null)
                        url = $"/camadaoito/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{indiceListaFiltrada}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                    else if (camadasete != null)
                        url = $"/camadasete/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{indiceListaFiltrada}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                    else if (camadaseis != null)
                        url = $"/camadaseis/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{indiceListaFiltrada}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                    else if (subsubgrupo != null)
                        url = $"/subsubgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{indiceListaFiltrada}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                    else if (subgrupo != null)
                        url = $"/subgrupo/{capitulo}/{substory}/{grupo}/{subgrupo}/{indiceListaFiltrada}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                    else if (grupo != null)
                        url = $"/grupo/{capitulo}/{substory}/{grupo}/{indiceListaFiltrada}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                    else if (substory != null)
                        url = $"/substory/{capitulo}/{substory}/{indiceListaFiltrada}/{auto}/{timeproduto}/{lista}/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}";
                    navigation!.NavigateTo(url);

            }
            }
            else
            {
                navigation!.NavigateTo($"/renderizar/{capitulo}/{verso}/{auto}/{timeproduto}/{lista}/1/{preferencia}/{indiceLivro}/{retroceder}/{dominio}/{compartilhante}");
            }
        
        }

        private int CountLikes( string conexao)
        {

            var _TotalRegistros = 0;
            try
            {
                using (var con = new SqlConnection(conexao))
                {
                    SqlCommand cmd = null;
                    if(substory == null)
                     cmd = new SqlCommand($"SELECT COUNT(*) FROM PageLiked as P  where P.capitulo={capitulo} and P.verso={indice}", con);
                    else
                     cmd = new SqlCommand($"SELECT COUNT(*) FROM PageLiked as P  where P.capitulo={capitulo} and P.verso={vers}", con);
                    
                    con.Open();
                    _TotalRegistros = int.Parse(cmd.ExecuteScalar().ToString());
                    con.Close();
                }
            }
            catch (Exception)
            {
                _TotalRegistros = 0;
            }
            return _TotalRegistros;
        }

        private bool CountFiltros(string conexao)
        {

            var _TotalRegistros = 0;
            try
            {
                using (var con = new SqlConnection(conexao))
                {
                    SqlCommand cmd = null;
                    cmd = new SqlCommand($"SELECT COUNT(*) FROM Filtro as P  where P.StoryId={Model.StoryId}", con);

                    con.Open();
                    _TotalRegistros = int.Parse(cmd.ExecuteScalar().ToString());
                    con.Close();
                }
            }
            catch (Exception)
            {
                _TotalRegistros = 0;
            }
            if (_TotalRegistros > 0)
                return true;
            else
                return false;
        }

        private int QuantFiltros()
        {
            if(story == null)            
                story = repositoryPagina.stories!
               .First(p => p.Id == Model!.StoryId);
            
                return story.Filtro.Count;
            
        }

        private int CountPaginas(string conexao)
        {

            var _TotalRegistros = 0;
            try
            {
                using (var con = new SqlConnection(conexao))
                {
                    SqlCommand cmd = null;
                    if(Model != null)
                    cmd = new SqlCommand($"SELECT COUNT(*) FROM Pagina as P  where P.StoryId={Model.StoryId} ", con);
                    else
                     _TotalRegistros = 0;


                    con.Open();
                    _TotalRegistros = int.Parse(cmd.ExecuteScalar().ToString());
                    con.Close();
                }
            }
            catch (Exception)
            {
                _TotalRegistros = 0;
            }

                return _TotalRegistros;
        }

        private void colocarAutoPlay(Pagina model)
        {
            var conteudoHtml = "";
            if (model.Content != null) conteudoHtml = model.Content;
            else conteudoHtml = model.ContentUser;
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
            if (Model.Content != null)
            {
                Model.Content = Model.Content.Replace(id_video, id_video + "?autoplay=1");
                Model.Content = Model.Content.Replace("<iframe", "<iframe" + " allow='accelerometer; autoplay; clipboard-write; encrypted-media;' ");

            }
            else if (Model.ContentUser != null) 
            {
                Model.ContentUser = Model.ContentUser.Replace(id_video, id_video + "?autoplay=1");
                Model.ContentUser = Model.ContentUser.Replace("<iframe", "<iframe" + " allow='accelerometer; autoplay; clipboard-write; encrypted-media;' ");
            }
        }

        protected async void StartTour()
        {
            desabilitarAuto();
            if(substory == null)
            await TourService.StartTour("FormGuidedTour1");
            else
            await TourService.StartTour("FormGuidedTour2");
        } 
        
    }

    public class UserPreferencesImage
    {
        public string? user { get; set; }
        public UserModel UserModel { get; set; }
    }
}
