using System.Collections.Generic;
using BlazorServerCms.Pages;
using BlazorServerCms.servicos;
using business;
using business.business;
using business.business.Book;
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

        public async Task<List<FiltroContent>> PaginarFiltro(long filtroId, int quantidadeLista, int quantDiv, int slideAtual, Livro livro, int? carregando = null)
        {
            Context = db.CreateDbContext(null);
            List<FiltroContent> conteudos;
           
                int carregar = 0;
                if (carregando != null && carregando != 0 && carregando < repositoryPagina.quantSlidesCarregando)
                    carregar = (int)carregando;
                else carregar = repositoryPagina.quantSlidesCarregando;

                if(livro == null)
                    conteudos = await Context!.FiltroContent!.OrderBy(p => p.ContentId)
                .Include(c => c.Filtro)
                .Include(c => c.Content)
                .ThenInclude(c => c.Produto)
               .ThenInclude(c => c.Produto)
               .Include(c => c.Content)
               .ThenInclude(c => c.Comentario)
                .Where(c => c.Content is Pagina &&
                 c.FiltroId == filtroId &&
                 c.Content.LivroId == null)
                .Skip(quantDiv * slideAtual).Take(quantDiv * carregar)
                .ToListAsync();
                else
                    conteudos = await Context!.FiltroContent!.OrderBy(p => p.ContentId)
               .Include(c => c.Filtro)
               .Include(c => c.Content)
               .ThenInclude(c => c.Produto)
              .ThenInclude(c => c.Produto)
              .Include(c => c.Content)
               .ThenInclude(c => c.Comentario)
               .Where(c => c.Content is Pagina
                && c.FiltroId == filtroId
                 && c.Content.LivroId == livro.Id)
               .Skip(quantDiv * slideAtual).Take(quantDiv * carregar)
               .ToListAsync();
            
           
            foreach (var item in conteudos.Select(c => c.Content).ToList())
                if (RepositoryPagina.Conteudo!.FirstOrDefault(c => c.Id == item!.Id) == null)
                    RepositoryPagina.Conteudo!.Add(item!);

            return conteudos;
        }

        public async Task<List<Content>> PaginarStory(long storyId, int quantidadeLista, int quantDiv, int slideAtual, Livro livro, int? carregando = null )
        {
            Context = db.CreateDbContext(null);
            List<Content> conteudos;
            if (quantidadeLista > 999)
            {
                int carregar = 0;
                if (carregando != null && carregando != 0 && carregando < repositoryPagina.quantSlidesCarregando)
                    carregar = (int)carregando;
                else carregar = repositoryPagina.quantSlidesCarregando;

                if(livro == null)
                    conteudos = await Context!.Content!.OrderBy(p => p.Id)
               .Include(c => c.Filtro)
               .Include(c => c.Comentario)
               .Include(c => c.Produto)
               .ThenInclude(c => c.Produto)
               .Where(c => c is Pagina && c.StoryId == storyId && c.LivroId == null)
               .Skip(quantDiv * slideAtual).Take(quantDiv * carregar)
               .ToListAsync();
                else
                    conteudos = await Context!.Content!.OrderBy(p => p.Id)
              .Include(c => c.Filtro)
              .Include(c => c.Produto)
              .ThenInclude(c => c.Produto)
              .Where(c => c is Pagina && c.StoryId == storyId && c.LivroId == livro.Id)
              .Skip(quantDiv * slideAtual).Take(quantDiv * carregar)
              .ToListAsync();

            }
            else
            {
                if (livro == null)
                conteudos = await Context!.Content!.OrderBy(p => p.Id)
               .Include(c => c.Filtro)
               .Include(c => c.Comentario)
               .Include(c => c.Produto)
               .ThenInclude(c => c.Produto)
               .Where(c => c is Pagina && c.StoryId == storyId && c.LivroId == null)
               .ToListAsync();
                else
                conteudos = await Context!.Content!.OrderBy(p => p.Id)
               .Include(c => c.Filtro)
               .Include(c => c.Produto)
               .ThenInclude(c => c.Produto)
               .Where(c => c is Pagina && c.StoryId == storyId && c.LivroId == livro.Id)
               .ToListAsync();

            }

            foreach (var item in conteudos)
                if (RepositoryPagina.Conteudo!.FirstOrDefault(c => c.Id == item.Id) == null)
                    RepositoryPagina.Conteudo!.Add(item!);

            return conteudos;
        }

        public async Task<List<Content>> GetFiltroByIdAsync(long filtroId, Livro livro, int slide = 0, int quantDiv = 0)
        {
            List<Content> resultados = null;
            int Count = CountPagesInFilterAsync(filtroId, livro);
            if(Count < 1000)
            {
                if (livro == null)
                    resultados = await Context.Content                       
                         .Include(c => c.Filtro)
                       .OrderBy(c => c.Id)
                       .Where(f => f.Filtro.FirstOrDefault(fi => fi.FiltroId == filtroId) != null &&
                        f.LivroId == null)
                       .AsNoTracking().ToListAsync();
                else
                    resultados = await Context.Content                         
                         .Include(c => c.Filtro)
                   .OrderBy(c => c.Id)
                   .Where(f => f.Filtro.FirstOrDefault(fi => fi.FiltroId == filtroId) != null &&
                        f.LivroId == livro.Id)
                   .AsNoTracking().ToListAsync();
            }
            else
            {
                if (livro == null)
                    resultados = await Context.Content                       
                         .Include(c => c.Filtro)
                       .OrderBy(c => c.Id)
                       .Where(f => f.Filtro.FirstOrDefault(fi => fi.FiltroId == filtroId) != null &&
                        f.LivroId == null)
                       .Skip(slide * quantDiv).Take(quantDiv * repositoryPagina!.quantSlidesCarregando)
                       .AsNoTracking().ToListAsync();
                else
                    resultados = await Context.Content                         
                         .Include(c => c.Filtro)
                   .OrderBy(c => c.Id)
                   .Where(f => f.Filtro.FirstOrDefault(fi => fi.FiltroId == filtroId) != null &&
                        f.LivroId == livro.Id)
                   .Skip(slide * quantDiv).Take(quantDiv * repositoryPagina!.quantSlidesCarregando)
                   .AsNoTracking().ToListAsync();

            }

            foreach (var item in resultados.ToList())
                if (RepositoryPagina.Conteudo!.FirstOrDefault(c => c.Id == item!.Id) == null)
                    RepositoryPagina.Conteudo!.Add(item!);

            return resultados;
        }

        public async Task<Story> GetStoryByIdAsync(long storyId)
        {
            Story story = null;
             story = await Context.Story!
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

        public bool HasFiltersAsync(long storyId, Livro livro)
        {
            var _TotalRegistros = 0;
            try
            {
                using (var con = new SqlConnection(ApplicationDbContext._connectionString))
                {
                    SqlCommand cmd = null;
                    if(livro == null)
                    cmd = new SqlCommand($"SELECT COUNT(*) FROM Filtro as P  where P.StoryId={storyId} and p.LivroId is null", con);
                    else
                    cmd = new SqlCommand($"SELECT COUNT(*) FROM Filtro as P  where P.StoryId={storyId} and p.LivroId={livro.Id}", con);
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

        
        public int CountPagesInFilterAsync(long filtroId, Livro livro)
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
                            $" FC.FiltroId={filtroId} and C.Discriminator='Pagina' and C.LivroId is null or " +
                            $" FC.FiltroId={filtroId} and C.Discriminator='AdminContent' and C.LivroId is null or " +
                            $" FC.FiltroId={filtroId} and C.Discriminator='ProductContent' and C.LivroId is null or " +
                            $" FC.FiltroId={filtroId} and C.Discriminator='ChangeContent' and C.LivroId is null or " +
                            $" FC.FiltroId={filtroId} and C.Discriminator='Chave' and C.LivroId is null  "
                        , con);
                    else
                        cmd = new SqlCommand($"SELECT COUNT(*) FROM FiltroContent as FC " +
                             " inner join Content as C on FC.ContentId=C.Id where " +
                            $" FC.FiltroId={filtroId} and C.Discriminator='Pagina' and C.LivroId={livro.Id} or " +
                            $" FC.FiltroId={filtroId} and C.Discriminator='AdminContent' and C.LivroId={livro.Id} or " +
                            $" FC.FiltroId={filtroId} and C.Discriminator='ProductContent' and C.LivroId={livro.Id} or " +
                            $" FC.FiltroId={filtroId} and C.Discriminator='ChangeContent' and C.LivroId={livro.Id} or  " +
                            $" FC.FiltroId={filtroId} and C.Discriminator='Chave' and C.LivroId={livro.Id}  "
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
       
        public int CountPagesAsync(long storyId, Livro livro)
        {
            var _TotalRegistros = 0;
            try
            {
                using (var con = new SqlConnection(ApplicationDbContext._connectionString))
                {
                    SqlCommand cmd = null;                    
                    if(livro == null)
                        cmd = new SqlCommand($"SELECT COUNT(*) FROM Content as P " +
                            $" where P.StoryId={storyId} and P.Discriminator='Pagina' and P.LivroId is null or " +
                            $" P.StoryId={storyId} and P.Discriminator='AdminContent' and P.LivroId is null or " +
                            $" P.StoryId={storyId} and P.Discriminator='ProductContent' and P.LivroId is null or " +
                            $" P.StoryId={storyId} and P.Discriminator='ChangeContent' and P.LivroId is null or " +
                            $" P.StoryId={storyId} and P.Discriminator='Chave' and P.LivroId is null  "
                            , con);
                    else
                        cmd = new SqlCommand($"SELECT COUNT(*) FROM Content as P " +
                           $" where P.StoryId={storyId} and P.Discriminator='Pagina' and P.LivroId={livro.Id} or " +
                           $" P.StoryId={storyId} and P.Discriminator='AdminContent' and P.LivroId={livro.Id} or " +
                           $" P.StoryId={storyId} and P.Discriminator='ProductContent' and P.LivroId={livro.Id} or " +
                           $" P.StoryId={storyId} and P.Discriminator='ChangeContent' and P.LivroId={livro.Id} or " +
                           $" P.StoryId={storyId} and P.Discriminator='Chave' and P.LivroId={livro.Id}  "
                           , con);
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
