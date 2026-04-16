namespace Dominio.Entidades
{
    /// <summary>
    /// Entidade de domínio que representa um produto no sistema de estoque.
    /// O SKU é gerado automaticamente pelo sistema e é único por produto.
    /// </summary>
    public class Produto
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public bool Ativo { get; set; } = true;
    }
}
