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
    [TestClass]
    public class SetorServicoTestes
    {
        private Mock<ISetorRepositorio> _mockRepo;
        private SetorServico _servico;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<ISetorRepositorio>();
            _servico = new SetorServico(
                _mockRepo.Object,
                new global::Servicos.Validacoes.SetorCadastroDtoValidator(),
                new global::Servicos.Validacoes.SetorAtualizacaoDtoValidator()
            );
        }

        [TestMethod]
        public async Task Adicionar_DadosValidos_DeveRetornarId()
        {
            _mockRepo.Setup(r => r.Adicionar(It.IsAny<Setor>())).ReturnsAsync(1);

            var dto = new SetorCadastroDto { Nome = "TI", Descricao = "Tecnologia da Informacao" };
            int id = await _servico.Adicionar(dto);

            Assert.AreEqual(1, id);
            _mockRepo.Verify(r => r.Adicionar(It.Is<Setor>(s => s.Nome == "TI")), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(FluentValidation.ValidationException))]
        public async Task Adicionar_NomeVazio_DeveLancarExcecao()
        {
            var dto = new SetorCadastroDto { Nome = "", Descricao = "TI" };
            await _servico.Adicionar(dto);
        }

        [TestMethod]
        public async Task Atualizar_SetorExistente_DeveAtualizarComSucesso()
        {
            var setorExistente = new Setor { Id = 1, Nome = "TI Antigo", Ativo = true };
            _mockRepo.Setup(r => r.ObterPorId(1)).ReturnsAsync(setorExistente);

            var dto = new SetorAtualizacaoDto { Id = 1, Nome = "TI Novo", Descricao = "Atualizado" };
            await _servico.Atualizar(dto);

            _mockRepo.Verify(r => r.Atualizar(It.Is<Setor>(s => s.Nome == "TI Novo")), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task Atualizar_SetorInexistente_DeveLancarExcecao()
        {
            _mockRepo.Setup(r => r.ObterPorId(999)).ReturnsAsync((Setor)null);

            var dto = new SetorAtualizacaoDto { Id = 999, Nome = "Novo", Descricao = "Atualizado" };
            await _servico.Atualizar(dto);
        }

        [TestMethod]
        public async Task Remover_SetorAtivo_DeveInativarComSucesso()
        {
            var setor = new Setor { Id = 1, Ativo = true };
            _mockRepo.Setup(r => r.ObterPorId(1)).ReturnsAsync(setor);

            await _servico.Remover(1);

            _mockRepo.Verify(r => r.Remover(1), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Remover_SetorJaInativo_DeveLancarExcecao()
        {
            var setor = new Setor { Id = 1, Ativo = false };
            _mockRepo.Setup(r => r.ObterPorId(1)).ReturnsAsync(setor);

            await _servico.Remover(1);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task Remover_SetorInexistente_DeveLancarExcecao()
        {
            _mockRepo.Setup(r => r.ObterPorId(999)).ReturnsAsync((Setor)null);
            await _servico.Remover(999);
        }

        [TestMethod]
        public async Task Restaurar_SetorInativo_DeveRestaurarComSucesso()
        {
            var setor = new Setor { Id = 1, Ativo = false };
            _mockRepo.Setup(r => r.ObterPorId(1)).ReturnsAsync(setor);

            await _servico.Restaurar(1);

            _mockRepo.Verify(r => r.Restaurar(1), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Restaurar_SetorJaAtivo_DeveLancarExcecao()
        {
            var setor = new Setor { Id = 1, Ativo = true };
            _mockRepo.Setup(r => r.ObterPorId(1)).ReturnsAsync(setor);

            await _servico.Restaurar(1);
        }
    }
}
