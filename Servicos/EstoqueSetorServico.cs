using Dominio.Entidades;
using Dominio.Interfaces;
using FluentValidation;
using Servicos.Interfaces;
using Servicos.Validacoes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servicos
{
    /// <summary>
    /// Serviço responsável pela consulta e manutenção do saldo de estoque por setor.
    /// As operações de crédito e débito são realizadas pelo MovimentacaoEstoqueServico.
    /// </summary>
    public class EstoqueSetorServico : IEstoqueSetorServico
    {
        private readonly IEstoqueSetorRepositorio _estoqueSetorRepositorio;

        private readonly IValidator<EstoqueSetor> _validator;

        public EstoqueSetorServico(IEstoqueSetorRepositorio estoqueSetorRepositorio, IValidator<EstoqueSetor> validator)
        {
            _estoqueSetorRepositorio = estoqueSetorRepositorio;
            _validator = validator;
        }

        public async Task<int> Adicionar(EstoqueSetor estoqueSetor)
        {
            _validator.ValidateAndThrow(estoqueSetor);

            return await _estoqueSetorRepositorio.Adicionar(estoqueSetor);
        }

        public async Task Atualizar(EstoqueSetor estoqueSetor)
        {
            _validator.ValidateAndThrow(estoqueSetor);

            await _estoqueSetorRepositorio.Atualizar(estoqueSetor);
        }

        public async Task<IEnumerable<EstoqueSetor>> ObterPorSetorId(int setorId) => await _estoqueSetorRepositorio.ObterPorSetorId(setorId);

        public async Task<IEnumerable<EstoqueSetor>> ObterPorProdutoId(int produtoId) => await _estoqueSetorRepositorio.ObterPorProdutoId(produtoId);

        public async Task<IEnumerable<EstoqueSetor>> ObterTodos() => await _estoqueSetorRepositorio.ObterTodos();

        public async Task<EstoqueSetor> ObterPorProdutoIdESetorId(int produtoId, int setorId) => await _estoqueSetorRepositorio.ObterPorProdutoIdESetorId(produtoId, setorId);
    }
}

