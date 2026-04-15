using Dominio.Entidades;
using System.Collections.Generic;

namespace Servicos.Interfaces
{
    public interface ITipoMovimentacaoServico
    {
        IEnumerable<TipoMovimentacao> ObterTodos();
        TipoMovimentacao ObterPorId(int id);
    }
}
