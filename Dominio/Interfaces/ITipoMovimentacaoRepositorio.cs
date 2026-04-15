using Dominio.Entidades;
using System.Collections.Generic;

namespace Dominio.Interfaces
{
    public interface ITipoMovimentacaoRepositorio
    {
        IEnumerable<TipoMovimentacao> ObterTodos();
        TipoMovimentacao ObterPorId(int id);
    }
}
