using Dominio.Entidades;
using Dominio.Interfaces;
using FluentValidation;
using Servicos.Dtos;
using Servicos.Interfaces;
using Servicos.Validacoes;
using Servicos.Mapeamentos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos
{
    /// <summary>
    /// Serviço responsável pelas regras de negócio de Usuário.
    /// Inclui autenticação via CPF/Senha, validação de CPF e criptografia BCrypt.
    /// </summary>
    public class UsuarioServico : IUsuarioServico
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        private readonly IValidator<LoginDto> _loginValidator;
        private readonly IValidator<UsuarioCadastroDto> _cadastroValidator;
        private readonly IValidator<UsuarioAtualizacaoDto> _atualizacaoValidator;

        public UsuarioServico(IUsuarioRepositorio usuarioRepositorio, IValidator<LoginDto> loginValidator, IValidator<UsuarioCadastroDto> cadastroValidator, IValidator<UsuarioAtualizacaoDto> atualizacaoValidator)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _loginValidator = loginValidator;
            _cadastroValidator = cadastroValidator;
            _atualizacaoValidator = atualizacaoValidator;
        }

        /// <summary>
        /// Realiza a autenticação do usuário via CPF e Senha.
        /// Verifica se o usuário existe, está ativo e a senha corresponde ao hash BCrypt armazenado.
        /// </summary>
        public async Task<UsuarioRetornoDto> RealizarLogin(LoginDto loginDto)
        {
            _loginValidator.ValidateAndThrow(loginDto);

            string cpfLimpo = CpfValidacao.LimparCpf(loginDto.Cpf); 

            var usuario = await _usuarioRepositorio.ObterPorCpfComSenha(cpfLimpo);

            if (usuario == null || !usuario.Ativo)
                throw new UnauthorizedAccessException("CPF não encontrado ou usuário inativo.");

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Senha, usuario.SenhaHash))
                throw new UnauthorizedAccessException("Senha incorreta.");

            return MapearParaDto(usuario);
        }

        /// <summary>
        /// Cadastra um novo usuário. Valida unicidade do CPF e aplica hash BCrypt na senha.
        /// </summary>
        public async Task<int> Adicionar(UsuarioCadastroDto dto)
        {
            _cadastroValidator.ValidateAndThrow(dto);

            string cpfLimpo = CpfValidacao.LimparCpf(dto.Cpf);

            if (await _usuarioRepositorio.ObterPorCpf(cpfLimpo) != null)
                throw new InvalidOperationException("Já existe um usuário com este CPF.");

            var usuario = dto.ToEntity(cpfLimpo, BCrypt.Net.BCrypt.HashPassword(dto.Senha));

            return await _usuarioRepositorio.Adicionar(usuario);
        }

        /// <summary>
        /// Atualiza os dados de um usuário. Verifica duplicidade de CPF com outros registros.
        /// A senha só é re-hashada se fornecida no DTO.
        /// </summary>
        public async Task Atualizar(UsuarioAtualizacaoDto dto)
        {
            _atualizacaoValidator.ValidateAndThrow(dto);

            string cpfLimpo = CpfValidacao.LimparCpf(dto.Cpf);

            // Verifica se o CPF já pertence a outro usuário
            var usuarioExistenteCpf = await _usuarioRepositorio.ObterPorCpf(cpfLimpo);

            if (usuarioExistenteCpf != null && usuarioExistenteCpf.Id != dto.Id)
            {
                throw new InvalidOperationException("Este CPF já está sendo utilizado por outro usuário.");
            }

            var usuarioAntigo = await _usuarioRepositorio.ObterPorIdComSenha(dto.Id);
            if (usuarioAntigo == null)
                throw new KeyNotFoundException("Usuário não encontrado para atualização.");

            usuarioAntigo.AplicarAtualizacao(dto, cpfLimpo);

            // Apenas atualiza a senha se ela foi informada na requisição
            if (!string.IsNullOrWhiteSpace(dto.Senha))
            {
                usuarioAntigo.SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);
            }

            await _usuarioRepositorio.Atualizar(usuarioAntigo);
        }

        /// <summary>
        /// Realiza o soft delete do usuário (marca como inativo).
        /// </summary>
        public async Task Remover(int id)
        {
            var usuario = await _usuarioRepositorio.ObterPorId(id);

            if (usuario == null)
                throw new KeyNotFoundException("Usuário não encontrado.");

            if (!usuario.Ativo)
                throw new InvalidOperationException("Usuário já está inativo.");

            await _usuarioRepositorio.Remover(id);
        }

        /// <summary>
        /// Restaura um usuário previamente inativado.
        /// </summary>
        public async Task Restaurar(int id)
        {
            var usuario = await _usuarioRepositorio.ObterPorId(id);

            if (usuario == null)
                throw new KeyNotFoundException("Usuário não encontrado.");

            if (usuario.Ativo)
                throw new InvalidOperationException("Usuário já está ativo.");

            await _usuarioRepositorio.Restaurar(id);
        }

        public async Task<UsuarioRetornoDto> ObterPorId(int id)
        {
            var usuario = await _usuarioRepositorio.ObterPorId(id);
            return MapearParaDto(usuario);
        }

        public async Task<IEnumerable<UsuarioRetornoDto>> ObterTodos()
        {
            var usuarios = await _usuarioRepositorio.ObterTodos();
            return usuarios.Select(MapearParaDto);
        }

        /// <summary>
        /// Retorna usuários com paginação.
        /// </summary>
        public async Task<PaginacaoResultadoDto<UsuarioRetornoDto>> ObterTodosPaginado(int pagina, int tamanhoPagina)
        {
            int offset = (pagina - 1) * tamanhoPagina;
            var itens = await _usuarioRepositorio.ObterTodosPaginado(offset, tamanhoPagina);
            var total = await _usuarioRepositorio.ContarTodosAtivos();

            return itens.Select(MapearParaDto).ToPaginacaoDto<Usuario, UsuarioRetornoDto>(total, pagina, tamanhoPagina);
        }

        public async Task<IEnumerable<UsuarioRetornoDto>> ObterTodosInativos()
        {
            var usuarios = await _usuarioRepositorio.ObterTodosInativos();
            return usuarios.Select(MapearParaDto);
        }

        public async Task<UsuarioRetornoDto> ObterPorCpf(string cpf)
        {
            var usuario = await _usuarioRepositorio.ObterPorCpf(cpf);
            return MapearParaDto(usuario);
        }

        public async Task<IEnumerable<UsuarioRetornoDto>> ObterPorNome(string nome)
        {
            var usuarios = await _usuarioRepositorio.ObterPorNome(nome);
            return usuarios.Select(MapearParaDto);
        }

        /// <summary>
        /// Mapeia a entidade de domínio para o DTO de retorno, isolando o domínio da camada de apresentação.
        /// Nunca expõe o SenhaHash ao cliente.
        /// </summary>
        private static UsuarioRetornoDto MapearParaDto(Usuario usuario)
        {
            return usuario.ToRetornoDto();
        }
    }
}

