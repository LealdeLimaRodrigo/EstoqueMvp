namespace Servicos.Dtos
{
    /// <summary>
    /// DTO de retorno do usuário após autenticação. Não inclui dados sensíveis.
    /// </summary>
    public class UsuarioRetornoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
    }
}
