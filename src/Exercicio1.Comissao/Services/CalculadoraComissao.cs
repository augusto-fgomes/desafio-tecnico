using Exercicio1.Comissao.Models;

namespace Exercicio1.Comissao.Services;

/// <summary>
/// Regras de comissão por venda:
/// <list type="bullet">
///   <item>abaixo de R$ 100,00: sem comissão;</item>
///   <item>de R$ 100,00 até abaixo de R$ 500,00: 1%;</item>
///   <item>a partir de R$ 500,00: 5%.</item>
/// </list>
/// </summary>
public class CalculadoraComissao
{
    public const decimal ValorMinimoComissao = 100m;
    public const decimal ValorMinimoFaixaSuperior = 500m;
    public const decimal PercentualFaixaInferior = 0.01m;
    public const decimal PercentualFaixaSuperior = 0.05m;

    /// <summary>Calcula a comissão de uma única venda, arredondada em centavos.</summary>
    public decimal Calcular(decimal valorVenda)
    {
        if (valorVenda < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(valorVenda), valorVenda, "O valor da venda não pode ser negativo.");
        }

        decimal percentual = valorVenda switch
        {
            < ValorMinimoComissao => 0m,
            < ValorMinimoFaixaSuperior => PercentualFaixaInferior,
            _ => PercentualFaixaSuperior
        };

        return Math.Round(valorVenda * percentual, 2, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Agrupa as vendas por vendedor e soma a comissão de cada venda.
    /// O resultado vem ordenado da maior para a menor comissão.
    /// </summary>
    public IReadOnlyList<ComissaoVendedor> CalcularPorVendedor(IEnumerable<Venda> vendas)
    {
        return vendas
            .GroupBy(v => v.Vendedor)
            .Select(grupo => new ComissaoVendedor(
                Vendedor: grupo.Key,
                QuantidadeVendas: grupo.Count(),
                TotalVendido: grupo.Sum(v => v.Valor),
                TotalComissao: grupo.Sum(v => Calcular(v.Valor))))
            .OrderByDescending(c => c.TotalComissao)
            .ThenBy(c => c.Vendedor)
            .ToList();
    }
}
