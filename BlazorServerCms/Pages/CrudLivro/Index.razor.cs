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

        private DemoContextFactory db = new DemoContextFactory();
        private ApplicationDbContext Context;

        protected override async Task OnInitializedAsync()
        {
            Context = db.CreateDbContext(null);
        }


        protected async void DeletarLivro(long Id)
        {
            var livro = await Context.Livro!.FirstAsync(l => l.Id == Id);
            Context.Remove(livro);
            await Context.SaveChangesAsync();
            //confirma.Exibir();
            //codigoCategoria = categoriaId;
        }
    }
}
