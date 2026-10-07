namespace Exercicio2.Estoque.Models;

/// <summary>
/// Produto do depósito, com a quantidade em estoque.
/// É imutável: o estoque só muda por meio de <c>ServicoEstoque.Movimentar</c>.
/// </summary>
public record Produto
{
    public required int CodigoProduto { get; init; }
    public required string DescricaoProduto { get; init; }
    public required int Estoque { get; init; }
}
