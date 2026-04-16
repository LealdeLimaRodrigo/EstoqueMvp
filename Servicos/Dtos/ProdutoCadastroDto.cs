namespace Servicos.Dtos
{
    /// <summary>
    /// DTO para cadastro de um novo produto. O SKU é gerado automaticamente pelo sistema.
    /// </summary>
    public class ProdutoCadastroDto
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
    }
}
