using BlazorServerCms.Data;
using BlazorServerCms.servicos;
using business;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace BlazorServerCms.Pages.CrudLivro
{
    public class IndexCamdaDezBase : ComponentBase
    {
        [Inject] public RepositoryPagina? repositoryPagina { get; set; }

       protected  Livro[] livros { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await CarregaLivros();
        }

        async Task CarregaLivros()
        {
            livros = await repositoryPagina!.Context.Livro!.ToArrayAsync();
        }

        protected async void DeletarLivro(long Id)
        {
            var livro = await repositoryPagina!.Context.Livro!.FirstAsync(l => l.Id == Id);
            repositoryPagina!.Context.Remove(livro);
            await repositoryPagina!.Context.SaveChangesAsync();
            await CarregaLivros();
            //confirma.Exibir();
            //codigoCategoria = categoriaId;
        }
    }
}
