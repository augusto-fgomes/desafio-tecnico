namespace Exercicio2.Estoque.Models;

/// <summary>Registro de uma entrada ou saída de mercadoria.</summary>
public record Movimentacao(
    int Id,
    int CodigoProduto,
    string DescricaoProduto,
    TipoMovimentacao Tipo,
    int Quantidade,
    string Descricao,
    DateTimeOffset DataHora,
    int EstoqueAnterior,
    int EstoqueFinal);
