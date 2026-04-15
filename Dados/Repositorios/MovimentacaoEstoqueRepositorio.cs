using Dapper;
using Dominio.Entidades;
using Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Dados.Repositorios
{
    public class MovimentacaoEstoqueRepositorio : IMovimentacaoEstoqueRepositorio
    {
        private readonly string _connectionString;

        public MovimentacaoEstoqueRepositorio(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Adicionar(MovimentacaoEstoque movimentacaoEstoque)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO MovimentacaoEstoque (ProdutoId, SetorId, Quantidade, TipoMovimentacaoId, UsuarioId, TransacaoId, Data)
                    VALUES (@ProdutoId, @SetorId, @Quantidade, @TipoMovimentacaoId, @UsuarioId, @TransacaoId, @Data);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                return db.Query<int>(sql, movimentacaoEstoque).Single();
            }
        }

        public IEnumerable<MovimentacaoEstoque> ObterPorData(DateTime data)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, ProdutoId, SetorId, Quantidade, TipoMovimentacaoId, UsuarioId, TransacaoId, Data FROM MovimentacaoEstoque WHERE Data = @Data";
                return db.Query<MovimentacaoEstoque>(sql, new { Data = data }).ToList();
            }
        }

        public IEnumerable<MovimentacaoEstoque> ObterPorProdutoId(int produtoId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, ProdutoId, SetorId, Quantidade, TipoMovimentacaoId, UsuarioId, TransacaoId, Data FROM MovimentacaoEstoque WHERE ProdutoId = @ProdutoId";
                return db.Query<MovimentacaoEstoque>(sql, new { ProdutoId = produtoId }).ToList();
            }
        }

        public IEnumerable<MovimentacaoEstoque> ObterPorSetorId(int setorId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, ProdutoId, SetorId, Quantidade, TipoMovimentacaoId, UsuarioId, TransacaoId, Data FROM MovimentacaoEstoque WHERE SetorId = @SetorId";
                return db.Query<MovimentacaoEstoque>(sql, new { SetorId = setorId }).ToList();
            }
        }

        public IEnumerable<MovimentacaoEstoque> ObterPorTipoMovimentacaoId(int tipoMovimentacaoId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, ProdutoId, SetorId, Quantidade, TipoMovimentacaoId, UsuarioId, TransacaoId, Data FROM MovimentacaoEstoque WHERE TipoMovimentacaoId = @TipoMovimentacaoId";
                return db.Query<MovimentacaoEstoque>(sql, new { TipoMovimentacaoId = tipoMovimentacaoId }).ToList();
            }
        }

        public IEnumerable<MovimentacaoEstoque> ObterPorTransacaoId(Guid transacaoId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, ProdutoId, SetorId, Quantidade, TipoMovimentacaoId, UsuarioId, TransacaoId, Data FROM MovimentacaoEstoque WHERE TransacaoId = @TransacaoId";
                return db.Query<MovimentacaoEstoque>(sql, new { TransacaoId = transacaoId }).ToList();
            }
        }

        public IEnumerable<MovimentacaoEstoque> ObterPorUsuarioId(int usuarioId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, ProdutoId, SetorId, Quantidade, TipoMovimentacaoId, UsuarioId, TransacaoId, Data FROM MovimentacaoEstoque WHERE UsuarioId = @UsuarioId";
                return db.Query<MovimentacaoEstoque>(sql, new { UsuarioId = usuarioId }).ToList();
            }
        }

        public IEnumerable<MovimentacaoEstoque> ObterTodos()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, ProdutoId, SetorId, Quantidade, TipoMovimentacaoId, UsuarioId, TransacaoId, Data FROM MovimentacaoEstoque";
                return db.Query<MovimentacaoEstoque>(sql).ToList();
            }
        }

    }
}
