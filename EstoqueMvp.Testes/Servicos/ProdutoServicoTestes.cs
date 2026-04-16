using Dominio.Entidades;
using Dominio.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Servicos;
using Servicos.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EstoqueMvp.Testes.Servicos
{
    /// <summary>
    /// Testes unitários para o ProdutoServico.
    /// Utiliza Moq para simular o repositório e isolar a lógica de negócio.
    /// </summary>
    [TestClass]
    public class ProdutoServicoTestes
    {
        private Mock<IProdutoRepositorio> _mockRepo;
        private Mock<IEstoqueSetorRepositorio> _mockEstoqueRepo;
        private ProdutoServico _servico;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IProdutoRepositorio>();
            _mockEstoqueRepo = new Mock<IEstoqueSetorRepositorio>();
            _mockEstoqueRepo.Setup(r => r.ObterQuantidadeTotalPorProdutoId(It.IsAny<int>())).ReturnsAsync(0);
            _servico = new ProdutoServico(_mockRepo.Object, _mockEstoqueRepo.Object, new global::Servicos.Validacoes.ProdutoCadastroDtoValidator(), new global::Servicos.Validacoes.ProdutoAtualizacaoDtoValidator());
        }

        [TestMethod]
        public async Task Adicionar_DadosValidos_DeveRetornarId()
        {
            _mockRepo.Setup(r => r.Adicionar(It.IsAny<Produto>())).ReturnsAsync(1);

            var dto = new ProdutoCadastroDto { Nome = "Mouse", Descricao = "Mouse USB", Preco = 29.90m };
            int id = await _servico.Adicionar(dto);

            Assert.AreEqual(1, id);
            _mockRepo.Verify(r => r.Adicionar(It.Is<Produto>(p => p.Nome == "Mouse" && p.Sku.StartsWith("PRD-"))), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(FluentValidation.ValidationException))]
        public async Task Adicionar_PrecoZero_DeveLancarExcecao()
        {
            var dto = new ProdutoCadastroDto { Nome = "Mouse", Preco = 0 };
            await _servico.Adicionar(dto);
        }

        [TestMethod]
        public async Task Atualizar_ProdutoExistente_DeveAtualizarComSucesso()
        {
            var produtoExistente = new Produto { Id = 1, Sku = "PRD-AAA", Nome = "Mouse Antigo", Preco = 20m, Ativo = true };
            _mockRepo.Setup(r => r.ObterPorId(1)).ReturnsAsync(produtoExistente);

            var dto = new ProdutoAtualizacaoDto { Id = 1, Nome = "Mouse Novo", Descricao = "Atualizado", Preco = 30m };
            await _servico.Atualizar(dto);

            _mockRepo.Verify(r => r.Atualizar(It.Is<Produto>(p => p.Nome == "Mouse Novo" && p.Preco == 30m)), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task Atualizar_ProdutoInexistente_DeveLancarExcecao()
        {
            _mockRepo.Setup(r => r.ObterPorId(999)).ReturnsAsync((Produto)null);

            var dto = new ProdutoAtualizacaoDto { Id = 999, Nome = "Produto", Preco = 10m };
            await _servico.Atualizar(dto);
        }

        [TestMethod]
        public async Task Remover_ProdutoAtivo_DeveInativarComSucesso()
        {
            var produto = new Produto { Id = 1, Ativo = true };
            _mockRepo.Setup(r => r.ObterPorId(1)).ReturnsAsync(produto);

            await _servico.Remover(1);

            _mockRepo.Verify(r => r.Remover(1), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Remover_ProdutoJaInativo_DeveLancarExcecao()
        {
            var produto = new Produto { Id = 1, Ativo = false };
            _mockRepo.Setup(r => r.ObterPorId(1)).ReturnsAsync(produto);

            await _servico.Remover(1);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task Remover_ProdutoInexistente_DeveLancarExcecao()
        {
            _mockRepo.Setup(r => r.ObterPorId(999)).ReturnsAsync((Produto)null);
            await _servico.Remover(999);
        }

        [TestMethod]
        public async Task Restaurar_ProdutoInativo_DeveRestaurarComSucesso()
        {
            var produto = new Produto { Id = 1, Ativo = false };
            _mockRepo.Setup(r => r.ObterPorId(1)).ReturnsAsync(produto);

            await _servico.Restaurar(1);

            _mockRepo.Verify(r => r.Restaurar(1), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Restaurar_ProdutoJaAtivo_DeveLancarExcecao()
        {
            var produto = new Produto { Id = 1, Ativo = true };
            _mockRepo.Setup(r => r.ObterPorId(1)).ReturnsAsync(produto);

            await _servico.Restaurar(1);
        }
    }
}



