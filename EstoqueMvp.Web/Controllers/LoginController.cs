using System.Web.Mvc;

namespace EstoqueMvp.Web.Controllers
{
    /// <summary>
    /// Controller MVC responsável por servir a view de login.
    /// </summary>
    public class LoginController : Controller
    {
        /// <summary>
        /// Retorna a view de login.
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }
    }
}
