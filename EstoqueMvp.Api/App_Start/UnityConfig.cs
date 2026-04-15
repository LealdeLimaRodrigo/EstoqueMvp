using Dados.Repositorios;
using Dominio.Interfaces;
using Servicos;
using Servicos.Interfaces;
using System.Configuration;
using System.Web.Http;
using Unity;
using Unity.Injection;
using Unity.WebApi;

namespace EstoqueMvp.Api
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            string connectionString = ConfigurationManager.ConnectionStrings["EstoqueConexao"].ConnectionString;

            //Registrando os repositórios com o construtor que recebe a string de conexão
            container.RegisterType<IProdutoRepositorio, ProdutoRepositorio>(new InjectionConstructor(connectionString));
            container.RegisterType<IUsuarioRepositorio, UsuarioRepositorio>(new InjectionConstructor(connectionString));
            container.RegisterType<ISetorRepositorio, SetorRepositorio>(new InjectionConstructor(connectionString));
            container.RegisterType<IEstoqueSetorRepositorio, EstoqueSetorRepositorio>(new InjectionConstructor(connectionString));
            container.RegisterType<IMovimentacaoEstoqueRepositorio, MovimentacaoEstoqueRepositorio>(new InjectionConstructor(connectionString));
            container.RegisterType<ITipoMovimentacaoRepositorio, TipoMovimentacaoRepositorio>(new InjectionConstructor(connectionString));

            // Registrando os serviços
            container.RegisterType<IProdutoServico, ProdutoServico>();
            container.RegisterType<IUsuarioServico, UsuarioServico>();
            container.RegisterType<ITipoMovimentacaoServico, TipoMovimentacaoServico>();
            container.RegisterType<ISetorServico, SetorServico>();
            container.RegisterType<IEstoqueSetorServico, EstoqueSetorServico>();
            container.RegisterType<IMovimentacaoEstoqueServico, MovimentacaoEstoqueServico>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}