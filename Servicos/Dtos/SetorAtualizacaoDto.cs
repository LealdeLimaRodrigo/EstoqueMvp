namespace Servicos.Dtos
{
    /// <summary>
    /// DTO para atualização de um setor existente.
    /// </summary>
    public class SetorAtualizacaoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
    }
}
