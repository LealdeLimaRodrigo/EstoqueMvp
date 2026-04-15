using Dapper;
using Dominio.Entidades;
using Dominio.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Dados.Repositorios
{
    public class ProdutoRepositorio : IProdutoRepositorio
    {
        private readonly string _connectionString;

        public ProdutoRepositorio(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Adicionar(Produto produto)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Produto (Sku, Nome, Descricao, Preco)
                    VALUES (@Sku, @Nome, @Descricao, @Preco);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                return db.Query<int>(sql, produto).Single();
            }
        }

        public void Atualizar(Produto produto)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Produto
                    SET Sku = @Sku, Nome = @Nome, Descricao = @Descricao, Preco = @Preco, Ativo = @Ativo
                    WHERE Id = @Id";
                db.Execute(sql, produto);
            }
        }

        public Produto ObterPorId(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Sku, Nome, Descricao, Preco, Ativo FROM Produto WHERE Id = @Id";
                return db.Query<Produto>(sql, new { Id = id }).SingleOrDefault();
            }
        }

        public Produto ObterPorSku(string sku)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Sku, Nome, Descricao, Preco, Ativo FROM Produto WHERE Sku = @Sku AND Ativo = 1";
                return db.Query<Produto>(sql, new { Sku = sku }).SingleOrDefault();
            }
        }

        public IEnumerable<Produto> ObterTodos()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Sku, Nome, Descricao, Preco, Ativo FROM Produto WHERE Ativo = 1";
                return db.Query<Produto>(sql).ToList();
            }
        }

        public IEnumerable<Produto> ObterTodosInativos()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Sku, Nome, Descricao, Preco, Ativo FROM Produto WHERE Ativo = 0";
                return db.Query<Produto>(sql).ToList();
            }
        }

        public void Remover(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Produto SET Ativo = 0 WHERE Id = @Id";
                db.Execute(sql, new { Id = id });
            }
        }

        public void Restaurar(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Produto SET Ativo = 1 WHERE Id = @Id";
                db.Execute(sql, new { Id = id });
            }
        }
    }
}
