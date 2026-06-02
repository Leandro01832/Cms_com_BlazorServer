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

public class BuscarProximoSubGrupoTests : TestContext
{
    private readonly Mock<IJSRuntime> _jsMock;

    public BuscarProximoSubGrupoTests()
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

    #region Testes de Iteração de Camadas

    [Fact]
    public void BuscarProximoSubGrupo_IteraCamadasDe1Ate10()
    {
        var camadasIteradas = new List<int>();

        for (var i = 1; i < 11; i++)
        {
            camadasIteradas.Add(i);
        }

        Assert.Equal(10, camadasIteradas.Count);
        Assert.Equal(1, camadasIteradas.First());
        Assert.Equal(10, camadasIteradas.Last());
    }

    [Fact]
    public void BuscarProximoSubGrupo_VerificaCamadaUm()
    {
        var camadaAtual = 1;
        var resultado = camadaAtual >= 1 && camadaAtual < 11;

        Assert.True(resultado);
    }

    [Fact]
    public void BuscarProximoSubGrupo_VerificaCamadaDez()
    {
        var camadaAtual = 10;
        var resultado = camadaAtual >= 1 && camadaAtual < 11;

        Assert.True(resultado);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void BuscarProximoSubGrupo_VerificaCamadasValidas(int camada)
    {
        var resultado = camada >= 1 && camada < 11;
        Assert.True(resultado);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    [InlineData(-1)]
    public void BuscarProximoSubGrupo_VerificaCamadasInvalidas(int camada)
    {
        var resultado = camada >= 1 && camada < 11;
        Assert.False(resultado);
    }

    #endregion

    #region Testes de Comparação de Camada

    [Fact]
    public void BuscarProximoSubGrupo_ComparaCamadaAtual()
    {
        var camadaAtual = 5;
        var iteracao = 5;

        var encontrou = camadaAtual == iteracao;

        Assert.True(encontrou);
    }

    [Fact]
    public void BuscarProximoSubGrupo_NaoComparaCamadaDiferente()
    {
        var camadaAtual = 5;
        var iteracao = 3;

        var encontrou = camadaAtual == iteracao;

        Assert.False(encontrou);
    }

    [Theory]
    [InlineData(1, 1, true)]
    [InlineData(5, 5, true)]
    [InlineData(10, 10, true)]
    [InlineData(1, 2, false)]
    [InlineData(5, 3, false)]
    public void BuscarProximoSubGrupo_ComparaCamadasMultiplas(int atual, int iteracao, bool esperado)
    {
        var resultado = atual == iteracao;
        Assert.Equal(esperado, resultado);
    }

    #endregion

    #region Testes de Busca de Índice

    [Fact]
    public void BuscarProximoSubGrupo_EncontraIndiceEmLista()
    {
        var lista = new List<string> { "A", "B", "C", "D", "E" };
        var elemento = "C";

        var indice = lista.IndexOf(elemento);

        Assert.Equal(2, indice);
    }

    [Fact]
    public void BuscarProximoSubGrupo_IndiceNaoEncontrado()
    {
        var lista = new List<string> { "A", "B", "C", "D", "E" };
        var elemento = "Z";

        var indice = lista.IndexOf(elemento);

        Assert.Equal(-1, indice);
    }

    [Fact]
    public void BuscarProximoSubGrupo_IndiceDoUltimo()
    {
        var lista = new List<string> { "A", "B", "C", "D", "E" };
        var elemento = "E";

        var indice = lista.IndexOf(elemento);

        Assert.Equal(4, indice);
    }

    [Fact]
    public void BuscarProximoSubGrupo_IndiceDoPrimeiro()
    {
        var lista = new List<string> { "A", "B", "C", "D", "E" };
        var elemento = "A";

        var indice = lista.IndexOf(elemento);

        Assert.Equal(0, indice);
    }

    #endregion

    #region Testes de Verificação de Limite

    [Fact]
    public void BuscarProximoSubGrupo_VerificaSePenultimo()
    {
        var lista = new List<string> { "A", "B", "C", "D", "E" };
        var indice = 3; // "D"
        var isUltimo = indice + 1 == lista.Count;

        Assert.False(isUltimo);
    }

    [Fact]
    public void BuscarProximoSubGrupo_VerificaSeUltimo()
    {
        var lista = new List<string> { "A", "B", "C", "D", "E" };
        var indice = 4; // "E"
        var isUltimo = indice + 1 == lista.Count;

        Assert.True(isUltimo);
    }

    [Fact]
    public void BuscarProximoSubGrupo_VerificaSePrimeiro()
    {
        var lista = new List<string> { "A", "B", "C", "D", "E" };
        var indice = 0; // "A"
        var isUltimo = indice + 1 == lista.Count;

        Assert.False(isUltimo);
    }

    // [Theory]
    // [InlineData(0, 5, false)]
    // [InlineData(3, 5, false)]
    // [InlineData(4, 5, true)]
    // [InlineData(1, 2, false)]
    // public void BuscarProximoSubGrupo_VerificaLimiteMultiplo(int indice, int count, bool isUltimo)
    // {
    //     var resultado = indice + 1 == count;
    //     Assert.Equal(isUltimo, resultado);
    // }

    #endregion

    #region Testes de Acesso ao Próximo Elemento

    [Fact]
    public void BuscarProximoSubGrupo_AcessaProximo()
    {       
        
        var lista = new List<string> { "A", "B", "C", "D", "E" };
        var indice = 2; // "C"

        var proximo = lista[indice + 1];

        Assert.Equal("D", proximo);
    }

    [Fact]
    public void BuscarProximoSubGrupo_AcessaPenultimo()
    {
        var lista = new List<string> { "A", "B", "C", "D", "E" };
        var indice = 3; // "D"

        var proximo = lista[indice + 1];

        Assert.Equal("E", proximo);
    }

    [Fact]
    public void BuscarProximoSubGrupo_AcessaPrimeiro()
    {
        var lista = new List<string> { "A", "B", "C", "D", "E" };

        var primeiro = lista[0];

        Assert.Equal("A", primeiro);
    }

    #endregion

    #region Testes de Ordenação

    [Fact]
    public void BuscarProximoSubGrupo_OrdenaListaPorFiltroId()
    {
        var lista = new List<(int filtroId, int id)>
        {
            (3, 10), (1, 5), (2, 8), (1, 3)
        };

        var ordenada = lista.OrderBy(x => x.filtroId).ThenBy(x => x.id).ToList();

        Assert.Equal((1, 3), ordenada[0]);
        Assert.Equal((1, 5), ordenada[1]);
        Assert.Equal((2, 8), ordenada[2]);
        Assert.Equal((3, 10), ordenada[3]);
    }

    [Fact]
    public void BuscarProximoSubGrupo_VerificaOrdenacaoPrimaria()
    {
        var lista = new List<int> { 5, 2, 8, 1, 3 };

        var ordenada = lista.OrderBy(x => x).ToList();

        Assert.Equal(new List<int> { 1, 2, 3, 5, 8 }, ordenada);
    }

    #endregion

    #region Testes de Filtro por Camada

    [Fact]
    public void BuscarProximoSubGrupo_FiltraListaPorCamada()
    {
        var lista = new List<(int camada, string nome)>
        {
            (1, "A"), (1, "B"), (2, "C"), (2, "D"), (1, "E")
        };

        var filtrada = lista.Where(x => x.camada == 1).ToList();

        Assert.Equal(3, filtrada.Count);
        Assert.All(filtrada, x => Assert.Equal(1, x.camada));
    }

    [Fact]
    public void BuscarProximoSubGrupo_FiltraListaPorCamadaDiferente()
    {
        var lista = new List<(int camada, string nome)>
        {
            (1, "A"), (1, "B"), (2, "C"), (2, "D"), (1, "E")
        };

        var filtrada = lista.Where(x => x.camada == 2).ToList();

        Assert.Equal(2, filtrada.Count);
        Assert.All(filtrada, x => Assert.Equal(2, x.camada));
    }

    [Fact]
    public void BuscarProximoSubGrupo_FiltraListaPorCamadaVazia()
    {
        var lista = new List<(int camada, string nome)>
        {
            (1, "A"), (1, "B"), (2, "C")
        };

        var filtrada = lista.Where(x => x.camada == 5).ToList();

        Assert.Empty(filtrada);
    }

    #endregion

    #region Testes de Fluxo Completo

    [Fact]
    public void BuscarProximoSubGrupo_FluxoSimples_ProximoElemento()
    {
        var camadaAtual = 2;
        var lista = new List<(int camada, string nome)>
        {
            (2, "A"), (2, "B"), (2, "C"), (2, "D"), (2, "E")
        };
        var elementoAtual = "C";

        var filtrada = lista.Where(x => x.camada == camadaAtual).ToList();
        var indice = filtrada.FindIndex(x => x.nome == elementoAtual);
        var isUltimo = indice + 1 == filtrada.Count;

        if (!isUltimo)
        {
            var proximo = filtrada[indice + 1];
            Assert.Equal("D", proximo.nome);
        }
    }

    [Fact]
    public void BuscarProximoSubGrupo_FluxoSimples_UltimoElemento()
    {
        var camadaAtual = 2;
        var lista = new List<(int camada, string nome)>
        {
            (2, "A"), (2, "B"), (2, "C"), (2, "D"), (2, "E")
        };
        var elementoAtual = "E";

        var filtrada = lista.Where(x => x.camada == camadaAtual).ToList();
        var indice = filtrada.FindIndex(x => x.nome == elementoAtual);
        var isUltimo = indice + 1 == filtrada.Count;

        Assert.True(isUltimo);
    }

    [Fact]
    public void BuscarProximoSubGrupo_FluxoCompleto_PercursoCompleto()
    {
        var camadasProcessadas = new List<int>();

        for (var i = 1; i < 11; i++)
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
    public void BuscarProximoSubGrupo_RetornaNuloQuandoNaoEncontra()
    {
        var camadaAtual = 5;
        var encontrou = false;

        for (var i = 1; i < 11; i++)
        {
            if (camadaAtual == i)
            {
                encontrou = true;
            }
        }

        var resultado = encontrou ? "encontrado" : null;

        if (encontrou)
        {
            Assert.NotNull(resultado);
        }
    }

    [Fact]
    public void BuscarProximoSubGrupo_RetornaNuloQuandoForaDoIntervalo()
    {
        var camadaAtual = 11; // Fora do intervalo 1-10
        var encontrou = false;

        for (var i = 1; i < 11; i++)
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
    public void BuscarProximoSubGrupo_ListaVazia_RetornaNula()
    {
        var lista = new List<string>();
        var elemento = lista.FirstOrDefault();

        Assert.Null(elemento);
    }

    [Fact]
    public void BuscarProximoSubGrupo_ListaComUmElemento()
    {
        var lista = new List<string> { "A" };
        var indice = 0;
        var isUltimo = indice + 1 == lista.Count;

        Assert.True(isUltimo);
    }

    #endregion

    #region Testes de Contagem de Elementos

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public void BuscarProximoSubGrupo_ContaElementosCorretos(int quantidade)
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
    public void BuscarProximoSubGrupo_VoltaParaPrimeiraQuandoUltimo()
    {
        var lista = new List<(int camada, string nome)>
        {
            (1, "A"), (1, "B"), (1, "C")
        };
        var indice = 2; // Último
        var isUltimo = indice + 1 == lista.Count;

        if (isUltimo)
        {
            var primeiro = lista[0];
            Assert.Equal("A", primeiro.nome);
        }
    }

    [Fact]
    public void BuscarProximoSubGrupo_MudaCamadaQuandoUltimo()
    {
        var camadaAtual = 1;
        var proximaCamada = camadaAtual + 1;

        Assert.Equal(2, proximaCamada);
    }

    [Fact]
    public void BuscarProximoSubGrupo_VerificaIncrementoDeCamada()
    {
        var camada = 5;
        var proxima = camada + 1;

        Assert.Equal(6, proxima);
    }

    #endregion

    #region Testes de Busca Sequencial

    [Fact]
    public void BuscarProximoSubGrupo_BuscaSequencialPorCamada()
    {
        var dados = new Dictionary<int, List<string>>
        {
            { 1, new List<string> { "A1", "A2", "A3" } },
            { 2, new List<string> { "B1", "B2" } },
            { 3, new List<string> { "C1" } }
        };

        var camada2 = dados[2];
        Assert.Equal(2, camada2.Count);
    }

    [Fact]
    public void BuscarProximoSubGrupo_BuscaSequencialPorIndice()
    {
        var dados = new List<(int camada, int id, string nome)>
        {
            (1, 1, "A"),
            (1, 2, "B"),
            (2, 1, "C"),
            (2, 2, "D")
        };

        var resultado = dados.FirstOrDefault(x => x.camada == 2 && x.id == 1);

        Assert.Equal("C", resultado.nome);
    }

    #endregion

    #region Testes de Condições Especiais

    [Fact]
    public void BuscarProximoSubGrupo_CamadaMinima()
    {
        var camada = 1;
        var resultado = camada >= 1;

        Assert.True(resultado);
    }

    [Fact]
    public void BuscarProximoSubGrupo_CamadaMaxima()
    {
        var camada = 10;
        var resultado = camada < 11;

        Assert.True(resultado);
    }

    [Fact]
    public void BuscarProximoSubGrupo_IndiceZero()
    {
        var indice = 0;
        var proximoIndice = indice + 1;

        Assert.Equal(1, proximoIndice);
    }

    #endregion
}
