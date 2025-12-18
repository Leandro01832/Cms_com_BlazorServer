using business;
using business.business;
using business.business.Book;
using business.business.Group;
using business.Group;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorServerCms.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserModel>
    {
        public static string _connectionString = "Server=DESKTOP-7FH9109\\SQLEXPRESS;Database=cms;Trusted_Connection=True;";

        public ApplicationDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Criterio> Criterio { get; set; }
        public DbSet<Camada> Camada { get; set; }
        public DbSet<SubFiltro> SubFiltro { get; set; }
        public DbSet<MarcacaoVideoFilter> MarcacaoVideoFilter { get; set; }
        public DbSet<VideoFilter> VideoFilter { get; set; }
        public DbSet<MudancaEstado> MudancaEstado { get; set; }
        public DbSet<Time> Time { get; set; }
        public DbSet<Content> Content { get; set; }
        public DbSet<Pagina>? Pagina { get; set; }
        public DbSet<Chave> Chave { get; set; }
        public DbSet<ProductContent> ProductContent { get; set; }
        public DbSet<ChangeContent> ChangeContent { get; set; }
        public DbSet<AdminContent> AdminContent { get; set; }
        public DbSet<UserContent> UserContent { get; set; }
        public DbSet<ProdutoConteudo> ProdutoConteudo { get; set; }
        public DbSet<FiltroContent> FiltroContent { get; set; }
        public DbSet<UserModelTime> UserModelTime { get; set; }
        public DbSet<UserModelLivro> UserModelLivro { get; set; }
        public DbSet<UserModelFiltro> UserModelFiltro { get; set; }
        public DbSet<UserModelContent> UserModelPageLiked { get; set; }
        public DbSet<UserModelPastaSalva> UserModelPastaSalva { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<Rota>? Rota { get; set; }
        public DbSet<Filtro>? Filtro { get; set; }
        public DbSet<Produto>? Produto { get; set; }
        public DbSet<ImagemProduto>? ImagemProduto { get; set; }
        public DbSet<Pedido>? Pedido { get; set; }
        public DbSet<ItemPedido>? ItemPedido { get; set; }       
        public DbSet<Story>? Story { get; set; }
        public DbSet<ShortStory>? ShortStory { get; set; }
        public DbSet<SmallStory>? SmallStory { get; set; }
        public DbSet<PatternStory>? PatternStory { get; set; }
        public DbSet<Telefone>? Telefone { get; set; }
        public DbSet<Livro>? Livro { get; set; }
        public DbSet<Comment>? Comentario { get; set; }
       
        public DbSet<Compartilhante>? Compartilhante { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {


            base.OnModelCreating(builder);

            builder.Entity<ProdutoConteudo>()
         .HasKey(p => new { p.ProdutoId, p.ContentId });

            builder.Entity<FiltroContent>()
          .HasKey(p => new { p.FiltroId, p.ContentId });

            builder.Entity<UserModelTime>()
         .HasKey(p => new { p.UserModelId, p.TimeId });
            
            builder.Entity<UserModelLivro>()
         .HasKey(p => new { p.UserModelId, p.LivroId });
            
            builder.Entity<UserModelFiltro>()
         .HasKey(p => new { p.UserModelId, p.FiltroId });
            
            builder.Entity<UserModelContent>()
         .HasKey(p => new { p.UserModelId, p.ContentId });
            
            builder.Entity<UserModelPastaSalva>()
         .HasKey(p => new { p.UserModelId, p.PastaSalvaId });

            builder.Entity<Time>()
            .HasIndex(u => u.nome)
            .IsUnique();

        }
    }
}