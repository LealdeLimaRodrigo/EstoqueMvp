using Servicos.Interfaces;
using System.Web.Http;

namespace EstoqueMvp.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/tipo-movimentacao")]
    public class TipoMovimentacaoController : ApiController
    {
        private readonly ITipoMovimentacaoServico _tipoMovimentacaoServico;

        public TipoMovimentacaoController(ITipoMovimentacaoServico tipoMovimentacaoServico)
        {
            _tipoMovimentacaoServico = tipoMovimentacaoServico;
        }

        // GET: Setor
        [HttpGet]
        [Route("")]
        public IHttpActionResult ObterTodos()
        {
            var tiposMovimentacao = _tipoMovimentacaoServico.ObterTodos();
            return Ok(tiposMovimentacao);
        }

        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult ObterPorId(int id)
        {
            var tipoMovimentacao = _tipoMovimentacaoServico.ObterPorId(id);
            if (tipoMovimentacao == null)
                return NotFound();

            return Ok(tipoMovimentacao);
        }

    }
}