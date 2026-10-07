namespace Exercicio2.Estoque.Services;

/// <summary>Movimentação recusada por violar uma regra de negócio.</summary>
public class MovimentacaoInvalidaException(string mensagem) : Exception(mensagem);
