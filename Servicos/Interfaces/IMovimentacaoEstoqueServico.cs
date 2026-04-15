using Dominio.Entidades;
using Servicos.Dtos;
using System;
using System.Collections.Generic;

namespace Servicos.Interfaces
{
    public interface IMovimentacaoEstoqueServico
    {
        IEnumerable<MovimentacaoEstoque> ObterTodos();
        IEnumerable<MovimentacaoEstoque> ObterPorSetorId(int setorId);
        IEnumerable<MovimentacaoEstoque> ObterPorProdutoId(int produtoId);
        IEnumerable<MovimentacaoEstoque> ObterPorUsuarioId(int usuarioId);
        IEnumerable<MovimentacaoEstoque> ObterPorTipoMovimentacaoId(int tipoMovimentacaoId);
        IEnumerable<MovimentacaoEstoque> ObterPorData(DateTime data);
        IEnumerable<MovimentacaoEstoque> ObterPorTransacaoId(Guid transacaoId);
        void TransferirProduto(TransferenciaProdutoDto dto);
        void EntradaProduto(ProdutoDto dto);
        void ConsumoProduto(ProdutoDto dto);
    }
}
