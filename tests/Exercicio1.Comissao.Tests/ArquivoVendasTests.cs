using Exercicio1.Comissao.Services;

namespace Exercicio1.Comissao.Tests;

/// <summary>Teste de ponta a ponta com o Data/vendas.json real do exercício.</summary>
public class ArquivoVendasTests
{
    [Fact]
    public void ArquivoDoExercicio_GeraOsTotaisDocumentadosNoReadme()
    {
        var caminho = Path.Combine(AppContext.BaseDirectory, "Data", "vendas.json");
        var vendas = new LeitorVendas().LerArquivo(caminho);

        var resultado = new CalculadoraComissao().CalcularPorVendedor(vendas);

        Assert.Equal(
            [
                ("João Silva", 10, 10754.70m, 495.69m),
                ("Maria Souza", 9, 9874.30m, 465.96m),
                ("Ana Lima", 9, 8763.95m, 404.99m),
                ("Carlos Oliveira", 8, 7928.35m, 379.38m)
            ],
            resultado.Select(c => (c.Vendedor, c.QuantidadeVendas, c.TotalVendido, c.TotalComissao)));
        Assert.Equal(1746.02m, resultado.Sum(c => c.TotalComissao));
    }
}
