using BlazorServerCms.Pages;
using BlazorServerCms.servicos;
using business;
using business.business;
using business.Group;
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


        public async Task<List<FiltroContent>> PaginarFiltro(long filtroId, int quantidadeLista, int quantDiv, int slideAtual, int? carregando = null)
        {
            Context = db.CreateDbContext(null);
            List<FiltroContent> conteudos;
            var fil = await Context.Filtro!.FirstAsync(f => f.Id ==  filtroId);
            if (quantidadeLista > 1999)
            {
                int carregar = 0;
                if (carregando != null && carregando != 0 && carregando < repositoryPagina.quantSlidesCarregando)
                    carregar = (int)carregando;
                else carregar = repositoryPagina.quantSlidesCarregando;

                    conteudos = await Context!.FiltroContent!.OrderBy(p => p.ContentId)
                .Include(c => c.Filtro)
                .Include(c => c.Content)
                .ThenInclude(c => c.Produto)
               .ThenInclude(c => c.Produto)
                .Where(c => c.Content is Pagina && fil.Id == filtroId)
                .Skip(quantDiv * slideAtual).Take(quantDiv * carregar)
                .ToListAsync();
            }
            else
                conteudos = await Context!.FiltroContent!.OrderBy(p => p.ContentId)
               .Include(c => c.Filtro)
               .Include(c => c.Content)
               .ThenInclude(c => c.Produto)
               .ThenInclude(c => c.Produto)
               .Where(c => c.Content is Pagina && c.FiltroId == filtroId)
               .ToListAsync();

            foreach (var item in conteudos.Select(c => c.Content).ToList())
                if (RepositoryPagina.Conteudo.FirstOrDefault(c => c.Id == item!.Id) == null)
                    RepositoryPagina.Conteudo.Add(item!);

            return conteudos;
        }

        public async Task<List<Content>> PaginarStory(long storyId, int quantidadeLista, int quantDiv, int slideAtual, int? carregando = null )
        {
            Context = db.CreateDbContext(null);
            List<Content> conteudos;
            if (quantidadeLista > 999)
            {
                int carregar = 0;
                if (carregando != null && carregando != 0 && carregando < repositoryPagina.quantSlidesCarregando)
                    carregar = (int)carregando;
                else carregar = repositoryPagina.quantSlidesCarregando;

                    conteudos = await Context!.Content!.OrderBy(p => p.Id)
               .Include(c => c.Filtro)
               .Include(c => c.Produto)
               .ThenInclude(c => c.Produto)
               .Where(c => c is Pagina && c.StoryId == storyId)
               .Skip(quantDiv * slideAtual).Take(quantDiv * carregar)
               .ToListAsync();
            }
            else
                conteudos = await Context!.Content!.OrderBy(p => p.Id)
               .Include(c => c.Filtro)
               .Include(c => c.Produto)
               .ThenInclude(c => c.Produto)
               .Where(c => c is Pagina && c.StoryId == storyId)
               .ToListAsync();

            foreach (var item in conteudos)
                if (RepositoryPagina.Conteudo.FirstOrDefault(c => c.Id == item.Id) == null)
                    RepositoryPagina.Conteudo.Add(item!);

            return conteudos;
        }

        public async Task<List<Content>> GetFiltroByIdAsync(long filtroId)
        {
            List<Content> lista = await Context!.Content!.OrderBy(p => p.Id)
                .Include(c => c.Filtro)
               .Where(c => c is Pagina && c.Filtro != null &&
                    c.Filtro!.FirstOrDefault(f => f.FiltroId == filtroId) != null)
               .ToListAsync();    

            return  lista;
        }

        public async Task<Story> GetStoryByIdAsync(long storyId)
        {
            Story story = await Context.Story!
            .Include(p => p.Filtro)!
            .ThenInclude(p => p.Pagina)!
            .Include(p => p.Filtro)!
            .ThenInclude(p => p.usuarios)!
            .OrderBy(st => st.Id)
            .FirstAsync(st => st.Id == storyId);
            return story;
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

        public bool HasFiltersAsync(long storyId)
        {
            var _TotalRegistros = 0;
            try
            {
                using (var con = new SqlConnection(ApplicationDbContext._connectionString))
                {
                    SqlCommand cmd = null;
                    cmd = new SqlCommand($"SELECT COUNT(*) FROM Filtro as P  where P.StoryId={storyId}", con);

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

        public int CountLikesAsync(long ContentId)
        {
            var _TotalRegistros = 0;
            try
            {
                using (var con = new SqlConnection(ApplicationDbContext._connectionString))
                {
                    SqlCommand cmd = null;
                    cmd = new SqlCommand($"SELECT COUNT(*) FROM PageLiked as P  where P.capitulo={ContentId} ", con);

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

        public int CountPagesInFilterAsync(long filtroId)
        {
            var _TotalRegistros = 0;
            try
            {
                using (var con = new SqlConnection(ApplicationDbContext._connectionString))
                {
                    SqlCommand cmd = null;
                    cmd = new SqlCommand($"SELECT COUNT(*) FROM FiltroContent as FC " +
                             " inner join Content as C on FC.ContentId=C.Id where " +
                            $" FC.FiltroId={filtroId} and C.Discriminator='Pagina' or " +
                            $" FC.FiltroId={filtroId} and C.Discriminator='AdminContent' or " +
                            $" FC.FiltroId={filtroId} and C.Discriminator='ProductContent' or " +
                            $" FC.FiltroId={filtroId} and C.Discriminator='ChangeContent'  "
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
       
        public int CountPagesAsync(long storyId)
        {
            var _TotalRegistros = 0;
            try
            {
                using (var con = new SqlConnection(ApplicationDbContext._connectionString))
                {
                    SqlCommand cmd = null;                    
                    
                        cmd = new SqlCommand($"SELECT COUNT(*) FROM Content as P " +
                            $" where P.StoryId={storyId} and P.Discriminator='Pagina' or " +
                            $" P.StoryId={storyId} and P.Discriminator='AdminContent' or " +
                            $" P.StoryId={storyId} and P.Discriminator='ProductContent' or " +
                            $" P.StoryId={storyId} and P.Discriminator='ChangeContent'  ", con);
                        con.Open();
                        _TotalRegistros = int.Parse(cmd.ExecuteScalar().ToString()!);
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
