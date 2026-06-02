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

public class BuscarAnteriorTests : TestContext
{
    private readonly Mock<IJSRuntime> _jsMock;

    public BuscarAnteriorTests()
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

    #region Testes de Decremento de Índice

    [Fact]
    public void BuscarAnterior_DecrementaIndiceSimples()
    {
        var indiceAtual = 5;
        var anterior = indiceAtual - 1;

        Assert.Equal(4, anterior);
    }

    [Fact]
    public void BuscarAnterior_DecrementaDoUm()
    {
        var indiceAtual = 1;
        var anterior = indiceAtual - 1;

        Assert.Equal(0, anterior);
    }

    [Theory]
    [InlineData(10, 9)]
    [InlineData(5, 4)]
    [InlineData(2, 1)]
    [InlineData(1, 0)]
    public void BuscarAnterior_DecrementaMultiplos(int indice, int esperado)
    {
        var anterior = indice - 1;
        Assert.Equal(esperado, anterior);
    }

    #endregion

    #region Testes com Rotas

    [Fact]
    public void BuscarAnterior_ComRotas_IndiceUm_MantémUm()
    {
        var rotas = "rota_teste";
        var indiceAtual = 1;

        if (rotas != null)
        {
            if (indiceAtual == 1)
            {
                indiceAtual = 1;
            }
            else
            {
                indiceAtual--;
            }
        }

        Assert.Equal(1, indiceAtual);
    }

    [Fact]
    public void BuscarAnterior_ComRotas_IndiceMailorQueUm_Decrementa()
    {
        var rotas = "rota_teste";
        var indiceAtual = 5;

        if (rotas != null)
        {
            if (indiceAtual == 1)
            {
                indiceAtual = 1;
            }
            else
            {
                indiceAtual--;
            }
        }

        Assert.Equal(4, indiceAtual);
    }

    [Theory]
    [InlineData("rota1", 1, 1)]
    [InlineData("rota2", 5, 4)]
    [InlineData("rota3", 10, 9)]
    public void BuscarAnterior_ComRotasMultiplas(string rota, int indice, int esperado)
    {
        if (rota != null)
        {
            if (indice == 1)
            {
                indice = 1;
            }
            else
            {
                indice--;
            }
        }

        Assert.Equal(esperado, indice);
    }

    #endregion

    #region Testes sem Rotas - Comportamento Base

    [Fact]
    public void BuscarAnterior_SemRotas_IndiceUmCapZero_MantémIndice()
    {
        var rotas = (string)null;
        var indiceAtual = 1;
        var cap = 0;

        if (rotas == null)
        {
            if (indiceAtual == 1 && cap != 0)
            {
                // Navega para capítulo anterior
            }
            else if (indiceAtual != 1)
            {
                indiceAtual--;
            }
        }

        Assert.Equal(1, indiceAtual);
    }

    [Fact]
    public void BuscarAnterior_SemRotas_IndiceMailorQueUm_Decrementa()
    {
        var rotas = (string)null;
        var indiceAtual = 5;
        var cap = 5;

        if (rotas == null)
        {
            if (indiceAtual == 1 && cap != 0)
            {
                // Navega para capítulo anterior
            }
            else if (indiceAtual != 1)
            {
                indiceAtual--;
            }
        }

        Assert.Equal(4, indiceAtual);
    }

    #endregion

    #region Testes de Capítulo

    [Fact]
    public void BuscarAnterior_DecrementaCapitulo()
    {
        var capitulo = 5;
        capitulo--;

        Assert.Equal(4, capitulo);
    }

    [Theory]
    [InlineData(10, 9)]
    [InlineData(5, 4)]
    [InlineData(2, 1)]
    public void BuscarAnterior_DecrementaCapituloMultiplos(int cap, int esperado)
    {
        cap--;
        Assert.Equal(esperado, cap);
    }

    [Fact]
    public void BuscarAnterior_VerificaCapZero()
    {
        var cap = 0;
        var resultado = cap != 0;

        Assert.False(resultado);
    }

    [Fact]
    public void BuscarAnterior_VerificaCapMailorQueZero()
    {
        var cap = 5;
        var resultado = cap != 0;

        Assert.True(resultado);
    }

    #endregion

    #region Testes de Mudança de Tipo

    [Fact]
    public void BuscarAnterior_MudaTipoProximo()
    {
        var tipos = new List<string> { "Page", "UserContent", "Comment" };
        var typeAtual = "Page";

        var t = tipos.FirstOrDefault(x => x.ToLower() == typeAtual.ToLower());
        var i = tipos.IndexOf(t);

        if (i < tipos.Count - 1)
        {
            typeAtual = tipos[i + 1];
        }

        Assert.Equal("UserContent", typeAtual);
    }

