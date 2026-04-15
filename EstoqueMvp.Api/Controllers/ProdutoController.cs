using Dominio.Entidades;
using Servicos.Interfaces;
using System.Web.Http;

namespace EstoqueMvp.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/produto")]
    public class ProdutoController : ApiController
    {
        private readonly IProdutoServico _produtoServico;

        public ProdutoController(IProdutoServico produtoServico)
        {
            _produtoServico = produtoServico;
        }

        // GET: Produto
        [HttpGet]
        [Route("")]
        public IHttpActionResult ObterTodos()
        {
            var produtos = _produtoServico.ObterTodos();
            return Ok(produtos);
        }

        [HttpGet]
        [Route("inativos")]
        public IHttpActionResult ObterInativos()
        {
            var produtosInativos = _produtoServico.ObterTodosInativos();
            return Ok(produtosInativos);
        }

        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult ObterPorId(int id)
        {
            var produto = _produtoServico.ObterPorId(id);
            if (produto == null)
                return NotFound();

            return Ok(produto);
        }

        [HttpGet]
        [Route("sku/{sku}")]
        public IHttpActionResult ObterPorSku(string sku)
        {
            var produto = _produtoServico.ObterPorSku(sku);
            if (produto == null)
                return NotFound();

            return Ok(produto);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Adicionar(Produto produto)
        {
            int id = _produtoServico.Adicionar(produto);
            return Ok(new { Mensagem = "Produto cadastrado com sucesso!", ProdutoId = id });
        }

        [HttpPut]
        [Route("")]
        public IHttpActionResult Atualizar([FromBody] Produto produto)
        {
            _produtoServico.Atualizar(produto);
            return Ok(new { Mensagem = "Produto atualizado com sucesso!" });
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult Remover(int id)
        {
            _produtoServico.Remover(id);
            return Ok(new { Mensagem = "Produto inativado com sucesso!" });
        }

        [HttpPatch]
        [Route("restaurar/{id:int}")]
        public IHttpActionResult Restaurar(int id)
        {
            _produtoServico.Restaurar(id);
            return Ok(new { Mensagem = "Produto restaurado com sucesso!" });
        }
    }
}