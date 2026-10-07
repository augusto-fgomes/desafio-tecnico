using Exercicio2.Estoque.Models;

namespace Exercicio2.Estoque.Services;

/// <summary>
/// Mantém o estoque em memória e registra as movimentações.
/// Cada movimentação recebe um identificador sequencial único (1, 2, 3...).
/// </summary>
public class ServicoEstoque
{
    private readonly Dictionary<int, Produto> _produtos;
    private readonly List<Movimentacao> _movimentacoes = [];
    private readonly TimeProvider _relogio;
    private int _ultimoId;

    public ServicoEstoque(IEnumerable<Produto> produtos, TimeProvider? relogio = null)
    {
        _produtos = produtos.ToDictionary(p => p.CodigoProduto);
        _relogio = relogio ?? TimeProvider.System;
    }

    public IReadOnlyList<Produto> Produtos =>
        _produtos.Values.OrderBy(p => p.CodigoProduto).ToList();

    public IReadOnlyList<Movimentacao> Movimentacoes => _movimentacoes.AsReadOnly();

    public Produto? BuscarProduto(int codigoProduto) =>
        _produtos.GetValueOrDefault(codigoProduto);

    /// <summary>
    /// Lança uma entrada ou saída de mercadoria e retorna a movimentação,
    /// incluindo o estoque final do produto.
    /// </summary>
    public Movimentacao Movimentar(int codigoProduto, TipoMovimentacao tipo, int quantidade, string descricao)
    {
        var produto = BuscarProduto(codigoProduto)
            ?? throw new MovimentacaoInvalidaException($"Produto {codigoProduto} não encontrado.");

        if (quantidade <= 0)
        {
            throw new MovimentacaoInvalidaException("A quantidade deve ser maior que zero.");
        }

        if (string.IsNullOrWhiteSpace(descricao))
        {
            throw new MovimentacaoInvalidaException("A descrição da movimentação é obrigatória.");
        }

        if (tipo == TipoMovimentacao.Saida && quantidade > produto.Estoque)
        {
            throw new MovimentacaoInvalidaException(
                $"Estoque insuficiente de '{produto.DescricaoProduto}': " +
                $"disponível {produto.Estoque}, solicitado {quantidade}.");
        }

        if (tipo == TipoMovimentacao.Entrada && quantidade > int.MaxValue - produto.Estoque)
        {
            throw new MovimentacaoInvalidaException(
                $"A entrada ultrapassa o estoque máximo permitido ({int.MaxValue}).");
        }

        int estoqueFinal = tipo == TipoMovimentacao.Entrada
            ? produto.Estoque + quantidade
            : produto.Estoque - quantidade;
        _produtos[codigoProduto] = produto with { Estoque = estoqueFinal };

        var movimentacao = new Movimentacao(
            Id: ++_ultimoId,
            CodigoProduto: produto.CodigoProduto,
            DescricaoProduto: produto.DescricaoProduto,
            Tipo: tipo,
            Quantidade: quantidade,
            Descricao: descricao.Trim(),
            DataHora: _relogio.GetLocalNow(),
            EstoqueAnterior: produto.Estoque,
            EstoqueFinal: estoqueFinal);

        _movimentacoes.Add(movimentacao);
        return movimentacao;
    }
}
