# Desafio Técnico — C#

Solução em C# / .NET 10 para um desafio técnico de entrevista composto por 3 exercícios.
Cada exercício é um projeto de console independente, com seu próprio arquivo JSON de entrada
e seu próprio projeto de testes.

| # | Exercício | Projeto | Status |
|---|-----------|---------|--------|
| 1 | Cálculo de comissão por vendedor | [`src/Exercicio1.Comissao`](src/Exercicio1.Comissao) | ✅ Concluído |
| 2 | Movimentação de estoque | [`src/Exercicio2.Estoque`](src/Exercicio2.Estoque) | ✅ Concluído |
| 3 | Juros por atraso | [`src/Exercicio3.Juros`](src/Exercicio3.Juros) | ✅ Concluído |

## Como rodar na sua máquina

### 1. Instalar o .NET SDK 10

O projeto usa o **.NET 10** (versão fixada em `global.json`). Instale o **SDK**; o runtime
sozinho não é suficiente.

| Sistema | Comando |
|---------|---------|
| **Windows** | `winget install Microsoft.DotNet.SDK.10` |
| **macOS** | `brew install --cask dotnet-sdk` |
| **Linux (Ubuntu/Debian)** | `sudo apt-get update && sudo apt-get install -y dotnet-sdk-10.0` |

Também é possível baixar o instalador em <https://dotnet.microsoft.com/download/dotnet/10.0>.

Depois de instalar, **abra um terminal novo** e confira:

```bash
dotnet --version    # deve mostrar 10.0.x
```

### 2. Baixar o repositório

```bash
git clone https://github.com/augusto-fgomes/desafio-comissao.git
cd desafio-comissao
```

Sem Git, use **Code → Download ZIP** na página do GitHub, extraia o arquivo e abra um terminal
dentro da pasta extraída.

### 3. Compilar e rodar os testes

Na raiz do repositório (a pasta que contém `DesafioTecnico.slnx`):

```bash
dotnet build    # baixa as dependências e compila todos os projetos
dotnet test     # roda os testes dos 3 exercícios
```

O resultado esperado é `Compilação com êxito` / `Build succeeded` e todos os testes aprovados.

### 4. Rodar os exercícios

Também na raiz do repositório:

```bash
# Exercício 1 — comissões (lê src/Exercicio1.Comissao/Data/vendas.json)
dotnet run --project src/Exercicio1.Comissao

# Exercício 2 — estoque (abre um menu interativo; digite 0 para sair)
dotnet run --project src/Exercicio2.Estoque

# Exercício 3 — juros (valor e data de vencimento)
dotnet run --project src/Exercicio3.Juros -- 1.000,00 27/09/2026
dotnet run --project src/Exercicio3.Juros -- 1.000,00 27/09/2026 --composto
dotnet run --project src/Exercicio3.Juros    # modo interativo *Recomendado*
```

O `--` separa as opções do `dotnet run` dos argumentos do programa. Os detalhes de cada exercício
(regras, formatos aceitos e exemplos de saída) estão nas seções abaixo.

> **No PowerShell**, coloque o valor entre aspas se ele tiver vírgula:
> `dotnet run --project src/Exercicio3.Juros -- "1.000,00" 27/09/2026`.

### Pela IDE (opcional)

- **Visual Studio 2026** ou **JetBrains Rider**: abra o arquivo `DesafioTecnico.slnx`, escolha o
  projeto do exercício como projeto de inicialização e execute (F5). Os testes ficam no
  *Test Explorer*.
- **VS Code**: instale a extensão **C# Dev Kit**, abra a pasta do repositório e use o painel
  *Solution Explorer* para rodar cada projeto e o painel *Testing* para os testes.

### Problemas comuns

| Sintoma | Causa e solução |
|---------|-----------------|
| `dotnet: command not found` / `não é reconhecido como comando` | O SDK não está instalado ou o terminal foi aberto antes da instalação. Instale o SDK e abra um terminal novo. |
| `A compatible .NET SDK was not found` / `Requested SDK version: 10.0.100` | Há um SDK mais antigo (ex.: 8 ou 9). Instale o SDK 10 (passo 1). |
| `The provided file path does not exist` / `O caminho do arquivo fornecido não existe` | O comando foi executado fora da raiz do repositório. Volte para a pasta que contém `DesafioTecnico.slnx`. |
| Acentos aparecem quebrados no Windows | Use o **Windows Terminal** ou rode `chcp 65001` antes de executar. |
| `CultureNotFoundException` em contêiner Linux | O ambiente não tem dados de globalização (ICU). Instale `icu-libs`/`libicu` ou desative `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT`. |

