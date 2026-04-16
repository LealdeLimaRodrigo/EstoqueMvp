using Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dominio.Interfaces
{
    /// <summary>
    /// Contrato de acesso a dados para tipos de movimentação (consulta apenas).
    /// </summary>
    public interface ITipoMovimentacaoRepositorio
    {
        Task<IEnumerable<TipoMovimentacao>> ObterTodos();
        Task<TipoMovimentacao> ObterPorId(int id);
    }
}
