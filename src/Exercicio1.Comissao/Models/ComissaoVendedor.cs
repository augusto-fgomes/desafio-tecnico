namespace Exercicio1.Comissao.Models;

/// <summary>Resultado consolidado das vendas e da comissão de um vendedor.</summary>
public record ComissaoVendedor(
    string Vendedor,
    int QuantidadeVendas,
    decimal TotalVendido,
    decimal TotalComissao);
