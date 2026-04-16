namespace Dominio.Entidades
{
    /// <summary>
    /// Entidade de domínio que representa o saldo de um produto em um setor específico.
    /// A chave composta (SetorId, ProdutoId) garante unicidade por combinação.
    /// </summary>
    public class EstoqueSetor
    {
        public int SetorId { get; set; }
        public int ProdutoId { get; set; }
        public decimal QuantidadeEstoque { get; set; }
    }
}
