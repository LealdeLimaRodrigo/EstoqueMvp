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
    /// Repositório de acesso a dados para a entidade Produto.
    /// Utiliza Dapper para queries parametrizadas, prevenindo SQL Injection.
    /// O soft delete é implementado pelo campo Ativo.
    /// </summary>
    public class ProdutoRepositorio : IProdutoRepositorio
    {
        private readonly string _connectionString;

        public ProdutoRepositorio(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Insere um novo produto e retorna o ID gerado.
        /// </summary>
        public async Task<int> Adicionar(Produto produto)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Produto (Sku, Nome, Descricao, Preco)
                    VALUES (@Sku, @Nome, @Descricao, @Preco);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                var result = await db.QueryAsync<int>(sql, produto);
                return result.Single();
            }
        }

        /// <summary>
        /// Atualiza os dados de um produto existente.
        /// </summary>
        public async Task Atualizar(Produto produto)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Produto
                    SET Nome = @Nome, Descricao = @Descricao, Preco = @Preco, Ativo = @Ativo
                    WHERE Id = @Id";
                await db.ExecuteAsync(sql, produto);
            }
        }

        /// <summary>
        /// Retorna um produto pelo ID, independente do status ativo.
        /// </summary>
        public async Task<Produto> ObterPorId(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Sku, Nome, Descricao, Preco, Ativo FROM Produto WHERE Id = @Id";
                var result = await db.QueryAsync<Produto>(sql, new { Id = id });
                return result.SingleOrDefault();
            }
        }

        /// <summary>
        /// Retorna um produto ativo pelo código SKU.
        /// </summary>
        public async Task<Produto> ObterPorSku(string sku)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Sku, Nome, Descricao, Preco, Ativo FROM Produto WHERE Sku = @Sku AND Ativo = 1";
                var result = await db.QueryAsync<Produto>(sql, new { Sku = sku });
                return result.SingleOrDefault();
            }
        }

        /// <summary>
        /// Retorna produtos ativos cujo nome contenha o termo informado.
        /// </summary>
        public async Task<IEnumerable<Produto>> ObterPorNome(string nome)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Sku, Nome, Descricao, Preco, Ativo FROM Produto WHERE Nome LIKE @Nome AND Ativo = 1";
                return (await db.QueryAsync<Produto>(sql, new { Nome = $"%{nome}%" })).ToList();
            }
        }

        /// <summary>
        /// Retorna todos os produtos ativos.
        /// </summary>
        public async Task<IEnumerable<Produto>> ObterTodos()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Sku, Nome, Descricao, Preco, Ativo FROM Produto WHERE Ativo = 1";
                return (await db.QueryAsync<Produto>(sql)).ToList();
            }
        }

        /// <summary>
        /// Retorna produtos ativos com paginação via OFFSET/FETCH.
        /// </summary>
        public async Task<IEnumerable<Produto>> ObterTodosPaginado(int offset, int tamanhoPagina)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Sku, Nome, Descricao, Preco, Ativo FROM Produto WHERE Ativo = 1
                    ORDER BY Id OFFSET @Offset ROWS FETCH NEXT @TamanhoPagina ROWS ONLY";
                return (await db.QueryAsync<Produto>(sql, new { Offset = offset, TamanhoPagina = tamanhoPagina })).ToList();
            }
        }

        /// <summary>
        /// Retorna a contagem total de produtos ativos.
        /// </summary>
        public async Task<int> ContarTodosAtivos()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT COUNT(*) FROM Produto WHERE Ativo = 1";
                return await db.ExecuteScalarAsync<int>(sql);
            }
        }

        /// <summary>
        /// Retorna todos os produtos inativos (soft deleted).
        /// </summary>
        public async Task<IEnumerable<Produto>> ObterTodosInativos()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Sku, Nome, Descricao, Preco, Ativo FROM Produto WHERE Ativo = 0";
                return (await db.QueryAsync<Produto>(sql)).ToList();
            }
        }

        /// <summary>
        /// Realiza o soft delete do produto, marcando como inativo.
        /// </summary>
        public async Task Remover(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Produto SET Ativo = 0 WHERE Id = @Id";
                await db.ExecuteAsync(sql, new { Id = id });
            }
        }

        /// <summary>
        /// Restaura um produto previamente inativado.
        /// </summary>
        public async Task Restaurar(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Produto SET Ativo = 1 WHERE Id = @Id";
                await db.ExecuteAsync(sql, new { Id = id });
            }
        }

        /// <summary>
        /// Busca produtos ativos por nome ou SKU com paginação.
        /// </summary>
        public async Task<IEnumerable<Produto>> BuscarPaginado(string termo, int offset, int tamanhoPagina)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Sku, Nome, Descricao, Preco, Ativo FROM Produto
                    WHERE Ativo = 1 AND (Nome LIKE @Termo OR Sku LIKE @Termo)
                    ORDER BY Id OFFSET @Offset ROWS FETCH NEXT @TamanhoPagina ROWS ONLY";
                return (await db.QueryAsync<Produto>(sql, new { Termo = $"%{termo}%", Offset = offset, TamanhoPagina = tamanhoPagina })).ToList();
            }
        }

        /// <summary>
        /// Retorna a contagem de produtos ativos que correspondem ao termo de busca.
        /// </summary>
        public async Task<int> ContarPorBusca(string termo)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT COUNT(*) FROM Produto WHERE Ativo = 1 AND (Nome LIKE @Termo OR Sku LIKE @Termo)";
                return await db.ExecuteScalarAsync<int>(sql, new { Termo = $"%{termo}%" });
            }
        }
    }
}
