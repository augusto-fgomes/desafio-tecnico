using Exercicio3.Juros.Models;

namespace Exercicio3.Juros.Services;

/// <summary>
/// Calcula os juros por atraso de um título, à taxa de 2,5% por dia corrido após o vencimento.
/// Pagamento no dia do vencimento (ou antes) não gera juros.
/// </summary>
public class CalculadoraJuros
{
    public const decimal TaxaDiaria = 0.025m;

    public ResultadoJuros Calcular(
        decimal valor, DateOnly vencimento, DateOnly dataCalculo, TipoJuros tipo = TipoJuros.Simples)
    {
        if (valor <= 0)
        {
            throw new CalculoInvalidoException("O valor deve ser maior que zero.");
        }

        int diasAtraso = Math.Max(0, dataCalculo.DayNumber - vencimento.DayNumber);

        decimal juros;
        try
        {
            juros = tipo switch
            {
                TipoJuros.Simples => valor * TaxaDiaria * diasAtraso,
                TipoJuros.Composto => valor * (FatorComposto(diasAtraso) - 1),
                _ => throw new ArgumentOutOfRangeException(nameof(tipo), tipo, "Tipo de juros desconhecido.")
            };

            // O total (valor + juros) também precisa caber em decimal.
            _ = valor + juros;
        }
        catch (OverflowException)
        {
            throw new CalculoInvalidoException(
                $"Valor ou atraso ({diasAtraso} dias) grande demais para o cálculo.");
        }

        return new ResultadoJuros(
            valor, vencimento, dataCalculo, tipo, diasAtraso,
            Math.Round(juros, 2, MidpointRounding.AwayFromZero));
    }

    // (1 + taxa)^dias calculado em decimal, sem passar por double.
    private static decimal FatorComposto(int dias)
    {
        decimal fator = 1m;
        for (int i = 0; i < dias; i++)
        {
            fator *= 1 + TaxaDiaria;
        }

        return fator;
    }
}
