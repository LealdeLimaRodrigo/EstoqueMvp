using Dominio.Entidades;
using Dominio.Interfaces;
using FluentValidation;
using Servicos.Dtos;
using Servicos.Exceptions;
using Servicos.Interfaces;
using Servicos.Mapeamentos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos
{
    /// <summary>
    /// Serviço responsável pelas regras de negócio de Setor.
    /// Gerencia o CRUD de setores com suporte a soft delete e paginação.
    /// </summary>
    public class SetorServico : ISetorServico
    {
        private readonly ISetorRepositorio _setorRepositorio;

        private readonly IValidator<SetorCadastroDto> _cadastroValidator;
        private readonly IValidator<SetorAtualizacaoDto> _atualizacaoValidator;

        public SetorServico(ISetorRepositorio setorRepositorio, IValidator<SetorCadastroDto> cadastroValidator, IValidator<SetorAtualizacaoDto> atualizacaoValidator)
        {
            _setorRepositorio = setorRepositorio;
            _cadastroValidator = cadastroValidator;
            _atualizacaoValidator = atualizacaoValidator;
        }

        /// <summary>
        /// Cadastra um novo setor após validação dos dados.
        /// </summary>
        public async Task<int> Adicionar(SetorCadastroDto dto, bool ignorarInativos = false)
        {
            _cadastroValidator.ValidateAndThrow(dto);

            if (!ignorarInativos)
            {
                var inativos = await _setorRepositorio.ObterTodosInativos();
                var matches = inativos.Where(s => s.Nome.Equals(dto.Nome, StringComparison.OrdinalIgnoreCase)).ToList();
                if (matches.Any())
                    throw new RegistroInativoException("Já existe um setor inativo com este nome.",
                        matches.Select(s => new RegistroInativoInfo { Id = s.Id, Nome = s.Nome }).ToList());
            }

            return await _setorRepositorio.Adicionar(dto.ToEntity());
        }

        /// <summary>
        /// Atualiza os dados de um setor existente.
        /// </summary>
        public async Task Atualizar(SetorAtualizacaoDto dto)
        {
            _atualizacaoValidator.ValidateAndThrow(dto);

            var setorExistente = await _setorRepositorio.ObterPorId(dto.Id);
            if (setorExistente == null)
                throw new KeyNotFoundException("Setor não encontrado para atualização.");

            setorExistente.AplicarAtualizacao(dto);

            await _setorRepositorio.Atualizar(setorExistente);
        }

        /// <summary>
        /// Realiza o soft delete do setor.
        /// </summary>
        public async Task Remover(int id)
        {
            var setor = await _setorRepositorio.ObterPorId(id);

            if (setor == null)
                throw new KeyNotFoundException("Setor não encontrado.");

            if (!setor.Ativo)
                throw new InvalidOperationException("Setor já está inativo.");

            await _setorRepositorio.Remover(id);
        }

        /// <summary>
        /// Restaura um setor previamente inativado.
        /// </summary>
        public async Task Restaurar(int id)
        {
            var setor = await _setorRepositorio.ObterPorId(id);

            if (setor == null)
                throw new KeyNotFoundException("Setor não encontrado.");

            if (setor.Ativo)
                throw new InvalidOperationException("Setor já está ativo.");

            await _setorRepositorio.Restaurar(id);
        }

        /// <summary>
        /// Restaura um setor inativo e atualiza seu nome.
        /// </summary>
        public async Task RestaurarComDados(int id, SetorCadastroDto dto)
        {
            var setor = await _setorRepositorio.ObterPorId(id);
            if (setor == null)
                throw new KeyNotFoundException("Setor não encontrado.");

            setor.Nome = dto.Nome;
            setor.Ativo = true;

            await _setorRepositorio.Atualizar(setor);
        }

        /// <summary>
        /// Retorna um setor pelo ID.
        /// </summary>
        public async Task<SetorRetornoDto> ObterPorId(int id)
        {
            var setor = await _setorRepositorio.ObterPorId(id);
            return MapearParaDto(setor);
        }

        /// <summary>
        /// Retorna todos os setores ativos.
        /// </summary>
        public async Task<IEnumerable<SetorRetornoDto>> ObterTodos()
        {
            var setores = await _setorRepositorio.ObterTodos();
            return setores.Select(MapearParaDto);
        }

        /// <summary>
        /// Retorna setores ativos com paginação.
        /// </summary>
        public async Task<PaginacaoResultadoDto<SetorRetornoDto>> ObterTodosPaginado(int pagina, int tamanhoPagina)
        {
            int offset = (pagina - 1) * tamanhoPagina;
            var itens = await _setorRepositorio.ObterTodosPaginado(offset, tamanhoPagina);
            var total = await _setorRepositorio.ContarTodosAtivos();

            return itens.Select(MapearParaDto).ToPaginacaoDto<Setor, SetorRetornoDto>(total, pagina, tamanhoPagina);
        }

        /// <summary>
        /// Busca setores por termo com paginação server-side.
        /// </summary>
        public async Task<PaginacaoResultadoDto<SetorRetornoDto>> BuscarPaginado(string termo, int pagina, int tamanhoPagina)
        {
            int offset = (pagina - 1) * tamanhoPagina;
            var itens = await _setorRepositorio.BuscarPaginado(termo, offset, tamanhoPagina);
            var total = await _setorRepositorio.ContarPorBusca(termo);

            return itens.Select(MapearParaDto).ToPaginacaoDto<Setor, SetorRetornoDto>(total, pagina, tamanhoPagina);
        }

        /// <summary>
        /// Retorna todos os setores inativos.
        /// </summary>
        public async Task<IEnumerable<SetorRetornoDto>> ObterTodosInativos()
        {
            var setores = await _setorRepositorio.ObterTodosInativos();
            return setores.Select(MapearParaDto);
        }

        /// <summary>
        /// Retorna setores cujo nome contenha o termo informado.
        /// </summary>
        public async Task<IEnumerable<SetorRetornoDto>> ObterPorNome(string nome)
        {
            var setores = await _setorRepositorio.ObterPorNome(nome);
            return setores.Select(MapearParaDto);
        }

        /// <summary>
        /// Mapeia a entidade de domínio para o DTO de retorno, isolando o domínio da camada de apresentação.
        /// </summary>
        private static SetorRetornoDto MapearParaDto(Setor setor)
        {
            return setor.ToRetornoDto();
        }
    }
}

