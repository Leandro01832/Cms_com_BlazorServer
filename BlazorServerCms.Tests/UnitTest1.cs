using Bunit;
using Bunit.TestDoubles;
using Xunit;
using Moq;
using BlazorServerCms.Pages; // ajuste para o namespace correto
using BlazorServerCms.servicos;
using BlazorServerCms.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PSC.Blazor.Components.Tours;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using business.business;
using Microsoft.AspNetCore.Components.Authorization;



public class UnitTest1 : TestContext
{
    public UnitTest1()
    {      

         // Aqui você registra os serviços necessários   
          // Simula uma configuração com valores em memória
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "MinhaChave", "MeuValor" },
                { "ApiUrl", "https://localhost/api" }
            })
            .Build();

        // Registra IConfiguration no container de serviços
        Services.AddSingleton<IConfiguration>(config);

        // Simula um UserStore vazio
        var userStore = new Mock<IUserStore<UserModel>>().Object;


        // Cria instância do UserManager
        var options = new Mock<IOptions<IdentityOptions>>().Object;
        var passwordHasher = new Mock<IPasswordHasher<UserModel>>().Object;
        var userValidators = new List<IUserValidator<UserModel>>();
        var passwordValidators = new List<IPasswordValidator<UserModel>>();
        var normalizer = new Mock<ILookupNormalizer>().Object;
        var errorDescriber = new IdentityErrorDescriber();
        var serviceProvider = new Mock<IServiceProvider>().Object;
        var logger = new Mock<ILogger<UserManager<UserModel>>>().Object;

        // Cria o UserManager
        var userManager = new UserManager<UserModel>(
            userStore,
            options,
            passwordHasher,
            userValidators,
            passwordValidators,
            normalizer,
            errorDescriber,
            serviceProvider,
            logger
        );

        Services.AddSingleton<UserManager<UserModel>>(userManager);
      
        Services.AddScoped<IStoryService, StoryService>();
        Services.AddScoped<MarcacaoVideoFilter>();
        Services.AddScoped<HttpClient>();
        Services.AddScoped<RepositoryPagina>();
        Services.AddScoped<BlazorTimer>();
        Services.AddScoped<ChatGpt>();
        Services.UseTour();
    }

    [Fact]
    public void CounterStartsAtZero()
    {        
        var cut = Render<Renderizar>();

        // Verifica se o HTML contém "Current count: 0"
         cut.MarkupMatches("<h1>");
    }

    [Fact]
    public void ClickingButtonIncrementsCounter()
    {
        var cut = Render<Renderizar>();

         cut.MarkupMatches("<h1>Pagina não encontrada</h1>");

        // Clica no botão
        // cut.Find("button").Click();

        // // Verifica se o contador foi incrementado
        // cut.MarkupMatches("<p>Current count: 1</p>");
    }




}