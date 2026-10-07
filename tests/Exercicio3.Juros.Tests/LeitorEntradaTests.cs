using Exercicio3.Juros.Services;

namespace Exercicio3.Juros.Tests;

public class LeitorEntradaTests
{
    [Theory]
    [InlineData("1500", 1500)]
    [InlineData("1500,5", 1500.5)]
    [InlineData("1500,50", 1500.50)]
    [InlineData("1.500", 1500)]          // ponto é milhar: nunca vira 1,50
    [InlineData("1.500,50", 1500.50)]
    [InlineData("1.000.000,00", 1000000)]
    [InlineData("R$ 1.500,00", 1500)]
    [InlineData(" 0,01 ", 0.01)]
    public void TentarLerValor_FormatosValidos(string texto, decimal esperado)
    {
        Assert.True(LeitorEntrada.TentarLerValor(texto, out var valor, out _));
        Assert.Equal(esperado, valor);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("abc")]
    [InlineData("1500.50")]      // ponto decimal (formato americano)
    [InlineData("1,500.50")]
    [InlineData("1.50")]         // grupo de milhar incompleto
    [InlineData("1500,505")]     // mais de 2 casas decimais
    [InlineData("0,001")]
    [InlineData("-100")]
    [InlineData("0")]
    [InlineData("0,00")]
    [InlineData("99999999999999999999999999999999")] // não cabe em decimal
    public void TentarLerValor_FormatosInvalidos(string? texto)
    {
        Assert.False(LeitorEntrada.TentarLerValor(texto, out _, out var erro));
        Assert.NotEmpty(erro);
    }

    [Fact]
    public void TentarLerData_FormatoBrasileiro()
    {
        Assert.True(LeitorEntrada.TentarLerData("27/09/2026", out var data, out _));
        Assert.Equal(new DateOnly(2026, 9, 27), data);
    }

    [Theory]
    [InlineData("2026-09-27")]
    [InlineData("09/27/2026")]
    [InlineData("31/02/2026")]
    [InlineData("27/9/26")]
    [InlineData("")]
    [InlineData(null)]
    public void TentarLerData_FormatosInvalidos(string? texto)
    {
        Assert.False(LeitorEntrada.TentarLerData(texto, out _, out var erro));
        Assert.NotEmpty(erro);
    }
}
