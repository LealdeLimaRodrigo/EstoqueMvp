namespace Dominio.Entidades
{
    /// <summary>
    /// Entidade de domínio que categoriza os tipos de movimentação de estoque.
    /// Tipos fixos: Entrada (1), Consumo (2), Envio (3), Recebimento (4).
    /// </summary>
    public class TipoMovimentacao
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
    }
}
