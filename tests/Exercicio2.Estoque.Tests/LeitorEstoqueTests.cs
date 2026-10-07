using System.Text.Json;
using Exercicio2.Estoque.Services;

namespace Exercicio2.Estoque.Tests;

public class LeitorEstoqueTests
{
    private readonly LeitorEstoque _leitor = new();

    [Fact]
    public void LerJson_FormatoValido_RetornaProdutos()
    {
        var produtos = _leitor.LerJson("""
            { "estoque": [ { "codigoProduto": 101, "descricaoProduto": "Caneta Azul", "estoque": 150 } ] }
            """);

        var produto = Assert.Single(produtos);
        Assert.Equal(101, produto.CodigoProduto);
        Assert.Equal("Caneta Azul", produto.DescricaoProduto);
        Assert.Equal(150, produto.Estoque);
    }

    [Fact]
    public void LerJson_Malformado_LancaJsonException()
    {
        Assert.ThrowsAny<JsonException>(() => _leitor.LerJson("{ estoque: "));
    }

    [Theory]
    [InlineData("""{ "estoque": [ { "codigoProduto": 1, "descricaoProduto": "A", "estoque": 1 }, { "codigoProduto": 1, "descricaoProduto": "B", "estoque": 1 } ] }""")]
    [InlineData("""{ "estoque": [ { "codigoProduto": 1, "descricaoProduto": " ", "estoque": 1 } ] }""")]
    [InlineData("""{ "estoque": [ { "codigoProduto": 1, "descricaoProduto": "A", "estoque": -1 } ] }""")]
    [InlineData("""{ "estoque": [ null ] }""")]
    public void LerJson_DadosInvalidos_LancaInvalidDataException(string json)
    {
        Assert.Throws<InvalidDataException>(() => _leitor.LerJson(json));
    }

    [Theory]
    [InlineData("""{ "estoque": [ { "descricaoProduto": "A", "estoque": 1 } ] }""")]   // sem código
    [InlineData("""{ "estoque": [ { "codigoProduto": 1, "estoque": 1 } ] }""")]          // sem descrição
    [InlineData("""{ "estoque": [ { "codigoProduto": 1, "descricaoProduto": "A" } ] }""")] // sem estoque
    [InlineData("""{ "estoque": [ { "codigoProduto": 1, "descricaoProduto": null, "estoque": 1 } ] }""")]
    public void LerJson_CampoAusenteOuNulo_LancaJsonException(string json)
    {
        Assert.ThrowsAny<JsonException>(() => _leitor.LerJson(json));
    }

    [Fact]
    public void LerArquivo_Inexistente_LancaFileNotFound()
    {
        Assert.Throws<FileNotFoundException>(() => _leitor.LerArquivo("nao-existe.json"));
    }
}
