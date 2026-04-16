using EstoqueMvp.Api.Security;
using Servicos.Dtos;
using Servicos.Interfaces;
using System.Threading.Tasks;
using System.Web.Http;

namespace EstoqueMvp.Api.Controllers
{
    /// <summary>
    /// Controller responsável pelo CRUD de usuários e autenticação via CPF/Senha.
    /// O endpoint de login é o único com acesso anônimo, retornando um token JWT.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/usuario")]
    public class UsuarioController : ApiController
    {
        private readonly IUsuarioServico _usuarioServico;

        public UsuarioController(IUsuarioServico usuarioServico)
        {
            _usuarioServico = usuarioServico;
        }

        /// <summary>
        /// Realiza a autenticação do usuário via CPF e Senha.
        /// Retorna um token JWT para autorizar as demais operações protegidas.
        /// </summary>
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Login([FromBody] LoginDto loginDto)
        {
            var usuarioLogado = await _usuarioServico.RealizarLogin(loginDto);            
            var tokenString = TokenService.GerarToken(usuarioLogado);
            return Ok(new { Usuario = usuarioLogado, Token = tokenString });
        }

        /// <summary>
        /// Retorna todos os usuários ativos. Suporta paginação via query string.
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> ObterTodos([FromUri] int? pagina = null, [FromUri] int? tamanhoPagina = null)
        {
            if (pagina.HasValue && tamanhoPagina.HasValue && pagina > 0 && tamanhoPagina > 0)
            {
                var resultado = await _usuarioServico.ObterTodosPaginado(pagina.Value, tamanhoPagina.Value);
                return Ok(resultado);
            }

            var usuarios = await _usuarioServico.ObterTodos();
            return Ok(usuarios);
        }

        /// <summary>
        /// Retorna todos os usuários inativos (soft deleted).
        /// </summary>
        [HttpGet]
        [Route("inativos")]
        public async Task<IHttpActionResult> ObterInativos()
        {
            var usuariosInativos = await _usuarioServico.ObterTodosInativos();
            return Ok(usuariosInativos);
        }

        /// <summary>
        /// Retorna um usuário específico pelo seu ID.
        /// </summary>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> ObterPorId(int id)
        {
            var usuario = await _usuarioServico.ObterPorId(id);
            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        /// <summary>
        /// Retorna um usuário pelo CPF.
        /// </summary>
        [HttpGet]
        [Route("cpf/{cpf}")]
        public async Task<IHttpActionResult> ObterPorCpf(string cpf)
        {
            var usuario = await _usuarioServico.ObterPorCpf(cpf);
            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        /// <summary>
        /// Busca usuários por nome (busca parcial com LIKE).
        /// </summary>
        [HttpGet]
        [Route("nome/{nome}")]
        public async Task<IHttpActionResult> ObterPorNome(string nome)
        {
            var usuarios = await _usuarioServico.ObterPorNome(nome);
            return Ok(usuarios);
        }

        /// <summary>
        /// Cadastra um novo usuário com validação de CPF e criptografia de senha (BCrypt).
        /// </summary>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Adicionar([FromBody] UsuarioCadastroDto dto)
        {
            int id = await _usuarioServico.Adicionar(dto);
            return Created($"api/usuario/{id}", new { Mensagem = "Usuário cadastrado com sucesso!", UsuarioId = id });
        }

        /// <summary>
        /// Atualiza os dados de um usuário existente. A senha só é alterada se fornecida.
        /// </summary>
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Atualizar(int id, [FromBody] UsuarioAtualizacaoDto dto)
        {
            dto.Id = id;
            await _usuarioServico.Atualizar(dto);
            return Ok(new { Mensagem = "Usuário atualizado com sucesso!" });
        }

        /// <summary>
        /// Inativa um usuário (soft delete).
        /// </summary>
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Remover(int id)
        {
            await _usuarioServico.Remover(id);
            return Ok(new { Mensagem = "Usuário inativado com sucesso!" });
        }

        /// <summary>
        /// Restaura um usuário previamente inativado.
        /// </summary>
        [HttpPatch]
        [Route("restaurar/{id:int}")]
        public async Task<IHttpActionResult> Restaurar(int id)
        {
            await _usuarioServico.Restaurar(id);
            return Ok(new { Mensagem = "Usuário restaurado com sucesso!" });
        }
    }
}