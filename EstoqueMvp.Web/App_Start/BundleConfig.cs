using System.Web.Optimization;

namespace EstoqueMvp.Web
{
    /// <summary>
    /// Configuração dos bundles de CSS e JavaScript por view.
    /// </summary>
    public class BundleConfig
    {
        /// <summary>
        /// Registra os bundles de CSS e JS organizados por módulo e view.
        /// </summary>
        public static void RegisterBundles(BundleCollection bundles)
        {
            // CSS
            bundles.Add(new StyleBundle("~/bundles/css").Include(
                "~/Content/Site.css"));

            // JS global (core → ui-feedback → app) — ordem importa
            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/Scripts/core/constants.js",
                "~/Scripts/core/api.js",
                "~/Scripts/core/formatters.js",
                "~/Scripts/core/dom-utils.js",
                "~/Scripts/core/pagination.js",
                "~/Scripts/core/data-store.js",
                "~/Scripts/core/csv-export.js",
                "~/Scripts/core/crud-controller.js",
                "~/Scripts/ui-feedback.js",
                "~/Scripts/app.js"));

            // JS por view
            bundles.Add(new ScriptBundle("~/bundles/home-index").Include(
                "~/Scripts/Views/Home/index.js"));

            bundles.Add(new ScriptBundle("~/bundles/login").Include(
                "~/Scripts/Views/Login/login.js"));

            bundles.Add(new ScriptBundle("~/bundles/produto").Include(
                "~/Scripts/Views/Produto/index.js"));

            bundles.Add(new ScriptBundle("~/bundles/movimentacao").Include(
                "~/Scripts/Views/Movimentacao/index.js"));

            bundles.Add(new ScriptBundle("~/bundles/usuario").Include(
                "~/Scripts/Views/Usuario/index.js"));

            bundles.Add(new ScriptBundle("~/bundles/setor").Include(
                "~/Scripts/Views/Setor/index.js"));

            // Em Release, ativa minificação automaticamente
#if !DEBUG
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}
