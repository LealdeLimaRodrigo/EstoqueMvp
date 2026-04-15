using Dominio.Entidades;
using System.Collections.Generic;

namespace Servicos.Interfaces
{
    public interface ISetorServico
    {
        IEnumerable<Setor> ObterTodos();
        IEnumerable<Setor> ObterTodosInativos();
        Setor ObterPorId(int id);
        int Adicionar(Setor setor);
        void Atualizar(Setor setor);
        void Remover(int id);
        void Restaurar(int id);
    }
}
