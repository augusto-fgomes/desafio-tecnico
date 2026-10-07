namespace Exercicio3.Juros.Models;

/// <summary>Resultado do cálculo de juros de um título em uma data.</summary>
public record ResultadoJuros(
    decimal Valor,
    DateOnly Vencimento,
    DateOnly DataCalculo,
    TipoJuros Tipo,
    int DiasAtraso,
    decimal Juros)
{
    public decimal Total => Valor + Juros;
}
