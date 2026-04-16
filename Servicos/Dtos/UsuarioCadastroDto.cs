namespace Servicos.Dtos
{
    /// <summary>
    /// DTO para cadastro de um novo usuário com validação de CPF.
    /// </summary>
    public class UsuarioCadastroDto
    {
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Senha { get; set; }
    }
}
