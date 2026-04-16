using Dados.Repositorios;
using Dominio.Interfaces;
using Servicos;
using Servicos.Interfaces;
using FluentValidation;
using Servicos.Dtos;
using Servicos.Validacoes;
using Dominio.Entidades;
using System.Configuration;
using System.Web.Http;
using Unity;
using Unity.Injection;
using Unity.WebApi;

namespace EstoqueMvp.Api
{
	/// <summary>
	/// Configuração do container de Injeção de Dependência (Unity).
	/// Registra todos os repositórios e serviços utilizados pela aplicação.
	/// </summary>
	public static class UnityConfig
	{
		public static void RegisterComponents()
		{
			var container = new UnityContainer();

			string connectionString = ConfigurationManager.ConnectionStrings["EstoqueConexao"].ConnectionString;

			// Repositórios: recebem a string de conexão via construtor
			container.RegisterType<IProdutoRepositorio, ProdutoRepositorio>(new InjectionConstructor(connectionString));
			container.RegisterType<IUsuarioRepositorio, UsuarioRepositorio>(new InjectionConstructor(connectionString));
			container.RegisterType<ISetorRepositorio, SetorRepositorio>(new InjectionConstructor(connectionString));
			container.RegisterType<IEstoqueSetorRepositorio, EstoqueSetorRepositorio>(new InjectionConstructor(connectionString));
			container.RegisterType<IMovimentacaoEstoqueRepositorio, MovimentacaoEstoqueRepositorio>(new InjectionConstructor(connectionString));
			container.RegisterType<ITipoMovimentacaoRepositorio, TipoMovimentacaoRepositorio>(new InjectionConstructor(connectionString));

			// Validators (FluentValidation)
container.RegisterType<IValidator<ProdutoCadastroDto>, ProdutoCadastroDtoValidator>();
container.RegisterType<IValidator<ProdutoAtualizacaoDto>, ProdutoAtualizacaoDtoValidator>();
container.RegisterType<IValidator<LoginDto>, LoginDtoValidator>();
container.RegisterType<IValidator<UsuarioCadastroDto>, UsuarioCadastroDtoValidator>();
container.RegisterType<IValidator<UsuarioAtualizacaoDto>, UsuarioAtualizacaoDtoValidator>();
container.RegisterType<IValidator<SetorCadastroDto>, SetorCadastroDtoValidator>();
container.RegisterType<IValidator<SetorAtualizacaoDto>, SetorAtualizacaoDtoValidator>();
container.RegisterType<IValidator<EstoqueSetor>, EstoqueSetorValidator>();
container.RegisterType<IValidator<MovimentacaoEstoqueDto>, MovimentacaoEstoqueDtoValidator>();
container.RegisterType<IValidator<TransferenciaProdutoDto>, TransferenciaProdutoDtoValidator>();

// Serviços// Serviços: resolvidos automaticamente pelo container via interfaces de repositório
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
