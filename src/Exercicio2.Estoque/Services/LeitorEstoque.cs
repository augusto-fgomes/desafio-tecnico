using System.Text.Json;
using Exercicio2.Estoque.Models;

namespace Exercicio2.Estoque.Services;

/// <summary>Lê e valida o arquivo JSON de estoque.</summary>
public class LeitorEstoque
{
    private static readonly JsonSerializerOptions Opcoes = new()
    {
        PropertyNameCaseInsensitive = true,
        // Campos ausentes ou nulos geram JsonException em vez de virarem 0 ou null silenciosamente.
        RespectNullableAnnotations = true
    };

    public IReadOnlyList<Produto> LerArquivo(string caminho)
    {
        if (!File.Exists(caminho))
        {
            throw new FileNotFoundException($"Arquivo '{caminho}' não encontrado.", caminho);
        }

        return LerJson(File.ReadAllText(caminho));
    }

    public IReadOnlyList<Produto> LerJson(string json)
    {
        var dados = JsonSerializer.Deserialize<DadosEstoque>(json, Opcoes);
        var produtos = dados?.Estoque ?? [];
        var codigos = new HashSet<int>();

        for (int i = 0; i < produtos.Count; i++)
        {
            var produto = produtos[i];

            if (produto is null)
            {
                throw new InvalidDataException($"Produto na posição {i} está vazio (null).");
            }

            if (!codigos.Add(produto.CodigoProduto))
            {
                throw new InvalidDataException($"Código de produto duplicado: {produto.CodigoProduto}.");
            }

            if (string.IsNullOrWhiteSpace(produto.DescricaoProduto))
            {
                throw new InvalidDataException($"Produto {produto.CodigoProduto} está sem descrição.");
            }

            if (produto.Estoque < 0)
            {
                throw new InvalidDataException(
                    $"Produto {produto.CodigoProduto} tem estoque negativo: {produto.Estoque}.");
            }
        }

        return produtos.Select(p => p with { DescricaoProduto = p.DescricaoProduto.Trim() }).ToList();
    }
}
