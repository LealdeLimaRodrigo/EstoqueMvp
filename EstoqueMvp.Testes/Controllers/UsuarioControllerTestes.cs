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
    public class UsuarioControllerTestes
    {
        private Mock<IUsuarioServico> _mockServico;
        private UsuarioController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockServico = new Mock<IUsuarioServico>();
            _controller = new UsuarioController(_mockServico.Object);
        }

        [TestMethod]
        public async Task ObterTodos_SemPaginacao_DeveRetornarOk()
        {
            _mockServico.Setup(s => s.ObterTodos()).ReturnsAsync(new List<UsuarioRetornoDto>());
            var result = await _controller.ObterTodos(null, null, null);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<IEnumerable<UsuarioRetornoDto>>));
        }

        [TestMethod]
        public async Task ObterTodos_ComBusca_DeveRetornarOk()
        {
            var paginado = new PaginacaoResultadoDto<UsuarioRetornoDto> { Itens = new List<UsuarioRetornoDto>(), TotalRegistros = 0, Pagina = 1, TamanhoPagina = 10 };
            _mockServico.Setup(s => s.BuscarPaginado("admin", 1, 10)).ReturnsAsync(paginado);
            var result = await _controller.ObterTodos(1, 10, "admin");
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<PaginacaoResultadoDto<UsuarioRetornoDto>>));
        }

        [TestMethod]
        public async Task ObterPorId_Existente_DeveRetornarOk()
        {
            _mockServico.Setup(s => s.ObterPorId(1)).ReturnsAsync(new UsuarioRetornoDto { Id = 1, Nome = "Admin" });
            var result = await _controller.ObterPorId(1);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<UsuarioRetornoDto>));
        }

        [TestMethod]
        public async Task ObterPorId_Inexistente_DeveRetornarNotFound()
        {
            _mockServico.Setup(s => s.ObterPorId(999)).ReturnsAsync((UsuarioRetornoDto)null);
            var result = await _controller.ObterPorId(999);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}