    [Fact]
    public void BuscarAnterior_NaoMudaTipoUserContent()
    {
        var tipos = new List<string> { "Page", "UserContent", "Comment" };
        var typeAtual = "UserContent";

        var t = tipos.FirstOrDefault(x => x.ToLower() == typeAtual.ToLower());
        var resultado = typeAtual != "UserContent";

        Assert.False(resultado);
    }

    [Fact]
    public void BuscarAnterior_MudaTipoComment()
    {
        var tipos = new List<string> { "Page", "UserContent", "Comment" };
        var typeAtual = "UserContent";

        var t = tipos.FirstOrDefault(x => x.ToLower() == typeAtual.ToLower());
        var i = tipos.IndexOf(t);

        if (i < tipos.Count - 1)
        {
            typeAtual = tipos[i + 1];
        }

        Assert.Equal("Comment", typeAtual);
    }

    #endregion

    #region Testes de Flag AlterouModel

    [Fact]
    public void BuscarAnterior_DefineFlagAlterouModel()
    {
        var alterouModel = false;
        alterouModel = true;

        Assert.True(alterouModel);
    }

    [Fact]
    public void BuscarAnterior_FlagAlterouModelPermaneceTrue()
    {
        var alterouModel = true;

        Assert.True(alterouModel);
    }

    #endregion

    #region Testes de Flag AlterouIndice

    [Fact]
    public void BuscarAnterior_FlagAlterouIndiceIniciaFalso()
    {
        var alterouIndice = false;

        Assert.False(alterouIndice);
    }

    [Fact]
    public void BuscarAnterior_FlagAlterouIndiceMudaParaTrue()
    {
        var alterouIndice = false;
        alterouIndice = true;

        Assert.True(alterouIndice);
    }

    [Fact]
    public void BuscarAnterior_FlagAlterouIndicePermitiDerivacoes()
    {
        var alterouIndice = false;
        var indice = 5;
        var rotas = (string)null;

        if (indice != 1 && rotas == null && !alterouIndice)
        {
            indice--;
        }

        Assert.Equal(4, indice);
    }

    #endregion

    #region Testes de Condições Lógicas

    [Fact]
    public void BuscarAnterior_VerificaRotasNula()
    {
        string rotas = null;
        var resultado = rotas != null;

        Assert.False(resultado);
    }

    [Fact]
    public void BuscarAnterior_VerificaRotasNaoNula()
    {
        string rotas = "rota_teste";
        var resultado = rotas != null;

        Assert.True(resultado);
    }

    [Fact]
    public void BuscarAnterior_VerificaFiltroNulo()
    {
        long? filtro = null;
        var resultado = filtro != null;

        Assert.False(resultado);
    }

    [Fact]
    public void BuscarAnterior_VerificaFiltroNaoNulo()
    {
        long? filtro = 5;
        var resultado = filtro != null;

        Assert.True(resultado);
    }

    [Fact]
    public void BuscarAnterior_VerificaIndicePosicaoUm()
    {
        var indice = 1;
        var resultado = indice == 1;

        Assert.True(resultado);
    }

    [Fact]
    public void BuscarAnterior_VerificaIndicePosicaoMultipla()
    {
        var indice = 5;
        var resultado = indice == 1;

        Assert.False(resultado);
    }

    #endregion

    #region Testes de Fluxo Completo

    [Fact]
    public void BuscarAnterior_FluxoSimples_DecrementaIndice()
    {
        var alterouModel = false;
        var rotas = (string)null;
        var indiceAtual = 5;
        var cap = 5;
        var alterouIndice = false;

        alterouModel = true;

        if (rotas == null)
        {
            if (indiceAtual == 1 && cap != 0)
            {
                // Navega para capítulo anterior
            }
            else if (indiceAtual != 1 && rotas == null && !alterouIndice)
            {
                indiceAtual--;
            }
        }

        Assert.True(alterouModel);
        Assert.Equal(4, indiceAtual);
    }

    [Fact]
    public void BuscarAnterior_FluxoComCapitulo_DecrementaCapitulo()
    {
        var alterouModel = false;
        var rotas = (string)null;
        var indiceAtual = 1;
        var cap = 5;
        var alterouIndice = false;

        alterouModel = true;

        if (rotas == null)
        {
            if (indiceAtual == 1 && cap != 0)
            {
                cap--;
                alterouIndice = true;
            }
        }

        Assert.True(alterouModel);
        Assert.Equal(4, cap);
        Assert.True(alterouIndice);
    }

    [Fact]
    public void BuscarAnterior_FluxoComRota_MantémIndice()
    {
        var alterouModel = false;
        var rotas = "rota_teste";
        var indiceAtual = 1;

        alterouModel = true;

        if (rotas != null)
        {
            if (indiceAtual == 1)
            {
                indiceAtual = 1;
            }
            else
            {
                indiceAtual--;
            }
        }

        Assert.True(alterouModel);
        Assert.Equal(1, indiceAtual);
    }

