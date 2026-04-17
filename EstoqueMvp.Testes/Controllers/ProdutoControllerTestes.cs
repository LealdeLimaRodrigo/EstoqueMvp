using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Servicos.Dtos;
using Servicos.Interfaces;
using EstoqueMvp.Api.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace EstoqueMvp.Testes.Controllers
{
    [TestClass]
    public class ProdutoControllerTestes
    {
        private Mock<IProdutoServico> _mockServico;
        private ProdutoController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockServico = new Mock<IProdutoServico>();
            _controller = new ProdutoController(_mockServico.Object);
        }

        [TestMethod]
        public async Task ObterTodos_SemPaginacao_DeveRetornarOk()
        {
            _mockServico.Setup(s => s.ObterTodos()).ReturnsAsync(new List<ProdutoRetornoDto>());
            var result = await _controller.ObterTodos(null, null, null);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<IEnumerable<ProdutoRetornoDto>>));
        }

        [TestMethod]
        public async Task ObterTodos_ComBusca_DeveRetornarOk()
        {
            var paginado = new PaginacaoResultadoDto<ProdutoRetornoDto> { Itens = new List<ProdutoRetornoDto>(), TotalRegistros = 0, Pagina = 1, TamanhoPagina = 10 };
            _mockServico.Setup(s => s.BuscarPaginado("mouse", 1, 10)).ReturnsAsync(paginado);
            var result = await _controller.ObterTodos(1, 10, "mouse");
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<PaginacaoResultadoDto<ProdutoRetornoDto>>));
        }

        [TestMethod]
        public async Task ObterPorId_Existente_DeveRetornarOk()
        {
            _mockServico.Setup(s => s.ObterPorId(1)).ReturnsAsync(new ProdutoRetornoDto { Id = 1, Nome = "Mouse" });
            var result = await _controller.ObterPorId(1);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<ProdutoRetornoDto>));
        }

        [TestMethod]
        public async Task ObterPorId_Inexistente_DeveRetornarNotFound()
        {
            _mockServico.Setup(s => s.ObterPorId(999)).ReturnsAsync((ProdutoRetornoDto)null);
            var result = await _controller.ObterPorId(999);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Adicionar_DadosValidos_DeveRetornarCreated()
        {
            _mockServico.Setup(s => s.Adicionar(It.IsAny<ProdutoCadastroDto>(), It.IsAny<bool>())).ReturnsAsync(1);
            var result = await _controller.Adicionar(new ProdutoCadastroDto { Nome = "Mouse", Preco = 29.90m });
            Assert.IsTrue(result.GetType().Name.StartsWith("CreatedNegotiatedContentResult"));
        }
    }
}


