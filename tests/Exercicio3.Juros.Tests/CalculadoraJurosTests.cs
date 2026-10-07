using Exercicio3.Juros.Models;
using Exercicio3.Juros.Services;

namespace Exercicio3.Juros.Tests;

public class CalculadoraJurosTests
{
    private readonly CalculadoraJuros _calculadora = new();
    private static readonly DateOnly Hoje = new(2026, 10, 7);

    [Theory]
    [InlineData("2026-10-07", 0)]   // vence hoje: sem atraso
    [InlineData("2026-10-20", 0)]   // vence no futuro: sem atraso
    [InlineData("2026-10-06", 1)]
    [InlineData("2026-09-27", 10)]
    [InlineData("2026-09-07", 30)]  // atravessa a virada do mês
    public void Calcular_ContaDiasCorridosDeAtraso(string vencimento, int diasEsperados)
    {
        var r = _calculadora.Calcular(1000m, DateOnly.Parse(vencimento), Hoje);

        Assert.Equal(diasEsperados, r.DiasAtraso);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 25.00)]
    [InlineData(10, 250.00)]
    [InlineData(40, 1000.00)]
    public void Calcular_JurosSimples(int dias, decimal jurosEsperados)
    {
        var r = _calculadora.Calcular(1000m, Hoje.AddDays(-dias), Hoje);

        Assert.Equal(jurosEsperados, r.Juros);
        Assert.Equal(1000m + jurosEsperados, r.Total);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 25.00)]          // 1 dia: igual ao simples
    [InlineData(2, 50.63)]          // 1000 × (1,025² − 1) = 50,625
    [InlineData(10, 280.08)]        // 1000 × (1,025¹⁰ − 1) ≈ 280,0845
    public void Calcular_JurosCompostos(int dias, decimal jurosEsperados)
    {
        var r = _calculadora.Calcular(1000m, Hoje.AddDays(-dias), Hoje, TipoJuros.Composto);

        Assert.Equal(jurosEsperados, r.Juros);
    }

    [Fact]
    public void Calcular_ArredondaParaCentavos()
    {
        // 33,33 × 2,5% × 1 = 0,83325 -> 0,83
        var r = _calculadora.Calcular(33.33m, Hoje.AddDays(-1), Hoje);

        Assert.Equal(0.83m, r.Juros);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Calcular_ValorInvalido_LancaExcecao(decimal valor)
    {
        Assert.Throws<CalculoInvalidoException>(() => _calculadora.Calcular(valor, Hoje, Hoje));
    }

    [Fact]
    public void Calcular_CompostoComAtrasoExtremo_LancaExcecao()
    {
        Assert.Throws<CalculoInvalidoException>(
            () => _calculadora.Calcular(1000m, Hoje.AddDays(-10_000), Hoje, TipoJuros.Composto));
    }

    [Fact]
    public void Calcular_SimplesComValorExtremo_LancaExcecao()
    {
        Assert.Throws<CalculoInvalidoException>(
            () => _calculadora.Calcular(decimal.MaxValue, Hoje.AddDays(-100), Hoje));
    }
}
