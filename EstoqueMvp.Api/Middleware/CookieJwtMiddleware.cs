using Microsoft.Owin;
using System.Threading.Tasks;

namespace EstoqueMvp.Api.Middleware
{
    /// <summary>
    /// Middleware OWIN que extrai o JWT do cookie httpOnly e popula o header Authorization.
    /// Permite que a autenticação JWT funcione com cookies seguros enviados automaticamente pelo navegador.
    /// </summary>
    public class CookieJwtMiddleware : OwinMiddleware
    {
        public CookieJwtMiddleware(OwinMiddleware next) : base(next) { }

        /// <summary>
        /// Extrai o JWT do cookie e injeta no header Authorization da requisição.
        /// </summary>
        public override async Task Invoke(IOwinContext context)
        {
            var jwtCookie = context.Request.Cookies["jwt_token"];
            if (!string.IsNullOrEmpty(jwtCookie))
            {
                // Sempre sobrescreve o header Authorization
                context.Request.Headers["Authorization"] = "Bearer " + jwtCookie;
            }
            await Next.Invoke(context);
        }
    }
}
