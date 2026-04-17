using EstoqueMvp.Api.Filters;
using System.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EstoqueMvp.Api
{
    /// <summary>
    /// Configuração central da Web API: CORS, filtros globais e rotas.
    /// As origens CORS são lidas do Web.config (AppSettings["CorsOrigins"]) para facilitar deploy.
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Registra CORS, filtros globais e rotas da Web API.
        /// </summary>
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // CORS com credenciais para permitir cookies JWT
            // Origens configuráveis via Web.config AppSettings
            string corsOrigins = ConfigurationManager.AppSettings["CorsOrigins"] ?? "*";
            var cors = new EnableCorsAttribute(corsOrigins, "*", "*");
            cors.SupportsCredentials = true;
            config.EnableCors(cors);

            config.Filters.Add(new GlobalExceptionFilterAttribute());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
