using Dominio.Entidades;
using Dominio.Enums;
using Dominio.Interfaces;
using FluentValidation;
using Servicos.Dtos;
using Servicos.Interfaces;
using Servicos.Validacoes;
using System;
using System.Collections.Generic;
using System.Transactions;

namespace Servicos
{
    public class MovimentacaoEstoqueServico : IMovimentacaoEstoqueServico
    {
        private readonly IMovimentacaoEstoqueRepositorio _movimentacaoEstoqueRepositorio;
        private readonly IEstoqueSetorRepositorio _estoqueSetorRepositorio;

        public MovimentacaoEstoqueServico(IMovimentacaoEstoqueRepositorio movimentacaoEstoqueRepositorio, IEstoqueSetorRepositorio estoqueSetorRepositorio)
        {
            _movimentacaoEstoqueRepositorio = movimentacaoEstoqueRepositorio;
            _estoqueSetorRepositorio = estoqueSetorRepositorio;
        }

        public void EntradaProduto(ProdutoDto dto)
        {
            var validator = new ProdutoDtoValidator();
            validator.ValidateAndThrow(dto);

            using (var scope = new TransactionScope())
            {
                CreditarEstoque(dto.ProdutoId, dto.SetorId, dto.Quantidade);
                RegistrarMovimentacao(dto.ProdutoId, dto.SetorId, dto.Quantidade, dto.UsuarioId, TipoMovimentacaoEnum.Entrada, Guid.NewGuid());

                scope.Complete();
            }
        }

        public void ConsumoProduto(ProdutoDto dto)
        {
            var validator = new ProdutoDtoValidator();
            validator.ValidateAndThrow(dto);

            using (var scope = new TransactionScope())
            {
                DebitarEstoque(dto.ProdutoId, dto.SetorId, dto.Quantidade);
                RegistrarMovimentacao(dto.ProdutoId, dto.SetorId, dto.Quantidade, dto.UsuarioId, TipoMovimentacaoEnum.Consumo, Guid.NewGuid());

                scope.Complete();
            }
        }

        public void TransferirProduto(TransferenciaProdutoDto dto)
        {
            var validator = new TransferenciaProdutoDtoValidator();
            validator.ValidateAndThrow(dto);

            using (var scope = new TransactionScope())
            {
                Guid transacaoId = Guid.NewGuid();

                DebitarEstoque(dto.ProdutoId, dto.SetorOrigemId, dto.Quantidade);
                RegistrarMovimentacao(dto.ProdutoId, dto.SetorOrigemId, dto.Quantidade, dto.UsuarioId, TipoMovimentacaoEnum.TransferenciaSaida, transacaoId);

                CreditarEstoque(dto.ProdutoId, dto.SetorDestinoId, dto.Quantidade);
                RegistrarMovimentacao(dto.ProdutoId, dto.SetorDestinoId, dto.Quantidade, dto.UsuarioId, TipoMovimentacaoEnum.TransferenciaEntrada, transacaoId);

                scope.Complete();
            }            
        }

        private void CreditarEstoque(int produtoId, int setorId, decimal quantidade) 
        {
            var estoqueSetor = _estoqueSetorRepositorio.ObterPorProdutoIdESetorId(produtoId, setorId);
            if (estoqueSetor == null)
            {
                _estoqueSetorRepositorio.Adicionar(new EstoqueSetor
                {
                    ProdutoId = produtoId,
                    SetorId = setorId,
                    QuantidadeEstoque = quantidade
                });
            }
            else
            {
                estoqueSetor.QuantidadeEstoque += quantidade;
                _estoqueSetorRepositorio.Atualizar(estoqueSetor);
            }
        }

        private void DebitarEstoque(int produtoId, int setorId, decimal quantidade)
        {
            var estoqueSetor = _estoqueSetorRepositorio.ObterPorProdutoIdESetorId(produtoId, setorId);
            if (estoqueSetor == null || estoqueSetor.QuantidadeEstoque < quantidade)
            {
                throw new InvalidOperationException($"Saldo insuficiente no setor. Saldo atual: {estoqueSetor?.QuantidadeEstoque ?? 0}");
            }
            estoqueSetor.QuantidadeEstoque -= quantidade;
            _estoqueSetorRepositorio.Atualizar(estoqueSetor);
        }

        private void RegistrarMovimentacao(int produtoId, int setorId, decimal quantidade, int usuarioId, TipoMovimentacaoEnum tipoMovimentacao, Guid transacaoId)
        {
            _movimentacaoEstoqueRepositorio.Adicionar(new MovimentacaoEstoque
            {
                ProdutoId = produtoId,
                SetorId = setorId,
                Quantidade = quantidade,
                TipoMovimentacaoId = (int)tipoMovimentacao,
                UsuarioId = usuarioId,
                TransacaoId = transacaoId
            });
        }

        public IEnumerable<MovimentacaoEstoque> ObterPorSetorId(int setorId) => _movimentacaoEstoqueRepositorio.ObterPorSetorId(setorId);

        public IEnumerable<MovimentacaoEstoque> ObterPorProdutoId(int produtoId) => _movimentacaoEstoqueRepositorio.ObterPorProdutoId(produtoId);

        public IEnumerable<MovimentacaoEstoque> ObterTodos() => _movimentacaoEstoqueRepositorio.ObterTodos();

        public IEnumerable<MovimentacaoEstoque> ObterPorUsuarioId(int usuarioId) => _movimentacaoEstoqueRepositorio.ObterPorUsuarioId(usuarioId);

        public IEnumerable<MovimentacaoEstoque> ObterPorTipoMovimentacaoId(int tipoMovimentacaoId) => _movimentacaoEstoqueRepositorio.ObterPorTipoMovimentacaoId(tipoMovimentacaoId);

        public IEnumerable<MovimentacaoEstoque> ObterPorData(DateTime data) => _movimentacaoEstoqueRepositorio.ObterPorData(data);

        public IEnumerable<MovimentacaoEstoque> ObterPorTransacaoId(Guid transacaoId) => _movimentacaoEstoqueRepositorio.ObterPorTransacaoId(transacaoId);
    }
}
