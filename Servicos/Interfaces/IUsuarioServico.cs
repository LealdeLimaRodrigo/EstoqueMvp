using Dominio.Entidades;
using Servicos.Dtos;
using System.Collections.Generic;

namespace Servicos.Interfaces
{
    public interface IUsuarioServico
    {
        IEnumerable<Usuario> ObterTodos();
        IEnumerable<Usuario> ObterTodosInativos();
        Usuario ObterPorId(int id);
        Usuario ObterPorCpf(string cpf);
        int Adicionar(Usuario usuario);
        void Atualizar(Usuario usuario);
        void Remover(int id);
        void Restaurar(int id);

        UsuarioRetornoDto RealizarLogin(LoginDto loginDto);
    }
}
