using Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servicos.Interfaces
{
    /// <summary>
    /// Contrato de serviço para consulta de saldos de estoque por setor.
    /// </summary>
    public interface IEstoqueSetorServico
    {
        Task<IEnumerable<EstoqueSetor>> ObterTodos();
        Task<IEnumerable<EstoqueSetor>> ObterPorSetorId(int setorId);
        Task<IEnumerable<EstoqueSetor>> ObterPorProdutoId(int produtoId);
        Task<EstoqueSetor> ObterPorProdutoIdESetorId(int produtoId, int setorId);
        Task<int> Adicionar(EstoqueSetor estoqueSetor);
        Task Atualizar(EstoqueSetor estoqueSetor);
    }
}
