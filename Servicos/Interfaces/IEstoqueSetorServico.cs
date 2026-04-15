using Dominio.Entidades;
using System.Collections.Generic;

namespace Servicos.Interfaces
{
    public interface IEstoqueSetorServico
    {
        IEnumerable<EstoqueSetor> ObterTodos();
        IEnumerable<EstoqueSetor> ObterPorSetorId(int setorId);
        IEnumerable<EstoqueSetor> ObterPorProdutoId(int produtoId);
        EstoqueSetor ObterPorProdutoIdESetorId(int produtoId, int setorId);
        int Adicionar(EstoqueSetor estoqueSetor);
        void Atualizar(EstoqueSetor estoqueSetor);
    }
}
