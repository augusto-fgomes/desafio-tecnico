using System.Text.Json;
using Exercicio1.Comissao.Services;

namespace Exercicio1.Comissao.Tests;

public class LeitorVendasTests
{
    private readonly LeitorVendas _leitor = new();

    [Fact]
    public void LerJson_FormatoValido_RetornaVendas()
    {
        var vendas = _leitor.LerJson("""
            { "vendas": [ { "vendedor": "João Silva", "valor": 1200.50 } ] }
            """);

        var venda = Assert.Single(vendas);
        Assert.Equal("João Silva", venda.Vendedor);
        Assert.Equal(1200.50m, venda.Valor);
    }

    [Fact]
    public void LerJson_SemVendas_RetornaVazio()
    {
        Assert.Empty(_leitor.LerJson("{}"));
    }

    [Fact]
    public void LerJson_Malformado_LancaJsonException()
    {
        Assert.ThrowsAny<JsonException>(() => _leitor.LerJson("{ vendas: "));
    }

    [Theory]
    [InlineData("""{ "vendas": [ { "vendedor": "   ", "valor": 10 } ] }""")]
    [InlineData("""{ "vendas": [ { "vendedor": "Ana", "valor": -10 } ] }""")]
    [InlineData("""{ "vendas": [ null ] }""")]
    public void LerJson_DadosInvalidos_LancaInvalidDataException(string json)
    {
        Assert.Throws<InvalidDataException>(() => _leitor.LerJson(json));
    }

    [Theory]
    [InlineData("""{ "vendas": [ { "vendedor": "Ana" } ] }""")]                 // sem valor
    [InlineData("""{ "vendas": [ { "valor": 10 } ] }""")]                       // sem vendedor
    [InlineData("""{ "vendas": [ { "vendedor": null, "valor": 10 } ] }""")]
    [InlineData("""{ "vendas": [ { "vendedor": "Ana", "valor": null } ] }""")]
    public void LerJson_CampoAusenteOuNulo_LancaJsonException(string json)
    {
        Assert.ThrowsAny<JsonException>(() => _leitor.LerJson(json));
    }

    [Fact]
    public void LerJson_RemoveEspacosDoNomeDoVendedor()
    {
        var vendas = _leitor.LerJson("""
            { "vendas": [ { "vendedor": "  Ana ", "valor": 10 } ] }
            """);

        Assert.Equal("Ana", Assert.Single(vendas).Vendedor);
    }

    [Fact]
    public void LerArquivo_Inexistente_LancaFileNotFound()
    {
        Assert.Throws<FileNotFoundException>(() => _leitor.LerArquivo("nao-existe.json"));
    }
}
