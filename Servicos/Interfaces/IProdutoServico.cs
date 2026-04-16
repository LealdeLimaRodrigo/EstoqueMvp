using Servicos.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servicos.Interfaces
{
    /// <summary>
    /// Contrato de serviço para operações de negócio sobre Produto.
    /// Retorna DTOs para garantir a separação entre domínio e camada de apresentação.
    /// </summary>
    public interface IProdutoServico
    {
        Task<int> Adicionar(ProdutoCadastroDto dto);
        Task Atualizar(ProdutoAtualizacaoDto dto);
        Task<ProdutoRetornoDto> ObterPorId(int id);
        Task<ProdutoRetornoDto> ObterPorSku(string sku);
        Task<IEnumerable<ProdutoRetornoDto>> ObterPorNome(string nome);
        Task<IEnumerable<ProdutoRetornoDto>> ObterTodos();
        Task<PaginacaoResultadoDto<ProdutoRetornoDto>> ObterTodosPaginado(int pagina, int tamanhoPagina);
        Task<IEnumerable<ProdutoRetornoDto>> ObterTodosInativos();
        Task Remover(int id);
        Task Restaurar(int id);
    }
}
