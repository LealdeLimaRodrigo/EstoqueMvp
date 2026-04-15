using Dominio.Entidades;
using System.Collections.Generic;

namespace Dominio.Interfaces
{
    public interface IEstoqueSetorRepositorio
    {
        IEnumerable<EstoqueSetor> ObterTodos();
        IEnumerable<EstoqueSetor> ObterPorSetorId(int setorId);
        IEnumerable<EstoqueSetor> ObterPorProdutoId(int produtoId);
        int Adicionar(EstoqueSetor estoqueSetor);
        void Atualizar(EstoqueSetor estoqueSetor);
    }
}
