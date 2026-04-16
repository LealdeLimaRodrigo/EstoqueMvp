namespace Dominio.Entidades
{
    /// <summary>
    /// Entidade de domínio que representa um usuário do sistema.
    /// A serialização e proteção de campos sensíveis é responsabilidade da camada de API (DTOs).
    /// </summary>
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string SenhaHash { get; set; }
        public bool Ativo { get; set; } = true;
    }
}
