using Dominio.Entidades;
using Dominio.Interfaces;
using Servicos.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servicos
{
    /// <summary>
    /// Serviço de consulta dos tipos de movimentação.
    /// Os tipos são fixos e definidos na carga inicial do banco de dados.
    /// </summary>
    public class TipoMovimentacaoServico : ITipoMovimentacaoServico
    {
        private readonly ITipoMovimentacaoRepositorio _tipoMovimentacaoRepositorio;

        public TipoMovimentacaoServico(ITipoMovimentacaoRepositorio tipoMovimentacaoRepositorio)
        {
            _tipoMovimentacaoRepositorio = tipoMovimentacaoRepositorio;
        }

        public async Task<TipoMovimentacao> ObterPorId(int id) => await _tipoMovimentacaoRepositorio.ObterPorId(id);

        public async Task<IEnumerable<TipoMovimentacao>> ObterTodos() => await _tipoMovimentacaoRepositorio.ObterTodos();
    }
}
