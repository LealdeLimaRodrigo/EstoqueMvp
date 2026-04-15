using Dominio.Entidades;
using EstoqueMvp.Api.Security;
using Servicos.Dtos;
using Servicos.Interfaces;
using System.Web.Http;

namespace EstoqueMvp.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/usuario")]
    public class UsuarioController : ApiController
    {
        private readonly IUsuarioServico _usuarioServico;

        public UsuarioController(IUsuarioServico usuarioServico)
        {
            _usuarioServico = usuarioServico;
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public IHttpActionResult Login([FromBody] LoginDto loginDto)
        {
            var usuarioLogado = _usuarioServico.RealizarLogin(loginDto);            
            var tokenString = TokenService.GerarToken(usuarioLogado);
            return Ok(new { Usuario = usuarioLogado, Token = tokenString });
        }

        // GET: Usuario
        [HttpGet]
        [Route("")]
        public IHttpActionResult ObterTodos()
        {
            var usuarios = _usuarioServico.ObterTodos();
            return Ok(usuarios);
        }


        [HttpGet]
        [Route("inativos")]
        public IHttpActionResult ObterInativos()
        {
            var usuariosInativos = _usuarioServico.ObterTodosInativos();
            return Ok(usuariosInativos);
        }


        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult ObterPorId(int id)
        {
            var usuario = _usuarioServico.ObterPorId(id);
            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        [HttpGet]
        [Route("cpf/{cpf}")]
        public IHttpActionResult ObterPorCpf(string cpf)
        {
            var usuario = _usuarioServico.ObterPorCpf(cpf);
            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Adicionar(Usuario usuario)
        {
            int id = _usuarioServico.Adicionar(usuario);
            return Ok(new { Mensagem = "Usuário cadastrado com sucesso!", UsuarioId = id });
        }

        [HttpPut]
        [Route("")]
        public IHttpActionResult Atualizar([FromBody] Usuario usuario)
        {
            _usuarioServico.Atualizar(usuario);
            return Ok(new { Mensagem = "Usuário atualizado com sucesso!" });
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult Remover(int id)
        {
            _usuarioServico.Remover(id);
            return Ok(new { Mensagem = "Usuário inativado com sucesso!" });
        }

        [HttpPatch]
        [Route("restaurar/{id:int}")]
        public IHttpActionResult Restaurar(int id)
        {
            _usuarioServico.Restaurar(id);
            return Ok(new { Mensagem = "Usuário restaurado com sucesso!" });
        }
    }
}