using System;

namespace Servicos.Exceptions
{
    /// <summary>
    /// Exceção lançada quando se tenta cadastrar um CPF que já pertence a um usuário inativo.
    /// Contém o ID do usuário inativo para possibilitar a restauração.
    /// </summary>
    public class UsuarioInativoException : InvalidOperationException
    {
        public int UsuarioInativoId { get; }
        public string Nome { get; }
        public string Cpf { get; }

        public UsuarioInativoException(string message, int usuarioInativoId, string nome = null, string cpf = null)
            : base(message)
        {
            UsuarioInativoId = usuarioInativoId;
            Nome = nome;
            Cpf = cpf;
        }
    }
}
