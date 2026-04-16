using Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servicos.Interfaces
{
    /// <summary>
    /// Contrato de serviço para consulta de tipos de movimentação.
    /// </summary>
    public interface ITipoMovimentacaoServico
    {
        Task<IEnumerable<TipoMovimentacao>> ObterTodos();
        Task<TipoMovimentacao> ObterPorId(int id);
    }
}
