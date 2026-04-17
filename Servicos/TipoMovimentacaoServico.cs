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

        /// <summary>
        /// Retorna um tipo de movimentação pelo ID.
        /// </summary>
        public async Task<TipoMovimentacao> ObterPorId(int id) => await _tipoMovimentacaoRepositorio.ObterPorId(id);

        /// <summary>
        /// Retorna todos os tipos de movimentação cadastrados.
        /// </summary>
        public async Task<IEnumerable<TipoMovimentacao>> ObterTodos() => await _tipoMovimentacaoRepositorio.ObterTodos();
    }
}
