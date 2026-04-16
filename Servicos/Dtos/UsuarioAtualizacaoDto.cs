namespace Servicos.Dtos
{
    /// <summary>
    /// DTO para atualização de usuário. A senha só é alterada se fornecida.
    /// </summary>
    public class UsuarioAtualizacaoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Senha { get; set; }
    }
}
