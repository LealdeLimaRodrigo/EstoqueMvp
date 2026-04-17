using System;
using System.Web.Mvc;

namespace EstoqueMvp.Web.Controllers
{
    /// <summary>
    /// Controller MVC responsável por servir a view de gestão de setores.
    /// </summary>
    public class SetorController : Controller
    {
        /// <summary>
        /// Retorna a view de gestão de setores.
        /// </summary>
        public ActionResult Index()
        {
            ViewBag.Breadcrumbs = new[] {
                Tuple.Create("Dashboard", "/"),
                Tuple.Create("Setores", (string)null)
            };
            return View();
        }
    }
}
