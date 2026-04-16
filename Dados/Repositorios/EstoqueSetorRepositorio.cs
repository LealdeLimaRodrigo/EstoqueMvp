using Dapper;
using Dominio.Entidades;
using Dominio.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Dados.Repositorios
{
    /// <summary>
    /// Repositório de acesso a dados para o saldo de estoque por setor.
    /// A chave composta (SetorId, ProdutoId) garante um registro por combinação.
    /// </summary>
    public class EstoqueSetorRepositorio : IEstoqueSetorRepositorio
    {
        private readonly string _connectionString;

        public EstoqueSetorRepositorio(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> Adicionar(EstoqueSetor estoqueSetor)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO EstoqueSetor (ProdutoId, SetorId, QuantidadeEstoque)
                    VALUES (@ProdutoId, @SetorId, @QuantidadeEstoque);
                    SELECT @ProdutoId";
                var result = await db.QueryAsync<int>(sql, estoqueSetor);
                return result.Single();
            }
        }

        public async Task Atualizar(EstoqueSetor estoqueSetor)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE EstoqueSetor
                    SET QuantidadeEstoque = @QuantidadeEstoque
                    WHERE ProdutoId = @ProdutoId AND SetorId = @SetorId";
                await db.ExecuteAsync(sql, estoqueSetor);
            }
        }

        public async Task<IEnumerable<EstoqueSetor>> ObterPorProdutoId(int produtoId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT ProdutoId, SetorId, QuantidadeEstoque FROM EstoqueSetor WHERE ProdutoId = @ProdutoId";
                return (await db.QueryAsync<EstoqueSetor>(sql, new { ProdutoId = produtoId })).ToList();
            }
        }

        public async Task<EstoqueSetor> ObterPorProdutoIdESetorId(int produtoId, int setorId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT ProdutoId, SetorId, QuantidadeEstoque FROM EstoqueSetor WHERE ProdutoId = @ProdutoId AND SetorId = @SetorId";
                var result = await db.QueryAsync<EstoqueSetor>(sql, new { ProdutoId = produtoId, SetorId = setorId });
                return result.FirstOrDefault();
            }
        }

        public async Task<IEnumerable<EstoqueSetor>> ObterPorSetorId(int setorId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT ProdutoId, SetorId, QuantidadeEstoque FROM EstoqueSetor WHERE SetorId = @SetorId";
                return (await db.QueryAsync<EstoqueSetor>(sql, new { SetorId = setorId })).ToList();
            }
        }

        public async Task<IEnumerable<EstoqueSetor>> ObterTodos()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT ProdutoId, SetorId, QuantidadeEstoque FROM EstoqueSetor";
                return (await db.QueryAsync<EstoqueSetor>(sql)).ToList();
            }
        }

        /// <summary>
        /// Retorna a soma total de estoque de um produto em todos os setores.
        /// Utiliza SUM no banco para evitar trazer múltiplos registros para a aplicação.
        /// </summary>
        public async Task<decimal> ObterQuantidadeTotalPorProdutoId(int produtoId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT ISNULL(SUM(QuantidadeEstoque), 0) FROM EstoqueSetor WHERE ProdutoId = @ProdutoId";
                return await db.ExecuteScalarAsync<decimal>(sql, new { ProdutoId = produtoId });
            }
        }
    }
}
