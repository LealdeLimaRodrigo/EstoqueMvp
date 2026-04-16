using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
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
        public void Configuration(IAppBuilder app)
        {
            string chaveSecreta = ConfigurationManager.AppSettings["JwtSecretKey"];
            var key = Encoding.ASCII.GetBytes(chaveSecreta);

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