using Servicos.Dtos;
using Servicos.Interfaces;
using System.Threading.Tasks;
using System.Web.Http;

namespace EstoqueMvp.Api.Controllers
{
    /// <summary>
    /// Controller responsável pelo CRUD de setores/departamentos.
    /// Cada setor possui seu próprio estoque de produtos.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/setor")]
    public class SetorController : ApiController
    {
        private readonly ISetorServico _setorServico;

        public SetorController(ISetorServico setorServico)
        {
            _setorServico = setorServico;
        }

        /// <summary>
        /// Retorna todos os setores ativos. Suporta paginação via query string.
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> ObterTodos([FromUri] int? pagina = null, [FromUri] int? tamanhoPagina = null)
        {
            if (pagina.HasValue && tamanhoPagina.HasValue && pagina > 0 && tamanhoPagina > 0)
            {
                var resultado = await _setorServico.ObterTodosPaginado(pagina.Value, tamanhoPagina.Value);
                return Ok(resultado);
            }

            var setores = await _setorServico.ObterTodos();
            return Ok(setores);
        }

        /// <summary>
        /// Retorna todos os setores inativos (soft deleted).
        /// </summary>
        [HttpGet]
        [Route("inativos")]
        public async Task<IHttpActionResult> ObterInativos()
        {
            var setoresInativos = await _setorServico.ObterTodosInativos();
            return Ok(setoresInativos);
        }

        /// <summary>
        /// Retorna um setor específico pelo seu ID.
        /// </summary>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> ObterPorId(int id)
        {
            var setor = await _setorServico.ObterPorId(id);
            if (setor == null)
                return NotFound();

            return Ok(setor);
        }

        /// <summary>
        /// Busca setores por nome (busca parcial com LIKE).
        /// </summary>
        [HttpGet]
        [Route("nome/{nome}")]
        public async Task<IHttpActionResult> ObterPorNome(string nome)
        {
            var setores = await _setorServico.ObterPorNome(nome);
            return Ok(setores);
        }

        /// <summary>
        /// Cadastra um novo setor no sistema.
        /// </summary>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Adicionar([FromBody] SetorCadastroDto dto)
        {
            int id = await _setorServico.Adicionar(dto);
            return Created($"api/setor/{id}", new { Mensagem = "Setor cadastrado com sucesso!", SetorId = id });
        }

        /// <summary>
        /// Atualiza os dados de um setor existente.
        /// </summary>
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Atualizar(int id, [FromBody] SetorAtualizacaoDto dto)
        {
            dto.Id = id;
            await _setorServico.Atualizar(dto);
            return Ok(new { Mensagem = "Setor atualizado com sucesso!" });
        }

        /// <summary>
        /// Inativa um setor (soft delete).
        /// </summary>
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Remover(int id)
        {
            await _setorServico.Remover(id);
            return Ok(new { Mensagem = "Setor inativado com sucesso!" });
        }

        /// <summary>
        /// Restaura um setor previamente inativado.
        /// </summary>
        [HttpPatch]
        [Route("restaurar/{id:int}")]
        public async Task<IHttpActionResult> Restaurar(int id)
        {
            await _setorServico.Restaurar(id);
            return Ok(new { Mensagem = "Setor restaurado com sucesso!" });
        }
    }
}