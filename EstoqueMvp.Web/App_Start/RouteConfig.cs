using System.Web.Mvc;
using System.Web.Routing;

namespace EstoqueMvp.Web
{
    /// <summary>
    /// Configuração das rotas MVC.
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// Registra as rotas da aplicação MVC.
        /// </summary>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Login",
                url: "Login",
                defaults: new { controller = "Login", action = "Index" }
            );

            routes.MapRoute(
                name: "ControllerOnly",
                url: "{controller}",
                defaults: new { action = "Index" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
