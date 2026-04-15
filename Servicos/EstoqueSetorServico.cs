using Dominio.Entidades;
using Dominio.Interfaces;
using FluentValidation;
using Servicos.Interfaces;
using Servicos.Validacoes;
using System.Collections.Generic;

namespace Servicos
{
    public class EstoqueSetorServico : IEstoqueSetorServico
    {
        private readonly IEstoqueSetorRepositorio _estoqueSetorRepositorio;

        public EstoqueSetorServico(IEstoqueSetorRepositorio estoqueSetorRepositorio)
        {
            _estoqueSetorRepositorio = estoqueSetorRepositorio;
        }

        public int Adicionar(EstoqueSetor estoqueSetor)
        {
            var validator = new EstoqueSetorValidator();
            validator.ValidateAndThrow(estoqueSetor);

            return _estoqueSetorRepositorio.Adicionar(estoqueSetor);
        }

        public void Atualizar(EstoqueSetor estoqueSetor)
        {
            var validator = new EstoqueSetorValidator();
            validator.ValidateAndThrow(estoqueSetor);

            _estoqueSetorRepositorio.Atualizar(estoqueSetor);
        }

        public IEnumerable<EstoqueSetor> ObterPorSetorId(int setorId) => _estoqueSetorRepositorio.ObterPorSetorId(setorId);

        public IEnumerable<EstoqueSetor> ObterPorProdutoId(int produtoId) => _estoqueSetorRepositorio.ObterPorProdutoId(produtoId);

        public IEnumerable<EstoqueSetor> ObterTodos() => _estoqueSetorRepositorio.ObterTodos();

        public EstoqueSetor ObterPorProdutoIdESetorId(int produtoId, int setorId) => _estoqueSetorRepositorio.ObterPorProdutoIdESetorId(produtoId, setorId);
    }
}
