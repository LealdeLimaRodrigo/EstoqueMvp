using System;

namespace Dominio.Entidades
{
    /// <summary>
    /// Entidade de domínio que representa um registro de movimentação de estoque.
    /// O TransacaoId agrupa movimentações relacionadas (ex: débito e crédito de uma transferência).
    /// </summary>
    public class MovimentacaoEstoque
    {
        public int Id { get; set; }
        public Guid TransacaoId { get; set; }
        public int ProdutoId { get; set; }
        public int SetorId { get; set; }
        public int TipoMovimentacaoId { get; set; }
        public decimal Quantidade { get; set; }
        public DateTime DataMovimentacao { get; set; }
        public int UsuarioId { get; set; }
    }
}
