using Dapper;
using Dominio.Entidades;
using Dominio.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Dados.Repositorios
{
    public class SetorRepositorio : ISetorRepositorio
    {
        private readonly string _connectionString;

        public SetorRepositorio(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Adicionar(Setor setor)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Setor (Nome, Descricao)
                    VALUES (@Nome, @Descricao);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                return db.Query<int>(sql, setor).Single();
            }
        }

        public void Atualizar(Setor setor)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Setor
                    SET Nome = @Nome, Descricao = @Descricao, Ativo = @Ativo
                    WHERE Id = @Id";
                db.Execute(sql, setor);
            }
        }

        public Setor ObterPorId(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Descricao, Ativo FROM Setor WHERE Id = @Id";
                return db.Query<Setor>(sql, new { Id = id }).SingleOrDefault();
            }
        }

        public Setor ObterPorNome(string nome)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Descricao, Ativo FROM Setor WHERE Nome = @Nome AND Ativo = 1";
                return db.Query<Setor>(sql, new { Nome = nome }).SingleOrDefault();
            }
        }

        public IEnumerable<Setor> ObterTodos()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Descricao, Ativo FROM Setor WHERE Ativo = 1";
                return db.Query<Setor>(sql).ToList();
            }
        }

        public IEnumerable<Setor> ObterTodosInativos()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Descricao, Ativo FROM Setor WHERE Ativo = 0";
                return db.Query<Setor>(sql).ToList();
            }
        }

        public void Remover(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Setor SET Ativo = 0 WHERE Id = @Id";
                db.Execute(sql, new { Id = id });
            }
        }

        public void Restaurar(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Setor SET Ativo = 1 WHERE Id = @Id";
                db.Execute(sql, new { Id = id });
            }

        }
    }
}
