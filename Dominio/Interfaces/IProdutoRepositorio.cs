using Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dominio.Interfaces
{
    /// <summary>
    /// Contrato de acesso a dados para a entidade Produto.
    /// Definido na camada de domínio para garantir o Dependency Inversion Principle.
    /// </summary>
    public interface IProdutoRepositorio
    {
        Task<IEnumerable<Produto>> ObterTodos();
        Task<IEnumerable<Produto>> ObterTodosPaginado(int offset, int tamanhoPagina);
        Task<int> ContarTodosAtivos();
        Task<IEnumerable<Produto>> ObterTodosInativos();
        Task<Produto> ObterPorId(int id);
        Task<Produto> ObterPorSku(string sku);
        Task<IEnumerable<Produto>> ObterPorNome(string nome);
        Task<IEnumerable<Produto>> BuscarPaginado(string termo, int offset, int tamanhoPagina);
        Task<int> ContarPorBusca(string termo);
        Task<int> Adicionar(Produto produto);
        Task Atualizar(Produto produto);
        Task Remover(int id);
        Task Restaurar(int id);
    }
}
