using Dominio.Entidades;
using Dominio.Interfaces;
using FluentValidation;
using Servicos.Dtos;
using Servicos.Exceptions;
using Servicos.Interfaces;
using Servicos.Mapeamentos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos
{
    /// <summary>
    /// Serviço responsável pelas regras de negócio de Produto.
    /// Gerencia o cadastro com geração automática de SKU, atualização e soft delete.
    /// </summary>
    public class ProdutoServico : IProdutoServico
    {
        private readonly IProdutoRepositorio _produtoRepositorio;
        private readonly IEstoqueSetorRepositorio _estoqueSetorRepositorio;

        private readonly IValidator<ProdutoCadastroDto> _cadastroValidator;
        private readonly IValidator<ProdutoAtualizacaoDto> _atualizacaoValidator;

        public ProdutoServico(IProdutoRepositorio produtoRepositorio, IEstoqueSetorRepositorio estoqueSetorRepositorio, IValidator<ProdutoCadastroDto> cadastroValidator, IValidator<ProdutoAtualizacaoDto> atualizacaoValidator)
        {
            _produtoRepositorio = produtoRepositorio;
            _estoqueSetorRepositorio = estoqueSetorRepositorio;
            _cadastroValidator = cadastroValidator;
            _atualizacaoValidator = atualizacaoValidator;
        }

        /// <summary>
        /// Cadastra um novo produto com SKU gerado automaticamente.
        /// O SKU é único e imutável após a criação.
        /// </summary>
        public async Task<int> Adicionar(ProdutoCadastroDto dto, bool ignorarInativos = false)
        {
            _cadastroValidator.ValidateAndThrow(dto);

            if (!ignorarInativos)
            {
                var inativos = await _produtoRepositorio.ObterTodosInativos();
                var matches = inativos.Where(p => p.Nome.Equals(dto.Nome, StringComparison.OrdinalIgnoreCase)).ToList();
                if (matches.Any())
                    throw new RegistroInativoException("Já existe um produto inativo com este nome.",
                        matches.Select(p => new RegistroInativoInfo { Id = p.Id, Nome = p.Nome, Sku = p.Sku, Descricao = p.Descricao, Preco = p.Preco }).ToList());
            }

            var produto = dto.ToEntity(GerarSkuUnico());

            return await _produtoRepositorio.Adicionar(produto);
        }

        /// <summary>
        /// Gera um código SKU único baseado em GUID.
        /// Formato: PRD-{12 caracteres hex} — garantia de unicidade sem consulta ao banco.
        /// </summary>
        private string GerarSkuUnico()
        {
            string hashCurto = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();            
            return $"PRD-{hashCurto}";
        }

        /// <summary>
        /// Atualiza os dados de um produto existente. O SKU não pode ser alterado.
        /// </summary>
        public async Task Atualizar(ProdutoAtualizacaoDto dto)
        {
            _atualizacaoValidator.ValidateAndThrow(dto);

            var produtoExistente = await _produtoRepositorio.ObterPorId(dto.Id);
            if (produtoExistente == null)
                throw new KeyNotFoundException("Produto não encontrado para atualização.");

            produtoExistente.AplicarAtualizacao(dto);

            await _produtoRepositorio.Atualizar(produtoExistente);
        }

        /// <summary>
        /// Realiza o soft delete do produto (marca como inativo).
        /// </summary>
        public async Task Remover(int id)
        {
            var produto = await _produtoRepositorio.ObterPorId(id);

            if (produto == null)
                throw new KeyNotFoundException("Produto não encontrado.");

            if (!produto.Ativo)
                throw new InvalidOperationException("Produto já está inativo.");

            await _produtoRepositorio.Remover(id);
        }

        /// <summary>
        /// Restaura um produto previamente inativado.
        /// </summary>
        public async Task Restaurar(int id)
        {
            var produto = await _produtoRepositorio.ObterPorId(id);

            if (produto == null)
                throw new KeyNotFoundException("Produto não encontrado.");

            if (produto.Ativo)
                throw new InvalidOperationException("Produto já está ativo.");

            await _produtoRepositorio.Restaurar(id);
        }

        /// <summary>
        /// Restaura um produto inativo e atualiza sua descrição e preço. Mantém o SKU original.
        /// </summary>
        public async Task RestaurarComDados(int id, ProdutoCadastroDto dto)
        {
            var produto = await _produtoRepositorio.ObterPorId(id);
            if (produto == null)
                throw new KeyNotFoundException("Produto não encontrado.");

            produto.Nome = dto.Nome;
            produto.Descricao = dto.Descricao;
            produto.Preco = dto.Preco;
            produto.Ativo = true;

            await _produtoRepositorio.Atualizar(produto);
        }

        /// <summary>
        /// Retorna um produto pelo ID
        /// </summary>
        public async Task<ProdutoRetornoDto> ObterPorId(int id)
        {
            var produto = await _produtoRepositorio.ObterPorId(id);
            return await MapearParaDto(produto);
        }

        /// <summary>
        /// Retorna um produto pelo código SKU.
        /// </summary>
        public async Task<ProdutoRetornoDto> ObterPorSku(string sku)
        {
            var produto = await _produtoRepositorio.ObterPorSku(sku);
            return await MapearParaDto(produto);
        }

        /// <summary>
        /// Retorna produtos cujo nome contenha o termo informado.
        /// </summary>
        public async Task<IEnumerable<ProdutoRetornoDto>> ObterPorNome(string nome)
        {
            var produtos = await _produtoRepositorio.ObterPorNome(nome);
            return await MapearListaParaDto(produtos);
        }

        /// <summary>
        /// Retorna todos os produtos ativos com a quantidade total em estoque.
        /// </summary>
        public async Task<IEnumerable<ProdutoRetornoDto>> ObterTodos()
        {
            var produtos = await _produtoRepositorio.ObterTodos();
            return await MapearListaParaDto(produtos);
        }

        /// <summary>
        /// Retorna produtos com paginação. Calcula o offset com base na página solicitada.
        /// </summary>
        public async Task<PaginacaoResultadoDto<ProdutoRetornoDto>> ObterTodosPaginado(int pagina, int tamanhoPagina)
        {
            int offset = (pagina - 1) * tamanhoPagina;
            var itens = await _produtoRepositorio.ObterTodosPaginado(offset, tamanhoPagina);
            var total = await _produtoRepositorio.ContarTodosAtivos();

            var listDto = await MapearListaParaDto(itens);

            return listDto.ToPaginacaoDto<Produto, ProdutoRetornoDto>(total, pagina, tamanhoPagina);
        }

        /// <summary>
        /// Busca produtos por termo com paginação server-side.
        /// </summary>
        public async Task<PaginacaoResultadoDto<ProdutoRetornoDto>> BuscarPaginado(string termo, int pagina, int tamanhoPagina)
        {
            int offset = (pagina - 1) * tamanhoPagina;
            var itens = await _produtoRepositorio.BuscarPaginado(termo, offset, tamanhoPagina);
            var total = await _produtoRepositorio.ContarPorBusca(termo);

            var listDto = await MapearListaParaDto(itens);

            return listDto.ToPaginacaoDto<Produto, ProdutoRetornoDto>(total, pagina, tamanhoPagina);
        }

        /// <summary>
        /// Retorna todos os produtos inativos (soft deleted).
        /// </summary>
        public async Task<IEnumerable<ProdutoRetornoDto>> ObterTodosInativos()
        {
            var produtos = await _produtoRepositorio.ObterTodosInativos();
            return await MapearListaParaDto(produtos);
        }

        /// <summary>
        /// Mapeia a entidade de domínio para o DTO de retorno, incluindo a quantidade total em estoque.
        /// A quantidade é calculada via SUM no banco (soma de todos os setores).
        /// </summary>
        private async Task<ProdutoRetornoDto> MapearParaDto(Produto produto)
        {
            if (produto == null) return null;
            var qtd = await _estoqueSetorRepositorio.ObterQuantidadeTotalPorProdutoId(produto.Id);
            return produto.ToRetornoDto(qtd);
        }

        /// <summary>
        /// Mapeia uma lista de entidades para DTOs de retorno.
        /// </summary>
        private async Task<IEnumerable<ProdutoRetornoDto>> MapearListaParaDto(IEnumerable<Produto> produtos)
        {
            var lista = new List<ProdutoRetornoDto>();
            foreach (var produto in produtos)
            {
                lista.Add(await MapearParaDto(produto));
            }
            return lista;
        }
    }
}

