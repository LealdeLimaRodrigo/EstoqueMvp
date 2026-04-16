namespace Servicos.Dtos
{
    /// <summary>
    /// DTO para operações de entrada e consumo de estoque.
    /// O UsuarioId é preenchido automaticamente a partir do token JWT no controller.
    /// </summary>
    public class MovimentacaoEstoqueDto
    {
        public int ProdutoId { get; set; }
        public int SetorId { get; set; }
        public decimal Quantidade { get; set; }
        public int UsuarioId { get; set; }
    }
}
