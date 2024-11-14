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

        private async void StartTimer(Pagina p)
        {
            try
            {
                if (p != null && p.Content != null && p.Content.Contains("iframe") && outroHorizonte == 0)
                {
                    var conteudoHtml = p.Content;
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
            if (Auto == 1)
            {
                if (substory == null)
                {
                    if (capitulo == 0 && indice >= quant)
                    {
                        setarCamadas(null);
                        indice = 1;
                    }                    
                    else if (capitulo != 0 && indice >= quant && outroHorizonte == 0)
                    {
                        setarCamadas(null);
                        capitulo++;
                        indice = 1;
                    }                  
                    else if (capitulo != 0 && indice >= quant && outroHorizonte == 1)
                    {
                        setarCamadas(null);
                        indice = 1;
                    }                    
                    else
                    {
                        setarCamadas(null);
                        indice++;
                    }
                                        
                    acessar();
                }
                else
                {
                    navegarSubgrupos(false);
                }
            }
            else
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
                    setarCamadas(null);
                    capitulo = indice;
                    indice = 1;               
                }
                else if (args.Key == "Enter")
                {
                    setarCamadas(null);
                    capitulo = 0;
                    indice = capitulo;         
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
            Timer!.SetTimerAuto();
            Timer!.desligarAuto!.Elapsed += desligarAuto_Elapsed;
            
        }

        private void desabilitarAuto()
        {
            if (Timer!.desligarAuto != null)
            {
                Timer!.desligarAuto!.Elapsed -= desligarAuto_Elapsed;
                Timer!.desligarAuto!.Enabled = false;
                Timer.desligarAuto.Dispose();                
            }
        }

        private void desligarAuto_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {

            Console.WriteLine("Timer Elapsed auto.");
            Timer!.desligarAuto!.Elapsed -= desligarAuto_Elapsed;
            Timer.desligarAuto.Dispose();
            acessar("/");
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
                setarCamadas(null);
                automatico = false;
                await js!.InvokeAsync<object>("DarAlert", $"Esta pasta não possui versiculos");
                acessar();
            }
            else
            {
                acessar($"/filtro/{capitulo}/pasta-{indice_Filtro}/0/0/{dominio}/{compartilhante}");

            }

        }
        
        protected void listarPasta()
        {
            var lista = retornarListaFiltrada(null);
            int tamanho = 0;

            if (dominio == null) dominio = "dominio";

            if (lista.FirstOrDefault(p => p.Content != null) != null)
                tamanho = 5;
            else
                tamanho = 20;

            acessar($"/lista-filtro/1/{compartilhante}/{capitulo}/{indice_Filtro}/0");
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

        protected void acessarHorizonteMarcadores(int? i)
        {
            automatico = false;
            setarCamadas(null);
            outroHorizonte = 3;
            if(i != null)
            indice = (int) i;
            else
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
            Pagina pag = repositoryPagina.paginas
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
            if (rotas != null)
            {

                if (proximo <= quant)
                {
                    setarCamadas(new int?[10] {capitulo,(int) substory!, null, null, null,
                        null, null, null, null, null });
                    indice = proximo;
                }
                else
                {
                    setarCamadas(new int?[10] { capitulo, (int)substory!, null, null, null,
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
                capitulo++;
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
            if (somenteSubgrupos) automatico = false;
            if (camadadez != null)
            {
                if (indice >= quant || somenteSubgrupos)
                {
                    var arr = Arr.RetornarArray(story.Filtro, story, 1, 0, capitulo, (int)substory!,
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
                    var arr = Arr.RetornarArray(story.Filtro, story, 1, 0, capitulo, (int)substory!,
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
                    var arr = Arr.RetornarArray(story.Filtro, story, 1, 0, capitulo, (int)substory!,
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
                    var arr = Arr.RetornarArray(story.Filtro, story, 1, 0, capitulo, (int)substory!,
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
                    var arr = Arr.RetornarArray(story.Filtro, story, 1, 0, capitulo, (int)substory!,
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
                    var arr = Arr.RetornarArray(story.Filtro, story, 1, 0, capitulo, (int)substory!,
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
                    var arr = Arr.RetornarArray(story.Filtro, story, 1, 0, capitulo, (int)substory!,
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
                    var arr = Arr.RetornarArray(story.Filtro, story, 1, 0, capitulo, (int)substory!, grupo);
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
                    var arr = Arr.RetornarArray(story.Filtro, story, 1, 0, capitulo, (int)substory);
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
            automatico = false;

            if (rotas != null)
            {
                if (indice == 1)
                {
                    setarCamadas(new int?[10] { capitulo, (int)substory!, null, null, null,
                        null, null, null, null, null });
                    indice = 1;
                }                
                else
                {
                    setarCamadas(new int?[10] { capitulo, (int)substory!, null, null, null,
                        null, null, null, null, null });
                    indice--;
                }               
            }
            else
            if (indice == 1 && capitulo != 0)
            {
                if (substory != null)
                {
                    voltarSubgrupos();
                }

                indice = 1;
                retroceder = 1;
                if (substory == null)
                    capitulo--;

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
                        arr =  Arr.RetornarArray(story.Filtro, story, 2, (long)f.Id, 1, 1);
                        if(arr != null )
                        {
                            arr = Arr.RetornarArray(story.Filtro, story, 3, (long)f.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
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
                        arr =  Arr.RetornarArray(story.Filtro, story, 2, (long)f.Id, 1, 1);
                        if(arr != null )
                        {
                            arr = Arr.RetornarArray(story.Filtro, story, 3, (long)f.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                            if (arr[7] == camadaoito && arr[6] == camadasete &&
                                arr[5] == camadaseis && arr[4] == subsubgrupo && arr[3] == subgrupo &&
                                arr[2] == grupo && arr[1] == substory && arr[8] < camada)
                            {
                                var fi = story.Filtro.Where(fil => fil.Pagina
                                .FirstOrDefault(p => p.Pagina!.Versiculo ==
                                f.Pagina!.First().Pagina!.Versiculo && f is CamadaDez) != null)
                                .LastOrDefault()!;
                                if(fi != null)
                                arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
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
                        arr = Arr.RetornarArray(story.Filtro, story, 2, (long)f.Id, 1, 1);
                        if (arr != null)
                        {
                             arr = Arr.RetornarArray(story.Filtro, story, 3, (long)f.Id, 1, 1, 1, 1, 1, 1, 1, 1);
                            if (arr[6] == camadasete &&
                               arr[5] == camadaseis && arr[4] == subsubgrupo && arr[3] == subgrupo &&
                               arr[2] == grupo && arr[1] == substory && arr[7] < camada)
                            {
                                var fi = story.Filtro.Where(fil => fil.Pagina
                                 .FirstOrDefault(p => p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaDez ||
                                 p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaNove) != null)
                                 .LastOrDefault()!;
                                if (fi != null && fi is CamadaDez)
                                arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaNove)
                                arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1);
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
                        arr = Arr.RetornarArray(story.Filtro, story, 2, (long)f.Id, 1, 1);
                        if (arr != null)
                        {
                             arr = Arr.RetornarArray(story.Filtro, story, 3, (long)f.Id, 1, 1, 1, 1, 1, 1, 1);
                            if (arr != null && arr[5] == camadaseis && arr[4] == subsubgrupo && arr[3] == subgrupo &&
                               arr[2] == grupo && arr[1] == substory && arr[6] < camada)
                            {
                                var fi = story.Filtro.Where(fil => fil.Pagina
                                 .FirstOrDefault(p => p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaDez ||
                                 p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaNove ||
                                  p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaOito) != null)
                                 .LastOrDefault()!;
                                if (fi != null && fi is CamadaDez)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaNove)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaOito)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1);
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
                        arr = Arr.RetornarArray(story.Filtro, story, 2, (long)f.Id, 1, 1);
                        if (arr != null)
                        {
                            arr = Arr.RetornarArray(story.Filtro, story, 3, (long)f.Id, 1, 1, 1, 1, 1, 1);
                            if (arr[4] == subsubgrupo && arr[3] == subgrupo &&
                              arr[2] == grupo && arr[1] == substory && arr[5] < camada)
                            {
                                var fi = story.Filtro.Where(fil => fil.Pagina
                                 .FirstOrDefault(p => p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaDez ||
                                 p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaNove ||
                                  p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaOito ||
                                  p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaSete) != null)
                                 .LastOrDefault()!;
                                if (fi != null && fi is CamadaDez)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaNove)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaOito)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaSete)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1);
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
                        arr = Arr.RetornarArray(story.Filtro, story, 2, (long)f.Id, 1, 1);
                        if (arr != null)
                        {
                            arr = Arr.RetornarArray(story.Filtro, story, 3, (long)f.Id, 1, 1, 1, 1, 1);
                            if (arr[3] == subgrupo &&
                              arr[2] == grupo && arr[1] == substory && arr[4] < camada)
                            {
                                var fi = story.Filtro.Where(fil => fil.Pagina
                                 .FirstOrDefault(p => p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaDez ||
                                 p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaNove ||
                                  p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaOito ||
                                  p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaSete ||
                                  p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaSeis) != null)
                                 .LastOrDefault()!;
                                if (fi != null && fi is CamadaDez)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaNove)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaOito)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaSete)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaSeis)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1);
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
                        arr = Arr.RetornarArray(story.Filtro, story, 2, (long)f.Id, 1, 1);
                        if (arr != null)
                        {
                            arr = Arr.RetornarArray(story.Filtro, story, 3, (long)f.Id, 1, 1, 1, 1);
                            if (arr[2] == grupo && arr[1] == substory && arr[3] < camada)
                            {
                                var fi = story.Filtro.Where(fil => fil.Pagina
                                 .FirstOrDefault(p => p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaDez ||
                                 p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaNove ||
                                  p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaOito ||
                                  p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaSete ||
                                  p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaSeis ||
                                 p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is SubSubGrupo) != null)
                                 .LastOrDefault()!;
                                if (fi != null && fi is CamadaDez)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaNove)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaOito)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaSete)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaSeis)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is SubSubGrupo)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1);
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
                        arr = Arr.RetornarArray(story.Filtro, story, 2, (long)f.Id, 1, 1);
                        if (arr != null)
                        {
                            arr = Arr.RetornarArray(story.Filtro, story, 3, (long)f.Id, 1, 1, 1);
                            if (arr[1] == substory && arr[2] < camada)
                            {
                                var fi = story.Filtro.Where(fil => fil.Pagina
                                 .FirstOrDefault(p => p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaDez ||
                                 p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaNove ||
                                  p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaOito ||
                                  p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaSete ||
                                  p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaSeis ||
                                 p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is SubSubGrupo ||
                                 p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is SubGrupo) != null)
                                 .LastOrDefault()!;
                                if (fi != null && fi is CamadaDez)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaNove)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaOito)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaSete)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaSeis)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is SubSubGrupo)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1);
                                if (fi != null && fi is SubGrupo)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1);
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
                        arr = Arr.RetornarArray(story.Filtro, story, 2, (long)f.Id, 1, 1);
                        if (arr != null)
                        {
                            arr = Arr.RetornarArray(story.Filtro, story, 3, (long)f.Id, 1, 1);
                            if (arr[1] < camada)
                            {
                                var fi = story.Filtro.Where(fil => fil.Pagina
                                 .FirstOrDefault(p => p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaDez ||
                                 p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaNove ||
                                  p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaOito ||
                                  p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaSete ||
                                  p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is CamadaSeis ||
                                 p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is SubSubGrupo ||
                                 p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is SubGrupo ||
                                 p.Pagina!.Versiculo ==
                                 f.Pagina!.First().Pagina!.Versiculo && f is Grupo) != null)
                                 .LastOrDefault()!;
                                if (fi != null && fi is CamadaDez)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaNove)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaOito)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaSete)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is CamadaSeis)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1, 1);
                                if (fi != null && fi is SubSubGrupo)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1, 1);
                                if (fi != null && fi is SubGrupo)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1, 1);
                                if (fi != null && fi is Grupo)
                                    arr = Arr.RetornarArray(story.Filtro, story, 3, (long)fi.Id, 1, 1, 1);
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
            var u = await userManager.GetUserAsync(user);
            UserModel usuario = await Context.Users.Include(u => u.PageLiked)
            .FirstAsync(us => us.Id == u.Id);
            PageLiked pageLiked = new PageLiked();
            if (!Content)
                pageLiked.PaginaId = Model.Id;
            else
                pageLiked.ContentId = Model.Id;

            await Context.AddAsync(pageLiked);
            await Context.SaveChangesAsync();
            usuario.curtir(pageLiked);
            await Context.SaveChangesAsync();
            repositoryPagina.paginasCurtidas.Add(pageLiked);

           // acessar();
        }

        protected async Task Unlike(long Id)
        {
            PageLiked? page = null;
            if (conteudo == 0)            
                 page = await Context.PageLiked
                                .FirstOrDefaultAsync(p => p.ContentId == Model.Id);
            else
                page = await Context.PageLiked
                                   .FirstOrDefaultAsync(p => p.PaginaId == Model.Id);


            if (page != null)
            {
                Context.Remove(page);
                await Context.SaveChangesAsync();
                repositoryPagina.paginasCurtidas.Remove(page);
            }
             acessar();
        }

        protected async void acessarPreferenciasUsuario(string usu)
        {
            usuarios!.Clear();
            automatico = false;
            outroHorizonte = 3;
            compartilhante = usu;
            indice = 1;        
            acessar();
        }

        protected void alterarQuery(ChangeEventArgs e)
        {
            opcional = e.Value!.ToString()!;

            try
            {
                var num = int.Parse(opcional);
            }
            catch (Exception ex)
            {
                foreach (var item in userManager.Users)
                    usuarios.Add(new UserPreferencesImage { user = item.UserName, UserModel = item });

                if (string.IsNullOrEmpty(opcional))
                {
                    usuarios.Clear();
                    acessar();
                }
            }
        }

        protected void acessarVerso()
        {
            indice_Filtro = 0;
            setarCamadas(null);
            indice = (int) vers!;
            acessar();
        }

        protected async void redirecionarMarcar()
        {
            automatico = false;
            string prompted = await js.InvokeAsync<string>("prompt", "Informe o usuario (Opcional).");
            List<Filtro> fils = null;
            Filtro fi = null;

            var quant = story!.Filtro!.Where(f => f.user == null).ToList().Count;
                if (substory == null)
                    opcional = indice.ToString();
                else
                {
                    var lista = retornarListaFiltrada(null);
                    opcional = vers.ToString();
                }
                if (string.IsNullOrEmpty(prompted))
                    fils = story!.Filtro!.OrderBy(f => f.Id).ToList();
                else
                    fils = story!.Filtro!.Where(f => f.user == prompted).OrderBy(f => f.Id).ToList();

                if (fils.Count == 0)
                    fils = story!.Filtro!.OrderBy(f => f.Id).ToList();

            if(quant != 0)
            {
                fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => p.Pagina!.Versiculo == int.Parse(opcional!)) != null)
                        .LastOrDefault()!;

                if (fi == null)
                {
                    fils = story!.Filtro!.OrderBy(f => f.Id).ToList();
                    fi = fils.Where(f => f.Pagina
                    .FirstOrDefault(p => p.Pagina!.Versiculo == int.Parse(opcional!)) != null)
                           .LastOrDefault()!;
                }
            }
            else if(quant == 0)
            {
                // 1º time
                fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => p.Pagina!.Versiculo == int.Parse(opcional!)) != null &&
                f is CamadaSete).LastOrDefault()!;
                if(fi == null)
                    fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => p.Pagina!.Versiculo == int.Parse(opcional!)) != null &&
                f is CamadaSeis).LastOrDefault()!;
                if (fi == null)
                    fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => p.Pagina!.Versiculo == int.Parse(opcional!)) != null &&
                f is SubSubGrupo).LastOrDefault()!;
                if (fi == null)
                    fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => p.Pagina!.Versiculo == int.Parse(opcional!)) != null &&
                f is SubGrupo).LastOrDefault()!;
                if (fi == null)
                    fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => p.Pagina!.Versiculo == int.Parse(opcional!)) != null &&
                f is Grupo).LastOrDefault()!;
                if (fi == null)
                    fi = fils.Where(f => f.Pagina
                .FirstOrDefault(p => p.Pagina!.Versiculo == int.Parse(opcional!)) != null &&
                f is SubStory).LastOrDefault()!;
            }



            if (fi == null)
            {
                if(quant == 0 && !string.IsNullOrEmpty(prompted))
                await js!.InvokeAsync<object>("DarAlert",
                    $"A pasta do usuario {prompted} não pussui este verso!!!");
                  else
                await js!.InvokeAsync<object>("DarAlert", "Marque um versiculo que tenha pasta!!!");
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
            Auto = 0;
            if (substory == null)
                await TourService.StartTour("FormGuidedTour1");
            else
                await TourService.StartTour("FormGuidedTour2");
        }

        protected async void share()
        {
            Auto = 0;
            if (compartilhante == "comp" || title == null || resumo == null)
            {
                if (compartilhante == "comp")
                {
                    compartilhante = Context.Users
                    .FirstOrDefault(u => u.UserName == "Leandro01832")!.UserName;
                }
                if (title == null)
                {
                    title = await js.InvokeAsync<string>("prompt", "Informe o titulo da pagina.");
                }

                if (resumo == null)
                {
                    resumo = await js.InvokeAsync<string>("prompt", "Informe o resumo da pagina.");
                }
                await js!.InvokeAsync<object>("DarAlert", $"Agora Compartilhe!!!");

                    if(substory != null)
                    {
                            if(user.Identity!.IsAuthenticated)
                            
                                compartilhou = user.Identity.Name;
                            
                            else
                                compartilhou = "comp";

                        
                    }
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
                acessar($"/comentario/{capitulo}/{indice}");
            }
            else
            {
                acessar($"/comentario/{capitulo}/{vers}");

            }
        }

        protected void acessarPastas()
        {
            automatico = false;
            if (substory == null)
            {
                acessar($"/pastas/{capitulo}/{indice}/{dominio}/{compartilhante}");
            }
            else
            {
                acessar($"/pastas/{capitulo}/{vers}/{dominio}/{compartilhante}");

            }
        }

        protected void acessarCapitulos()
        {
            indice = capitulo;
            capitulo = 0;
            outroHorizonte = 0;
            setarCamadas(null);

            acessar();
        }

        protected void acessarPastasUser()
        {
            outroHorizonte = 3;
            indice = 1;
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
            outroHorizonte = 1;
            capitulo = indice;
            automatico = false;
            indice = 1;
            setarCamadas(null);

            acessar();
        }

        private void acessar(string url2 = null)
        {
            if (url2 != null) Auto = 0;

            if(url2 == null)
            {
                Auto = Convert.ToInt32(automatico);

                if (Content && conteudo == 0) indice = 1;

                conteudo = Convert.ToInt32(Content);

                string url = null;
                if (camadadez != null)
                    url = $"/camada10/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{camadadez}/{indice}/{Auto}/{timeproduto}/{conteudo}/{indiceLivro}/{retroceder}/{dominio}/{compartilhou}/{compartilhante}/{compartilhante2}/{compartilhante3}/{compartilhante4}/{compartilhante5}/{compartilhante6}";
                else if (camadanove != null)
                    url = $"/camada9/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{camadanove}/{indice}/{Auto}/{timeproduto}/{conteudo}/{indiceLivro}/{retroceder}/{dominio}/{compartilhou}/{compartilhante}/{compartilhante2}/{compartilhante3}/{compartilhante4}/{compartilhante5}/{compartilhante6}";
                else if (camadaoito != null)
                    url = $"/camada8/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{camadaoito}/{indice}/{Auto}/{timeproduto}/{conteudo}/{indiceLivro}/{retroceder}/{dominio}/{compartilhou}/{compartilhante}/{compartilhante2}/{compartilhante3}/{compartilhante4}/{compartilhante5}/{compartilhante6}";
                else if (camadasete != null)
                    url = $"/camada7/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{camadasete}/{indice}/{Auto}/{timeproduto}/{conteudo}/{indiceLivro}/{retroceder}/{dominio}/{compartilhou}/{compartilhante}/{compartilhante2}/{compartilhante3}/{compartilhante4}/{compartilhante5}/{compartilhante6}";
                else if (camadaseis != null)
                    url = $"/camada6/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{camadaseis}/{indice}/{Auto}/{timeproduto}/{conteudo}/{indiceLivro}/{retroceder}/{dominio}/{compartilhou}/{compartilhante}/{compartilhante2}/{compartilhante3}/{compartilhante4}/{compartilhante5}/{compartilhante6}";
                else if (subsubgrupo != null)
                    url = $"/camada5/{capitulo}/{substory}/{grupo}/{subgrupo}/{subsubgrupo}/{indice}/{Auto}/{timeproduto}/{conteudo}/{indiceLivro}/{retroceder}/{dominio}/{compartilhou}/{compartilhante}/{compartilhante2}/{compartilhante3}/{compartilhante4}/{compartilhante5}/{compartilhante6}";
                else if (subgrupo != null)
                    url = $"/camada4/{capitulo}/{substory}/{grupo}/{subgrupo}/{indice}/{Auto}/{timeproduto}/{conteudo}/{indiceLivro}/{retroceder}/{dominio}/{compartilhou}/{compartilhante}/{compartilhante2}/{compartilhante3}/{compartilhante4}/{compartilhante5}/{compartilhante6}";
                else if (grupo != null)
                    url = $"/camada3/{capitulo}/{substory}/{grupo}/{indice}/{Auto}/{timeproduto}/{conteudo}/{indiceLivro}/{retroceder}/{dominio}/{compartilhou}/{compartilhante}/{compartilhante2}/{compartilhante3}/{compartilhante4}/{compartilhante5}/{compartilhante6}";
                else if (substory != null)
                    url = $"/camada2/{capitulo}/{substory}/{indice}/{Auto}/{timeproduto}/{conteudo}/{indiceLivro}/{retroceder}/{dominio}/{compartilhou}/{compartilhante}/{compartilhante2}/{compartilhante3}/{compartilhante4}/{compartilhante5}/{compartilhante6}";
                else
                    url = $"/Renderizar/{capitulo}/{indice}/{Auto}/{timeproduto}/{outroHorizonte}/{indiceLivro}/{retroceder}/{dominio}/{compartilhou}/{compartilhante}/{compartilhante2}/{compartilhante3}/{compartilhante4}/{compartilhante5}/{compartilhante6}";

                if (compartilhante != "comp" && outroHorizonte == 0 && pontos == null) url += "/1";


                navigation!.NavigateTo(url);
            }
            else            
                navigation!.NavigateTo(url2);

            

        }

    }

    public class UserPreferencesImage
    {
        public string? user { get; set; }
        public UserModel UserModel { get; set; }
    }
}

            