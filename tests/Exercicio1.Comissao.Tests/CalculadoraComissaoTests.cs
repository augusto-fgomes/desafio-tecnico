using Exercicio1.Comissao.Models;
using Exercicio1.Comissao.Services;

namespace Exercicio1.Comissao.Tests;

public class CalculadoraComissaoTests
{
    private readonly CalculadoraComissao _calculadora = new();

    [Theory]
    [InlineData(0, 0)]
    [InlineData(99.99, 0)]          // abaixo de 100: sem comissão
    [InlineData(100, 1.00)]         // limite inferior da faixa de 1%
    [InlineData(250.30, 2.50)]      // 2,503 -> arredonda para 2,50
    [InlineData(499.99, 5.00)]      // 4,9999 -> arredonda para 5,00
    [InlineData(500, 25.00)]        // limite inferior da faixa de 5%
    [InlineData(1200.50, 60.03)]    // 60,025 -> arredonda para cima
    public void Calcular_AplicaFaixaCorreta(decimal valor, decimal esperado)
    {
        Assert.Equal(esperado, _calculadora.Calcular(valor));
    }

    [Fact]
    public void Calcular_ValorNegativo_LancaExcecao()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _calculadora.Calcular(-1m));
    }

    [Fact]
    public void CalcularPorVendedor_SomaPorVendedorEOrdenaPorComissao()
    {
        var vendas = new List<Venda>
        {
            new("Ana", 50m),      // 0
            new("Ana", 200m),     // 2
            new("Bruno", 1000m),  // 50
            new("Ana", 600m),     // 30
        };

        var resultado = _calculadora.CalcularPorVendedor(vendas);

        Assert.Collection(resultado,
            bruno =>
            {
                Assert.Equal("Bruno", bruno.Vendedor);
                Assert.Equal(1, bruno.QuantidadeVendas);
                Assert.Equal(1000m, bruno.TotalVendido);
                Assert.Equal(50m, bruno.TotalComissao);
            },
            ana =>
            {
                Assert.Equal("Ana", ana.Vendedor);
                Assert.Equal(3, ana.QuantidadeVendas);
                Assert.Equal(850m, ana.TotalVendido);
                Assert.Equal(32m, ana.TotalComissao);
            });
    }

    [Fact]
    public void CalcularPorVendedor_VendedorComUmaSoVendaAbaixoDoMinimo_ApareceComZero()
    {
        var resultado = _calculadora.CalcularPorVendedor([new Venda("Ana", 99.99m)]);

        var ana = Assert.Single(resultado);
        Assert.Equal(0m, ana.TotalComissao);
        Assert.Equal(99.99m, ana.TotalVendido);
    }

    [Fact]
    public void CalcularPorVendedor_ListaVazia_RetornaVazio()
    {
        Assert.Empty(_calculadora.CalcularPorVendedor([]));
    }
}
