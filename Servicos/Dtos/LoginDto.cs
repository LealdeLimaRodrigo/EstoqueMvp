namespace Servicos.Dtos
{
    /// <summary>
    /// DTO para autenticação de usuário via CPF e Senha.
    /// </summary>
    public class LoginDto
    {
        public string Cpf { get; set; }
        public string Senha { get; set; }
    }
}
