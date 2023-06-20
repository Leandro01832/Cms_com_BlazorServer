using BlazorServerCms.Data;
using business;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace BlazorServerCms.Pages.CrudLivro
{
    public class IndexCamdaDezBase : ComponentBase
    {
        static string conexao = "Data Source=DESKTOP-7TI5J9C\\SQLEXPRESS;Initial Catalog=BlazorCms;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";
        ApplicationDbContext context = new ApplicationDbContext(conexao);

       protected  Livro[] livros { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await CarregaLivros();
        }

        async Task CarregaLivros()
        {
            livros = await context.Livro!.ToArrayAsync();
        }

        protected async void DeletarLivro(long Id)
        {
            var livro = await context.Livro!.FirstAsync(l => l.Id == Id);
            context.Remove(livro);
            await context.SaveChangesAsync();
            await CarregaLivros();
            //confirma.Exibir();
            //codigoCategoria = categoriaId;
        }
    }
}
