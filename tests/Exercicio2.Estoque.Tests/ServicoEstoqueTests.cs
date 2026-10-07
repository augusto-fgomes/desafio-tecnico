using Exercicio2.Estoque.Models;
using Exercicio2.Estoque.Services;

namespace Exercicio2.Estoque.Tests;

public class ServicoEstoqueTests
{
    private static ServicoEstoque CriarServico() => new(
    [
        new Produto { CodigoProduto = 101, DescricaoProduto = "Caneta Azul", Estoque = 150 },
        new Produto { CodigoProduto = 102, DescricaoProduto = "Caderno Universitário", Estoque = 75 }
    ]);

    [Fact]
    public void Movimentar_Entrada_SomaAoEstoque()
    {
        var servico = CriarServico();

        var m = servico.Movimentar(101, TipoMovimentacao.Entrada, 50, "Compra de fornecedor");

        Assert.Equal(150, m.EstoqueAnterior);
        Assert.Equal(200, m.EstoqueFinal);
        Assert.Equal(200, servico.Produtos.Single(p => p.CodigoProduto == 101).Estoque);
    }

    [Fact]
    public void Movimentar_Saida_SubtraiDoEstoque()
    {
        var m = CriarServico().Movimentar(102, TipoMovimentacao.Saida, 25, "Venda");

        Assert.Equal(50, m.EstoqueFinal);
    }

    [Fact]
    public void Movimentar_SaidaDeTodoOEstoque_ZeraEstoque()
    {
        var m = CriarServico().Movimentar(102, TipoMovimentacao.Saida, 75, "Venda");

        Assert.Equal(0, m.EstoqueFinal);
    }

    [Fact]
    public void Movimentar_SaidaMaiorQueEstoque_RecusaENaoAlteraEstoque()
    {
        var servico = CriarServico();

        Assert.Throws<MovimentacaoInvalidaException>(
            () => servico.Movimentar(102, TipoMovimentacao.Saida, 76, "Venda"));
        Assert.Equal(75, servico.Produtos.Single(p => p.CodigoProduto == 102).Estoque);
        Assert.Empty(servico.Movimentacoes);
    }

    [Fact]
    public void Movimentar_ProdutoInexistente_Recusa()
    {
        Assert.Throws<MovimentacaoInvalidaException>(
            () => CriarServico().Movimentar(999, TipoMovimentacao.Entrada, 1, "Compra"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Movimentar_QuantidadeInvalida_Recusa(int quantidade)
    {
        Assert.Throws<MovimentacaoInvalidaException>(
            () => CriarServico().Movimentar(101, TipoMovimentacao.Entrada, quantidade, "Compra"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Movimentar_SemDescricao_Recusa(string descricao)
    {
        Assert.Throws<MovimentacaoInvalidaException>(
            () => CriarServico().Movimentar(101, TipoMovimentacao.Entrada, 1, descricao));
    }

    [Fact]
    public void Movimentar_GeraIdsUnicosESequenciais()
    {
        var servico = CriarServico();

        servico.Movimentar(101, TipoMovimentacao.Entrada, 10, "Compra");
        servico.Movimentar(102, TipoMovimentacao.Saida, 5, "Venda");
        servico.Movimentar(101, TipoMovimentacao.Saida, 3, "Avaria");

        Assert.Equal([1, 2, 3], servico.Movimentacoes.Select(m => m.Id));
    }

    [Fact]
    public void Movimentar_RecusadaNaoConsomeId()
    {
        var servico = CriarServico();

        Assert.Throws<MovimentacaoInvalidaException>(
            () => servico.Movimentar(999, TipoMovimentacao.Entrada, 1, "Compra"));
        var m = servico.Movimentar(101, TipoMovimentacao.Entrada, 1, "Compra");

        Assert.Equal(1, m.Id);
    }

    [Fact]
    public void Movimentar_EntradaQueEstouraOLimite_RecusaENaoAlteraEstoque()
    {
        var servico = CriarServico();

        Assert.Throws<MovimentacaoInvalidaException>(
            () => servico.Movimentar(101, TipoMovimentacao.Entrada, int.MaxValue, "Compra"));
        Assert.Equal(150, servico.BuscarProduto(101)!.Estoque);
    }

    [Fact]
    public void Movimentar_EntradaAteOLimite_Aceita()
    {
        var m = CriarServico().Movimentar(101, TipoMovimentacao.Entrada, int.MaxValue - 150, "Compra");

        Assert.Equal(int.MaxValue, m.EstoqueFinal);
    }

    [Fact]
    public void Movimentar_NaoAlteraOsProdutosRecebidosNoConstrutor()
    {
        var original = new Produto { CodigoProduto = 1, DescricaoProduto = "A", Estoque = 10 };
        var servico = new ServicoEstoque([original]);

        servico.Movimentar(1, TipoMovimentacao.Saida, 4, "Venda");

        Assert.Equal(10, original.Estoque);
        Assert.Equal(6, servico.BuscarProduto(1)!.Estoque);
    }

    [Fact]
    public void Movimentar_RegistraDataHoraDoRelogio()
    {
        var agora = new DateTimeOffset(2026, 10, 7, 14, 30, 0, TimeSpan.Zero);
        var servico = new ServicoEstoque(
            [new Produto { CodigoProduto = 1, DescricaoProduto = "A", Estoque = 10 }],
            new RelogioFixo(agora));

        var m = servico.Movimentar(1, TipoMovimentacao.Entrada, 1, "Compra");

        Assert.Equal(agora, m.DataHora);
    }

    [Fact]
    public void BuscarProduto_Inexistente_RetornaNull()
    {
        Assert.Null(CriarServico().BuscarProduto(999));
    }

    private sealed class RelogioFixo(DateTimeOffset agora) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => agora;
        public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
    }
}
