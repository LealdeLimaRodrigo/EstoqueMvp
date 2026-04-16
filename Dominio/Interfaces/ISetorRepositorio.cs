using Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dominio.Interfaces
{
    /// <summary>
    /// Contrato de acesso a dados para a entidade Setor.
    /// </summary>
    public interface ISetorRepositorio
    {
        Task<IEnumerable<Setor>> ObterTodos();
        Task<IEnumerable<Setor>> ObterTodosPaginado(int offset, int tamanhoPagina);
        Task<int> ContarTodosAtivos();
        Task<IEnumerable<Setor>> ObterTodosInativos();
        Task<Setor> ObterPorId(int id);
        Task<IEnumerable<Setor>> ObterPorNome(string nome);
        Task<int> Adicionar(Setor setor);
        Task Atualizar(Setor setor);
        Task Remover(int id);
        Task Restaurar(int id);
    }
}