## Estrutura do repositório

```text
.
├── DesafioTecnico.slnx                  # solution com todos os projetos
├── global.json                          # versão do SDK
├── src/
│   ├── Exercicio1.Comissao/             # exercício 1 (console)
│   │   ├── Data/vendas.json             # dados de entrada
│   │   ├── Models/                      # Venda, DadosVendas, ComissaoVendedor
│   │   ├── Services/                    # LeitorVendas, CalculadoraComissao
│   │   └── Program.cs                   # ponto de entrada e saída no console
│   ├── Exercicio2.Estoque/              # exercício 2 (console interativo)
│   │   ├── Data/estoque.json            # produtos e estoque inicial
│   │   ├── Models/                      # Produto, DadosEstoque, Movimentacao, TipoMovimentacao
│   │   ├── Services/                    # LeitorEstoque, ServicoEstoque, MovimentacaoInvalidaException
│   │   └── Program.cs                   # menu
│   └── Exercicio3.Juros/                # exercício 3 (console)
│       ├── Models/                      # ResultadoJuros, TipoJuros
│       ├── Services/                    # CalculadoraJuros, LeitorEntrada, CalculoInvalidoException
│       └── Program.cs                   # argumentos ou modo interativo
└── tests/
    ├── Exercicio1.Comissao.Tests/       # testes unitários (xUnit)
    ├── Exercicio2.Estoque.Tests/
    └── Exercicio3.Juros.Tests/
```

---

## Exercício 1 — Comissão de vendedores

### Enunciado

> Considerando que o JSON abaixo tem registros de vendas de um time comercial, faça um programa
> que leia os dados e calcule a comissão de cada vendedor, seguindo a seguinte regra para cada venda:
>
> - Vendas abaixo de R$ 100,00 não geram comissão
> - Vendas abaixo de R$ 500,00 geram 1% de comissão
> - A partir de R$ 500,00 geram 5% de comissão

### Regras implementadas

A comissão é calculada **venda a venda** e depois somada por vendedor:

| Valor da venda | Comissão |
|----------------|----------|
| `valor < 100,00` | 0% |
| `100,00 ≤ valor < 500,00` | 1% |
| `valor ≥ 500,00` | 5% |

### Formato do JSON

Arquivo padrão: [`src/Exercicio1.Comissao/Data/vendas.json`](src/Exercicio1.Comissao/Data/vendas.json)

```json
{
  "vendas": [
    { "vendedor": "João Silva", "valor": 1200.50 },
    { "vendedor": "Maria Souza", "valor": 950.75 }
  ]
}
```

### Como rodar

```bash
# com o arquivo padrão (Data/vendas.json)
dotnet run --project src/Exercicio1.Comissao

# com outro arquivo JSON
dotnet run --project src/Exercicio1.Comissao -- caminho/para/outro.json

# apenas os testes deste exercício
dotnet test tests/Exercicio1.Comissao.Tests
```

### Saída esperada

```text
------------------------------------------------------------
                   COMISSÕES POR VENDEDOR
------------------------------------------------------------
Vendedor              Vendas    Total vendido       Comissão
------------------------------------------------------------
João Silva                10     R$ 10.754,70      R$ 495,69
Maria Souza                9      R$ 9.874,30      R$ 465,96
Ana Lima                   9      R$ 8.763,95      R$ 404,99
Carlos Oliveira            8      R$ 7.928,35      R$ 379,38
------------------------------------------------------------
TOTAL                     36     R$ 37.321,30    R$ 1.746,02
```

### Decisões de implementação

- **`decimal` para valores monetários**, evitando erros de arredondamento de ponto flutuante.
- **Arredondamento por venda**: a comissão de cada venda é arredondada para centavos
  (`MidpointRounding.AwayFromZero`) antes da soma, já que cada venda gera um valor monetário próprio.
- **Separação de responsabilidades**: `LeitorVendas` lê e valida o JSON; `CalculadoraComissao`
  contém apenas a regra de negócio; `Program.cs` cuida da entrada/saída no console.
- **Validação dos dados**: campos ausentes ou nulos (`vendedor`, `valor`), itens `null` no array,
  vendedor em branco e valor negativo são rejeitados com mensagem clara — nada vira 0 silenciosamente.
