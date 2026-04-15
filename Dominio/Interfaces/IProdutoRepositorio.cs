using Dominio.Entidades;
using System.Collections.Generic;

namespace Dominio.Interfaces
{
    public interface IProdutoRepositorio
    {
        IEnumerable<Produto> ObterTodos();
        IEnumerable<Produto> ObterTodosInativos();
        Produto ObterPorId(int id);
        Produto ObterPorSku(string sku);
        int Adicionar(Produto produto);
        void Atualizar(Produto produto);
        void Remover(int id);
        void Restaurar(int id);
    }
}
