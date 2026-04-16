using Dominio.Entidades;
using Dominio.Interfaces;
using FluentValidation;
using Servicos.Dtos;
using Servicos.Interfaces;
using Servicos.Mapeamentos;
using Servicos.Validacoes;
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

        public async Task<int> Adicionar(SetorCadastroDto dto)
        {
            _cadastroValidator.ValidateAndThrow(dto);

            return await _setorRepositorio.Adicionar(dto.ToEntity());
        }

        public async Task Atualizar(SetorAtualizacaoDto dto)
        {
            _atualizacaoValidator.ValidateAndThrow(dto);

            var setorExistente = await _setorRepositorio.ObterPorId(dto.Id);
            if (setorExistente == null)
                throw new KeyNotFoundException("Setor não encontrado para atualização.");

            setorExistente.AplicarAtualizacao(dto);

            await _setorRepositorio.Atualizar(setorExistente);
        }

        public async Task Remover(int id)
        {
            var setor = await _setorRepositorio.ObterPorId(id);

            if (setor == null)
                throw new KeyNotFoundException("Setor não encontrado.");

            if (!setor.Ativo)
                throw new InvalidOperationException("Setor já está inativo.");

            await _setorRepositorio.Remover(id);
        }

        public async Task Restaurar(int id)
        {
            var setor = await _setorRepositorio.ObterPorId(id);

            if (setor == null)
                throw new KeyNotFoundException("Setor não encontrado.");

            if (setor.Ativo)
                throw new InvalidOperationException("Setor já está ativo.");

            await _setorRepositorio.Restaurar(id);
        }

        public async Task<SetorRetornoDto> ObterPorId(int id)
        {
            var setor = await _setorRepositorio.ObterPorId(id);
            return MapearParaDto(setor);
        }

        public async Task<IEnumerable<SetorRetornoDto>> ObterTodos()
        {
            var setores = await _setorRepositorio.ObterTodos();
            return setores.Select(MapearParaDto);
        }

        public async Task<PaginacaoResultadoDto<SetorRetornoDto>> ObterTodosPaginado(int pagina, int tamanhoPagina)
        {
            int offset = (pagina - 1) * tamanhoPagina;
            var itens = await _setorRepositorio.ObterTodosPaginado(offset, tamanhoPagina);
            var total = await _setorRepositorio.ContarTodosAtivos();

            return itens.Select(MapearParaDto).ToPaginacaoDto<Setor, SetorRetornoDto>(total, pagina, tamanhoPagina);
        }

        public async Task<IEnumerable<SetorRetornoDto>> ObterTodosInativos()
        {
            var setores = await _setorRepositorio.ObterTodosInativos();
            return setores.Select(MapearParaDto);
        }

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

