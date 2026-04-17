using Servicos.Dtos;
using System.Collections.Generic;

namespace Servicos.Mapeamentos
{
    /// <summary>
    /// Extensões para conversão entre coleções genéricas e o formato paginado (PaginacaoResultadoDto).
    /// Isola o cálculo das estruturas de paginação.
    /// </summary>
    public static class PaginacaoMapeamentoExtensions
    {
        /// <summary>
        /// Converte uma coleção de DTOs em um resultado paginado com metadados.
        /// </summary>
        public static PaginacaoResultadoDto<TDto> ToPaginacaoDto<TEntity, TDto>(
            this IEnumerable<TDto> itens, 
            int totalRegistros, 
            int pagina, 
            int tamanhoPagina)
        {
            return new PaginacaoResultadoDto<TDto>
            {
                Itens = itens,
                TotalRegistros = totalRegistros,
                Pagina = pagina,
                TamanhoPagina = tamanhoPagina
            };
        }
    }
}
