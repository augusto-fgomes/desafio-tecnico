namespace Exercicio2.Estoque.Models;

/// <summary>Raiz do arquivo JSON: <c>{ "estoque": [ ... ] }</c>.</summary>
public class DadosEstoque
{
    public List<Produto> Estoque { get; set; } = [];
}
