using Dominio.Entidades;
using Dominio.Enums;
using Dominio.Interfaces;
using FluentValidation;
using Servicos.Dtos;
using Servicos.Interfaces;
using Servicos.Validacoes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Transactions;

namespace Servicos
{
    /// <summary>
    /// Serviço responsável pelas regras de negócio de movimentação de estoque.
    /// Todas as operações são executadas dentro de TransactionScope para garantir atomicidade.
    /// Valida a existência de Produto e Setor antes de executar qualquer movimentação.
    /// </summary>
    public class MovimentacaoEstoqueServico : IMovimentacaoEstoqueServico
    {
        private readonly IMovimentacaoEstoqueRepositorio _movimentacaoEstoqueRepositorio;
        private readonly IEstoqueSetorRepositorio _estoqueSetorRepositorio;
        private readonly IProdutoRepositorio _produtoRepositorio;
        private readonly ISetorRepositorio _setorRepositorio;

        private readonly IValidator<MovimentacaoEstoqueDto> _movimentacaoValidator;
        private readonly IValidator<TransferenciaProdutoDto> _transferenciaValidator;

        public MovimentacaoEstoqueServico(
            IMovimentacaoEstoqueRepositorio movimentacaoEstoqueRepositorio,
            IEstoqueSetorRepositorio estoqueSetorRepositorio,
            IProdutoRepositorio produtoRepositorio,
            ISetorRepositorio setorRepositorio,
            IValidator<MovimentacaoEstoqueDto> movimentacaoValidator,
            IValidator<TransferenciaProdutoDto> transferenciaValidator)
        {
            _movimentacaoEstoqueRepositorio = movimentacaoEstoqueRepositorio;
            _estoqueSetorRepositorio = estoqueSetorRepositorio;
            _produtoRepositorio = produtoRepositorio;
            _setorRepositorio = setorRepositorio;
            _movimentacaoValidator = movimentacaoValidator;
            _transferenciaValidator = transferenciaValidator;
        }

        /// <summary>
        /// Registra a entrada de produto em um setor, incrementando o estoque.
        /// </summary>
        public async Task EntradaProduto(MovimentacaoEstoqueDto dto)
        {
            _movimentacaoValidator.ValidateAndThrow(dto);

            await ValidarProdutoExisteEAtivo(dto.ProdutoId);
            await ValidarSetorExisteEAtivo(dto.SetorId);

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await CreditarEstoque(dto.ProdutoId, dto.SetorId, dto.Quantidade);
                await RegistrarMovimentacao(dto.ProdutoId, dto.SetorId, dto.Quantidade, dto.UsuarioId, TipoMovimentacaoEnum.Entrada, Guid.NewGuid());

                scope.Complete();
            }

            Trace.TraceInformation($"[Movimentação] Entrada: ProdutoId={dto.ProdutoId}, SetorId={dto.SetorId}, Qtd={dto.Quantidade}, UsuarioId={dto.UsuarioId}");
        }

        /// <summary>
        /// Registra o consumo interno de um produto, debitando o estoque do setor.
        /// Impede a operação caso o saldo seja insuficiente.
        /// </summary>
        public async Task ConsumoProduto(MovimentacaoEstoqueDto dto)
        {
            _movimentacaoValidator.ValidateAndThrow(dto);

            await ValidarProdutoExisteEAtivo(dto.ProdutoId);
            await ValidarSetorExisteEAtivo(dto.SetorId);

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await DebitarEstoque(dto.ProdutoId, dto.SetorId, dto.Quantidade);
                await RegistrarMovimentacao(dto.ProdutoId, dto.SetorId, dto.Quantidade, dto.UsuarioId, TipoMovimentacaoEnum.Consumo, Guid.NewGuid());

                scope.Complete();
            }

