using System;

namespace Dominio.Entidades
{
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
