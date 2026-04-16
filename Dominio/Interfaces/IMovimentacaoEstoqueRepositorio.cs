using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dominio.Interfaces
{
    /// <summary>
    /// Contrato de acesso a dados para registros de movimentação de estoque.
    /// Suporta apenas inserção e consultas (tabela append-only).
    /// </summary>
    public interface IMovimentacaoEstoqueRepositorio
    {
        Task<IEnumerable<MovimentacaoEstoque>> ObterTodos();
        Task<IEnumerable<MovimentacaoEstoque>> ObterPorSetorId(int setorId);
        Task<IEnumerable<MovimentacaoEstoque>> ObterPorProdutoId(int produtoId);
        Task<IEnumerable<MovimentacaoEstoque>> ObterPorUsuarioId(int usuarioId);
        Task<IEnumerable<MovimentacaoEstoque>> ObterPorTipoMovimentacaoId(int tipoMovimentacaoId);
        Task<IEnumerable<MovimentacaoEstoque>> ObterPorData(DateTime data);
        Task<IEnumerable<MovimentacaoEstoque>> ObterPorTransacaoId(Guid transacaoId);
        Task<int> Adicionar(MovimentacaoEstoque movimentacaoEstoque);
    }
}
