namespace Dominio.Enums
{
    /// <summary>
    /// Enum que mapeia os tipos de movimentação de estoque conforme tabela TipoMovimentacao no banco.
    /// Os valores inteiros devem corresponder aos IDs no banco de dados.
    /// </summary>
    public enum TipoMovimentacaoEnum
    {
        Entrada = 1,
        Consumo = 2,
        TransferenciaSaida = 3,
        TransferenciaEntrada = 4
    }
}
