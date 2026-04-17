using System.Web.Mvc;

namespace EstoqueMvp.Web
{
    /// <summary>
    /// Configuração dos filtros globais MVC.
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// Registra filtros globais de tratamento de erros.
        /// </summary>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
