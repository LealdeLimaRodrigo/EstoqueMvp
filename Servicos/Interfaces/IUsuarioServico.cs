using Servicos.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servicos.Interfaces
{
    /// <summary>
    /// Contrato de serviço para operações de negócio sobre Usuário, incluindo autenticação.
    /// Retorna DTOs para garantir a separação entre domínio e camada de apresentação.
    /// </summary>
    public interface IUsuarioServico
    {
        Task<IEnumerable<UsuarioRetornoDto>> ObterTodos();
        Task<PaginacaoResultadoDto<UsuarioRetornoDto>> ObterTodosPaginado(int pagina, int tamanhoPagina);
        Task<IEnumerable<UsuarioRetornoDto>> ObterTodosInativos();
        Task<UsuarioRetornoDto> ObterPorId(int id);
        Task<UsuarioRetornoDto> ObterPorCpf(string cpf);
        Task<IEnumerable<UsuarioRetornoDto>> ObterPorNome(string nome);
        Task<int> Adicionar(UsuarioCadastroDto dto);
        Task Atualizar(UsuarioAtualizacaoDto dto);
        Task Remover(int id);
        Task Restaurar(int id);

        Task<UsuarioRetornoDto> RealizarLogin(LoginDto loginDto);
    }
}
