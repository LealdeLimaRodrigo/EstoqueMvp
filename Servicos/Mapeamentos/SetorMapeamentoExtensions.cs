using Dominio.Entidades;
using Servicos.Dtos;

namespace Servicos.Mapeamentos
{
    /// <summary>
    /// Métodos de extensão para mapeamento entre a entidade Setor e seus DTOs.
    /// Centraliza conversões, removendo duplicação nos serviços.
    /// </summary>
    public static class SetorMapeamentoExtensions
    {
        public static SetorRetornoDto ToRetornoDto(this Setor setor)
        {
            if (setor == null) return null;

            return new SetorRetornoDto
            {
                Id = setor.Id,
                Nome = setor.Nome,
                Descricao = setor.Descricao,
                Ativo = setor.Ativo
            };
        }

        public static Setor ToEntity(this SetorCadastroDto dto)
        {
            return new Setor
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao
            };
        }

        public static void AplicarAtualizacao(this Setor setor, SetorAtualizacaoDto dto)
        {
            setor.Nome = dto.Nome;
            setor.Descricao = dto.Descricao;
        }
    }
}
