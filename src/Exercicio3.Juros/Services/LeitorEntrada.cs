using System.Globalization;
using System.Text.RegularExpressions;

namespace Exercicio3.Juros.Services;

/// <summary>
/// Converte o texto digitado pelo usuário em valor e data, sempre no formato brasileiro.
/// </summary>
public static partial class LeitorEntrada
{
    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    // "1500", "1500,5", "1500,50", "1.500", "1.500,50", "1.000.000,00".
    // Ponto só como separador de milhar (grupos de 3) e no máximo 2 casas decimais,
    // para que "1.500" nunca seja lido como 1,5.
    [GeneratedRegex(@"^(\d{1,3}(\.\d{3})+|\d+)(,\d{1,2})?$")]
    private static partial Regex FormatoValor();

    public static bool TentarLerValor(string? texto, out decimal valor, out string erro)
    {
        valor = 0;
        texto = texto?.Replace("R$", "").Trim();

        if (string.IsNullOrEmpty(texto) || !FormatoValor().IsMatch(texto))
        {
            erro = $"valor inválido '{texto}'. Use o formato 1500,50 ou 1.500,50 " +
                   "(vírgula para decimais, no máximo 2 casas).";
            return false;
        }

        if (!decimal.TryParse(texto, NumberStyles.Number, PtBr, out valor))
        {
            erro = $"valor '{texto}' é grande demais.";
            return false;
        }

        if (valor <= 0)
        {
            erro = "o valor deve ser maior que zero.";
            return false;
        }

        erro = "";
        return true;
    }

    public static bool TentarLerData(string? texto, out DateOnly data, out string erro)
    {
        if (DateOnly.TryParseExact(texto?.Trim(), "dd/MM/yyyy", PtBr, DateTimeStyles.None, out data))
        {
            erro = "";
            return true;
        }

        erro = $"data inválida '{texto}'. Use o formato dd/MM/aaaa.";
        return false;
    }
}
