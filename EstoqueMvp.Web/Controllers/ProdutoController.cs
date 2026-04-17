using System;
using System.Web.Mvc;

namespace EstoqueMvp.Web.Controllers
{
    /// <summary>
    /// Controller MVC responsável por servir a view de gestão de produtos.
    /// </summary>
    public class ProdutoController : Controller
    {
        /// <summary>
        /// Retorna a view de gestão de produtos.
        /// </summary>
        public ActionResult Index()
        {
            ViewBag.Breadcrumbs = new[] {
                Tuple.Create("Dashboard", "/"),
                Tuple.Create("Produtos", (string)null)
            };
            return View();
        }
    }
}