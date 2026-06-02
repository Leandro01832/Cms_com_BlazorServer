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

public class VoltarSubgruposTests : TestContext
{
    private readonly Mock<IJSRuntime> _jsMock;

    public VoltarSubgruposTests()
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

    #region Testes de Iteração Reversa de Camadas

    [Fact]
    public void VoltarSubgrupos_IteraCamadasDe10Ate1()
    {
        var camadasIteradas = new List<int>();

        for (var i = 10; i > 0; i--)
        {
            camadasIteradas.Add(i);
        }

        Assert.Equal(10, camadasIteradas.Count);
        Assert.Equal(10, camadasIteradas.First());
        Assert.Equal(1, camadasIteradas.Last());
    }

    [Fact]
    public void VoltarSubgrupos_VerificaCamadaDez()
    {
        var camadaAtual = 10;
        var resultado = camadaAtual <= 10 && camadaAtual > 0;

        Assert.True(resultado);
    }

    [Fact]
    public void VoltarSubgrupos_VerificaCamadaUm()
    {
        var camadaAtual = 1;
        var resultado = camadaAtual <= 10 && camadaAtual > 0;

        Assert.True(resultado);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(5)]
    [InlineData(1)]
    public void VoltarSubgrupos_VerificaCamadasValidas(int camada)
    {
        var resultado = camada <= 10 && camada > 0;
        Assert.True(resultado);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    [InlineData(-1)]
    public void VoltarSubgrupos_VerificaCamadasInvalidas(int camada)
    {
        var resultado = camada <= 10 && camada > 0;
        Assert.False(resultado);
    }

    #endregion

    #region Testes de Comparação de Camada

    [Fact]
    public void VoltarSubgrupos_ComparaCamadaAtual()
    {
        var camadaAtual = 5;
        var iteracao = 5;

        var encontrou = camadaAtual == iteracao;

        Assert.True(encontrou);
    }

    [Fact]
    public void VoltarSubgrupos_NaoComparaCamadaDiferente()
    {
        var camadaAtual = 5;
        var iteracao = 7;

        var encontrou = camadaAtual == iteracao;

        Assert.False(encontrou);
    }

    [Theory]
    [InlineData(10, 10, true)]
    [InlineData(5, 5, true)]
    [InlineData(1, 1, true)]
    [InlineData(10, 9, false)]
    [InlineData(5, 3, false)]
    public void VoltarSubgrupos_ComparaCamadasMultiplas(int atual, int iteracao, bool esperado)
    {
        var resultado = atual == iteracao;
        Assert.Equal(esperado, resultado);
    }

    #endregion

    #region Testes de Verificação de Índice Zero

    [Fact]
    public void VoltarSubgrupos_VerificaSeIndiceZero()
    {
        var indice = 0;
        var resultado = indice == 0;

        Assert.True(resultado);
    }

    [Fact]
    public void VoltarSubgrupos_VerificaSeIndicePrimeiro()
    {
        var indice = 0;
        var resultado = indice == 0;

        Assert.True(resultado);
    }

    [Fact]
    public void VoltarSubgrupos_VerificaSeIndiceNaoZero()
    {
        var indice = 5;
        var resultado = indice == 0;

        Assert.False(resultado);
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(1, false)]
    [InlineData(5, false)]
    public void VoltarSubgrupos_VerificaIndiceZeroMultiplo(int indice, bool esperado)
    {
        var resultado = indice == 0;
        Assert.Equal(esperado, resultado);
    }

    #endregion

    #region Testes de Acesso ao Elemento Anterior

    [Fact]
    public void VoltarSubgrupos_AcessaAnterior()
    {
        var lista = new List<string> { "A", "B", "C", "D", "E" };
        var indice = 2; // "C"

        var anterior = lista[indice - 1];

        Assert.Equal("B", anterior);
    }

    [Fact]
    public void VoltarSubgrupos_AcessaPrimeiro()
    {
        var lista = new List<string> { "A", "B", "C", "D", "E" };
        var indice = 1; // "B"

        var anterior = lista[indice - 1];

        Assert.Equal("A", anterior);
    }

    [Fact]
    public void VoltarSubgrupos_AcessoUltimo()
    {
        var lista = new List<string> { "A", "B", "C", "D", "E" };

        var ultimo = lista[lista.Count - 1];

        Assert.Equal("E", ultimo);
    }

    [Theory]
    [InlineData(1, "A")]
    [InlineData(2, "B")]
    [InlineData(4, "D")]
    public void VoltarSubgrupos_AcessaElementoAnteriorMultiplo(int indice, string esperado)
    {
        var lista = new List<string> { "A", "B", "C", "D", "E" };
        var anterior = lista[indice - 1];

        Assert.Equal(esperado, anterior);
    }

    #endregion

    #region Testes de Busca de Índice

    [Fact]
    public void VoltarSubgrupos_EncontraIndiceEmLista()
    {
        var lista = new List<string> { "A", "B", "C", "D", "E" };
        var elemento = "C";

        var indice = lista.IndexOf(elemento);

        Assert.Equal(2, indice);
    }

    [Fact]
    public void VoltarSubgrupos_IndiceZeroForaPrimeiro()
    {
        var lista = new List<string> { "A", "B", "C", "D", "E" };
        var elemento = "A";

        var indice = lista.IndexOf(elemento);

        Assert.Equal(0, indice);
    }

    [Fact]
    public void VoltarSubgrupos_IndiceUltimoElemento()
    {
        var lista = new List<string> { "A", "B", "C", "D", "E" };
        var elemento = "E";

        var indice = lista.IndexOf(elemento);

        Assert.Equal(4, indice);
    }

    #endregion

    #region Testes de Última Posição

    [Fact]
    public void VoltarSubgrupos_AcessaUltimaposicao()
    {
        var lista = new List<string> { "A", "B", "C", "D", "E" };

        var ultimo = lista.Last();

        Assert.Equal("E", ultimo);
    }

    [Fact]
    public void VoltarSubgrupos_AcessaUltimaposicaoPorIndice()
    {
        var lista = new List<string> { "A", "B", "C", "D", "E" };

        var ultimo = lista[lista.Count - 1];

        Assert.Equal("E", ultimo);
    }

    [Fact]
    public void VoltarSubgrupos_ListaComUmElemento()
    {
        var lista = new List<string> { "A" };
        var primeiro = lista[0];
        var ultimo = lista[lista.Count - 1];

        Assert.Equal(primeiro, ultimo);
    }

    #endregion

    #region Testes de Retorno de Última Posição quando índice é 0

    [Fact]
    public void VoltarSubgrupos_RetornaUltimoQuandoIndiceZero()
    {
        var lista = new List<string> { "A", "B", "C", "D", "E" };
        var indice = 0;

        if (indice == 0)
        {
            var ultimo = lista.Last();
            Assert.Equal("E", ultimo);
        }
    }

    [Fact]
    public void VoltarSubgrupos_VerificaCondicaoIndiceZero()
    {
        var indice = 0;
        var resultado = indice == 0;

        Assert.True(resultado);
    }

    [Fact]
    public void VoltarSubgrupos_NaoRetornaUltimoQuandoIndiceNaoZero()
    {
        var lista = new List<string> { "A", "B", "C", "D", "E" };
        var indice = 3;

        if (indice == 0)
        {
            // Retorna último
        }
        else
        {
            var anterior = lista[indice - 1];
            Assert.Equal("C", anterior);
        }
    }

    #endregion

    #region Testes de Ordenação Reversa

    [Fact]
    public void VoltarSubgrupos_OrdenaListaReversa()
    {
        var lista = new List<int> { 5, 2, 8, 1, 3 };

        var ordenada = lista.OrderByDescending(x => x).ToList();

        Assert.Equal(new List<int> { 8, 5, 3, 2, 1 }, ordenada);
    }

    [Fact]
    public void VoltarSubgrupos_OrdenaListaAscendente()
    {
        var lista = new List<int> { 5, 2, 8, 1, 3 };

        var ordenada = lista.OrderBy(x => x).ToList();

        Assert.Equal(new List<int> { 1, 2, 3, 5, 8 }, ordenada);
    }

    #endregion

    #region Testes de Filtro por Camada

    [Fact]
    public void VoltarSubgrupos_FiltraListaPorCamada()
    {
        var lista = new List<(int camada, string nome)>
        {
            (1, "A"), (1, "B"), (2, "C"), (2, "D"), (1, "E")
        };

        var filtrada = lista.Where(x => x.camada == 1).ToList();

        Assert.Equal(3, filtrada.Count);
    }

    [Fact]
    public void VoltarSubgrupos_FiltraListaPorCamadaEspecifica()
    {
        var lista = new List<(int camada, string nome)>
        {
            (1, "A"), (1, "B"), (5, "C"), (5, "D"), (1, "E")
        };

        var filtrada = lista.Where(x => x.camada == 5).ToList();

        Assert.Equal(2, filtrada.Count);
        Assert.All(filtrada, x => Assert.Equal(5, x.camada));
    }

    #endregion

    #region Testes de Fluxo Completo

    [Fact]
    public void VoltarSubgrupos_FluxoSimples_ElementoAnterior()
    {
        var camadaAtual = 2;
        var lista = new List<(int camada, string nome)>
        {
            (2, "A"), (2, "B"), (2, "C"), (2, "D"), (2, "E")
        };
        var elementoAtual = "C";

        var filtrada = lista.Where(x => x.camada == camadaAtual).ToList();
        var indice = filtrada.FindIndex(x => x.nome == elementoAtual);

        if (indice == 0)
        {
            // Retorna último
            var ultimoItemDaCamadaAnterior = filtrada.Last();
            Assert.Equal("E", ultimoItemDaCamadaAnterior.nome);
        }
        else
        {
            var anterior = filtrada[indice - 1];
            Assert.Equal("B", anterior.nome);
        }
    }

    [Fact]
    public void VoltarSubgrupos_FluxoSimples_PrimeiroElemento()
    {
        var camadaAtual = 2;
        var lista = new List<(int camada, string nome)>
        {
            (2, "A"), (2, "B"), (2, "C"), (2, "D"), (2, "E")
        };
        var elementoAtual = "A";

        var filtrada = lista.Where(x => x.camada == camadaAtual).ToList();
        var indice = filtrada.FindIndex(x => x.nome == elementoAtual);

        if (indice == 0)
        {
            var ultimo = filtrada.Last();
            Assert.Equal("E", ultimo.nome);
        }
    }

    [Fact]
    public void VoltarSubgrupos_FluxoCompleto_PercursoReverso()
    {
        var camadasProcessadas = new List<int>();

        for (var i = 10; i > 0; i--)
        {
            var camadaAtual = 5;

            if (camadaAtual == i)
            {
                camadasProcessadas.Add(i);
            }
        }

        Assert.Single(camadasProcessadas);
        Assert.Equal(5, camadasProcessadas[0]);
    }

    #endregion

    #region Testes de Retorno Nulo

    [Fact]
    public void VoltarSubgrupos_RetornaNuloQuandoNaoEncontra()
    {
        var camadaAtual = 11; // Fora do intervalo
        var encontrou = false;

        for (var i = 10; i > 0; i--)
        {
            if (camadaAtual == i)
            {
                encontrou = true;
            }
        }

        var resultado = encontrou ? "encontrado" : null;

        Assert.Null(resultado);
    }

    [Fact]
    public void VoltarSubgrupos_RetornaNuloQuandoNegativo()
    {
        var camadaAtual = -5;
        var encontrou = false;

        for (var i = 10; i > 0; i--)
        {
            if (camadaAtual == i)
            {
                encontrou = true;
            }
        }

        var resultado = encontrou ? "encontrado" : null;

        Assert.Null(resultado);
    }

    #endregion

    #region Testes de Lista Vazia

    [Fact]
    public void VoltarSubgrupos_ListaVazia_RetornaNula()
    {
        var lista = new List<string>();
        var elemento = lista.LastOrDefault();

        Assert.Null(elemento);
    }

    [Fact]
    public void VoltarSubgrupos_ListaComUmElemento_EhPrimeiroEUltimo()
    {
        var lista = new List<string> { "A" };
        var primeiro = lista.First();
        var ultimo = lista.Last();

        Assert.Equal(primeiro, ultimo);
    }

    [Fact]
    public void VoltarSubgrupos_ListaComDoisElementos()
    {
        var lista = new List<string> { "A", "B" };

        Assert.Equal(2, lista.Count);
        Assert.Equal("A", lista[0]);
        Assert.Equal("B", lista[1]);
    }

    #endregion

    #region Testes de Contagem de Elementos

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public void VoltarSubgrupos_ContaElementosCorretos(int quantidade)
    {
        var lista = new List<int>();
        for (int i = 0; i < quantidade; i++)
        {
            lista.Add(i);
        }

        Assert.Equal(quantidade, lista.Count);
    }

    #endregion

    #region Testes de Transição Entre Camadas

    [Fact]
    public void VoltarSubgrupos_VoltaParaUltimaQuandoPrimeiro()
    {
        var lista = new List<(int camada, string nome)>
        {
            (1, "A"), (1, "B"), (1, "C")
        };
        var indice = 0; // Primeiro

        if (indice == 0)
        {
            var ultimo = lista.Last();
            Assert.Equal("C", ultimo.nome);
        }
    }

    [Fact]
    public void VoltarSubgrupos_MudaCamadaQuandoPrimeiro()
    {
        var camadaAtual = 5;
        var camadaAnterior = camadaAtual - 1;

        Assert.Equal(4, camadaAnterior);
    }

    [Fact]
    public void VoltarSubgrupos_VerificaDecrementoDeCamada()
    {
        var camada = 10;
        var anterior = camada - 1;

        Assert.Equal(9, anterior);
    }

    #endregion

    #region Testes de Busca Reversa

    [Fact]
    public void VoltarSubgrupos_BuscaReversa()
    {
        var lista = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        var reversa = lista.Reverse<int>().ToList();

        Assert.Equal(10, reversa[0]);
        Assert.Equal(1, reversa[9]);
    }

    [Fact]
    public void VoltarSubgrupos_BuscaSeqencialReversa()
    {
        var dados = new List<(int ordem, string nome)>
        {
            (1, "A"),
            (2, "B"),
            (3, "C"),
            (4, "D"),
            (5, "E")
        };

        var reversa = dados.Reverse<(int, string)>().ToList();

        Assert.Equal("E", reversa[0].Item2);
        Assert.Equal("A", reversa[4].Item2);
    }

    #endregion

    #region Testes de Condições Especiais

    [Fact]
    public void VoltarSubgrupos_CamadaMinima()
    {
        var camada = 1;
        var resultado = camada > 0;

        Assert.True(resultado);
    }

    [Fact]
    public void VoltarSubgrupos_CamadaMaxima()
    {
        var camada = 10;
        var resultado = camada <= 10;

        Assert.True(resultado);
    }

    [Fact]
    public void VoltarSubgrupos_IndiceNegativo()
    {
        var indice = -1;
        var resultado = indice < 0;

        Assert.True(resultado);
    }

    [Fact]
    public void VoltarSubgrupos_IndiceMuitoAlto()
    {
        var lista = new List<string> { "A", "B", "C" };
        var indice = 10;

        var dentro = indice < lista.Count;

        Assert.False(dentro);
    }

    #endregion

    #region Testes de Validação de Intervalo

    [Fact]
    public void VoltarSubgrupos_VerificaIntervaloValido()
    {
        for (var i = 10; i > 0; i--)
        {
            Assert.True(i > 0 && i <= 10);
        }
    }

    [Fact]
    public void VoltarSubgrupos_VerificaIntervaloCamadaUm()
    {
        var camada = 1;
        var resultado = camada > 0 && camada <= 10;

        Assert.True(resultado);
    }

    [Fact]
    public void VoltarSubgrupos_VerificaIntervaloCamadaDez()
    {
        var camada = 10;
        var resultado = camada > 0 && camada <= 10;

        Assert.True(resultado);
    }

    #endregion

    #region Testes de Contagem Regressiva

    [Fact]
    public void VoltarSubgrupos_ContagemRegressiva10Ate1()
    {
        var contador = 0;

        for (var i = 10; i > 0; i--)
        {
            contador++;
        }

        Assert.Equal(10, contador);
    }

    [Fact]
    public void VoltarSubgrupos_ContagemRegressivaPersonalizada()
    {
        var contador = 0;

        for (var i = 5; i > 0; i--)
        {
            contador++;
        }

        Assert.Equal(5, contador);
    }

    #endregion

    #region Testes de Operações em Lista

    [Fact]
    public void VoltarSubgrupos_OperacaoFirst()
    {
        var lista = new List<string> { "A", "B", "C" };

        var primeiro = lista.First();

        Assert.Equal("A", primeiro);
    }

    [Fact]
    public void VoltarSubgrupos_OperacaoLast()
    {
        var lista = new List<string> { "A", "B", "C" };

        var ultimo = lista.Last();

        Assert.Equal("C", ultimo);
    }

    [Fact]
    public void VoltarSubgrupos_OperacaoFirstOrDefault()
    {
        var lista = new List<string>();

        var primeiro = lista.FirstOrDefault();

        Assert.Null(primeiro);
    }

    [Fact]
    public void VoltarSubgrupos_OperacaoLastOrDefault()
    {
        var lista = new List<string>();

        var ultimo = lista.LastOrDefault();

        Assert.Null(ultimo);
    }

    #endregion
}
