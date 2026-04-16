using Microsoft.IdentityModel.Tokens;
using Servicos.Dtos;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EstoqueMvp.Api.Security
{
    /// <summary>
    /// Serviço responsável pela geração de tokens JWT para autenticação.
    /// A chave secreta é mantida privada para evitar exposição acidental.
    /// </summary>
    public static class TokenService
    {
        private static readonly string ChaveSecreta = ConfigurationManager.AppSettings["JwtSecretKey"];

        /// <summary>
        /// Gera um token JWT com os claims do usuário autenticado.
        /// O claim NameIdentifier é utilizado pelos controllers para identificar o usuário da requisição.
        /// </summary>
        public static string GerarToken(UsuarioRetornoDto usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(ChaveSecreta);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Nome),
                    new Claim("Cpf", usuario.Cpf)
                }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}