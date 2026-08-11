using BlazorServerCms.Areas.Identity;
using BlazorServerCms.Data;
using BlazorServerCms.servicos;
using business.business.conteudo;
using business.business.sistema;
using business.business.Group;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PSC.Blazor.Components.Tours;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;
using MercadoPago.Client.Preference;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;


var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";


// 3. Lê a string de conexão e substitui o placeholder pelo caminho real
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    ApplicationDbContext._connectionString = connectionString;

// Add services to the container.

//builder.Services.AddScoped(sp => new OllamaSharp.OllamaApiClient("http://localhost:11434"));
builder.Services.AddScoped<IStoryService, StoryService>();
builder.Services.AddScoped<MarcacaoVideoFilter>();
builder.Services.AddScoped<N8nService>();
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton<RepositoryPagina>();
builder.Services.AddSingleton<LiveKitService>();
builder.Services.AddSingleton<PixService>();
builder.Services.AddSingleton<BlazorTimer>();
// var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
 //  options.UseSqlServer(connectionString));
   //  builder.Services.AddDbContext<ApplicationDbContext>(options => 
   //  options.UseSqlServer(connectionString));
     builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<UserModel, IdentityRole>(options => 
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();


builder.Services.AddRazorPages();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<UserModel>>();
builder.Services.AddScoped<IEmailSender<UserModel>, EmailSender>();

builder.Services.UseTour();

builder.Services.AddControllersWithViews();


builder.Services.AddServerSideBlazor().AddHubOptions(options =>
{
    options.MaximumReceiveMessageSize = 64 * 1024;
});

// 1. Configuração do serviço de Autenticação
builder.Services.AddAuthentication(options =>
{
    // Define os esquemas padrão para o ASP.NET gerenciar o login via Cookie + Google
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddGoogle(options =>
{
    // Forma simplificada usando builder.Configuration
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;

    // Mapeia a chave "picture" da resposta JSON do Google para uma Claim chamada "picture"
        options.ClaimActions.MapJsonKey("picture", "picture");

        // Outros exemplos de claims que podem ser mapeadas do payload do Google:
        options.ClaimActions.MapJsonKey("locale", "locale");
        options.ClaimActions.MapJsonKey("given_name", "given_name");
        options.ClaimActions.MapJsonKey("family_name", "family_name");
        options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.Name, "name");
        options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.Email, "email");
});

builder.Services.AddMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpClient("n8nClient", client =>
{
    // A URL base deve terminar com /
    client.BaseAddress = new Uri("https://leandro01832.app.n8n.cloud/"); 
    client.DefaultRequestHeaders.Add("leandro", "Leandro01832");
}); 

try
{
    


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    // Isso cria o arquivo .db e as tabelas se eles não existirem
   // context.Database.EnsureCreated(); 
}

app.UseSession();



var webSocketOptions = new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromSeconds(120),
};

app.UseWebSockets(webSocketOptions);

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

// https://leandro01832.app.n8n.cloud/webhook-test/e605c19c-794b-4043-ba84-0f48322cdef3
//https://leandro01832.app.n8n.cloud/webhook-test/e605c19c-794b-4043-ba84-0f48322cdef3

using (var scope = app.Services.CreateScope())
{
    var contexto = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var repositoryPagina = scope.ServiceProvider.GetRequiredService<RepositoryPagina>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserModel>>();
    var email = builder.Configuration.GetConnectionString("Email");
    var password = builder.Configuration.GetConnectionString("Senha");
    var userASP = await userManager.FindByNameAsync(email);

    MercadoPagoConfig.AccessToken = repositoryPagina.buscarApiMercadoPago();

   


    // var lista = await repositoryPagina.buscarPatternStory();

    if (await contexto!.Set<Story>().AnyAsync())
    {
        List<Story> stories = await contexto.Story!
        .OrderBy(st => st.Capitulo)
        .ToListAsync();
        RepositoryPagina.stories.AddRange(stories);
    }
    else
    {
        // foreach (var item in lista!)
        //     contexto.Add(item);
        // contexto.SaveChanges();
    }

    if (await contexto!.Set<Content>().AnyAsync())
    {
        var conteudos = await contexto.UserContent
        .Include(f => f.Filtro)
        .Include(f => f.UserModel)
        .Where(c => c.Data > DateTime.Now.AddDays(-repositoryPagina.dias)
         || c.QuantLiked > 100000 || c.QuantShared > 100000)
        .OrderBy(co => co.Id).ToListAsync();
        RepositoryPagina.Conteudo!.UnionWith(conteudos);
    }


   


    string[] rolesNames = { "Admin", "Manager", "Assinante" };
    IdentityResult result;

    foreach (var namesRole in rolesNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(namesRole);
        if (!roleExist)
        {
            result = await roleManager.CreateAsync(new IdentityRole(namesRole));
        }
    }

    if (userASP == null)
    {
        var user = new UserModel()
        {
            UserName = "leandro01832",
            Email = email,
            EmailConfirmed = true,
            HashUserName = BCrypt.Net.BCrypt.HashPassword("leandro01832")
        };
        await userManager.CreateAsync(user, password);
        await userManager.AddToRoleAsync(user, "Admin");
    }

    



}

app.Run();

}
catch (Exception ex)
{
    System.IO.File.WriteAllText("LOG_ERRO_CRITICO.txt", ex.ToString());
    throw;
}

