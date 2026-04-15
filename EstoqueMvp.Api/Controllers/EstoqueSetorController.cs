using Servicos.Interfaces;
using System.Web.Http;

namespace EstoqueMvp.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/estoque-setor")]
    public class EstoqueSetorController : ApiController
    {
        private readonly IEstoqueSetorServico _estoqueSetorServico;

        public EstoqueSetorController(IEstoqueSetorServico estoqueSetorServico)
        {
            _estoqueSetorServico = estoqueSetorServico;
        }

        // GET: Setor
        [HttpGet]
        [Route("")]
        public IHttpActionResult ObterTodos()
        {
            var estoqueSetores = _estoqueSetorServico.ObterTodos();
            return Ok(estoqueSetores);
        }

        [HttpGet]
        [Route("setor/{setorId:int}")]
        public IHttpActionResult ObterPorSetorId(int setorId)
        {
            var estoqueSetores = _estoqueSetorServico.ObterPorSetorId(setorId);            
            return Ok(estoqueSetores);
        }

        [HttpGet]
        [Route("produto/{produtoId:int}")]
        public IHttpActionResult ObterPorProdutoId(int produtoId)
        {
            var estoqueSetores = _estoqueSetorServico.ObterPorProdutoId(produtoId);            
            return Ok(estoqueSetores);
        }

        [HttpGet]
        [Route("produto/{produtoId:int}/setor/{setorId:int}")]
        public IHttpActionResult ObterPorProdutoIdESetorId(int produtoId, int setorId)
        {
            var estoqueSetor = _estoqueSetorServico.ObterPorProdutoIdESetorId(produtoId, setorId);
            if (estoqueSetor == null)
                return NotFound();

            return Ok(estoqueSetor);
        }

    }
}