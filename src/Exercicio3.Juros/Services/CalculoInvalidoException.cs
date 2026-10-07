namespace Exercicio3.Juros.Services;

/// <summary>Cálculo recusado por valores fora do permitido.</summary>
public class CalculoInvalidoException(string mensagem) : Exception(mensagem);
