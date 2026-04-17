using System;
using System.Web.Mvc;

namespace EstoqueMvp.Web.Controllers
{
    /// <summary>
    /// Controller MVC responsável por servir a view do Dashboard.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Retorna a view do Dashboard.
        /// </summary>
        public ActionResult Index()
        {
            ViewBag.Breadcrumbs = new[] {
                Tuple.Create("Dashboard", (string)null)
            };
            return View();
        }

    }
}