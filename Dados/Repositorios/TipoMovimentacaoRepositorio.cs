using Dapper;
using Dominio.Entidades;
using Dominio.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Dados.Repositorios
{
    public class TipoMovimentacaoRepositorio : ITipoMovimentacaoRepositorio   
    {
        private readonly string _connectionString;

        public TipoMovimentacaoRepositorio(string connectionString)
        {
            _connectionString = connectionString;
        }

        public TipoMovimentacao ObterPorId(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Descricao FROM TipoMovimentacao WHERE Id = @Id";
                return db.Query<TipoMovimentacao>(sql, new { Id = id }).SingleOrDefault();
            }
        }

        public IEnumerable<TipoMovimentacao> ObterTodos()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Descricao FROM TipoMovimentacao";
                return db.Query<TipoMovimentacao>(sql).ToList();
            }
        }
    }
}
