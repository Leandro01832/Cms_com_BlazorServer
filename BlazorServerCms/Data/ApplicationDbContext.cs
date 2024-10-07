using business;
using business.business;
using business.business.Book;
using business.Group;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorServerCms.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserModel>
    {
        public static string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=cms;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

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

        public DbSet<Segue> Segue { get; set; }
        public DbSet<Content> Content { get; set; }
        public DbSet<Pergunta> Pergunta { get; set; }
        public DbSet<UserResponse> UserResponse { get; set; }
        public DbSet<PageLiked> PageLiked { get; set; }
        public DbSet<Estante> Instante { get; set; }
        public DbSet<FiltroPagina> FiltroPagina { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<savedFolder> savedFolder { get; set; }
        public DbSet<Rota>? Rota { get; set; }
        public DbSet<Filtro>? Filtro { get; set; }
        public DbSet<Pagina>? Pagina { get; set; }
        public DbSet<Produto>? Produto { get; set; }
        public DbSet<ImagemProduto>? ImagemProduto { get; set; }
        public DbSet<Pedido>? Pedido { get; set; }
        public DbSet<ItemPedido>? ItemPedido { get; set; }
        public DbSet<SubSubGrupo>? SubSubGrupo { get; set; }
        public DbSet<SubGrupo>? SubGrupo { get; set; }
        public DbSet<Grupo>? Grupo { get; set; }
        public DbSet<SubStory>? SubStory { get; set; }
        public DbSet<Story>? Story { get; set; }
        public DbSet<Telefone>? Telefone { get; set; }
        public DbSet<Livro>? Livro { get; set; }
        public DbSet<Comentario>? Comentario { get; set; }
        public DbSet<CamadaSeis>? CamadaSeis { get; set; }
        public DbSet<CamadaSete>? CamadaSete { get; set; }
        public DbSet<CamadaOito>? CamadaOito { get; set; }
        public DbSet<CamadaNove>? CamadaNove { get; set; }
        public DbSet<CamadaDez>? CamadaDez { get; set; }
        public DbSet<Compartilhante>? Compartilhante { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<FiltroPagina>()
          .HasKey(p => new { p.FiltroId, p.PaginaId });


        }
    }
}