using EstoqueMvp.Api.Filters;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EstoqueMvp.Api
{
    /// <summary>
    /// Configuração central da Web API: CORS, filtros globais e rotas.
    /// CORS está liberado (*) apenas por ser um MVP. Em produção, restringir origens permitidas.
    /// </summary>
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            //CORS liberado apenas por ser um MVP
            var cors = new EnableCorsAttribute("*", "*", "*");
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