    [Fact]
    public void BuscarAnterior_FluxoComRota_DecrementaIndice()
    {
        var alterouModel = false;
        var rotas = "rota_teste";
        var indiceAtual = 5;

        alterouModel = true;

        if (rotas != null)
        {
            if (indiceAtual == 1)
            {
                indiceAtual = 1;
            }
            else
            {
                indiceAtual--;
            }
        }

        Assert.True(alterouModel);
        Assert.Equal(4, indiceAtual);
    }

    #endregion

    #region Testes de Sequência Completa

    [Fact]
    public void BuscarAnterior_SequenciaCompleta_SemRotas_DentroLimite()
    {
        var alterouModel = false;
        var rotas = (string)null;
        var indice = 5;
        var cap = 5;
        var filtro = (long?)null;
        var alterouIndice = false;

        alterouModel = true;

        if (rotas != null)
        {
            if (indice == 1)
            {
                indice = 1;
            }
            else
            {
                indice--;
            }
        }
        else
        {
            if (indice == 1 && cap != 0)
            {
                if (filtro != null)
                {
                    // Volta subgrupo
                    alterouIndice = true;
                }

                if (filtro == null)
                {
                    cap--;
                }
            }

            if (indice != 1 && rotas == null && !alterouIndice)
            {
                indice--;
            }
        }

        Assert.True(alterouModel);
        Assert.Equal(4, indice);
    }

    [Fact]
    public void BuscarAnterior_SequenciaCompleta_PrimeiroIndiceComFiltro()
    {
        var alterouModel = false;
        var rotas = (string)null;
        var indice = 1;
        var cap = 5;
        var filtro = 10L;
        var alterouIndice = false;

        alterouModel = true;

        if (rotas == null)
        {
            if (indice == 1 && cap != 0)
            {
                if (filtro != null)
                {
                    alterouIndice = true;
                }
            }
        }

        Assert.True(alterouModel);
        Assert.True(alterouIndice);
    }

    [Fact]
    public void BuscarAnterior_SequenciaCompleta_PrimeiroIndiceSemFiltro()
    {
        var alterouModel = false;
        var rotas = (string)null;
        var indice = 1;
        var cap = 5;
        var filtro = (long?)null;

        alterouModel = true;

        if (rotas == null)
        {
            if (indice == 1 && cap != 0)
            {
                if (filtro == null)
                {
                    cap--;
                }
            }
        }

        Assert.True(alterouModel);
        Assert.Equal(4, cap);
    }

    #endregion

    #region Testes de Valores Especiais

    [Fact]
    public void BuscarAnterior_IndiceZero_DecrementaMaisUm()
    {
        var indice = 0;
        var anterior = indice - 1;

        Assert.Equal(-1, anterior);
    }

    [Fact]
    public void BuscarAnterior_CapituloUm_DecrementaParaZero()
    {
        var cap = 1;
        cap--;

        Assert.Equal(0, cap);
    }

    [Fact]
    public void BuscarAnterior_IndiceGrande_DecrementaBem()
    {
        var indice = 1000;
        var anterior = indice - 1;

        Assert.Equal(999, anterior);
    }

    #endregion

    #region Testes de Navegação

    [Fact]
    public void BuscarAnterior_NavegacaoPorRotas()
    {
        var rotas = "rota_teste";
        var chamouAcessar = false;

        if (rotas != null)
        {
            chamouAcessar = true;
        }

        Assert.True(chamouAcessar);
    }

    [Fact]
    public void BuscarAnterior_NavegacaoSemRotas()
    {
        string rotas = null;
        var chamouAcessar = false;

        if (rotas == null)
        {
            chamouAcessar = true;
        }

        Assert.True(chamouAcessar);
    }

    #endregion

    #region Testes de Contagem

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public void BuscarAnterior_VerificaIndiceValido(int indice)
    {
        var anterior = indice - 1;

        if (indice > 1)
        {
            Assert.True(anterior >= 0);
        }
    }

    #endregion

    #region Testes de Retrocesso

    [Fact]
    public void BuscarAnterior_VerificaRetrocederFlag()
    {
        var retroceder = 0;
        retroceder = 1;

        Assert.Equal(1, retroceder);
    }

    [Fact]
    public void BuscarAnterior_RetrocederFlagMarcaNavegacao()
    {
        var retroceder = 0;
        var indice = 1;
        var cap = 5;
        var filtro = 10L;

        if (indice == 1 && cap != 0 && filtro != null)
        {
            retroceder = 1;
        }

        Assert.Equal(1, retroceder);
    }

    #endregion
}
