using Dominio.Entidades;
using Dominio.Interfaces;
using FluentValidation;
using Servicos.Interfaces;
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

        /// <summary>
        /// Adiciona um registro de estoque após validação.
        /// </summary>
        public async Task<int> Adicionar(EstoqueSetor estoqueSetor)
        {
            _validator.ValidateAndThrow(estoqueSetor);

            return await _estoqueSetorRepositorio.Adicionar(estoqueSetor);
        }

        /// <summary>
        /// Atualiza a quantidade de estoque após validação.
        /// </summary>
        public async Task Atualizar(EstoqueSetor estoqueSetor)
        {
            _validator.ValidateAndThrow(estoqueSetor);

            await _estoqueSetorRepositorio.Atualizar(estoqueSetor);
        }

        /// <summary>
        /// Retorna o estoque de todos os produtos em um setor.
        /// </summary>
        public async Task<IEnumerable<EstoqueSetor>> ObterPorSetorId(int setorId) => await _estoqueSetorRepositorio.ObterPorSetorId(setorId);

        /// <summary>
        /// Retorna o estoque de um produto em todos os setores.
        /// </summary>
        public async Task<IEnumerable<EstoqueSetor>> ObterPorProdutoId(int produtoId) => await _estoqueSetorRepositorio.ObterPorProdutoId(produtoId);

        /// <summary>
        /// Retorna todos os registros de estoque por setor.
        /// </summary>
        public async Task<IEnumerable<EstoqueSetor>> ObterTodos() => await _estoqueSetorRepositorio.ObterTodos();

        /// <summary>
        /// Retorna o estoque de um produto em um setor específico.
        /// </summary>
        public async Task<EstoqueSetor> ObterPorProdutoIdESetorId(int produtoId, int setorId) => await _estoqueSetorRepositorio.ObterPorProdutoIdESetorId(produtoId, setorId);
    }
}

