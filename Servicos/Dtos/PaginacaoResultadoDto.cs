using System.Collections.Generic;

namespace Servicos.Dtos
{
    /// <summary>
    /// DTO genérico para retorno de dados paginados.
    /// Encapsula os itens da página, contagem total e metadados de paginação.
    /// </summary>
    public class PaginacaoResultadoDto<T>
    {
        public IEnumerable<T> Itens { get; set; }
        public int TotalRegistros { get; set; }
        public int Pagina { get; set; }
        public int TamanhoPagina { get; set; }
        public int TotalPaginas => TamanhoPagina > 0 ? (int)System.Math.Ceiling((double)TotalRegistros / TamanhoPagina) : 0;
    }
}
