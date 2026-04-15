using Dapper;
using Dominio.Entidades;
using Dominio.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Dados.Repositorios
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly string _connectionString;

        public UsuarioRepositorio(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Adicionar(Usuario usuario)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Usuario (Nome, Cpf, SenhaHash)
                    VALUES (@Nome, @Cpf, @SenhaHash);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                return db.Query<int>(sql, usuario).Single();
            }
        }

        public void Atualizar(Usuario usuario)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Usuario
                    SET Nome = @Nome, Cpf = @Cpf, SenhaHash = @SenhaHash, Ativo = @Ativo
                    WHERE Id = @Id";
                db.Execute(sql, usuario);
            }
        }

        public Usuario ObterPorId(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Cpf, SenhaHash, Ativo FROM Usuario WHERE Id = @Id";
                return db.Query<Usuario>(sql, new { Id = id }).SingleOrDefault();
            }
        }

        public Usuario ObterPorCpf(string cpf)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Cpf, SenhaHash, Ativo FROM Usuario WHERE Cpf = @Cpf AND Ativo = 1";
                return db.Query<Usuario>(sql, new { Cpf = cpf }).SingleOrDefault();
            }
        }

        public IEnumerable<Usuario> ObterTodos()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Cpf, SenhaHash, Ativo FROM Usuario WHERE Ativo = 1";
                return db.Query<Usuario>(sql).ToList();
            }
        }
        
        public IEnumerable<Usuario> ObterTodosInativos()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Cpf, SenhaHash, Ativo FROM Usuario WHERE Ativo = 0";
                return db.Query<Usuario>(sql).ToList();
            }
        }

        public void Remover(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Usuario SET Ativo = 0 WHERE Id = @Id";
                db.Execute(sql, new { Id = id });
            }
        }

        public void Restaurar(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Usuario SET Ativo = 1 WHERE Id = @Id";
                db.Execute(sql, new { Id = id });
            }
        }

    }
}