- **Nome do vendedor normalizado**: espaços no início/fim são removidos, para que `"Ana"` e `"Ana "`
  sejam o mesmo vendedor. Maiúsculas e minúsculas são preservadas (`"Ana"` ≠ `"ana"`).
- **Tratamento de erros**: arquivo inexistente ou sem permissão de leitura, JSON malformado e dados
  inválidos geram mensagem em `stderr` e código de saída `1`.
- **Ordenação**: vendedores listados da maior para a menor comissão.
- **Formatação pt-BR**: valores exibidos como `R$ 1.234,56`, independentemente da cultura da máquina.
  A coluna de vendedor se ajusta ao maior nome.

### Testes

Os testes em `tests/Exercicio1.Comissao.Tests` cobrem:

- os limites de cada faixa (`99,99`, `100,00`, `499,99`, `500,00`) e o arredondamento;
- a agregação por vendedor e a ordenação do resultado;
- a leitura do JSON: formato válido, lista vazia, JSON malformado, campos ausentes ou nulos, item
  `null`, valor negativo, remoção de espaços do nome e arquivo inexistente;
- um teste de ponta a ponta que lê o `Data/vendas.json` real e confere os totais mostrados acima.

---

## Exercício 2 — Movimentação de estoque

### Enunciado

> Faça um programa onde eu possa lançar movimentações de estoque dos produtos que estão no JSON
> abaixo, dando entrada ou saída da mercadoria no meu depósito, onde cada movimentação deve ter:
>
> - Um número identificador único.
> - Uma descrição para identificar o tipo da movimentação realizada
>
> E que ao final da movimentação me retorne a quantidade final do estoque do produto movimentado.

### Formato do JSON

Arquivo padrão: [`src/Exercicio2.Estoque/Data/estoque.json`](src/Exercicio2.Estoque/Data/estoque.json)

```json
{
  "estoque": [
    { "codigoProduto": 101, "descricaoProduto": "Caneta Azul", "estoque": 150 },
    { "codigoProduto": 102, "descricaoProduto": "Caderno Universitário", "estoque": 75 }
  ]
}
```

### Como rodar

```bash
# com o arquivo padrão (Data/estoque.json)
dotnet run --project src/Exercicio2.Estoque

# com outro arquivo JSON
dotnet run --project src/Exercicio2.Estoque -- caminho/para/outro.json

# apenas os testes deste exercício
dotnet test tests/Exercicio2.Estoque.Tests
```

O programa abre um menu interativo:

```text
1 - Listar produtos
2 - Lançar movimentação
3 - Histórico de movimentações
0 - Sair
```

Ao lançar uma movimentação, o usuário informa o código do produto (validado na hora), o tipo
(`E` = entrada, `S` = saída), a quantidade e uma descrição. Exemplo de retorno:

```text
Movimentação #1 registrada: Saída de 20 un. de Caneta Azul (Venda balcão).
Estoque final de Caneta Azul: 130 (antes: 150).
```

E o histórico:

```text
Id   Data/hora            Tipo     Produto                       Qtde  Final  Descrição
1    07/10/2026 10:42:58  Saída    Caneta Azul                     20    130  Venda balcão
2    07/10/2026 10:42:58  Entrada  Borracha Branca                 50    250  Compra de fornecedor
```

### Regras implementadas

- **Identificador único**: cada movimentação recebe um id sequencial (1, 2, 3…), gerado pelo
  `ServicoEstoque`. Movimentações recusadas não consomem id.
- **Tipo + descrição**: o tipo (Entrada/Saída) define a operação; a descrição livre identifica o
  motivo (compra, venda, devolução, avaria…) e é obrigatória.
- **Estoque final**: toda movimentação retorna o estoque anterior e o estoque final do produto.
- **Validações**: o produto deve existir, a quantidade deve ser maior que zero, uma saída não pode
  deixar o estoque negativo e uma entrada não pode ultrapassar o limite de `int` (2.147.483.647).
  Em caso de recusa, o estoque não é alterado.
- **Estado protegido**: `Produto` é um `record` imutável; o estoque só muda por
  `ServicoEstoque.Movimentar`, que substitui o produto por uma cópia atualizada.
- **Somente em memória**: o JSON original não é alterado; ao sair do programa o estoque volta ao inicial.
- **Validação do JSON**: campos ausentes ou nulos, item `null`, código de produto duplicado, produto
  sem descrição ou com estoque negativo geram erro na carga.
