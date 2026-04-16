using Dominio.Entidades;
using Servicos.Dtos;

namespace Servicos.Mapeamentos
{
    /// <summary>
    /// Métodos de extensão para mapeamento entre a entidade Usuario e seus DTOs.
    /// Garante que o SenhaHash nunca seja exposto ao cliente.
    /// </summary>
    public static class UsuarioMapeamentoExtensions
    {
        /// <summary>
        /// Converte a entidade Usuario para o DTO de retorno.
        /// Nunca inclui o SenhaHash.
        /// </summary>
        public static UsuarioRetornoDto ToRetornoDto(this Usuario usuario)
        {
            if (usuario == null) return null;

            return new UsuarioRetornoDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Cpf = usuario.Cpf
            };
        }

        /// <summary>
        /// Cria uma nova entidade Usuario a partir do DTO de cadastro.
        /// O SenhaHash deve ser gerado externamente (BCrypt no serviço).
        /// </summary>
        public static Usuario ToEntity(this UsuarioCadastroDto dto, string cpfLimpo, string senhaHash)
        {
            return new Usuario
            {
                Nome = dto.Nome,
                Cpf = cpfLimpo,
                SenhaHash = senhaHash
            };
        }

        /// <summary>
        /// Aplica as alterações do DTO de atualização na entidade existente.
        /// A senha só é alterada se informada (tratada no serviço).
        /// </summary>
        public static void AplicarAtualizacao(this Usuario usuario, UsuarioAtualizacaoDto dto, string cpfLimpo)
        {
            usuario.Nome = dto.Nome;
            usuario.Cpf = cpfLimpo;
        }
    }
}
