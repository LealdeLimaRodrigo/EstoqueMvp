using Dominio.Entidades;
using Servicos.Interfaces;
using System.Web.Http;

namespace EstoqueMvp.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/setor")]
    public class SetorController : ApiController
    {
        private readonly ISetorServico _setorServico;

        public SetorController(ISetorServico setorServico)
        {
            _setorServico = setorServico;
        }

        // GET: Setor
        [HttpGet]
        [Route("")]
        public IHttpActionResult ObterTodos()
        {
            var setores = _setorServico.ObterTodos();
            return Ok(setores);
        }

        [HttpGet]
        [Route("inativos")]
        public IHttpActionResult ObterInativos()
        {
            var setoresInativos = _setorServico.ObterTodosInativos();
            return Ok(setoresInativos);
        }

        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult ObterPorId(int id)
        {
            var setor = _setorServico.ObterPorId(id);
            if (setor == null)
                return NotFound();

            return Ok(setor);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Adicionar(Setor setor)
        {
            int id = _setorServico.Adicionar(setor);
            return Ok(new { Mensagem = "Setor cadastrado com sucesso!", SetorId = id });
        }

        [HttpPut]
        [Route("")]
        public IHttpActionResult Atualizar([FromBody] Setor setor)
        {
            _setorServico.Atualizar(setor);
            return Ok(new { Mensagem = "Setor atualizado com sucesso!" });
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult Remover(int id)
        {
            _setorServico.Remover(id);
            return Ok(new { Mensagem = "Setor inativado com sucesso!" });
        }

        [HttpPatch]
        [Route("restaurar/{id:int}")]
        public IHttpActionResult Restaurar(int id)
        {
            _setorServico.Restaurar(id);
            return Ok(new { Mensagem = "Setor restaurado com sucesso!" });
        }
    }
}