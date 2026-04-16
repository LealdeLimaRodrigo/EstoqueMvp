using Dominio.Entidades;
using Dominio.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Servicos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EstoqueMvp.Testes.Servicos
{
    [TestClass]
    public class EstoqueSetorServicoTestes
    {
        private Mock<IEstoqueSetorRepositorio> _mockRepo;
        private EstoqueSetorServico _servico;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IEstoqueSetorRepositorio>();
            _servico = new EstoqueSetorServico(
                _mockRepo.Object,
                new global::Servicos.Validacoes.EstoqueSetorValidator()
            );
        }

        [TestMethod]
        public async Task Adicionar_DadosValidos_DeveRetornarId()
        {
            _mockRepo.Setup(r => r.Adicionar(It.IsAny<EstoqueSetor>())).ReturnsAsync(1);

            var estoqueSetor = new EstoqueSetor { ProdutoId = 1, SetorId = 1, QuantidadeEstoque = 100 };
            int result = await _servico.Adicionar(estoqueSetor);

            Assert.AreEqual(1, result);
            _mockRepo.Verify(r => r.Adicionar(It.Is<EstoqueSetor>(e => e.QuantidadeEstoque == 100)), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(FluentValidation.ValidationException))]
        public async Task Adicionar_QuantidadeZero_DeveLancarExcecao()
        {
            var estoqueSetor = new EstoqueSetor { ProdutoId = 1, SetorId = 1, QuantidadeEstoque = 0 };
            await _servico.Adicionar(estoqueSetor);
        }

        [TestMethod]
        public async Task Atualizar_DadosValidos_DeveChamarRepositorio()
        {
            var estoqueSetor = new EstoqueSetor { ProdutoId = 1, SetorId = 1, QuantidadeEstoque = 50 };

            await _servico.Atualizar(estoqueSetor);

            _mockRepo.Verify(r => r.Atualizar(It.Is<EstoqueSetor>(e => e.QuantidadeEstoque == 50)), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(FluentValidation.ValidationException))]
        public async Task Atualizar_QuantidadeNegativa_DeveLancarExcecao()
        {
            var estoqueSetor = new EstoqueSetor { ProdutoId = 1, SetorId = 1, QuantidadeEstoque = -5 };
            await _servico.Atualizar(estoqueSetor);
        }
    }
}
