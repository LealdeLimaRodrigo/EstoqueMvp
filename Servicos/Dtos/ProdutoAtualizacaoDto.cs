namespace Servicos.Dtos
{
    /// <summary>
    /// DTO para atualização de um produto existente. O SKU não pode ser alterado.
    /// </summary>
    public class ProdutoAtualizacaoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
    }
}
