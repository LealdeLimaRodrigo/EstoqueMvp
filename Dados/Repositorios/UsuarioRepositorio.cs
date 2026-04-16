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
    /// Repositório de acesso a dados para a entidade Usuario.
    /// Queries de listagem omitem SenhaHash por segurança.
    /// Apenas ObterPorCpf retorna o hash, pois é utilizado exclusivamente no fluxo de login.
    /// </summary>
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly string _connectionString;

        public UsuarioRepositorio(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> Adicionar(Usuario usuario)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Usuario (Nome, Cpf, SenhaHash)
                    VALUES (@Nome, @Cpf, @SenhaHash);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                var result = await db.QueryAsync<int>(sql, usuario);
                return result.Single();
            }
        }

        public async Task Atualizar(Usuario usuario)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Usuario
                    SET Nome = @Nome, Cpf = @Cpf, SenhaHash = @SenhaHash, Ativo = @Ativo
                    WHERE Id = @Id";
                await db.ExecuteAsync(sql, usuario);
            }
        }

        public async Task<Usuario> ObterPorId(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                // Não retorna SenhaHash em consultas fora do fluxo de autenticação
                string sql = @"SELECT Id, Nome, Cpf, Ativo FROM Usuario WHERE Id = @Id";
                var result = await db.QueryAsync<Usuario>(sql, new { Id = id });
                return result.SingleOrDefault();
            }
        }

        public async Task<Usuario> ObterPorCpf(string cpf)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                // Não retorna SenhaHash — seguro para consultas públicas
                string sql = @"SELECT Id, Nome, Cpf, Ativo FROM Usuario WHERE Cpf = @Cpf";
                var result = await db.QueryAsync<Usuario>(sql, new { Cpf = cpf });
                return result.SingleOrDefault();
            }
        }

        /// <summary>
        /// Busca o usuário pelo CPF incluindo o SenhaHash.
        /// Utilizado exclusivamente no fluxo de login para verificação de credenciais.
        /// </summary>
        public async Task<Usuario> ObterPorCpfComSenha(string cpf)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Cpf, SenhaHash, Ativo FROM Usuario WHERE Cpf = @Cpf";
                var result = await db.QueryAsync<Usuario>(sql, new { Cpf = cpf });
                return result.SingleOrDefault();
            }
        }

        /// <summary>
        /// Busca o usuário pelo ID incluindo o SenhaHash.
        /// Utilizado exclusivamente no fluxo de atualização para preservar a senha existente.
        /// </summary>
        public async Task<Usuario> ObterPorIdComSenha(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Cpf, SenhaHash, Ativo FROM Usuario WHERE Id = @Id";
                var result = await db.QueryAsync<Usuario>(sql, new { Id = id });
                return result.SingleOrDefault();
            }
        }

        public async Task<IEnumerable<Usuario>> ObterPorNome(string nome)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Cpf, Ativo FROM Usuario WHERE Nome LIKE @Nome AND Ativo = 1";
                return (await db.QueryAsync<Usuario>(sql, new { Nome = $"%{nome}%" })).ToList();
            }
        }

        public async Task<IEnumerable<Usuario>> ObterTodos()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Cpf, Ativo FROM Usuario WHERE Ativo = 1";
                return (await db.QueryAsync<Usuario>(sql)).ToList();
            }
        }

        public async Task<IEnumerable<Usuario>> ObterTodosPaginado(int offset, int tamanhoPagina)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Cpf, Ativo FROM Usuario WHERE Ativo = 1
                    ORDER BY Id OFFSET @Offset ROWS FETCH NEXT @TamanhoPagina ROWS ONLY";
                return (await db.QueryAsync<Usuario>(sql, new { Offset = offset, TamanhoPagina = tamanhoPagina })).ToList();
            }
        }

        public async Task<int> ContarTodosAtivos()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT COUNT(*) FROM Usuario WHERE Ativo = 1";
                return await db.ExecuteScalarAsync<int>(sql);
            }
        }

        public async Task<IEnumerable<Usuario>> ObterTodosInativos()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Cpf, Ativo FROM Usuario WHERE Ativo = 0";
                return (await db.QueryAsync<Usuario>(sql)).ToList();
            }
        }

        public async Task Remover(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Usuario SET Ativo = 0 WHERE Id = @Id";
                await db.ExecuteAsync(sql, new { Id = id });
            }
        }

        public async Task Restaurar(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Usuario SET Ativo = 1 WHERE Id = @Id";
                await db.ExecuteAsync(sql, new { Id = id });
            }
        }

    }
}
