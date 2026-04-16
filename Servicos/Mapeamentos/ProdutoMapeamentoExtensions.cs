using Dominio.Entidades;
using Servicos.Dtos;

namespace Servicos.Mapeamentos
{
    /// <summary>
    /// Métodos de extensão para mapeamento entre a entidade Produto e seus DTOs.
    /// Centraliza a conversão em um único local, mantendo os serviços limpos.
    /// </summary>
    public static class ProdutoMapeamentoExtensions
    {
        /// <summary>
        /// Converte a entidade Produto para o DTO de retorno, incluindo a quantidade total em estoque.
        /// </summary>
        public static ProdutoRetornoDto ToRetornoDto(this Produto produto, decimal quantidadeTotalEstoque)
        {
            if (produto == null) return null;

            return new ProdutoRetornoDto
            {
                Id = produto.Id,
                Sku = produto.Sku,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Preco = produto.Preco,
                Ativo = produto.Ativo,
                QuantidadeTotalEstoque = quantidadeTotalEstoque
            };
        }

        /// <summary>
        /// Cria uma nova entidade Produto a partir do DTO de cadastro.
        /// O SKU deve ser atribuído externamente (geração automática no serviço).
        /// </summary>
        public static Produto ToEntity(this ProdutoCadastroDto dto, string sku)
        {
            return new Produto
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                Preco = dto.Preco,
                Sku = sku
            };
        }

        /// <summary>
        /// Aplica as alterações do DTO de atualização na entidade existente.
        /// O SKU e o Id não são alterados.
        /// </summary>
        public static void AplicarAtualizacao(this Produto produto, ProdutoAtualizacaoDto dto)
        {
            produto.Nome = dto.Nome;
            produto.Descricao = dto.Descricao;
            produto.Preco = dto.Preco;
        }
    }
}
