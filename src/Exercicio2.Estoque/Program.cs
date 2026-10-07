using System.Globalization;
using System.Text;
using System.Text.Json;
using Exercicio2.Estoque.Models;
using Exercicio2.Estoque.Services;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;
var ptBr = CultureInfo.GetCultureInfo("pt-BR");

// Uso: dotnet run [caminho-do-json]
// Sem argumento, usa o Data/estoque.json copiado junto com o executável.
string caminho = args.Length > 0
    ? args[0]
    : Path.Combine(AppContext.BaseDirectory, "Data", "estoque.json");

ServicoEstoque estoque;
try
{
    estoque = new ServicoEstoque(new LeitorEstoque().LerArquivo(caminho));
}
catch (Exception ex) when (ex is IOException or JsonException or InvalidDataException
                               or UnauthorizedAccessException)
{
    Console.Error.WriteLine($"Erro ao carregar o estoque: {ex.Message}");
    return 1;
}

const string linha = "------------------------------------------------------------";

while (true)
{
    Console.WriteLine();
    Console.WriteLine(linha);
    Console.WriteLine("                  CONTROLE DE ESTOQUE");
    Console.WriteLine(linha);
    Console.WriteLine("1 - Listar produtos");
    Console.WriteLine("2 - Lançar movimentação");
    Console.WriteLine("3 - Histórico de movimentações");
    Console.WriteLine("0 - Sair");

    switch (Perguntar("Opção"))
    {
        case "1":
            ListarProdutos();
            break;
        case "2":
            LancarMovimentacao();
            break;
        case "3":
            ListarMovimentacoes();
            break;
        case "0" or null:
            return 0;
        default:
            Console.WriteLine("Opção inválida.");
            break;
    }
}

void ListarProdutos()
{
    Console.WriteLine();
    Console.WriteLine($"{"Código",-8}{"Produto",-30}{"Estoque",10}");
    foreach (var p in estoque.Produtos)
    {
        Console.WriteLine($"{p.CodigoProduto,-8}{p.DescricaoProduto,-30}{p.Estoque,10}");
    }
}

void LancarMovimentacao()
{
    ListarProdutos();
    Console.WriteLine();

    if (!int.TryParse(Perguntar("Código do produto"), out int codigo))
    {
        Console.WriteLine("Código inválido.");
        return;
    }

    var produto = estoque.BuscarProduto(codigo);
    if (produto is null)
    {
        Console.WriteLine($"Produto {codigo} não encontrado.");
        return;
    }

    Console.WriteLine($"Produto: {produto.DescricaoProduto} (estoque atual: {produto.Estoque})");

    TipoMovimentacao? tipo = Perguntar("Tipo (E = entrada, S = saída)")?.ToUpperInvariant() switch
    {
        "E" => TipoMovimentacao.Entrada,
        "S" => TipoMovimentacao.Saida,
        _ => null
    };
    if (tipo is null)
    {
        Console.WriteLine("Tipo inválido. Use E ou S.");
        return;
    }

    if (!int.TryParse(Perguntar("Quantidade"), out int quantidade))
    {
        Console.WriteLine("Quantidade inválida.");
        return;
    }

    string descricao = Perguntar("Descrição (ex.: compra de fornecedor, venda, devolução)") ?? "";

    try
    {
        var m = estoque.Movimentar(codigo, tipo.Value, quantidade, descricao);
        Console.WriteLine();
        Console.WriteLine(
            $"Movimentação #{m.Id} registrada: {NomeTipo(m.Tipo)} de {m.Quantidade} un. " +
            $"de {m.DescricaoProduto} ({m.Descricao}).");
        Console.WriteLine($"Estoque final de {m.DescricaoProduto}: {m.EstoqueFinal} " +
                          $"(antes: {m.EstoqueAnterior}).");
    }
    catch (MovimentacaoInvalidaException ex)
    {
        Console.WriteLine($"Movimentação recusada: {ex.Message}");
    }
}

void ListarMovimentacoes()
{
    Console.WriteLine();
    if (estoque.Movimentacoes.Count == 0)
    {
        Console.WriteLine("Nenhuma movimentação lançada.");
        return;
    }

    Console.WriteLine($"{"Id",-5}{"Data/hora",-21}{"Tipo",-9}{"Produto",-28}{"Qtde",6}{"Final",7}  Descrição");
    foreach (var m in estoque.Movimentacoes)
    {
        Console.WriteLine(
            string.Format(ptBr, "{0,-5}{1,-21:dd/MM/yyyy HH:mm:ss}{2,-9}{3,-28}{4,6}{5,7}  {6}",
                m.Id, m.DataHora, NomeTipo(m.Tipo), m.DescricaoProduto,
                m.Quantidade, m.EstoqueFinal, m.Descricao));
    }
}

static string NomeTipo(TipoMovimentacao tipo) =>
    tipo == TipoMovimentacao.Entrada ? "Entrada" : "Saída";

// Retorna null quando a entrada termina (Ctrl+D / fim do arquivo).
static string? Perguntar(string rotulo)
{
    Console.Write($"{rotulo}: ");
    return Console.ReadLine()?.Trim();
}
