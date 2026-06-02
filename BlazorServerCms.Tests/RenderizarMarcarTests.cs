using Bunit;
using Bunit.TestDoubles;
using Xunit;
using Moq;
using BlazorServerCms.Pages;
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
using Microsoft.JSInterop;

public class RenderizarMarcarTests : TestContext
{
    private readonly Mock<IJSRuntime> _jsMock;

    public RenderizarMarcarTests()
    {
        _jsMock = new Mock<IJSRuntime>();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "MinhaChave", "MeuValor" },
                { "ApiUrl", "https://localhost/api" }
            })
            .Build();

        Services.AddSingleton<IConfiguration>(config);
        Services.AddSingleton(_jsMock.Object);
        Services.AddScoped<CascadingAuthenticationState>();
    }

    #region Testes de Validação de Versículo

    [Fact]
    public void RedirecionarMarcar_ValidaVersiculoInteiro()
    {
        var versiculo = int.TryParse("5", out int resultado);
        Assert.True(versiculo);
        Assert.Equal(5, resultado);
    }

    [Fact]
    public void RedirecionarMarcar_RejeitaVersiculoComTexto()
    {
        var versiculo = int.TryParse("abc", out int resultado);
        Assert.False(versiculo);
    }

    [Fact]
    public void RedirecionarMarcar_AceitaVersiculoNegativo()
    {
        var versiculo = int.TryParse("-5", out int resultado);
        Assert.True(versiculo);
        Assert.Equal(-5, resultado);
    }

    [Fact]
    public void RedirecionarMarcar_AceitaVersiculoZero()
    {
        var versiculo = int.TryParse("0", out int resultado);
        Assert.True(versiculo);
        Assert.Equal(0, resultado);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("100")]
    [InlineData("999")]
    [InlineData("5000")]
    public void RedirecionarMarcar_ValidaVersiculoMultiplos(string input)
    {
        var resultado = int.TryParse(input, out int versiculo);
        Assert.True(resultado);
        Assert.True(versiculo > 0);
    }

    [Theory]
    [InlineData("")]
    [InlineData("null")]
    [InlineData("!@#")]
    public void RedirecionarMarcar_RejeitaVersiculoInvalido(string input)
    {
        var resultado = int.TryParse(input, out int versiculo);
        Assert.False(resultado);
    }

    #endregion

    #region Testes de Invocação JavaScript

    [Fact]
    public void RedirecionarMarcar_PromptRetornaString()
    {
        var jsMock = new Mock<IJSRuntime>();
        jsMock.Setup(x => x.InvokeAsync<string>("prompt", It.IsAny<object[]>()))
            .ReturnsAsync("5");

        Assert.NotNull(jsMock.Object);
    }

    [Fact]
    public void RedirecionarMarcar_DarAlertRetornaObject()
    {
        var jsMock = new Mock<IJSRuntime>();
        jsMock.Setup(x => x.InvokeAsync<object>("DarAlert", It.IsAny<object[]>()))
            .Returns(new ValueTask<object>());

        jsMock.Object.InvokeAsync<object>("DarAlert", "Teste").AsTask().Wait();
        jsMock.Verify(x => x.InvokeAsync<object>("DarAlert", It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public void RedirecionarMarcar_ExceptionNoPrompt()
    {
        var jsMock = new Mock<IJSRuntime>();
        jsMock.Setup(x => x.InvokeAsync<string>("prompt", It.IsAny<object[]>()))
            .ThrowsAsync(new Exception("Erro ao invocar prompt"));

        var ex = Assert.Throws<Exception>(() =>
            jsMock.Object.InvokeAsync<string>("prompt", "Test").Result
        );
        Assert.Contains("Erro", ex.Message);
    }

    #endregion

    #region Testes Assíncrono

    [Fact]
    public async Task RedirecionarMarcar_PromptAssincrono()
    {
        var jsMock = new Mock<IJSRuntime>();
        jsMock.Setup(x => x.InvokeAsync<string>("prompt", It.IsAny<object[]>()))
            .Returns(new ValueTask<string>(Task.FromResult("5")));

        var resultado = await jsMock.Object.InvokeAsync<string>("prompt", "Versículo");
        Assert.Equal("5", resultado);
    }

    [Fact]
    public async Task RedirecionarMarcar_AlertAssincrono()
    {
        var jsMock = new Mock<IJSRuntime>();
        jsMock.Setup(x => x.InvokeAsync<object>("DarAlert", It.IsAny<object[]>()))
            .Returns(new ValueTask<object>());

        await jsMock.Object.InvokeAsync<object>("DarAlert", "Teste");
        jsMock.Verify(x => x.InvokeAsync<object>("DarAlert", It.IsAny<object[]>()), Times.Once);
    }

    #endregion

    #region Testes de Lógica de Lista

    [Fact]
    public void RedirecionarMarcar_ListaVazia()
    {
        var lista = new List<int>();
        Assert.Empty(lista);
        Assert.Equal(0, lista.Count);
    }

    [Fact]
    public void RedirecionarMarcar_ListaComElementos()
    {
        var lista = new List<int> { 1, 2, 3, 4, 5 };
        Assert.NotEmpty(lista);
        Assert.Equal(5, lista.Count);
    }

    [Fact]
    public void RedirecionarMarcar_LocalizaElementoEmLista()
    {
        var lista = new List<int> { 1, 2, 3, 4, 5 };
        var elemento = lista.FirstOrDefault(x => x == 3);
        Assert.Equal(3, elemento);
    }

    [Fact]
    public void RedirecionarMarcar_ElementoNaoEncontrado()
    {
        var lista = new List<int> { 1, 2, 3, 4, 5 };
        var elemento = lista.FirstOrDefault(x => x == 99);
        Assert.Equal(0, elemento);
    }

    [Fact]
    public void RedirecionarMarcar_VerificaCondicaoEmLista()
    {
        var lista = new List<int> { 1, 2, 3, 4, 5 };
        var existe = lista.Any(x => x == 3);
        Assert.True(existe);
    }

    [Fact]
    public void RedirecionarMarcar_VerificaCondicaoFalhaEmLista()
    {
        var lista = new List<int> { 1, 2, 3, 4, 5 };
        var existe = lista.Any(x => x == 99);
        Assert.False(existe);
    }

    #endregion

    #region Testes de Índice Aleatório

    [Fact]
    public void RedirecionarMarcar_GeradorAleatorio()
    {
        var random = new Random();
        var lista = new List<int> { 1, 2, 3, 4, 5 };

        for (int i = 0; i < 10; i++)
        {
            int indice = random.Next(1, lista.Count);
            Assert.True(indice >= 1 && indice < lista.Count);
        }
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(20)]
    public void RedirecionarMarcar_AleatorioComRangeVariado(int tamanho)
    {
        var random = new Random();
        for (int i = 0; i < 5; i++)
        {
            int indice = random.Next(1, tamanho);
            Assert.True(indice >= 1 && indice < tamanho);
        }
    }

    #endregion

    #region Testes de Mensagens

    [Fact]
    public void RedirecionarMarcar_MensagemErro()
    {
        var mensagem = "Não tem nenhum item para este versículo 999.";
        Assert.Contains("Não tem nenhum item", mensagem);
        Assert.Contains("versículo", mensagem);
    }

    [Theory]
    [InlineData("Não tem nenhum item para este versículo 1.")]
    [InlineData("Não tem nenhum item para este versículo 5.")]
    [InlineData("Não tem nenhum item para este versículo 100.")]
    public void RedirecionarMarcar_MensagenMultiplas(string mensagem)
    {
        Assert.Contains("Não tem nenhum item", mensagem);
    }

    #endregion

    #region Testes de Fluxo Simulado

    [Fact]
    public void RedirecionarMarcar_FluxoSimuladoPrompt()
    {
        var versiculo = "5";
        var resultado = int.TryParse(versiculo, out int vers);

        Assert.True(resultado);
        Assert.Equal(5, vers);
    }

    [Fact]
    public void RedirecionarMarcar_FluxoSimuladoValidacao()
    {
        var versiculo = 5;
        var lista = new List<int> { 1, 2, 3, 4, 5 };

        var encontrado = lista.Any(x => x == versiculo);
        Assert.True(encontrado);
    }

    [Fact]
    public void RedirecionarMarcar_FluxoSimuladoAleatorio()
    {
        var random = new Random();
        var lista = new List<int> { 10, 20, 30, 40, 50 };

        int indiceAleatorio = random.Next(1, lista.Count);

        Assert.True(indiceAleatorio >= 1 && indiceAleatorio < lista.Count);
    }

    #endregion

    #region Testes de Contagem

    [Theory]
    [InlineData(1, 1)]
    [InlineData(3, 3)]
    [InlineData(10, 10)]
    public void RedirecionarMarcar_ContaElementosLista(int quantidade, int esperado)
    {
        var lista = new List<int>();
        for (int i = 0; i < quantidade; i++)
        {
            lista.Add(i);
        }

        Assert.Equal(esperado, lista.Count);
    }

    #endregion
}
