using Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dominio.Interfaces
{
    /// <summary>
    /// Contrato de acesso a dados para o saldo de estoque por setor.
    /// </summary>
    public interface IEstoqueSetorRepositorio
    {
        Task<IEnumerable<EstoqueSetor>> ObterTodos();
        Task<IEnumerable<EstoqueSetor>> ObterPorSetorId(int setorId);
        Task<IEnumerable<EstoqueSetor>> ObterPorProdutoId(int produtoId);
        Task<EstoqueSetor> ObterPorProdutoIdESetorId(int produtoId, int setorId);

        /// <summary>
        /// Retorna a soma total de estoque de um produto em todos os setores.
        /// </summary>
        Task<decimal> ObterQuantidadeTotalPorProdutoId(int produtoId);

        Task<int> Adicionar(EstoqueSetor estoqueSetor);
        Task Atualizar(EstoqueSetor estoqueSetor);
    }
}
