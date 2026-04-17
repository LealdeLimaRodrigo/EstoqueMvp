using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Servicos.Dtos;
using Servicos.Interfaces;
using EstoqueMvp.Api.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Dominio.Entidades;

namespace EstoqueMvp.Testes.Controllers
{
    [TestClass]
    public class MovimentacaoEstoqueControllerTestes
    {
        private Mock<IMovimentacaoEstoqueServico> _mockServico;
        private MovimentacaoEstoqueController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockServico = new Mock<IMovimentacaoEstoqueServico>();
            _controller = new MovimentacaoEstoqueController(_mockServico.Object);
        }

        [TestMethod]
        public async Task ObterTodos_DeveRetornarOk()
        {
            _mockServico.Setup(s => s.ObterTodos()).ReturnsAsync(new List<MovimentacaoEstoque>());
            var result = await _controller.ObterTodos();
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<IEnumerable<MovimentacaoEstoque>>));
        }

        [TestMethod]
        public async Task ObterPorProdutoId_DeveRetornarOk()
        {
            _mockServico.Setup(s => s.ObterPorProdutoId(1)).ReturnsAsync(new List<MovimentacaoEstoque>());
            var result = await _controller.ObterPorProdutoId(1);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<IEnumerable<MovimentacaoEstoque>>));
        }

        [TestMethod]
        public async Task ObterPorSetorId_DeveRetornarOk()
        {
            _mockServico.Setup(s => s.ObterPorSetorId(1)).ReturnsAsync(new List<MovimentacaoEstoque>());
            var result = await _controller.ObterPorSetorId(1);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<IEnumerable<MovimentacaoEstoque>>));
        }
    }
}
