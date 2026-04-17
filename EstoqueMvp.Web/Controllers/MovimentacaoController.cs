using System;
using System.Web.Mvc;

namespace EstoqueMvp.Web.Controllers
{
    /// <summary>
    /// Controller MVC responsável por servir a view de movimentações de estoque.
    /// </summary>
    public class MovimentacaoController : Controller
    {
        /// <summary>
        /// Retorna a view de movimentações de estoque.
        /// </summary>
        public ActionResult Index()
        {
            ViewBag.Breadcrumbs = new[] {
                Tuple.Create("Dashboard", "/"),
                Tuple.Create("Movimenta\u00e7\u00f5es", (string)null)
            };
            return View();
        }
    }
}