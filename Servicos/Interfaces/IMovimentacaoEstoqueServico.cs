using Dominio.Entidades;
using Servicos.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servicos.Interfaces
{
    /// <summary>
    /// Contrato de serviço para operações de movimentação de estoque
    /// (entrada, consumo e transferência entre setores).
    /// </summary>
    public interface IMovimentacaoEstoqueServico
    {
        Task<IEnumerable<MovimentacaoEstoque>> ObterTodos();
        Task<IEnumerable<MovimentacaoEstoque>> ObterPorSetorId(int setorId);
        Task<IEnumerable<MovimentacaoEstoque>> ObterPorProdutoId(int produtoId);
        Task<IEnumerable<MovimentacaoEstoque>> ObterPorUsuarioId(int usuarioId);
        Task<IEnumerable<MovimentacaoEstoque>> ObterPorTipoMovimentacaoId(int tipoMovimentacaoId);
        Task<IEnumerable<MovimentacaoEstoque>> ObterPorData(DateTime data);
        Task<IEnumerable<MovimentacaoEstoque>> ObterPorTransacaoId(Guid transacaoId);
        Task TransferirProduto(TransferenciaProdutoDto dto);
        Task EntradaProduto(MovimentacaoEstoqueDto dto);
        Task ConsumoProduto(MovimentacaoEstoqueDto dto);
    }
}
