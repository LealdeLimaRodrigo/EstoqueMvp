using Dapper;
using Dominio.Entidades;
using Dominio.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Dados.Repositorios
{
    public class EstoqueSetorRepositorio : IEstoqueSetorRepositorio
    {
        private readonly string _connectionString;

        public EstoqueSetorRepositorio(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Adicionar(EstoqueSetor estoqueSetor)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO EstoqueSetor (ProdutoId, SetorId, Quantidade)
                    VALUES (@ProdutoId, @SetorId, @Quantidade);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                return db.Query<int>(sql, estoqueSetor).Single();
            }
        }

        public void Atualizar(EstoqueSetor estoqueSetor)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE EstoqueSetor
                    SET ProdutoId = @ProdutoId, SetorId = @SetorId, Quantidade = @Quantidade
                    WHERE Id = @Id";
                db.Execute(sql, estoqueSetor);
            }
        }

        public IEnumerable<EstoqueSetor> ObterPorProdutoId(int produtoId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, ProdutoId, SetorId, Quantidade FROM EstoqueSetor WHERE ProdutoId = @ProdutoId";
                return db.Query<EstoqueSetor>(sql, new { ProdutoId = produtoId }).ToList();
            }
        }

        public IEnumerable<EstoqueSetor> ObterPorSetorId(int setorId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, ProdutoId, SetorId, Quantidade FROM EstoqueSetor WHERE SetorId = @SetorId";
                return db.Query<EstoqueSetor>(sql, new { SetorId = setorId }).ToList();
            }
        }

        public IEnumerable<EstoqueSetor> ObterTodos()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, ProdutoId, SetorId, Quantidade FROM EstoqueSetor";
                return db.Query<EstoqueSetor>(sql).ToList();
            }
        }
    }
}
