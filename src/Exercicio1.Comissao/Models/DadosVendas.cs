namespace Exercicio1.Comissao.Models;

/// <summary>Raiz do arquivo JSON: <c>{ "vendas": [ ... ] }</c>.</summary>
public class DadosVendas
{
    public List<Venda> Vendas { get; set; } = [];
}
