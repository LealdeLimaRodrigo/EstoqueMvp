using Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dominio.Interfaces
{
    /// <summary>
    /// Contrato de acesso a dados para a entidade Usuário.
    /// </summary>
    public interface IUsuarioRepositorio
    {
        Task<IEnumerable<Usuario>> ObterTodos();
        Task<IEnumerable<Usuario>> ObterTodosPaginado(int offset, int tamanhoPagina);
        Task<int> ContarTodosAtivos();
        Task<IEnumerable<Usuario>> ObterTodosInativos();
        Task<Usuario> ObterPorId(int id);
        Task<Usuario> ObterPorCpf(string cpf);

        /// <summary>
        /// Busca o usuário pelo CPF incluindo o SenhaHash.
        /// Utilizado exclusivamente no fluxo de login para verificação de credenciais.
        /// </summary>
        Task<Usuario> ObterPorCpfComSenha(string cpf);

        /// <summary>
        /// Busca o usuário pelo ID incluindo o SenhaHash.
        /// Utilizado exclusivamente no fluxo de atualização para preservar a senha existente.
        /// </summary>
        Task<Usuario> ObterPorIdComSenha(int id);
        Task<IEnumerable<Usuario>> ObterPorNome(string nome);
        Task<IEnumerable<Usuario>> BuscarPaginado(string termo, int offset, int tamanhoPagina);
        Task<int> ContarPorBusca(string termo);
        Task<int> Adicionar(Usuario usuario);
        Task Atualizar(Usuario usuario);
        Task Remover(int id);
        Task Restaurar(int id);
    }
}
