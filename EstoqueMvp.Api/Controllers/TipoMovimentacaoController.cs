using Servicos.Interfaces;
using System.Threading.Tasks;
using System.Web.Http;

namespace EstoqueMvp.Api.Controllers
{
    /// <summary>
    /// Controller de consulta dos tipos de movimentação de estoque.
    /// Os tipos são fixos (Entrada, Consumo, Envio, Recebimento) e gerenciados via seed no banco.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/tipo-movimentacao")]
    public class TipoMovimentacaoController : ApiController
    {
        private readonly ITipoMovimentacaoServico _tipoMovimentacaoServico;

        public TipoMovimentacaoController(ITipoMovimentacaoServico tipoMovimentacaoServico)
        {
            _tipoMovimentacaoServico = tipoMovimentacaoServico;
        }

        /// <summary>
        /// Retorna todos os tipos de movimentação disponíveis.
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> ObterTodos()
        {
            var tiposMovimentacao = await _tipoMovimentacaoServico.ObterTodos();
            return Ok(tiposMovimentacao);
        }

        /// <summary>
        /// Retorna um tipo de movimentação pelo seu ID.
        /// </summary>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> ObterPorId(int id)
        {
            var tipoMovimentacao = await _tipoMovimentacaoServico.ObterPorId(id);
            if (tipoMovimentacao == null)
                return NotFound();

            return Ok(tipoMovimentacao);
        }

    }
}