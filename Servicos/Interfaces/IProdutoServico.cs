using Dominio.Entidades;
using System.Collections.Generic;

namespace Servicos.Interfaces
{
    public interface IProdutoServico
    {
        int Adicionar(Produto produto);
        void Atualizar(Produto produto);
        Produto ObterPorId(int id);
        Produto ObterPorSku(string sku);
        IEnumerable<Produto> ObterTodos();
        IEnumerable<Produto> ObterTodosInativos();
        void Remover(int id);
        void Restaurar(int id);
    }
}
