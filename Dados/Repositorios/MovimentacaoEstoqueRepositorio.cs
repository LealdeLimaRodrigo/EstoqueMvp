using Dapper;
using Dominio.Entidades;
using Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Dados.Repositorios
{
    /// <summary>
    /// Repositório de acesso a dados para registros de movimentação de estoque.
    /// A tabela é append-only (apenas inserções), preservando o histórico completo.
    /// </summary>
    public class MovimentacaoEstoqueRepositorio : IMovimentacaoEstoqueRepositorio
    {
        private readonly string _connectionString;

        public MovimentacaoEstoqueRepositorio(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> Adicionar(MovimentacaoEstoque movimentacaoEstoque)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO MovimentacaoEstoque (ProdutoId, SetorId, Quantidade, TipoMovimentacaoId, UsuarioId, TransacaoId)
                    VALUES (@ProdutoId, @SetorId, @Quantidade, @TipoMovimentacaoId, @UsuarioId, @TransacaoId);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                var result = await db.QueryAsync<int>(sql, movimentacaoEstoque);
                return result.Single();
            }
        }

        public async Task<IEnumerable<MovimentacaoEstoque>> ObterPorData(DateTime data)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, ProdutoId, SetorId, Quantidade, TipoMovimentacaoId, UsuarioId, TransacaoId, DataMovimentacao 
                    FROM MovimentacaoEstoque 
                    WHERE DataMovimentacao >= @DataInicio AND DataMovimentacao < @DataFim";
                return (await db.QueryAsync<MovimentacaoEstoque>(sql, new { DataInicio = data.Date, DataFim = data.Date.AddDays(1) })).ToList();
            }
        }

        public async Task<IEnumerable<MovimentacaoEstoque>> ObterPorProdutoId(int produtoId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, ProdutoId, SetorId, Quantidade, TipoMovimentacaoId, UsuarioId, TransacaoId, DataMovimentacao FROM MovimentacaoEstoque WHERE ProdutoId = @ProdutoId";
                return (await db.QueryAsync<MovimentacaoEstoque>(sql, new { ProdutoId = produtoId })).ToList();
            }
        }

        public async Task<IEnumerable<MovimentacaoEstoque>> ObterPorSetorId(int setorId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, ProdutoId, SetorId, Quantidade, TipoMovimentacaoId, UsuarioId, TransacaoId, DataMovimentacao FROM MovimentacaoEstoque WHERE SetorId = @SetorId";
                return (await db.QueryAsync<MovimentacaoEstoque>(sql, new { SetorId = setorId })).ToList();
            }
        }

        public async Task<IEnumerable<MovimentacaoEstoque>> ObterPorTipoMovimentacaoId(int tipoMovimentacaoId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, ProdutoId, SetorId, Quantidade, TipoMovimentacaoId, UsuarioId, TransacaoId, DataMovimentacao FROM MovimentacaoEstoque WHERE TipoMovimentacaoId = @TipoMovimentacaoId";
                return (await db.QueryAsync<MovimentacaoEstoque>(sql, new { TipoMovimentacaoId = tipoMovimentacaoId })).ToList();
            }
        }

        public async Task<IEnumerable<MovimentacaoEstoque>> ObterPorTransacaoId(Guid transacaoId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, ProdutoId, SetorId, Quantidade, TipoMovimentacaoId, UsuarioId, TransacaoId, DataMovimentacao FROM MovimentacaoEstoque WHERE TransacaoId = @TransacaoId";
                return (await db.QueryAsync<MovimentacaoEstoque>(sql, new { TransacaoId = transacaoId })).ToList();
            }
        }

        public async Task<IEnumerable<MovimentacaoEstoque>> ObterPorUsuarioId(int usuarioId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, ProdutoId, SetorId, Quantidade, TipoMovimentacaoId, UsuarioId, TransacaoId, DataMovimentacao FROM MovimentacaoEstoque WHERE UsuarioId = @UsuarioId";
                return (await db.QueryAsync<MovimentacaoEstoque>(sql, new { UsuarioId = usuarioId })).ToList();
            }
        }

        public async Task<IEnumerable<MovimentacaoEstoque>> ObterTodos()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, ProdutoId, SetorId, Quantidade, TipoMovimentacaoId, UsuarioId, TransacaoId, DataMovimentacao FROM MovimentacaoEstoque";
                return (await db.QueryAsync<MovimentacaoEstoque>(sql)).ToList();
            }
        }

    }
}
