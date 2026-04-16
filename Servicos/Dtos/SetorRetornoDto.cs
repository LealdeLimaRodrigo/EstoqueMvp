namespace Servicos.Dtos
{
    /// <summary>
    /// DTO de retorno para exibição de dados de um setor.
    /// Isola a entidade de domínio da camada de apresentação.
    /// </summary>
    public class SetorRetornoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }
    }
}
