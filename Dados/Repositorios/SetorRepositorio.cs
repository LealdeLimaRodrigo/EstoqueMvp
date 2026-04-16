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
    /// Repositório de acesso a dados para a entidade Setor.
    /// Suporta soft delete e paginação server-side.
    /// </summary>
    public class SetorRepositorio : ISetorRepositorio
    {
        private readonly string _connectionString;

        public SetorRepositorio(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> Adicionar(Setor setor)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Setor (Nome, Descricao)
                    VALUES (@Nome, @Descricao);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                var result = await db.QueryAsync<int>(sql, setor);
                return result.Single();
            }
        }

        public async Task Atualizar(Setor setor)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Setor
                    SET Nome = @Nome, Descricao = @Descricao, Ativo = @Ativo
                    WHERE Id = @Id";
                await db.ExecuteAsync(sql, setor);
            }
        }

        public async Task<Setor> ObterPorId(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Descricao, Ativo FROM Setor WHERE Id = @Id";
                var result = await db.QueryAsync<Setor>(sql, new { Id = id });
                return result.SingleOrDefault();
            }
        }

        public async Task<IEnumerable<Setor>> ObterPorNome(string nome)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Descricao, Ativo FROM Setor WHERE Nome LIKE @Nome AND Ativo = 1";
                return (await db.QueryAsync<Setor>(sql, new { Nome = $"%{nome}%" })).ToList();
            }
        }

        public async Task<IEnumerable<Setor>> ObterTodos()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Descricao, Ativo FROM Setor WHERE Ativo = 1";
                return (await db.QueryAsync<Setor>(sql)).ToList();
            }
        }

        public async Task<IEnumerable<Setor>> ObterTodosPaginado(int offset, int tamanhoPagina)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Descricao, Ativo FROM Setor WHERE Ativo = 1
                    ORDER BY Id OFFSET @Offset ROWS FETCH NEXT @TamanhoPagina ROWS ONLY";
                return (await db.QueryAsync<Setor>(sql, new { Offset = offset, TamanhoPagina = tamanhoPagina })).ToList();
            }
        }

        public async Task<int> ContarTodosAtivos()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT COUNT(*) FROM Setor WHERE Ativo = 1";
                return await db.ExecuteScalarAsync<int>(sql);
            }
        }

        public async Task<IEnumerable<Setor>> ObterTodosInativos()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Descricao, Ativo FROM Setor WHERE Ativo = 0";
                return (await db.QueryAsync<Setor>(sql)).ToList();
            }
        }

        public async Task Remover(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Setor SET Ativo = 0 WHERE Id = @Id";
                await db.ExecuteAsync(sql, new { Id = id });
            }
        }

        public async Task Restaurar(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Setor SET Ativo = 1 WHERE Id = @Id";
                await db.ExecuteAsync(sql, new { Id = id });
            }

        }
    }
}
