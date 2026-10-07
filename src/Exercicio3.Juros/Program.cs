using System.Globalization;
using System.Text;
using Exercicio3.Juros.Models;
using Exercicio3.Juros.Services;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;
var ptBr = CultureInfo.GetCultureInfo("pt-BR");

const string linha = "------------------------------------------------------------";
const string titulo = "CÁLCULO DE JUROS";
Console.WriteLine(linha);
Console.WriteLine(titulo.PadLeft((linha.Length + titulo.Length) / 2));
Console.WriteLine(linha);

const string uso = "Uso: dotnet run -- <valor> <vencimento dd/MM/aaaa> [--composto]";

// Uso:
//   dotnet run -- <valor> <vencimento dd/MM/aaaa> [--composto]
//   dotnet run [--composto]         (modo interativo)
var posicionais = args.Where(a => !a.StartsWith("--")).ToArray();
var opcoes = args.Where(a => a.StartsWith("--")).Select(a => a.ToLowerInvariant()).ToArray();

var opcaoDesconhecida = opcoes.FirstOrDefault(o => o != "--composto");
if (opcaoDesconhecida is not null || posicionais.Length is not (0 or 2))
{
    Console.Error.WriteLine(opcaoDesconhecida is null
        ? "Erro: informe exatamente dois argumentos: o valor e a data de vencimento."
        : $"Erro: opção desconhecida '{opcaoDesconhecida}'.");
    Console.Error.WriteLine(uso);
    return 1;
}

var tipo = opcoes.Contains("--composto") ? TipoJuros.Composto : TipoJuros.Simples;
decimal valor;
DateOnly vencimento;

if (posicionais.Length == 2)
{
    if (!LeitorEntrada.TentarLerValor(posicionais[0], out valor, out var erroValor))
    {
        Console.Error.WriteLine($"Erro: {erroValor}");
        return 1;
    }

    if (!LeitorEntrada.TentarLerData(posicionais[1], out vencimento, out var erroData))
    {
        Console.Error.WriteLine($"Erro: {erroData}");
        return 1;
    }
}
else
{
    // Modo interativo: repete cada pergunta até receber uma resposta válida.
    decimal? valorLido = PerguntarAteValido<decimal>("Valor (ex.: 1.500,00)",
        (string texto, out decimal v, out string e) => LeitorEntrada.TentarLerValor(texto, out v, out e));
    DateOnly? vencimentoLido = valorLido is null ? null : PerguntarAteValido<DateOnly>(
        "Data de vencimento (dd/MM/aaaa)",
        (string texto, out DateOnly d, out string e) => LeitorEntrada.TentarLerData(texto, out d, out e));

    if (valorLido is null || vencimentoLido is null)
    {
        Console.Error.WriteLine("Erro: entrada encerrada antes de informar valor e vencimento.");
        return 1;
    }

    valor = valorLido.Value;
    vencimento = vencimentoLido.Value;

    if (!opcoes.Contains("--composto"))
    {
        tipo = PerguntarAteValido<TipoJuros>("Tipo de juros (S = simples, C = composto) [S]",
            (string texto, out TipoJuros t, out string e) =>
            {
                (t, e) = texto.ToUpperInvariant() switch
                {
                    "" or "S" => (TipoJuros.Simples, ""),
                    "C" => (TipoJuros.Composto, ""),
                    _ => (TipoJuros.Simples, "responda S ou C.")
                };
                return e == "";
            }) ?? TipoJuros.Simples;
    }
}

var hoje = DateOnly.FromDateTime(DateTime.Now);

try
{
    var r = new CalculadoraJuros().Calcular(valor, vencimento, hoje, tipo);

    Console.WriteLine(linha);
    Console.WriteLine(titulo.PadLeft((linha.Length + titulo.Length) / 2));
    Console.WriteLine(linha);
    Console.WriteLine(string.Format(ptBr, "{0,-22}{1,18:C}", "Valor original:", r.Valor));
    Console.WriteLine(string.Format(ptBr, "{0,-22}{1,18:dd/MM/yyyy}", "Vencimento:", r.Vencimento));
    Console.WriteLine(string.Format(ptBr, "{0,-22}{1,18:dd/MM/yyyy}", "Data do cálculo:", r.DataCalculo));
    Console.WriteLine($"{"Dias em atraso:",-22}{r.DiasAtraso,18}");
    Console.WriteLine(string.Format(ptBr, "{0,-22}{1,18}", "Taxa:",
        string.Format(ptBr, "{0:0.0}% ao dia", CalculadoraJuros.TaxaDiaria * 100)));
    Console.WriteLine($"{"Tipo de juros:",-22}{(r.Tipo == TipoJuros.Simples ? "Simples" : "Composto"),18}");
    Console.WriteLine(string.Format(ptBr, "{0,-22}{1,18:C}", "Juros:", r.Juros));
    Console.WriteLine(linha);
    Console.WriteLine(string.Format(ptBr, "{0,-22}{1,18:C}", "Total a pagar:", r.Total));
    return 0;
}
catch (CalculoInvalidoException ex)
{
    Console.Error.WriteLine($"Erro: {ex.Message}");
    return 1;
}

// Retorna null quando a entrada termina (Ctrl+D / fim do arquivo).
static T? PerguntarAteValido<T>(string rotulo, Conversor<T> converter) where T : struct
{
    while (true)
    {
        Console.Write($"{rotulo}: ");
        string? texto = Console.ReadLine();
        if (texto is null)
        {
            Console.WriteLine();
            return null;
        }

        if (converter(texto.Trim(), out T resultado, out string erro))
        {
            return resultado;
        }

        Console.WriteLine($"  {char.ToUpper(erro[0])}{erro[1..]}");
    }
}

delegate bool Conversor<T>(string texto, out T resultado, out string erro);
