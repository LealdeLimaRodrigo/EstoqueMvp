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

        /// <summary>
        /// Insere um novo setor e retorna o ID gerado.
        /// </summary>
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

        /// <summary>
        /// Atualiza os dados de um setor existente.
        /// </summary>
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

        /// <summary>
        /// Retorna um setor pelo ID.
        /// </summary>
        public async Task<Setor> ObterPorId(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Descricao, Ativo FROM Setor WHERE Id = @Id";
                var result = await db.QueryAsync<Setor>(sql, new { Id = id });
                return result.SingleOrDefault();
            }
        }

        /// <summary>
        /// Retorna setores ativos cujo nome contenha o termo informado.
        /// </summary>
        public async Task<IEnumerable<Setor>> ObterPorNome(string nome)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Descricao, Ativo FROM Setor WHERE Nome LIKE @Nome AND Ativo = 1";
                return (await db.QueryAsync<Setor>(sql, new { Nome = $"%{nome}%" })).ToList();
            }
        }

        /// <summary>
        /// Retorna todos os setores ativos.
        /// </summary>
        public async Task<IEnumerable<Setor>> ObterTodos()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Descricao, Ativo FROM Setor WHERE Ativo = 1";
                return (await db.QueryAsync<Setor>(sql)).ToList();
            }
        }

        /// <summary>
        /// Retorna setores ativos com paginação via OFFSET/FETCH.
        /// </summary>
        public async Task<IEnumerable<Setor>> ObterTodosPaginado(int offset, int tamanhoPagina)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Descricao, Ativo FROM Setor WHERE Ativo = 1
                    ORDER BY Id OFFSET @Offset ROWS FETCH NEXT @TamanhoPagina ROWS ONLY";
                return (await db.QueryAsync<Setor>(sql, new { Offset = offset, TamanhoPagina = tamanhoPagina })).ToList();
            }
        }

        /// <summary>
        /// Retorna a contagem total de setores ativos.
        /// </summary>
        public async Task<int> ContarTodosAtivos()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT COUNT(*) FROM Setor WHERE Ativo = 1";
                return await db.ExecuteScalarAsync<int>(sql);
            }
        }

        /// <summary>
        /// Retorna todos os setores inativos (soft deleted).
        /// </summary>
        public async Task<IEnumerable<Setor>> ObterTodosInativos()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Descricao, Ativo FROM Setor WHERE Ativo = 0";
                return (await db.QueryAsync<Setor>(sql)).ToList();
            }
        }

        /// <summary>
        /// Realiza o soft delete do setor, marcando como inativo.
        /// </summary>
        public async Task Remover(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Setor SET Ativo = 0 WHERE Id = @Id";
                await db.ExecuteAsync(sql, new { Id = id });
            }
        }

        /// <summary>
        /// Restaura um setor previamente inativado.
        /// </summary>
        public async Task Restaurar(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Setor SET Ativo = 1 WHERE Id = @Id";
                await db.ExecuteAsync(sql, new { Id = id });
            }

        }

        /// <summary>
        /// Busca setores ativos por nome com paginação.
        /// </summary>
        public async Task<IEnumerable<Setor>> BuscarPaginado(string termo, int offset, int tamanhoPagina)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Descricao, Ativo FROM Setor
                    WHERE Ativo = 1 AND Nome LIKE @Termo
                    ORDER BY Id OFFSET @Offset ROWS FETCH NEXT @TamanhoPagina ROWS ONLY";
                return (await db.QueryAsync<Setor>(sql, new { Termo = $"%{termo}%", Offset = offset, TamanhoPagina = tamanhoPagina })).ToList();
            }
        }

        /// <summary>
        /// Retorna a contagem de setores ativos que correspondem ao termo de busca.
        /// </summary>
        public async Task<int> ContarPorBusca(string termo)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT COUNT(*) FROM Setor WHERE Ativo = 1 AND Nome LIKE @Termo";
                return await db.ExecuteScalarAsync<int>(sql, new { Termo = $"%{termo}%" });
            }
        }
    }
}
