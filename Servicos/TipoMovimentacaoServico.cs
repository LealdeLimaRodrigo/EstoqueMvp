using Dominio.Entidades;
using Dominio.Interfaces;
using Servicos.Interfaces;
using System.Collections.Generic;

namespace Servicos
{
    public class TipoMovimentacaoServico : ITipoMovimentacaoServico
    {
        private readonly ITipoMovimentacaoRepositorio _tipoMovimentacaoRepositorio;

        public TipoMovimentacaoServico(ITipoMovimentacaoRepositorio tipoMovimentacaoRepositorio)
        {
            _tipoMovimentacaoRepositorio = tipoMovimentacaoRepositorio;
        }

        public TipoMovimentacao ObterPorId(int id) => _tipoMovimentacaoRepositorio.ObterPorId(id);

        public IEnumerable<TipoMovimentacao> ObterTodos() => _tipoMovimentacaoRepositorio.ObterTodos();
    }
}