- **Data/hora injetável**: o `ServicoEstoque` recebe um `TimeProvider`, o que permite testar a data
  registrada em cada movimentação.

### Testes

Os testes em `tests/Exercicio2.Estoque.Tests` cobrem:

- entrada, saída e saída de todo o estoque;
- recusas: saída maior que o estoque, entrada que estoura o limite, produto inexistente, quantidade
  zero/negativa e descrição vazia — sempre sem alterar o estoque;
- ids únicos e sequenciais, e recusas que não consomem id;
- imutabilidade dos produtos recebidos e a data/hora vinda do relógio injetado;
- a leitura do JSON: formato válido, JSON malformado, campos ausentes ou nulos, item `null`, código
  duplicado, descrição vazia, estoque negativo e arquivo inexistente.

---

## Exercício 3 — Juros por atraso

### Enunciado

> Faça um programa que, a partir de um valor e de uma data de vencimento, calcule o valor dos juros
> na data de hoje, considerando que a multa seja de 2,5% ao dia.

### Como rodar

```bash
# valor e vencimento por argumento (juros simples)
dotnet run --project src/Exercicio3.Juros -- 1000 27/09/2026

# juros compostos
dotnet run --project src/Exercicio3.Juros -- 1000 27/09/2026 --composto

# modo interativo (pergunta valor, vencimento e tipo de juros, repetindo se a resposta for inválida)
dotnet run --project src/Exercicio3.Juros

# apenas os testes deste exercício
dotnet test tests/Exercicio3.Juros.Tests
```

Formatos de entrada (sempre padrão brasileiro):

| Entrada | Aceita | Rejeitada |
|---------|--------|-----------|
| Valor | `1500`, `1500,50`, `1.500`, `1.500,50`, `1.000.000,00`, `R$ 1.500,00` | `1500.50`, `1,500.50`, `0,001` (mais de 2 casas), `0`, negativos |
| Vencimento | `27/09/2026` | `2026-09-27`, `09/27/2026`, `27/9/26` |

O ponto é sempre separador de milhar, então `1.500` é mil e quinhentos (nunca R$ 1,50).
Argumentos a mais e opções desconhecidas são recusados; `--composto` aceita maiúsculas ou minúsculas.

### Saída esperada (calculado em 07/10/2026)

```text
------------------------------------------------------------
                      CÁLCULO DE JUROS
------------------------------------------------------------
Valor original:              R$ 1.000,00
Vencimento:                   27/09/2026
Data do cálculo:              07/10/2026
Dias em atraso:                       10
Taxa:                        2,5% ao dia
Tipo de juros:                   Simples
Juros:                         R$ 250,00
------------------------------------------------------------
Total a pagar:               R$ 1.250,00
```

### Regras implementadas

- **Dias de atraso** = dias corridos entre o vencimento e hoje. Pagamento no dia do vencimento ou
  antes não gera juros.
- **Juros simples (padrão)**: `valor × 2,5% × dias` → R$ 1.000 com 10 dias = R$ 250,00.
- **Juros compostos (opcional, `--composto`)**: `valor × ((1 + 2,5%)^dias − 1)` → R$ 1.000 com
  10 dias = R$ 280,08.
- O enunciado não deixa claro se os juros são simples ou compostos. Por isso o padrão é o simples
  (leitura mais literal de "2,5% ao dia") e o composto fica disponível como opção.
- **Arredondamento** para centavos (`MidpointRounding.AwayFromZero`), com `decimal` em todo o cálculo.
- **Data de cálculo injetável**: `CalculadoraJuros.Calcular` recebe a data de referência, o que
  permite testes determinísticos; o programa usa a data de hoje.
- **Validações**: valor deve ser maior que zero; valor ou data em formato inválido, argumentos
  incorretos e valores/atrasos grandes demais para o cálculo geram mensagem em `stderr` e código de
  saída `1`.

### Testes

Os testes em `tests/Exercicio3.Juros.Tests` cobrem:

- contagem de dias de atraso: vencimento hoje, no futuro, 1 dia, vários dias e virada de mês;
- juros simples e compostos com valores conferidos à mão, e o arredondamento;
- recusas: valor zero/negativo e estouro de `decimal` (simples e composto);
- leitura do valor (`1.500` = 1500, ponto decimal recusado, mais de 2 casas recusado etc.) e da data.
