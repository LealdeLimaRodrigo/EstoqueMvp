using EstoqueMvp.Api.Extensions;
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

            // Setar cookie httpOnly, Secure (manual, compatível com .NET Framework 4.8)
            var resp = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new System.Net.Http.ObjectContent(
                    typeof(object),
                    new { Usuario = usuarioLogado },
                    System.Web.Http.GlobalConfiguration.Configuration.Formatters.JsonFormatter)
            };
            // SameSite=Lax funciona para same-site (localhost com portas diferentes)
            var cookie = $"jwt_token={tokenString}; HttpOnly; Path=/; Expires={System.DateTime.UtcNow.AddHours(8):R}; SameSite=None; Secure;";
            resp.Headers.Add("Set-Cookie", cookie);
            return ResponseMessage(resp);
        }

        /// <summary>
        /// Logout: remove o cookie JWT httpOnly.
        /// </summary>
        [HttpPost]
        [Route("logout")]
        [AllowAnonymous]
        public IHttpActionResult Logout()
        {
            // Expira o cookie JWT
            var resp = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new System.Net.Http.ObjectContent(
                    typeof(object),
                    new { Mensagem = "Logout realizado." },
                    System.Web.Http.GlobalConfiguration.Configuration.Formatters.JsonFormatter)
            };
            var cookie = $"jwt_token=; HttpOnly; Path=/; Expires={System.DateTime.UtcNow.AddDays(-1):R}; SameSite=None; Secure;";
            resp.Headers.Add("Set-Cookie", cookie);
            return ResponseMessage(resp);
        }

        /// <summary>
        /// Retorna todos os usuários ativos. Suporta paginação via query string.
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> ObterTodos([FromUri] int? pagina = null, [FromUri] int? tamanhoPagina = null, [FromUri] string busca = null)
        {
            if (!string.IsNullOrWhiteSpace(busca) && pagina.HasValue && tamanhoPagina.HasValue && pagina > 0 && tamanhoPagina > 0)
            {
                var resultadoBusca = await _usuarioServico.BuscarPaginado(busca.Trim(), pagina.Value, tamanhoPagina.Value);
                return Ok(resultadoBusca);
            }

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
        /// Retorna os dados do usuário autenticado (perfil).
        /// </summary>
        [HttpGet]
        [Route("perfil")]
        public async Task<IHttpActionResult> Perfil()
        {
            var userId = this.ObterUsuarioIdDoToken();
            var usuario = await _usuarioServico.ObterPorId(userId);
            if (usuario == null) return NotFound();
            return Ok(usuario);
        }

        /// <summary>
        /// Cadastra um novo usuário com validação de CPF e criptografia de senha (BCrypt).
        /// </summary>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Adicionar([FromBody] UsuarioCadastroDto dto, [FromUri] bool forcar = false)
        {
            try
            {
                int id = await _usuarioServico.Adicionar(dto, forcar);
                return Created($"api/usuario/{id}", new { Mensagem = "Usuário cadastrado com sucesso!", UsuarioId = id });
            }
            catch (Servicos.Exceptions.UsuarioInativoException ex)
            {
                return Content(System.Net.HttpStatusCode.Conflict, new { Mensagem = ex.Message, UsuarioInativoId = ex.UsuarioInativoId, Nome = ex.Nome, Cpf = ex.Cpf });
            }
        }

        /// <summary>
        /// Restaura um usuário inativo e atualiza seus dados (nome, senha).
        /// </summary>
        [HttpPost]
        [Route("{id:int}/restaurar")]
        public async Task<IHttpActionResult> RestaurarComDados(int id, [FromBody] UsuarioCadastroDto dto)
        {
            await _usuarioServico.RestaurarComDados(id, dto);
            return Ok(new { Mensagem = "Usuário restaurado com sucesso!" });
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