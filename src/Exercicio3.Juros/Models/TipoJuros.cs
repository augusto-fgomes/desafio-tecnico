namespace Exercicio3.Juros.Models;

public enum TipoJuros
{
    /// <summary>valor × taxa × dias</summary>
    Simples,

    /// <summary>valor × ((1 + taxa)^dias − 1)</summary>
    Composto
}
