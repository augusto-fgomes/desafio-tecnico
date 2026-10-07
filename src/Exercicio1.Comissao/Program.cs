using System.Globalization;
using System.Text;
using System.Text.Json;
using Exercicio1.Comissao.Services;

Console.OutputEncoding = Encoding.UTF8;
var ptBr = CultureInfo.GetCultureInfo("pt-BR");

// Uso: dotnet run [caminho-do-json]
// Sem argumento, usa o Data/vendas.json copiado junto com o executável.
string caminho = args.Length > 0
    ? args[0]
    : Path.Combine(AppContext.BaseDirectory, "Data", "vendas.json");

try
{
    var vendas = new LeitorVendas().LerArquivo(caminho);

    if (vendas.Count == 0)
    {
        Console.WriteLine("Nenhuma venda encontrada.");
        return 0;
    }

    var comissoes = new CalculadoraComissao().CalcularPorVendedor(vendas);

    // A primeira coluna se ajusta ao maior nome de vendedor.
    int larguraNome = Math.Max(20, comissoes.Max(c => c.Vendedor.Length) + 2);
    string formato = $"{{0,-{larguraNome}}}{{1,8}}{{2,17:C}}{{3,15:C}}";
    string linha = new('-', larguraNome + 40);

    Console.WriteLine(linha);
    Console.WriteLine("COMISSÕES POR VENDEDOR".PadLeft((linha.Length + 22) / 2));
    Console.WriteLine(linha);
    Console.WriteLine(string.Format($"{{0,-{larguraNome}}}{{1,8}}{{2,17}}{{3,15}}",
        "Vendedor", "Vendas", "Total vendido", "Comissão"));
    Console.WriteLine(linha);

    foreach (var c in comissoes)
    {
        Console.WriteLine(string.Format(ptBr, formato,
            c.Vendedor, c.QuantidadeVendas, c.TotalVendido, c.TotalComissao));
    }

    Console.WriteLine(linha);
    Console.WriteLine(string.Format(ptBr, formato,
        "TOTAL",
        comissoes.Sum(c => c.QuantidadeVendas),
        comissoes.Sum(c => c.TotalVendido),
        comissoes.Sum(c => c.TotalComissao)));

    return 0;
}
catch (FileNotFoundException ex)
{
    Console.Error.WriteLine($"Erro: {ex.Message}");
}
catch (JsonException ex)
{
    Console.Error.WriteLine($"Erro: o arquivo JSON possui um formato inválido. {ex.Message}");
}
catch (InvalidDataException ex)
{
    Console.Error.WriteLine($"Erro: dados inválidos. {ex.Message}");
}
catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
{
    Console.Error.WriteLine($"Erro ao ler o arquivo '{caminho}': {ex.Message}");
}

return 1;
