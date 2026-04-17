using System;
using System.Web.Mvc;

namespace EstoqueMvp.Web.Controllers
{
    /// <summary>
    /// Controller MVC responsável por servir a view de gestão de usuários.
    /// </summary>
    public class UsuarioController : Controller
    {
        /// <summary>
        /// Retorna a view de gestão de usuários.
        /// </summary>
        public ActionResult Index()
        {
            ViewBag.Breadcrumbs = new[] {
                Tuple.Create("Dashboard", "/"),
                Tuple.Create("Usu\u00e1rios", (string)null)
            };
            return View();
        }
    }
}
