namespace Servicos.Dtos
{
    /// <summary>
    /// DTO para transferência de produto entre setores.
    /// O UsuarioId é preenchido automaticamente a partir do token JWT no controller.
    /// </summary>
    public class TransferenciaProdutoDto
    {
        public int ProdutoId { get; set; }
        public int SetorOrigemId { get; set; }
        public int SetorDestinoId { get; set; }
        public decimal Quantidade { get; set; }
        public int UsuarioId { get; set; }
    }
}
