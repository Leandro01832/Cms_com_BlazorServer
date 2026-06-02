using System.Collections.Generic;
using BlazorServerCms.Pages;
using BlazorServerCms.servicos;
using business;
using business.business;
using business.business.Book;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BlazorServerCms.Data
{
    public class StoryService : IStoryService
    {
        public ApplicationDbContext Context { get; set; }
        public IConfiguration Configuration { get; }
        public RepositoryPagina repositoryPagina { get; }

        private DemoContextFactory db = new DemoContextFactory();

        public StoryService(IConfiguration configuration, RepositoryPagina RepositoryPagina)
        {
            Context = db.CreateDbContext(null);
            Configuration = configuration;
            repositoryPagina = RepositoryPagina;
        }

        public async Task<List<FiltroContent>> PaginarFiltro(long filtroId,   int slideAtual, Livro livro, int? carregando = null)
        {
            Context = db.CreateDbContext(null);
            List<FiltroContent> conteudos;
            var quantDiv = 40;
           
                int carregar = 0;
                if (carregando != null && carregando != 0 && carregando < repositoryPagina.quantSlidesCarregando)
                    carregar = (int)carregando;
                else carregar = repositoryPagina.quantSlidesCarregando;
               
                conteudos = await Context!.FiltroContent!.OrderBy(p => p.ContentId)
                .Include(c => c.Filtro)
                .Include(c => c.Content)
                .ThenInclude(c => c.Produto)
                .ThenInclude(c => c.Produto)
                .Include(c => c.Content)
                .ThenInclude(c => c.Filtro)
                .Include(c => c.Content)
                .ThenInclude(c => c.Comentario)
                .Where(c => c.Content is Pagina &&
                c.FiltroId == filtroId &&
                c.Content.LivroId == (livro != null ? livro.Id : null))
                .Skip(quantDiv * slideAtual).Take(quantDiv * carregar)
                .ToListAsync();           
           
            foreach (var item in conteudos.ToList())
                if (RepositoryPagina.Conteudo!.FirstOrDefault(c => c.Id == item!.ContentId) == null)
             RepositoryPagina.Conteudo!.Add(item!.Content!);

            return conteudos;
        }             

        public async Task<int> GetYouTubeVideoDurationAsync(string videoId)
        {
            int calculo = 0;
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Configuration.GetConnectionString("api_youtube"),
                ApplicationName = this.GetType().ToString()
            });
            var searchListRequest = youtubeService.Videos.List("snippet,contentDetails,statistics,status");
            searchListRequest.Id = videoId;
            var search = await searchListRequest.ExecuteAsync();
            var duration = search.Items[0].ContentDetails.Duration;
            if (duration.Contains("M"))
            {
                duration = duration.Replace("PT", "");
                int segundos = 0;
                var minutos = int.Parse(duration.Split('M')[0]);
                if(duration.Contains("S") == true)
                segundos = int.Parse(duration.Replace(minutos.ToString() + "M", "").Replace("S", ""));
                var minutosEmMileSegundos = minutos * 60 * 1000;
                var segundosEmMileSegundos = segundos * 1000;
                calculo = minutosEmMileSegundos + segundosEmMileSegundos;
            }
            else
                calculo = int.Parse(duration.Replace("PT", "").Replace("S", "")) * 1000;


            return calculo;
        }

        public bool HasFiltersAsync(long storyId, Livro livro)
        {
            var str = RepositoryPagina.stories.Skip(1).ToList()[(int)storyId -1];
            var _TotalRegistros = 0;
            try
            {
                using (var con = new SqlConnection(ApplicationDbContext._connectionString))
                {
                    SqlCommand cmd = null;
                    if(livro == null)
                    cmd = new SqlCommand($"SELECT COUNT(*) FROM Filtro as P  where P.StoryId={str.Id} and p.LivroId is null", con);
                    else
                    cmd = new SqlCommand($"SELECT COUNT(*) FROM Filtro as P  where P.StoryId={str.Id} and p.LivroId={livro.Id}", con);
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
        
        public int CountPagesInFilterAsync(long filtroId, Livro livro, Type type)
        {
            var _TotalRegistros = 0;
            try
            {
                using (var con = new SqlConnection(ApplicationDbContext._connectionString))
                {
                    SqlCommand cmd = null;
                    if(livro == null)
                    cmd = new SqlCommand($"SELECT COUNT(*) FROM FiltroContent as FC " +
                             " inner join Content as C on FC.ContentId=C.Id where " +
                            $" FC.FiltroId={filtroId} and C.Discriminator='{type.Name}' and C.LivroId is null "
                             , con);
                    else
                        cmd = new SqlCommand($"SELECT COUNT(*) FROM FiltroContent as FC " +
                             " inner join Content as C on FC.ContentId=C.Id where " +
                            $" FC.FiltroId={filtroId} and C.Discriminator='{type.Name}' and C.LivroId={livro.Id} "
                        , con);

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
       
      

    }
}
