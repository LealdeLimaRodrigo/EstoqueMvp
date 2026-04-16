namespace Servicos.Dtos
{
    /// <summary>
    /// DTO de retorno para exibição de dados de um produto.
    /// Isola a entidade de domínio da camada de apresentação.
    /// Inclui a quantidade total em estoque (soma de todos os setores).
    /// </summary>
    public class ProdutoRetornoDto
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public bool Ativo { get; set; }
        public decimal QuantidadeTotalEstoque { get; set; }
    }
}
