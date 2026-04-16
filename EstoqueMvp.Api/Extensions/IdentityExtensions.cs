using System.Linq;
using System.Security.Claims;
using System.Web.Http;

namespace EstoqueMvp.Api.Extensions
{
    /// <summary>
    /// Extensões para extrair informações do usuário autenticado a partir dos claims do JWT.
    /// Garante que o UsuarioId venha do token e não do body da requisição,
    /// prevenindo que um usuário se passe por outro (impersonation).
    /// </summary>
    public static class IdentityExtensions
    {
        /// <summary>
        /// Obtém o ID do usuário autenticado a partir do claim NameIdentifier do token JWT.
        /// </summary>
        public static int ObterUsuarioIdDoToken(this ApiController controller)
        {
            var identity = controller.User.Identity as ClaimsIdentity;
            if (identity == null)
                throw new System.UnauthorizedAccessException("Token de autenticação inválido.");

            var claim = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (claim == null)
                throw new System.UnauthorizedAccessException("Claim de identificação do usuário não encontrada no token.");

            if (!int.TryParse(claim.Value, out int id))
                throw new System.UnauthorizedAccessException("Claim de identificação do usuário em formato inválido.");

            return id;
        }
    }
}
