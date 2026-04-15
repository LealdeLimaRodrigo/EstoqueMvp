using Dominio.Entidades;
using System.Collections.Generic;

namespace Dominio.Interfaces
{
    public interface IUsuarioRepositorio
    {
        IEnumerable<Usuario> ObterTodos();
        IEnumerable<Usuario> ObterTodosInativos();
        Usuario ObterPorId(int id);
        Usuario ObterPorCpf(string cpf);
        int Adicionar(Usuario usuario);
        void Atualizar(Usuario usuario);
        void Remover(int id);
        void Restaurar(int id);
    }
}
