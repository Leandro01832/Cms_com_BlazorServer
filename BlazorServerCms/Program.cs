using BlazorServerCms.Areas.Identity;
using BlazorServerCms.Data;
using business.Group;
using business;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Models;
using Newtonsoft.Json;
using BlazorServerCms.servicos;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton<RepositoryPagina>();
builder.Services.AddSingleton<BlazorTimer>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

using (var scope = app.Services.CreateScope())
{
    var contexto = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var email = builder.Configuration.GetConnectionString("Email");
    var password = builder.Configuration.GetConnectionString("Senha");
    var userASP = await userManager.FindByNameAsync(email);

    if (userASP == null)
    {
        string[] rolesNames = { "Admin" };
    IdentityResult result;
    foreach (var namesRole in rolesNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(namesRole);
        if (!roleExist)
        {
            result = await roleManager.CreateAsync(new IdentityRole(namesRole));
        }
    }    
        var user = new IdentityUser { UserName = email, EmailConfirmed = true };
        await userManager.CreateAsync(user, password);
        await userManager.AddToRoleAsync(user, "Admin");
    }

    if (!await contexto!.Set<Story>().AnyAsync())
    {

        var storyPadrao = new Story
        {
            PaginaPadraoLink = 0,
            Comentario = false,
            Nome = "Padrao"
        };

        contexto.Add(storyPadrao);
        contexto.SaveChanges();


        string[] stories =
            {
                    "brinquedos", "moveis", "roupas", "calçados", "relogios", "eletrônicos", "eletrodomesticos",
                    "celular", "instrumentos", "cosmeticos", "notbooks"
                    };
        string[] storiesEn =
            {
                    "toy", "furniture", "clothes", "shoe", "clock", "electronics", "appliance",
                    "iphone", "instrument", "cosmetic", "notbooks"
                    };

        Random randNum = new Random();
        for (var t = 0; t < stories.Length; t++)
        {

            var name = stories[t];
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri =
            new Uri($"https://serpapi.com/search.json?q={storiesEn[t]}" +
            "&tbm=shop&location=Dallas&hl=pt&gl=us&key=d0ebdc40d5e725ce2764208d4153772f10f531519c3952b626ce2f3a73191dd3"),
                Headers =
                {
                    { "accept", "application/json" },
                },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                // Console.WriteLine(body);
                Welcome livro = JsonConvert.DeserializeObject<Welcome>(body)!;

                var indice = 0;
                var lst = await contexto.Story!.Where(st => st.Nome != "Padrao").ToListAsync();

                var str = new Story();

                str.PaginaPadraoLink = lst.Count + 1;
                str.Nome = name;


                contexto.Add(str);
                await contexto.SaveChangesAsync();

                var Story = await contexto.Story!.FirstAsync(st => st.Nome == "Padrao");

                var pag = new Pagina()
                {
                    Data = DateTime.Now,
                    ArquivoMusic = "",
                    Titulo = "Story - " + str.Nome,
                    StoryId = Story.Id,
                    Sobreescrita = null,
                    SubStoryId = null,
                    GrupoId = null,
                    SubGrupoId = null,
                    SubSubGrupoId = null,
                    Layout = false,
                    Music = false,
                    Tempo = 11000,
                    Content = "<a href='#' id='LinkPadrao'> <h1> Story " + str.Nome + "</h1> </a>"
            };

                contexto.Add(pag);
                try
                {
                    contexto.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }



                for (var i = 0; i < livro.ShoppingResults!.Length; i++)
                {
                    Pagina pagina = new Pagina();
                    pagina.Produto = new Produto
                    {
                        Descricao = livro.ShoppingResults[i].Title,
                        Nome = name,
                        Imagem = new List<ImagemProduto>
                        { new ImagemProduto { ArquivoImagem = livro.ShoppingResults[i].Thumbnail!.ToString() } },
                        Preco = decimal.Parse(randNum.Next(50, 3000).ToString()),
                        QuantEstoque = 10
                    };
                    pagina.Story = str;
                    pagina.Tempo = 11000;
                    pagina.Titulo = name;
                    contexto.Pagina!.Add(pagina);
                    try
                    {
                        contexto.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    indice++;

                    if (indice == 2)
                    {
                        for (var j = 0; j <= 2; j++)
                        {
                            Pagina pagi = new Pagina();
                            pagi.Story = str;
                            pagi.Tempo = 11000;
                            pagi.Titulo = name;
                            contexto.Pagina.Add(pagi);
                            try
                            {
                                contexto.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                        indice = 0;
                    }


                }
            }
        }

    }

}




    app.Run();

