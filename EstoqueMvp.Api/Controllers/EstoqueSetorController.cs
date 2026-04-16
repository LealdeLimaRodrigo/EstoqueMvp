using Servicos.Interfaces;
using System.Threading.Tasks;
using System.Web.Http;

namespace EstoqueMvp.Api.Controllers
{
    /// <summary>
    /// Controller responsável pela consulta do saldo de estoque por setor.
    /// Permite visualizar a quantidade de cada produto em cada setor.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/estoque-setor")]
    public class EstoqueSetorController : ApiController
    {
        private readonly IEstoqueSetorServico _estoqueSetorServico;

        public EstoqueSetorController(IEstoqueSetorServico estoqueSetorServico)
        {
            _estoqueSetorServico = estoqueSetorServico;
        }

        /// <summary>
        /// Retorna todos os registros de estoque por setor.
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> ObterTodos()
        {
            var estoqueSetores = await _estoqueSetorServico.ObterTodos();
            return Ok(estoqueSetores);
        }

        /// <summary>
        /// Retorna o estoque de todos os produtos de um setor específico.
        /// </summary>
        [HttpGet]
        [Route("setor/{setorId:int}")]
        public async Task<IHttpActionResult> ObterPorSetorId(int setorId)
        {
            var estoqueSetores = await _estoqueSetorServico.ObterPorSetorId(setorId);
            return Ok(estoqueSetores);
        }

        /// <summary>
        /// Retorna o estoque de um produto específico em todos os setores.
        /// </summary>
        [HttpGet]
        [Route("produto/{produtoId:int}")]
        public async Task<IHttpActionResult> ObterPorProdutoId(int produtoId)
        {
            var estoqueSetores = await _estoqueSetorServico.ObterPorProdutoId(produtoId);
            return Ok(estoqueSetores);
        }

        /// <summary>
        /// Retorna o saldo de um produto em um setor específico.
        /// </summary>
        [HttpGet]
        [Route("produto/{produtoId:int}/setor/{setorId:int}")]
        public async Task<IHttpActionResult> ObterPorProdutoIdESetorId(int produtoId, int setorId)
        {
            var estoqueSetor = await _estoqueSetorServico.ObterPorProdutoIdESetorId(produtoId, setorId);
            if (estoqueSetor == null)
                return NotFound();

            return Ok(estoqueSetor);
        }

    }
}