using BlazorServerCms.Data;
using business;
using business.business;
using business.Group;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace BlazorCms.Client.Pages
{
    public partial class RenderizarBase : ComponentBase
    {

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

            if (substory == null)
            {

                if (args.Key == "Enter" && cap == 0)
                {
                    setarCamadas(null);
                    storyid = repositoryPagina!.stories!
                    .OrderBy(str => str.PaginaPadraoLink).Skip(1).ToList()[indice - 1].Id;
                    indice = 1;               
                }
                else if (args.Key == "Enter")
                {
                    setarCamadas(null);
                    storyid = repositoryPagina.stories.First().Id;
                    indice = repositoryPagina.stories.IndexOf(story);         
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
            acessar("/");
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

            if (substory == null && condicao)
            {
                setarCamadas(null);
                indice = int.Parse(opcional);
                acessar();
            }
            else if (condicao)
            {
                redirecionarParaVerso(int.Parse(opcional));
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
            if(indice == 1 && tellStory)
            await js!.InvokeAsync<object>("DarAlert", "Conteudo só podera ser compartilhado quando a pessoa souber qual é a pasta");
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
            int calculo = 0;
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = repositoryPagina.buscarApiYoutube(),
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
                automatico = false;
                setarCamadas(null);
                await js!.InvokeAsync<object>("DarAlert", $"Esta pasta não possui versiculos");
                acessar();
            }
            else
            {
                setarCamadas(null);
                filtrar = $"pasta-{indice_Filtro}";
                acessar();

            }

        }
        
        protected void listarPasta()
        {
            acessar($"/lista-filtro/1/{Compartilhante}/{storyid}/{indice_Filtro}/0");
        }

        protected void listarPastas()
        {

            acessar($"/listar-pasta/{storyid}/{dominio}/{Compartilhante}");
        }

        protected void acessarHorizontePastas(int? i)
        {
            automatico = false;
            outroHorizonte = 1;
            setarCamadas(null);
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
            setarCamadas(null);
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
            Pagina pag = repositoryPagina.Conteudo.OfType<Pagina>()
                    .FirstOrDefault(p => p.Filtro!.FirstOrDefault(f => f.FiltroId == Model2!.Id) != null)!;


            if (pag != null)
            {
                Filtro fil = pag.Filtro!.FirstOrDefault(f => f.FiltroId == Model2!.Id)!.Filtro!;

                if (camada != 0)
                {
                    Filtro fl = null;
                    var filtros = story.Filtro;
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
            Timer!._timer!.Elapsed -= _timer_Elapsed;

            long quant = 0;
            if (substory == null && outroHorizonte == 0)
                quant = CountPaginas(ApplicationDbContext._connectionString);
            else if (outroHorizonte == 1)
            {
                quant = QuantFiltros();
            }
            else
            {
                List<Content> lista = null;
                if (rotas != null)
                    lista = retornarListaFiltrada(rotas);
                else
                    quant = quantidadeLista;
            }

            var proximo = indice + 1;
            if (rotas != null)
            {

                if (proximo <= quant)
                {
                    setarCamadas(new int?[10] {story.PaginaPadraoLink,(int) substory!, null, null, null,
                        null, null, null, null, null });
                    indice = proximo;
                }
                else
                {
                    setarCamadas(new int?[10] {story.PaginaPadraoLink, (int)substory!, null, null, null,
                        null, null, null, null, null });
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
            else if (substory != null)
                navegarSubgrupos(false);
            else
            {
                setarCamadas(null);
                cap++;
                 storyid = repositoryPagina.stories
                 .First(str => str.PaginaPadraoLink == cap).Id;
                indice = 1;
                acessar();
            }              
        }

        private void navegarSubgrupos(bool somenteSubgrupos)
        {
            var quant = 0;
            if (substory == null)
                quant = CountPaginas(ApplicationDbContext._connectionString);
            else
            {
                var lista = retornarListaFiltrada(null);
                quant = lista.Count;
            }           

            var proximo = indice + 1;


            if (camadadez != null)
            {
                if (indice >= quant || somenteSubgrupos)
                {
                    var arr = Array.RetornarArray(story.Filtro, story, 1, 0, story.PaginaPadraoLink, (int)substory!,
                        grupo, subgrupo, subsubgrupo, camadaseis, camadasete,
                        camadaoito, camadanove, camadadez);
                    if (arr != null)                    
                        setarCamadas(arr);                        
                    else                    
                        setarCamadas(new int?[10] { 1, 1, 1, 1, 1, 1, 1, 1, 1, null });                    
                }                
                
            }
            else if (camadanove != null)
            {
                if (indice >= quant || somenteSubgrupos)
                {
                    var arr = Array.RetornarArray(story.Filtro, story, 1, 0, story.PaginaPadraoLink, (int)substory!,
                        grupo, subgrupo, subsubgrupo, camadaseis, camadasete, camadaoito, camadanove);
                    if (arr != null)                    
                        setarCamadas(arr);                    
                    else                    
                        setarCamadas(new int?[10] { 1, 1, 1, 1, 1, 1, 1, 1, null, null });                    
                }              
            }
            else if (camadaoito != null)
            {
                if (indice >= quant || somenteSubgrupos)
                {
                    var arr = Array.RetornarArray(story.Filtro, story, 1, 0, story.PaginaPadraoLink, (int)substory!,
                        grupo, subgrupo, subsubgrupo, camadaseis, camadasete, camadaoito);
                    if (arr != null)                    
                        setarCamadas(arr);                   
                    else                    
                        setarCamadas(new int?[10] { 1,1,1,1,1,1,1, null, null, null });  
                }                
            }
            else if (camadasete != null)
            {
                if (indice >= quant || somenteSubgrupos)
                {
                    var arr = Array.RetornarArray(story.Filtro, story, 1, 0, story.PaginaPadraoLink, (int)substory!,
                        grupo, subgrupo, subsubgrupo, camadaseis, camadasete);
                    if (arr != null)                    
                        setarCamadas(arr);                    
                    else                    
                        setarCamadas(new int?[10] { 1, 1, 1, 1, 1, 1, null, null, null, null });                 
                }              
            }
            else if (camadaseis != null)
            {
                if (indice >= quant || somenteSubgrupos)
                {
                    var arr = Array.RetornarArray(story.Filtro, story, 1, 0, story.PaginaPadraoLink, (int)substory!,
                        grupo, subgrupo, subsubgrupo, camadaseis);
                    if (arr != null)                    
                        setarCamadas(arr);                    
                    
                    else                    
                        setarCamadas(new int?[10] { 1, 1, 1, 1, 1, null, null, null, null, null });
                }              
            }
            else if (subsubgrupo != null)
            {
                if (indice >= quant || somenteSubgrupos)
                {
                    var arr = Array.RetornarArray(story.Filtro, story, 1, 0, story.PaginaPadraoLink, (int)substory!,
                        grupo, subgrupo, subsubgrupo);
                    if (arr != null)                    
                        setarCamadas(arr);                 
                    else                    
                        setarCamadas(new int?[10] { 1, 1, 1, 1, null, null, null, null, null, null });              
                }               
            }
            else if (subgrupo != null)
            {
                if (indice >= quant || somenteSubgrupos)
                {
                    var arr = Array.RetornarArray(story.Filtro, story, 1, 0, story.PaginaPadraoLink, (int)substory!,
                        grupo, subgrupo);
                    if (arr != null)                    
                        setarCamadas(arr);                    
                    else                    
                        setarCamadas(new int?[10] { 1, 1, 1, null, null, null, null, null, null,
                        null});  
                }                                
            }
            else if (grupo != null)
            {
                if (indice >= quant || somenteSubgrupos)
                {
                    var arr = Array.RetornarArray(story.Filtro, story, 1, 0, story.PaginaPadraoLink, (int)substory!, grupo);
                    if (arr != null)                    
                        setarCamadas(arr);                      
                    else                    
                        setarCamadas(new int?[10] { 1, 1, null, null, null, null, null, null, null,
                        null});      
                }             
            }
            else if (substory != null)
            {
                if (indice >= quant || somenteSubgrupos)
                {
                    var arr = Array.RetornarArray(story.Filtro, story, 1, 0, story.PaginaPadraoLink, (int)substory);
                    if (arr != null)                    
                        setarCamadas(arr);                   
                    else                    
                        setarCamadas(null);  
                }               
            }
            indice = 1;
            acessar();
        }

        protected void buscarAnterior()
        {
            Timer!._timer!.Elapsed -= _timer_Elapsed;

            if (rotas != null)
            {
                if (indice == 1)
                {
                    setarCamadas(new int?[10] { story.PaginaPadraoLink, (int)substory!, null, null, null,
                        null, null, null, null, null });
                    indice = 1;
                }                
                else
                {
                    setarCamadas(new int?[10] { story.PaginaPadraoLink, (int)substory!, null, null, null,
                        null, null, null, null, null });
                    indice--;
                }               
            }
            else
            if (indice == 1 && cap != 0)
            {
                if (substory != null)
                {
                    voltarSubgrupos();
                }

                indice = 1;
                retroceder = 1;
                if (substory == null)
                {
                    cap--;
                    storyid = repositoryPagina.stories
                    .First(str => str.PaginaPadraoLink == cap).Id;
                }

            }
            if (indice != 1 && rotas == null)
            {
                var anterior = indice - 1;
                indice = anterior;
            }
                 acessar();
        }

        private void voltarSubgrupos()
        {
            
                int?[] arr = null;
                int?[] result = null;
            if (camadadez != null && camadadez != 1)
            {
                var camada = camadadez;
                while(camadadez != 0)
                {
                    var lista = story.Filtro.OrderBy(f => f.Id).Where(f => f is CamadaDez).ToList();
                    foreach(var f in lista)
                    {
                        arr =  Array.RetornarArray(story.Filtro, story, 2, (long)f.Id, 1, 1);
                        if(arr != null )
                        {
                            arr = Array.RetornarArray(story.Filtro, story, 3, (long)f.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                            if (arr[8] == camadanove && arr[7] == camadaoito && arr[6] == camadasete &&
                                arr[5] == camadaseis && arr[4] == subsubgrupo && arr[3] == subgrupo &&
                                arr[2] == grupo && arr[1] == substory && arr[9] < camada)
                            {                               
                                result = arr;
                                break;
                            }
                        }
                    }

                    if (result != null && result.Length == 10)
                    {
                        setarCamadas(result);
                        break;
                    }
                    camadadez -= 1; 
                }
            }           
            else if (camadanove != null && camadanove != 1)
            {
                var camada = camadanove;
                while(camadanove != 0)
                {
                    var lista = story.Filtro.Where(f => f is CamadaNove).OrderByDescending(f => f.Id).ToList();
                    foreach(var f in lista)
                    {
                        arr =  Array.RetornarArray(story.Filtro, story, 2, (long)f.Id, 1, 1);
                        if(arr != null )
                        {
                            arr = Array.RetornarArray(story.Filtro, story, 3, (long)f.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                            if (arr[7] == camadaoito && arr[6] == camadasete &&
                                arr[5] == camadaseis && arr[4] == subsubgrupo && arr[3] == subgrupo &&
                                arr[2] == grupo && arr[1] == substory && arr[8] < camada)
                            {
                                var fi = story.Filtro.Where(fil => fil.Pagina
                                .FirstOrDefault(p => p.Content!.Id ==
                                f.Pagina!.First().Content!.Id && f is CamadaDez) != null)
                                .LastOrDefault()!;
                                if(fi != null)
                                arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                result = arr;
                                break;
                            }
                        }
                    }

                    if (result != null && result.Length >= 9)
                    {
                        setarCamadas(result);
                        break;
                    }
                    camadanove -= 1; 
                }
            }
            else if (camadaoito != null && camadaoito != 1)
            {
                var camada = camadaoito;
                while (camadaoito != 0)
                {
                    var lista = story.Filtro.Where(f => f is CamadaOito).OrderByDescending(f => f.Id).ToList();
                    foreach (var f in lista)
                    {
                        arr = Array.RetornarArray(story.Filtro, story, 2, (long)f.Id, 1, 1);
                        if (arr != null)
                        {
                             arr = Array.RetornarArray(story.Filtro, story, 3, (long)f.Id, 1, 1, 1, 1, 1, 1, 1, 1);
                            if (arr[6] == camadasete &&
                               arr[5] == camadaseis && arr[4] == subsubgrupo && arr[3] == subgrupo &&
                               arr[2] == grupo && arr[1] == substory && arr[7] < camada)
                            {
                                var fi = story.Filtro.Where(fil => fil.Pagina
                                 .FirstOrDefault(p => p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaDez ||
                                 p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaNove) != null)
                                 .LastOrDefault()!;
                                if (fi != null && fi is CamadaDez)
                                arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaNove)
                                arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1);
                                result = arr;
                                break;
                            }
                        }
                    }

                    if (result != null && result.Length >= 8)
                    {
                        setarCamadas(result);
                        break;
                    }
                    camadaoito -= 1;
                }
            }
            else if (camadasete != null && camadasete != 1)
            {
                var camada = camadasete;
                while (camadasete != 0)
                {
                    var lista = story.Filtro.Where(f => f is CamadaSete).OrderByDescending(f => f.Id).ToList();
                    foreach (var f in lista)
                    {
                        arr = Array.RetornarArray(story.Filtro, story, 2, (long)f.Id, 1, 1);
                        if (arr != null)
                        {
                             arr = Array.RetornarArray(story.Filtro, story, 3, (long)f.Id, 1, 1, 1, 1, 1, 1, 1);
                            if (arr != null && arr[5] == camadaseis && arr[4] == subsubgrupo && arr[3] == subgrupo &&
                               arr[2] == grupo && arr[1] == substory && arr[6] < camada)
                            {
                                var fi = story.Filtro.Where(fil => fil.Pagina
                                 .FirstOrDefault(p => p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaDez ||
                                 p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaNove ||
                                  p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaOito) != null)
                                 .LastOrDefault()!;
                                if (fi != null && fi is CamadaDez)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaNove)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaOito)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1);
                                result = arr;
                                break;
                            }
                        }
                    }

                    if (result != null && result.Length >= 7)
                    {
                        setarCamadas(result);
                        break;
                    }
                    camadasete -= 1;
                }
            }
            else if (camadaseis != null && camadaseis != 1)
            {
                var camada = camadaseis;
                while (camadaseis != 0)
                {
                    var lista = story.Filtro.Where(f => f is CamadaSeis).OrderByDescending(f => f.Id).ToList();
                    foreach (var f in lista)
                    {
                        arr = Array.RetornarArray(story.Filtro, story, 2, (long)f.Id, 1, 1);
                        if (arr != null)
                        {
                            arr = Array.RetornarArray(story.Filtro, story, 3, (long)f.Id, 1, 1, 1, 1, 1, 1);
                            if (arr[4] == subsubgrupo && arr[3] == subgrupo &&
                              arr[2] == grupo && arr[1] == substory && arr[5] < camada)
                            {
                                var fi = story.Filtro.Where(fil => fil.Pagina
                                 .FirstOrDefault(p => p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaDez ||
                                 p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaNove ||
                                  p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaOito ||
                                  p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaSete) != null)
                                 .LastOrDefault()!;
                                if (fi != null && fi is CamadaDez)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaNove)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaOito)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaSete)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1);
                                result = arr;
                                break;
                            }
                        }
                    }

                    if (result != null && result.Length >= 6)
                    {
                        setarCamadas(result);
                        break;
                    }
                    camadaseis -= 1;
                }
            }
            else if (subsubgrupo != null && subsubgrupo != 1)
            {
                var camada = subsubgrupo;
                while (subsubgrupo != 0)
                {
                    var lista = story.Filtro.Where(f => f is SubSubGrupo).OrderByDescending(f => f.Id)
                        .ToList();
                    foreach (var f in lista)
                    {
                        arr = Array.RetornarArray(story.Filtro, story, 2, (long)f.Id, 1, 1);
                        if (arr != null)
                        {
                            arr = Array.RetornarArray(story.Filtro, story, 3, (long)f.Id, 1, 1, 1, 1, 1);
                            if (arr[3] == subgrupo &&
                              arr[2] == grupo && arr[1] == substory && arr[4] < camada)
                            {
                                var fi = story.Filtro.Where(fil => fil.Pagina
                                 .FirstOrDefault(p => p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaDez ||
                                 p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaNove ||
                                  p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaOito ||
                                  p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaSete ||
                                  p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaSeis) != null)
                                 .LastOrDefault()!;
                                if (fi != null && fi is CamadaDez)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaNove)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaOito)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaSete)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaSeis)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1);
                                result = arr;
                                break;
                            }
                        }
                    }

                    if (result != null && result.Length >= 5)
                    {
                        setarCamadas(result);
                        break;
                    }
                    subsubgrupo -= 1;
                }
            }
            else if (subgrupo != null && subgrupo != 1)
            {
                var camada = subgrupo;
                while (subgrupo != 0)
                {
                    var lista = story.Filtro.Where(f => f is SubGrupo).OrderByDescending(f => f.Id)
                        .ToList();
                    foreach (var f in lista)
                    {
                        arr = Array.RetornarArray(story.Filtro, story, 2, (long)f.Id, 1, 1);
                        if (arr != null)
                        {
                            arr = Array.RetornarArray(story.Filtro, story, 3, (long)f.Id, 1, 1, 1, 1);
                            if (arr[2] == grupo && arr[1] == substory && arr[3] < camada)
                            {
                                var fi = story.Filtro.Where(fil => fil.Pagina
                                 .FirstOrDefault(p => p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaDez ||
                                 p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaNove ||
                                  p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaOito ||
                                  p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaSete ||
                                  p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaSeis ||
                                 p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is SubSubGrupo) != null)
                                 .LastOrDefault()!;
                                if (fi != null && fi is CamadaDez)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaNove)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaOito)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaSete)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaSeis)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is SubSubGrupo)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1);
                                result = arr;
                                break;
                            }
                        }
                    }

                    if (result != null && result.Length >= 4)
                    {
                        setarCamadas(result);
                        break;
                    }
                    subgrupo -= 1;
                }
            }
            else if (grupo != null && grupo != 1)
            {
                var camada = grupo;
                while (grupo != 0)
                {
                    var lista = story.Filtro.Where(f => f is Grupo).OrderByDescending(f => f.Id)
                        .ToList();
                    foreach (var f in lista)
                    {
                        arr = Array.RetornarArray(story.Filtro, story, 2, (long)f.Id, 1, 1);
                        if (arr != null)
                        {
                            arr = Array.RetornarArray(story.Filtro, story, 3, (long)f.Id, 1, 1, 1);
                            if (arr[1] == substory && arr[2] < camada)
                            {
                                var fi = story.Filtro.Where(fil => fil.Pagina
                                 .FirstOrDefault(p => p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaDez ||
                                 p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaNove ||
                                  p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaOito ||
                                  p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaSete ||
                                  p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaSeis ||
                                 p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is SubSubGrupo ||
                                 p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is SubGrupo) != null)
                                 .LastOrDefault()!;
                                if (fi != null && fi is CamadaDez)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaNove)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaOito)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaSete)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaSeis)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is SubSubGrupo)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1);
                                if (fi != null && fi is SubGrupo)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1);
                                result = arr;
                                break;
                            }
                                
                        }
                    }

                    if (result != null && result.Length >= 3)
                    {
                        setarCamadas(result);
                        break;
                    }
                    grupo -= 1;
                }
            }
            else if (substory != null && substory != 1)
            {
                var camada = substory;
                while (substory != 0)
                {
                    var lista = story.Filtro.Where(f => f is SubStory).OrderByDescending(f => f.Id).ToList();
                    foreach (var f in lista)
                    {
                        arr = Array.RetornarArray(story.Filtro, story, 2, (long)f.Id, 1, 1);
                        if (arr != null)
                        {
                            arr = Array.RetornarArray(story.Filtro, story, 3, (long)f.Id, 1, 1);
                            if (arr[1] < camada)
                            {
                                var fi = story.Filtro.Where(fil => fil.Pagina
                                 .FirstOrDefault(p => p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaDez ||
                                 p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaNove ||
                                  p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaOito ||
                                  p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaSete ||
                                  p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is CamadaSeis ||
                                 p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is SubSubGrupo ||
                                 p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is SubGrupo ||
                                 p.Content!.Id ==
                                 f.Pagina!.First().Content!.Id && f is Grupo) != null)
                                 .LastOrDefault()!;
                                if (fi != null && fi is CamadaDez)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaNove)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaOito)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaSete)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaSeis)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is SubSubGrupo)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1);
                                if (fi != null && fi is SubGrupo)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1);
                                if (fi != null && fi is Grupo)
                                    arr = Array.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1);
                                result = arr;
                                break;
                            }
                        }
                    }

                    if (result != null && result.Length >= 2) 
                    {
                        setarCamadas(result);
                        break;
                    } 
                    substory -= 1;
                }
            }
            else if(camadadez != null)
            {
                var fi = Context.Filtro!
                    .Include(p => p.Pagina)
                    .Where(p => p is CamadaNove && p.Pagina.Count > 0)
                    .ToList()
                    .LastOrDefault()!;
                result = retornarArray(fi);
                setarCamadas(result);
            }
            else if (camadanove != null)
            {
                var fi = Context.Filtro!
                    .Include(p => p.Pagina)
                    .Where(p => p is CamadaOito && p.Pagina.Count > 0)
                    .ToList()
                    .LastOrDefault()!;
                result = retornarArray(fi);
                setarCamadas(result);
            }
            else if (camadaoito != null)
            {
                var fi = Context.Filtro!
                    .Include(p => p.Pagina)
                    .Where(p => p is CamadaSete && p.Pagina.Count > 0)
                    .ToList()
                    .LastOrDefault()!;
                result = retornarArray(fi);
                setarCamadas(result);
            }
            else if (camadasete != null)
            {
                var fi = Context.Filtro!
                    .Include(p => p.Pagina)
                    .Where(p => p is CamadaSeis && p.Pagina.Count > 0)
                    .ToList()
                    .LastOrDefault()!;
                result = retornarArray(fi);
                setarCamadas(result);
            }
            else if (camadaseis != null)
            {
                var fi = Context.Filtro!
                    .Include(p => p.Pagina)
                    .Where(p => p is SubSubGrupo && p.Pagina.Count > 0)
                    .ToList()
                    .LastOrDefault()!;
                result = retornarArray(fi);
                setarCamadas(result);
            }
            else if (subsubgrupo != null)
            {
                var fi = Context.Filtro!
                    .Include(p => p.Pagina)
                    .Where(p => p is SubGrupo && p.Pagina.Count > 0) 
                    .ToList()
                    .LastOrDefault()!;
                result = retornarArray(fi);
                setarCamadas(result);
            }
            else if (subgrupo != null)
            {
                var fi = Context.Filtro!
                   .Include(p => p.Pagina)
                    .Where(p => p is Grupo && p.Pagina.Count > 0)
                    .ToList()
                    .LastOrDefault()!;
                result = retornarArray(fi);
                setarCamadas(result);
            }
            else if (grupo != null)
            {
                var fi = Context.Filtro!
                .Include(p => p.Pagina)
                .Where(p => p is SubStory && p.Pagina.Count > 0)
                .ToList()
                .LastOrDefault()!;
                result = retornarArray(fi);
                setarCamadas(result);
            }
            else if (substory != null)
            {
                setarCamadas(null);
            }

           
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
            setarCamadas(null);
            indice = (int) vers!;
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

                if (substory == null)
                    opcional = indice.ToString();
                else
                {
                    opcional = vers.ToString();
                }
                if (compartilhou == "comp")
                    fils = story!.Filtro!.OrderBy(f => f.Id).ToList();
            else
            {
                var usu = Context.Users.FirstOrDefault(u => u.UserName == compartilhou);
                fils = repositoryPagina.retornarMarcadores(usu, story!).OrderBy(f => f.Id).ToList();
            }

            // 1º time
            if (camada == 7)
            {                   
                fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => retornarVerso(p.Content) == int.Parse(opcional!)) != null &&
                f is CamadaSete).LastOrDefault()!;
                if (fi == null)
                    fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => retornarVerso(p.Content) == int.Parse(opcional!)) != null &&
                f is CamadaSeis).LastOrDefault()!;
                if (fi == null)
                    fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => retornarVerso(p.Content) == int.Parse(opcional!)) != null &&
                f is SubSubGrupo).LastOrDefault()!;
                if (fi == null)
                    fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => retornarVerso(p.Content) == int.Parse(opcional!)) != null &&
                f is SubGrupo).LastOrDefault()!;
                if (fi == null)
                    fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => retornarVerso(p.Content) == int.Parse(opcional!)) != null &&
                f is Grupo).LastOrDefault()!;
                if (fi == null)
                    fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => retornarVerso(p.Content) == int.Parse(opcional!)) != null &&
                f is SubStory).LastOrDefault()!;                
            }
           
            else if (camada == 6)
            {
                 fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => retornarVerso(p.Content) == int.Parse(opcional!)) != null &&
                f is CamadaSeis).LastOrDefault()!;
                if (fi == null)
                    fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => retornarVerso(p.Content) == int.Parse(opcional!)) != null &&
                f is SubSubGrupo).LastOrDefault()!;
                if (fi == null)
                    fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => retornarVerso(p.Content) == int.Parse(opcional!)) != null &&
                f is SubGrupo).LastOrDefault()!;
                if (fi == null)
                    fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => retornarVerso(p.Content) == int.Parse(opcional!)) != null &&
                f is Grupo).LastOrDefault()!;
                if (fi == null)
                    fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => retornarVerso(p.Content) == int.Parse(opcional!)) != null &&
                f is SubStory).LastOrDefault()!;
            }
           
            else if (camada == 5)
            {
                    fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => retornarVerso(p.Content) == int.Parse(opcional!)) != null &&
                f is SubSubGrupo).LastOrDefault()!;
                if (fi == null)
                    fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => retornarVerso(p.Content) == int.Parse(opcional!)) != null &&
                f is SubGrupo).LastOrDefault()!;
                if (fi == null)
                    fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => retornarVerso(p.Content) == int.Parse(opcional!)) != null &&
                f is Grupo).LastOrDefault()!;
                if (fi == null)
                    fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => retornarVerso(p.Content) == int.Parse(opcional!)) != null &&
                f is SubStory).LastOrDefault()!;
            }

            else if (camada == 4)
            {
                    fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => retornarVerso(p.Content) == int.Parse(opcional!)) != null &&
                f is SubGrupo).LastOrDefault()!;
                if (fi == null)
                    fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => retornarVerso(p.Content) == int.Parse(opcional!)) != null &&
                f is Grupo).LastOrDefault()!;
                if (fi == null)
                    fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => retornarVerso(p.Content) == int.Parse(opcional!)) != null &&
                f is SubStory).LastOrDefault()!;
            }

            else if (camada == 3)
            {
                    fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => retornarVerso(p.Content) == int.Parse(opcional!)) != null &&
                f is Grupo).LastOrDefault()!;
                if (fi == null)
                    fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => retornarVerso(p.Content) == int.Parse(opcional!)) != null &&
                f is SubStory).LastOrDefault()!;
            }



            if (fi == null)
            {
                if(fi is null && compartilhou != "comp")
                await js!.InvokeAsync<object>("DarAlert",
                    $"A pasta do usuario {compartilhou} não pussui este verso!!!");
                  else
                await js!.InvokeAsync<object>("DarAlert", "Marque um versiculo que esta em uma pasta!!!");
            }
            else
            {
                var arr = retornarArray(fi);
                setarCamadas(arr);
                redirecionarParaVerso(int.Parse(opcional!));

                
            }




        }

        private async void redirecionarParaVerso(int verso)
        {
            if (outroHorizonte == 0)
            {
                var list = retornarListaFiltrada(null);
                if (list == null)
                {
                    setarCamadas(null);
                    indice = verso;
                    outroHorizonte = 0;
                    acessar();                   
                    return;
                }
                if (!tellStory)
                {
                    opcional = verso.ToString();
                    int indiceListaFiltrada = 0;
                    foreach (var item in list)
                    {
                        Pagina p = repositoryPagina!.Conteudo.OfType<Pagina>()!.First(p => p.Id == item.Id);
                        if (int.Parse(opcional) == p.Versiculo)
                        {
                            indiceListaFiltrada = list.IndexOf(item) + 1;
                            break;
                        }
                    }

                    if (indiceListaFiltrada == 0)
                    {
                        indiceListaFiltrada = indice;
                        await js!.InvokeAsync<object>("DarAlert", $"Não foi encontrado o versiculo {verso} na pasta {indice_Filtro}. O versiculo {verso} não é {Model2.Nome}.");
                    }
                    else
                    {
                        indice = indiceListaFiltrada;
                        acessar();
                    }
                }
                else
                {
                    indice = int.Parse(opcional);
                    acessar();
                }
            }
            else
            {
                setarCamadas(null);
                indice = verso;
                outroHorizonte = 1;               
                acessar();
            }

        }

        private int CountLikes(string conexao)
        {

            var _TotalRegistros = 0;
            try
            {
                using (var con = new SqlConnection(conexao))
                {
                    SqlCommand cmd = null;
                    if (substory == null)
                        cmd = new SqlCommand($"SELECT COUNT(*) FROM PageLiked as P  where P.capitulo={story.PaginaPadraoLink} and P.verso={indice}", con);
                    else
                        cmd = new SqlCommand($"SELECT COUNT(*) FROM PageLiked as P  where P.capitulo={story.PaginaPadraoLink} and P.verso={vers}", con);

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
            if (story == null)
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
                    if (Model != null)
                    {
                        cmd = new SqlCommand($"SELECT COUNT(*) FROM Content as P " +
                            $" where P.StoryId={Model.StoryId} and P.Discriminator='Pagina' or " +
                            $" P.StoryId={Model.StoryId} and P.Discriminator='AdminContent' or " +
                            $" P.StoryId={Model.StoryId} and P.Discriminator='ProductContent' or " +
                            $" P.StoryId={Model.StoryId} and P.Discriminator='ChangeContent'  ", con);
                        con.Open();
                        _TotalRegistros = int.Parse(cmd.ExecuteScalar().ToString());
                        con.Close();
                    }
                    else
                        _TotalRegistros = 0;


                }
            }
            catch (Exception)
            {
                _TotalRegistros = 0;
            }

            return _TotalRegistros;
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
                html = html.Replace("<iframe", "<iframe" + " allow='accelerometer; autoplay; clipboard-write; encrypted-media;' ");

            return html;
        }

        protected async void StartTour()
        {
            automatico = false;
            if (substory == null)
                await TourService.StartTour("FormGuidedTour1");
            else
                await TourService.StartTour("FormGuidedTour2");
        }

        protected async void share()
        {
            automatico = false;
            if (Compartilhante == 0 || title == null || resumo == null)
            {
                if (Compartilhante == 0)
                {
                    Compartilhante =  0;
                }
                if (title == null)
                {
                    title = Model.Titulo;
                }

                if (resumo == null)
                {
                    resumo = await js.InvokeAsync<string>("prompt", "Informe o resumo da pagina.");
                }
                await js!.InvokeAsync<object>("DarAlert", $"Agora Compartilhe!!!");

                    if(substory != null)
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
            if(substory == null)
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
            if (substory == null)
            {
                acessar($"/pastas/{storyid}/{indice}/{dominio}/{Compartilhante}");
            }
            else
            {
                acessar($"/pastas/{storyid}/{vers}/{dominio}/{Compartilhante}");

            }
        }

        protected void acessarCapitulos()
        {
            indice = repositoryPagina.stories.IndexOf(story);
            storyid = repositoryPagina!.stories!.First().Id;
            outroHorizonte = 0;
            setarCamadas(null);

            acessar();
        }

        protected void marcarPasta(int i)
        {
            outroHorizonte = 1;
            indice = i;
            setarCamadas(null);

            acessar();
        }

        protected void acessarStory()
        {
            outroHorizonte = 0;
            storyid = repositoryPagina!.stories!
                .OrderBy(str => str.PaginaPadraoLink).Skip(1).ToList()[indice - 1].Id;
            automatico = false;
            indice = 1;
            setarCamadas(null);

            acessar();
        }

        protected void AdicionarAoCarrinho(long ProdutoId)
        {
            criptografar = true;
            var url = $"/carrinho/{ProdutoId}/{Compartilhou}/{Compartilhante}/" +
            $"{Compartilhante2}/{Compartilhante3}/{Compartilhante4}/{Compartilhante5}/" +
            $"{Compartilhante6}/{Compartilhante7}/{Compartilhante8}/{Compartilhante9}/" +
            $"{Compartilhante10}";
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
                if (camadadez != null)
                    url = $"/camada10/{storyid}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{indice}/{Auto}/{timeproduto}/{conteudo}/{indiceLivro}/{retroceder}/{dominio}/{Compartilhou}/{Compartilhante}/{Compartilhante2}/{Compartilhante3}/{Compartilhante4}/{Compartilhante5}/{Compartilhante6}/{Compartilhante7}/{Compartilhante8}/{Compartilhante9}/{Compartilhante10}";
                else if (camadanove != null)
                    url = $"/camada9/{storyid}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{indice}/{Auto}/{timeproduto}/{conteudo}/{indiceLivro}/{retroceder}/{dominio}/{Compartilhou}/{Compartilhante}/{Compartilhante2}/{Compartilhante3}/{Compartilhante4}/{Compartilhante5}/{Compartilhante6}/{Compartilhante7}/{Compartilhante8}/{Compartilhante9}/{Compartilhante10}";
                else if (camadaoito != null)
                    url = $"/camada8/{storyid}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{indice}/{Auto}/{timeproduto}/{conteudo}/{indiceLivro}/{retroceder}/{dominio}/{Compartilhou}/{Compartilhante}/{Compartilhante2}/{Compartilhante3}/{Compartilhante4}/{Compartilhante5}/{Compartilhante6}/{Compartilhante7}/{Compartilhante8}/{Compartilhante9}/{Compartilhante10}";
                else if (camadasete != null)
                    url = $"/camada7/{storyid}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{indice}/{Auto}/{timeproduto}/{conteudo}/{indiceLivro}/{retroceder}/{dominio}/{Compartilhou}/{Compartilhante}/{Compartilhante2}/{Compartilhante3}/{Compartilhante4}/{Compartilhante5}/{Compartilhante6}/{Compartilhante7}/{Compartilhante8}/{Compartilhante9}/{Compartilhante10}";
                else if (camadaseis != null)
                    url = $"/camada6/{storyid}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{indice}/{Auto}/{timeproduto}/{conteudo}/{indiceLivro}/{retroceder}/{dominio}/{Compartilhou}/{Compartilhante}/{Compartilhante2}/{Compartilhante3}/{Compartilhante4}/{Compartilhante5}/{Compartilhante6}/{Compartilhante7}/{Compartilhante8}/{Compartilhante9}/{Compartilhante10}";
                else if (subsubgrupo != null)
                    url = $"/camada5/{storyid}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{indice}/{Auto}/{timeproduto}/{conteudo}/{indiceLivro}/{retroceder}/{dominio}/{Compartilhou}/{Compartilhante}/{Compartilhante2}/{Compartilhante3}/{Compartilhante4}/{Compartilhante5}/{Compartilhante6}/{Compartilhante7}/{Compartilhante8}/{Compartilhante9}/{Compartilhante10}";
                else if (subgrupo != null)
                    url = $"/camada4/{storyid}/{substory}/{grupo}/{subgrupo}/{indice}/{Auto}/{timeproduto}/{conteudo}/{indiceLivro}/{retroceder}/{dominio}/{Compartilhou}/{Compartilhante}/{Compartilhante2}/{Compartilhante3}/{Compartilhante4}/{Compartilhante5}/{Compartilhante6}/{Compartilhante7}/{Compartilhante8}/{Compartilhante9}/{Compartilhante10}";
                else if (grupo != null)
                    url = $"/camada3/{storyid}/{substory}/{grupo}/{indice}/{Auto}/{timeproduto}/{conteudo}/{indiceLivro}/{retroceder}/{dominio}/{Compartilhou}/{Compartilhante}/{Compartilhante2}/{Compartilhante3}/{Compartilhante4}/{Compartilhante5}/{Compartilhante6}/{Compartilhante7}/{Compartilhante8}/{Compartilhante9}/{Compartilhante10}";
                else if (substory != null)
                    url = $"/camada2/{storyid}/{substory}/{indice}/{Auto}/{timeproduto}/{conteudo}/{indiceLivro}/{retroceder}/{dominio}/{Compartilhou}/{Compartilhante}/{Compartilhante2}/{Compartilhante3}/{Compartilhante4}/{Compartilhante5}/{Compartilhante6}/{Compartilhante7}/{Compartilhante8}/{Compartilhante9}/{Compartilhante10}";
                else if (filtrar != null)
                    url = $"/filtro/{storyid}/{filtrar}/0/0/{dominio}/{Compartilhou}/{Compartilhante}/{Compartilhante2}/{Compartilhante3}/{Compartilhante4}/{Compartilhante5}/{Compartilhante6}/{Compartilhante7}/{Compartilhante8}/{Compartilhante9}/{Compartilhante10}/{redirecionar}";
                else
                    url = $"/Renderizar/{storyid}/{indice}/{Auto}/{timeproduto}/{outroHorizonte}/{indiceLivro}/{retroceder}/{dominio}/{Compartilhou}/{Compartilhante}/{Compartilhante2}/{Compartilhante3}/{Compartilhante4}/{Compartilhante5}/{Compartilhante6}/{Compartilhante7}/{Compartilhante8}/{Compartilhante9}/{Compartilhante10}";

                if (Compartilhante != 0 && outroHorizonte == 0 && pontos == null) url += "/1";

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

    }

    public class UserPreferencesImage
    {
        public string? user { get; set; }
        public UserModel UserModel { get; set; }
    }
}

            