            Trace.TraceInformation($"[Movimentação] Consumo: ProdutoId={dto.ProdutoId}, SetorId={dto.SetorId}, Qtd={dto.Quantidade}, UsuarioId={dto.UsuarioId}");
        }

        /// <summary>
        /// Realiza a transferência de produto entre dois setores.
        /// Debita do setor de origem e credita no setor de destino em uma única transação.
        /// Um TransacaoId compartilhado vincula os dois registros para rastreabilidade.
        /// </summary>
        public async Task TransferirProduto(TransferenciaProdutoDto dto)
        {
            _transferenciaValidator.ValidateAndThrow(dto);

            await ValidarProdutoExisteEAtivo(dto.ProdutoId);
            await ValidarSetorExisteEAtivo(dto.SetorOrigemId);
            await ValidarSetorExisteEAtivo(dto.SetorDestinoId);

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                Guid transacaoId = Guid.NewGuid();

                await DebitarEstoque(dto.ProdutoId, dto.SetorOrigemId, dto.Quantidade);
                await RegistrarMovimentacao(dto.ProdutoId, dto.SetorOrigemId, dto.Quantidade, dto.UsuarioId, TipoMovimentacaoEnum.TransferenciaSaida, transacaoId);

                await CreditarEstoque(dto.ProdutoId, dto.SetorDestinoId, dto.Quantidade);
                await RegistrarMovimentacao(dto.ProdutoId, dto.SetorDestinoId, dto.Quantidade, dto.UsuarioId, TipoMovimentacaoEnum.TransferenciaEntrada, transacaoId);

                scope.Complete();
            }

            Trace.TraceInformation($"[Movimentação] Transferência: ProdutoId={dto.ProdutoId}, Origem={dto.SetorOrigemId}, Destino={dto.SetorDestinoId}, Qtd={dto.Quantidade}, UsuarioId={dto.UsuarioId}");
        }

        /// <summary>
        /// Valida se o produto existe e está ativo antes de permitir uma movimentação.
        /// </summary>
        private async Task ValidarProdutoExisteEAtivo(int produtoId)
        {
            var produto = await _produtoRepositorio.ObterPorId(produtoId);
            if (produto == null)
                throw new KeyNotFoundException($"Produto com ID {produtoId} não encontrado.");
            if (!produto.Ativo)
                throw new InvalidOperationException($"Produto '{produto.Nome}' está inativo e não pode participar de movimentações.");
        }

        /// <summary>
        /// Valida se o setor existe e está ativo antes de permitir uma movimentação.
        /// </summary>
        private async Task ValidarSetorExisteEAtivo(int setorId)
        {
            var setor = await _setorRepositorio.ObterPorId(setorId);
            if (setor == null)
                throw new KeyNotFoundException($"Setor com ID {setorId} não encontrado.");
            if (!setor.Ativo)
                throw new InvalidOperationException($"Setor '{setor.Nome}' está inativo e não pode participar de movimentações.");
        }

        /// <summary>
        /// Credita a quantidade no estoque do setor. Cria o registro caso não exista.
        /// </summary>
        private async Task CreditarEstoque(int produtoId, int setorId, decimal quantidade) 
        {
            var estoqueSetor = await _estoqueSetorRepositorio.ObterPorProdutoIdESetorId(produtoId, setorId);
            if (estoqueSetor == null)
            {
                await _estoqueSetorRepositorio.Adicionar(new EstoqueSetor
                {
                    ProdutoId = produtoId,
                    SetorId = setorId,
                    QuantidadeEstoque = quantidade
                });
            }
            else
            {
                estoqueSetor.QuantidadeEstoque += quantidade;
                await _estoqueSetorRepositorio.Atualizar(estoqueSetor);
            }
        }

        /// <summary>
        /// Debita a quantidade do estoque do setor. Lança exceção se o saldo for insuficiente.
        /// </summary>
        private async Task DebitarEstoque(int produtoId, int setorId, decimal quantidade)
        {
            var estoqueSetor = await _estoqueSetorRepositorio.ObterPorProdutoIdESetorId(produtoId, setorId);
            if (estoqueSetor == null || estoqueSetor.QuantidadeEstoque < quantidade)
            {
                throw new InvalidOperationException($"Saldo insuficiente no setor. Saldo atual: {estoqueSetor?.QuantidadeEstoque ?? 0}");
            }
            estoqueSetor.QuantidadeEstoque -= quantidade;
            await _estoqueSetorRepositorio.Atualizar(estoqueSetor);
        }

        /// <summary>
        /// Registra a movimentação na tabela de histórico para rastreabilidade completa.
        /// </summary>
        private async Task RegistrarMovimentacao(int produtoId, int setorId, decimal quantidade, int usuarioId, TipoMovimentacaoEnum tipoMovimentacao, Guid transacaoId)
        {
            await _movimentacaoEstoqueRepositorio.Adicionar(new MovimentacaoEstoque
            {
                ProdutoId = produtoId,
                SetorId = setorId,
                Quantidade = quantidade,
                TipoMovimentacaoId = (int)tipoMovimentacao,
                UsuarioId = usuarioId,
                TransacaoId = transacaoId
            });
        }

        public async Task<IEnumerable<MovimentacaoEstoque>> ObterPorSetorId(int setorId) => await _movimentacaoEstoqueRepositorio.ObterPorSetorId(setorId);

        public async Task<IEnumerable<MovimentacaoEstoque>> ObterPorProdutoId(int produtoId) => await _movimentacaoEstoqueRepositorio.ObterPorProdutoId(produtoId);

        public async Task<IEnumerable<MovimentacaoEstoque>> ObterTodos() => await _movimentacaoEstoqueRepositorio.ObterTodos();

        public async Task<IEnumerable<MovimentacaoEstoque>> ObterPorUsuarioId(int usuarioId) => await _movimentacaoEstoqueRepositorio.ObterPorUsuarioId(usuarioId);

        public async Task<IEnumerable<MovimentacaoEstoque>> ObterPorTipoMovimentacaoId(int tipoMovimentacaoId) => await _movimentacaoEstoqueRepositorio.ObterPorTipoMovimentacaoId(tipoMovimentacaoId);

        public async Task<IEnumerable<MovimentacaoEstoque>> ObterPorData(DateTime data) => await _movimentacaoEstoqueRepositorio.ObterPorData(data);

        public async Task<IEnumerable<MovimentacaoEstoque>> ObterPorTransacaoId(Guid transacaoId) => await _movimentacaoEstoqueRepositorio.ObterPorTransacaoId(transacaoId);
    }
}

