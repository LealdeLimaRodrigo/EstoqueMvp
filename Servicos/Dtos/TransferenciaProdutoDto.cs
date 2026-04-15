namespace Servicos.Dtos
{
    public class TransferenciaProdutoDto
    {
        public int ProdutoId { get; set; }
        public int SetorOrigemId { get; set; }
        public int SetorDestinoId { get; set; }
        public decimal Quantidade { get; set; }
        public int UsuarioId { get; set; }
    }
}
