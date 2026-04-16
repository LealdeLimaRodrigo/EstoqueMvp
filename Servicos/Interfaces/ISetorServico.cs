using Servicos.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servicos.Interfaces
{
    /// <summary>
    /// Contrato de serviço para operações de negócio sobre Setor.
    /// Retorna DTOs para garantir a separação entre domínio e camada de apresentação.
    /// </summary>
    public interface ISetorServico
    {
        Task<IEnumerable<SetorRetornoDto>> ObterTodos();
        Task<PaginacaoResultadoDto<SetorRetornoDto>> ObterTodosPaginado(int pagina, int tamanhoPagina);
        Task<IEnumerable<SetorRetornoDto>> ObterTodosInativos();
        Task<SetorRetornoDto> ObterPorId(int id);
        Task<IEnumerable<SetorRetornoDto>> ObterPorNome(string nome);
        Task<int> Adicionar(SetorCadastroDto dto);
        Task Atualizar(SetorAtualizacaoDto dto);
        Task Remover(int id);
        Task Restaurar(int id);
    }
}
