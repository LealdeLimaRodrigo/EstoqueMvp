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
    /// Repositório de consulta dos tipos de movimentação.
    /// Dados fixos gerenciados via seed SQL.
    /// </summary>
    public class TipoMovimentacaoRepositorio : ITipoMovimentacaoRepositorio   
    {
        private readonly string _connectionString;

        public TipoMovimentacaoRepositorio(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Retorna um tipo de movimentação pelo ID.
        /// </summary>
        public async Task<TipoMovimentacao> ObterPorId(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Descricao FROM TipoMovimentacao WHERE Id = @Id";
                var result = await db.QueryAsync<TipoMovimentacao>(sql, new { Id = id });
                return result.SingleOrDefault();
            }
        }

        /// <summary>
        /// Retorna todos os tipos de movimentação cadastrados.
        /// </summary>
        public async Task<IEnumerable<TipoMovimentacao>> ObterTodos()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT Id, Nome, Descricao FROM TipoMovimentacao";
                return (await db.QueryAsync<TipoMovimentacao>(sql)).ToList();
            }
        }
    }
}
