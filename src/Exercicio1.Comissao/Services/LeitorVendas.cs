using System.Text.Json;
using Exercicio1.Comissao.Models;

namespace Exercicio1.Comissao.Services;

/// <summary>Lê e valida o arquivo JSON de vendas.</summary>
public class LeitorVendas
{
    private static readonly JsonSerializerOptions Opcoes = new()
    {
        PropertyNameCaseInsensitive = true,
        // Campos ausentes ou nulos ("valor" faltando, "vendedor": null) geram JsonException
        // em vez de virarem 0 ou null silenciosamente.
        RespectRequiredConstructorParameters = true,
        RespectNullableAnnotations = true
    };

    public IReadOnlyList<Venda> LerArquivo(string caminho)
    {
        if (!File.Exists(caminho))
        {
            throw new FileNotFoundException($"Arquivo '{caminho}' não encontrado.", caminho);
        }

        return LerJson(File.ReadAllText(caminho));
    }

    public IReadOnlyList<Venda> LerJson(string json)
    {
        var dados = JsonSerializer.Deserialize<DadosVendas>(json, Opcoes);
        var vendas = dados?.Vendas ?? [];
        var resultado = new List<Venda>(vendas.Count);

        for (int i = 0; i < vendas.Count; i++)
        {
            var venda = vendas[i];

            if (venda is null)
            {
                throw new InvalidDataException($"Venda na posição {i} está vazia (null).");
            }

            if (string.IsNullOrWhiteSpace(venda.Vendedor))
            {
                throw new InvalidDataException($"Venda na posição {i} está sem vendedor.");
            }

            if (venda.Valor < 0)
            {
                throw new InvalidDataException(
                    $"Venda na posição {i} ({venda.Vendedor}) tem valor negativo: {venda.Valor}.");
            }

            // Espaços extras não podem separar o mesmo vendedor em dois grupos.
            resultado.Add(venda with { Vendedor = venda.Vendedor.Trim() });
        }

        return resultado;
    }
}
