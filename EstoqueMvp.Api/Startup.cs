using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using EstoqueMvp.Api.Middleware;
using Owin;
using System.Configuration;
using System.Text;

[assembly: OwinStartup(typeof(EstoqueMvp.Api.Startup))]

namespace EstoqueMvp.Api
{
    /// <summary>
    /// Configuração OWIN para autenticação JWT Bearer.
    /// Valida tokens enviados no header Authorization em todas as requisições protegidas.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Configura a autenticação JWT Bearer via OWIN.
        /// </summary>
        public void Configuration(IAppBuilder app)
        {
            string chaveSecreta = ConfigurationManager.AppSettings["JwtSecretKey"];
            var key = Encoding.ASCII.GetBytes(chaveSecreta);

            // Extrai JWT do cookie httpOnly e injeta no header Authorization
            app.Use<CookieJwtMiddleware>();

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true
                }
            });
        }
    }
}