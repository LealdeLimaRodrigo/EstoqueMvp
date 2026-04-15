using Dominio.Entidades;
using System;
using System.Collections.Generic;

namespace Dominio.Interfaces
{
    public interface IMovimentacaoEstoqueRepositorio
    {
        IEnumerable<MovimentacaoEstoque> ObterTodos();
        IEnumerable<MovimentacaoEstoque> ObterPorSetorId(int setorId);
        IEnumerable<MovimentacaoEstoque> ObterPorProdutoId(int produtoId);
        IEnumerable<MovimentacaoEstoque> ObterPorUsuarioId(int usuarioId);
        IEnumerable<MovimentacaoEstoque> ObterPorTipoMovimentacaoId(int tipoMovimentacaoId);
        IEnumerable<MovimentacaoEstoque> ObterPorData(DateTime data);
        IEnumerable<MovimentacaoEstoque> ObterPorTransacaoId(Guid transacaoId);
        int Adicionar(MovimentacaoEstoque movimentacaoEstoque);
    }
}
