using Servicos.Dtos;
using Servicos.Interfaces;
using System.Threading.Tasks;
using System.Web.Http;

namespace EstoqueMvp.Api.Controllers
{
    /// <summary>
    /// Controller responsável pelo CRUD completo de produtos.
    /// Utiliza soft delete (Ativo/Inativo) em vez de exclusão física.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/produto")]
    public class ProdutoController : ApiController
    {
        private readonly IProdutoServico _produtoServico;

        public ProdutoController(IProdutoServico produtoServico)
        {
            _produtoServico = produtoServico;
        }

        /// <summary>
        /// Retorna todos os produtos ativos. Suporta paginação via query string.
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> ObterTodos([FromUri] int? pagina = null, [FromUri] int? tamanhoPagina = null, [FromUri] string busca = null)
        {
            if (!string.IsNullOrWhiteSpace(busca) && pagina.HasValue && tamanhoPagina.HasValue && pagina > 0 && tamanhoPagina > 0)
            {
                var resultadoBusca = await _produtoServico.BuscarPaginado(busca.Trim(), pagina.Value, tamanhoPagina.Value);
                return Ok(resultadoBusca);
            }

            if (pagina.HasValue && tamanhoPagina.HasValue && pagina > 0 && tamanhoPagina > 0)
            {
                var resultado = await _produtoServico.ObterTodosPaginado(pagina.Value, tamanhoPagina.Value);
                return Ok(resultado);
            }

            var produtos = await _produtoServico.ObterTodos();
            return Ok(produtos);
        }

        /// <summary>
        /// Retorna todos os produtos inativos (soft deleted).
        /// </summary>
        [HttpGet]
        [Route("inativos")]
        public async Task<IHttpActionResult> ObterInativos()
        {
            var produtosInativos = await _produtoServico.ObterTodosInativos();
            return Ok(produtosInativos);
        }

        /// <summary>
        /// Retorna um produto específico pelo seu ID.
        /// </summary>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> ObterPorId(int id)
        {
            var produto = await _produtoServico.ObterPorId(id);
            if (produto == null)
                return NotFound();

            return Ok(produto);
        }

        /// <summary>
        /// Retorna um produto pelo seu código SKU único.
        /// </summary>
        [HttpGet]
        [Route("sku/{sku}")]
        public async Task<IHttpActionResult> ObterPorSku(string sku)
        {
            var produto = await _produtoServico.ObterPorSku(sku);
            if (produto == null)
                return NotFound();

            return Ok(produto);
        }

        /// <summary>
        /// Busca produtos por nome (busca parcial com LIKE).
        /// </summary>
        [HttpGet]
        [Route("nome/{nome}")]
        public async Task<IHttpActionResult> ObterPorNome(string nome)
        {
            var produtos = await _produtoServico.ObterPorNome(nome);
            return Ok(produtos);
        }

        /// <summary>
        /// Cadastra um novo produto com geração automática de SKU.
        /// </summary>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Adicionar([FromBody] ProdutoCadastroDto dto, [FromUri] bool forcar = false)
        {
            try
            {
                int id = await _produtoServico.Adicionar(dto, forcar);
                return Created($"api/produto/{id}", new { Mensagem = "Produto cadastrado com sucesso!", ProdutoId = id });
            }
            catch (Servicos.Exceptions.RegistroInativoException ex)
            {
                return Content(System.Net.HttpStatusCode.Conflict, new { Mensagem = ex.Message, Registros = ex.Registros });
            }
        }

        /// <summary>
        /// Atualiza os dados de um produto existente. O SKU não é alterável.
        /// </summary>
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Atualizar(int id, [FromBody] ProdutoAtualizacaoDto dto)
        {
            // Garante que o ID da rota prevalece sobre o do body
            dto.Id = id;
            await _produtoServico.Atualizar(dto);
            return Ok(new { Mensagem = "Produto atualizado com sucesso!" });
        }

        /// <summary>
        /// Inativa um produto (soft delete). O produto permanece no banco para histórico.
        /// </summary>
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Remover(int id)
        {
            await _produtoServico.Remover(id);
            return Ok(new { Mensagem = "Produto inativado com sucesso!" });
        }

        /// <summary>
        /// Restaura um produto previamente inativado.
        /// </summary>
        [HttpPatch]
        [Route("restaurar/{id:int}")]
        public async Task<IHttpActionResult> Restaurar(int id)
        {
            await _produtoServico.Restaurar(id);
            return Ok(new { Mensagem = "Produto restaurado com sucesso!" });
        }

        /// <summary>
        /// Restaura um produto inativo e atualiza seus dados (descrição, preço). Mantém o SKU.
        /// </summary>
        [HttpPost]
        [Route("{id:int}/restaurar")]
        public async Task<IHttpActionResult> RestaurarComDados(int id, [FromBody] ProdutoCadastroDto dto)
        {
            await _produtoServico.RestaurarComDados(id, dto);
            return Ok(new { Mensagem = "Produto restaurado com sucesso!" });
        }
    }
}