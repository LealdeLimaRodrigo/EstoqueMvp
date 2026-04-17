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
    /// Testes unitários para o MovimentacaoEstoqueServico.
    /// Valida regras de saldo insuficiente, validação de existência de entidades
    /// e operações de entrada, consumo e transferência.
    /// </summary>
    [TestClass]
    public class MovimentacaoEstoqueServicoTestes
    {
        private Mock<IMovimentacaoEstoqueRepositorio> _mockMovRepo;
        private Mock<IEstoqueSetorRepositorio> _mockEstoqueRepo;
        private Mock<IProdutoRepositorio> _mockProdutoRepo;
        private Mock<ISetorRepositorio> _mockSetorRepo;
        private MovimentacaoEstoqueServico _servico;

        [TestInitialize]
        public void Setup()
        {
            _mockMovRepo = new Mock<IMovimentacaoEstoqueRepositorio>();
            _mockEstoqueRepo = new Mock<IEstoqueSetorRepositorio>();
            _mockProdutoRepo = new Mock<IProdutoRepositorio>();
            _mockSetorRepo = new Mock<ISetorRepositorio>();

            _servico = new MovimentacaoEstoqueServico(
                _mockMovRepo.Object,
                _mockEstoqueRepo.Object,
                _mockProdutoRepo.Object,
                _mockSetorRepo.Object,
                new global::Servicos.Validacoes.MovimentacaoEstoqueDtoValidator(),
                new global::Servicos.Validacoes.TransferenciaProdutoDtoValidator());
        }

        /// <summary>
        /// Configura mocks de Produto e Setor como ativos e existentes para os testes.
        /// </summary>
        private void ConfigurarProdutoESetorValidos()
        {
            _mockProdutoRepo.Setup(r => r.ObterPorId(It.IsAny<int>()))
                .ReturnsAsync(new Produto { Id = 1, Nome = "Mouse", Ativo = true });
            _mockSetorRepo.Setup(r => r.ObterPorId(It.IsAny<int>()))
                .ReturnsAsync(new Setor { Id = 1, Nome = "Almoxarifado", Ativo = true });
        }

        [TestMethod]
        public async Task EntradaProduto_DadosValidos_DeveCreditarEstoque()
        {
            ConfigurarProdutoESetorValidos();
            _mockEstoqueRepo.Setup(r => r.ObterPorProdutoIdESetorId(1, 1))
                .ReturnsAsync(new EstoqueSetor { ProdutoId = 1, SetorId = 1, QuantidadeEstoque = 10 });
            _mockMovRepo.Setup(r => r.Adicionar(It.IsAny<MovimentacaoEstoque>())).ReturnsAsync(1);

            var dto = new MovimentacaoEstoqueDto { ProdutoId = 1, SetorId = 1, Quantidade = 5, UsuarioId = 1 };
            await _servico.EntradaProduto(dto);

            _mockEstoqueRepo.Verify(r => r.Atualizar(It.Is<EstoqueSetor>(e => e.QuantidadeEstoque == 15)), Times.Once);
        }

        [TestMethod]
        public async Task EntradaProduto_EstoqueInexistente_DeveCriarNovoRegistro()
        {
            ConfigurarProdutoESetorValidos();
            _mockEstoqueRepo.Setup(r => r.ObterPorProdutoIdESetorId(1, 1)).ReturnsAsync((EstoqueSetor)null);
            _mockEstoqueRepo.Setup(r => r.Adicionar(It.IsAny<EstoqueSetor>())).ReturnsAsync(1);
            _mockMovRepo.Setup(r => r.Adicionar(It.IsAny<MovimentacaoEstoque>())).ReturnsAsync(1);

            var dto = new MovimentacaoEstoqueDto { ProdutoId = 1, SetorId = 1, Quantidade = 20, UsuarioId = 1 };
            await _servico.EntradaProduto(dto);

            _mockEstoqueRepo.Verify(r => r.Adicionar(It.Is<EstoqueSetor>(e => e.QuantidadeEstoque == 20)), Times.Once);
        }

        [TestMethod]
        public async Task ConsumoProduto_SaldoSuficiente_DeveDebitarEstoque()
        {
            ConfigurarProdutoESetorValidos();
            _mockEstoqueRepo.Setup(r => r.ObterPorProdutoIdESetorId(1, 1))
                .ReturnsAsync(new EstoqueSetor { ProdutoId = 1, SetorId = 1, QuantidadeEstoque = 50 });
            _mockMovRepo.Setup(r => r.Adicionar(It.IsAny<MovimentacaoEstoque>())).ReturnsAsync(1);

            var dto = new MovimentacaoEstoqueDto { ProdutoId = 1, SetorId = 1, Quantidade = 10, UsuarioId = 1 };
            await _servico.ConsumoProduto(dto);

            _mockEstoqueRepo.Verify(r => r.Atualizar(It.Is<EstoqueSetor>(e => e.QuantidadeEstoque == 40)), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task ConsumoProduto_SaldoInsuficiente_DeveLancarExcecao()
        {
            ConfigurarProdutoESetorValidos();
            _mockEstoqueRepo.Setup(r => r.ObterPorProdutoIdESetorId(1, 1))
                .ReturnsAsync(new EstoqueSetor { ProdutoId = 1, SetorId = 1, QuantidadeEstoque = 3 });

            var dto = new MovimentacaoEstoqueDto { ProdutoId = 1, SetorId = 1, Quantidade = 10, UsuarioId = 1 };
            await _servico.ConsumoProduto(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task ConsumoProduto_EstoqueZero_DeveLancarExcecao()
        {
            ConfigurarProdutoESetorValidos();
            _mockEstoqueRepo.Setup(r => r.ObterPorProdutoIdESetorId(1, 1))
                .ReturnsAsync(new EstoqueSetor { ProdutoId = 1, SetorId = 1, QuantidadeEstoque = 0 });

            var dto = new MovimentacaoEstoqueDto { ProdutoId = 1, SetorId = 1, Quantidade = 1, UsuarioId = 1 };
            await _servico.ConsumoProduto(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task EntradaProduto_ProdutoInexistente_DeveLancarExcecao()
        {
            _mockProdutoRepo.Setup(r => r.ObterPorId(999)).ReturnsAsync((Produto)null);

            var dto = new MovimentacaoEstoqueDto { ProdutoId = 999, SetorId = 1, Quantidade = 5, UsuarioId = 1 };
            await _servico.EntradaProduto(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task ConsumoProduto_SetorInexistente_DeveLancarExcecao()
        {
            _mockProdutoRepo.Setup(r => r.ObterPorId(1))
                .ReturnsAsync(new Produto { Id = 1, Ativo = true });
            _mockSetorRepo.Setup(r => r.ObterPorId(999)).ReturnsAsync((Setor)null);

            var dto = new MovimentacaoEstoqueDto { ProdutoId = 1, SetorId = 999, Quantidade = 5, UsuarioId = 1 };
            await _servico.ConsumoProduto(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task EntradaProduto_ProdutoInativo_DeveLancarExcecao()
        {
            _mockProdutoRepo.Setup(r => r.ObterPorId(1))
                .ReturnsAsync(new Produto { Id = 1, Nome = "Mouse", Ativo = false });

            var dto = new MovimentacaoEstoqueDto { ProdutoId = 1, SetorId = 1, Quantidade = 5, UsuarioId = 1 };
            await _servico.EntradaProduto(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Transferencia_SetorInativo_DeveLancarExcecao()
        {
            _mockProdutoRepo.Setup(r => r.ObterPorId(1))
                .ReturnsAsync(new Produto { Id = 1, Ativo = true });
            _mockSetorRepo.Setup(r => r.ObterPorId(1))
                .ReturnsAsync(new Setor { Id = 1, Nome = "Ativo", Ativo = true });
            _mockSetorRepo.Setup(r => r.ObterPorId(2))
                .ReturnsAsync(new Setor { Id = 2, Nome = "Inativo", Ativo = false });

            var dto = new TransferenciaProdutoDto { ProdutoId = 1, SetorOrigemId = 1, SetorDestinoId = 2, Quantidade = 5, UsuarioId = 1 };
            await _servico.TransferirProduto(dto);
        }

        [TestMethod]
        public async Task TransferirProduto_DadosValidos_DeveDebitarOrigemECreditarDestino()
        {
            ConfigurarProdutoESetorValidos();

            var estoqueOrigem = new EstoqueSetor { ProdutoId = 1, SetorId = 1, QuantidadeEstoque = 50 };
            var estoqueDestino = new EstoqueSetor { ProdutoId = 1, SetorId = 2, QuantidadeEstoque = 10 };

            _mockEstoqueRepo.Setup(r => r.ObterPorProdutoIdESetorId(1, 1)).ReturnsAsync(estoqueOrigem);
            _mockEstoqueRepo.Setup(r => r.ObterPorProdutoIdESetorId(1, 2)).ReturnsAsync(estoqueDestino);
            _mockMovRepo.Setup(r => r.Adicionar(It.IsAny<MovimentacaoEstoque>())).ReturnsAsync(1);

            var dto = new TransferenciaProdutoDto { ProdutoId = 1, SetorOrigemId = 1, SetorDestinoId = 2, Quantidade = 15, UsuarioId = 1 };
            await _servico.TransferirProduto(dto);

            // Verifica débito na origem: 50 - 15 = 35
            _mockEstoqueRepo.Verify(r => r.Atualizar(It.Is<EstoqueSetor>(e => e.SetorId == 1 && e.QuantidadeEstoque == 35)), Times.Once);

            // Verifica crédito no destino: 10 + 15 = 25
            _mockEstoqueRepo.Verify(r => r.Atualizar(It.Is<EstoqueSetor>(e => e.SetorId == 2 && e.QuantidadeEstoque == 25)), Times.Once);

            // Verifica que foram registradas exatamente 2 movimentações (Envio + Recebimento)
            _mockMovRepo.Verify(r => r.Adicionar(It.IsAny<MovimentacaoEstoque>()), Times.Exactly(2));
        }

        [TestMethod]
        public async Task TransferirProduto_DadosValidos_DeveRegistrarMovimentacoesComMesmoTransacaoId()
        {
            ConfigurarProdutoESetorValidos();

            var estoqueOrigem = new EstoqueSetor { ProdutoId = 1, SetorId = 1, QuantidadeEstoque = 30 };
            _mockEstoqueRepo.Setup(r => r.ObterPorProdutoIdESetorId(1, 1)).ReturnsAsync(estoqueOrigem);
            _mockEstoqueRepo.Setup(r => r.ObterPorProdutoIdESetorId(1, 2)).ReturnsAsync((EstoqueSetor)null);
            _mockEstoqueRepo.Setup(r => r.Adicionar(It.IsAny<EstoqueSetor>())).ReturnsAsync(1);

            var movimentacoesRegistradas = new List<MovimentacaoEstoque>();
            _mockMovRepo.Setup(r => r.Adicionar(It.IsAny<MovimentacaoEstoque>()))
                .Callback<MovimentacaoEstoque>(m => movimentacoesRegistradas.Add(m))
                .ReturnsAsync(1);

            var dto = new TransferenciaProdutoDto { ProdutoId = 1, SetorOrigemId = 1, SetorDestinoId = 2, Quantidade = 10, UsuarioId = 1 };
            await _servico.TransferirProduto(dto);

            // Verifica que as 2 movimentações compartilham o mesmo TransacaoId
            Assert.AreEqual(2, movimentacoesRegistradas.Count);
            Assert.AreEqual(movimentacoesRegistradas[0].TransacaoId, movimentacoesRegistradas[1].TransacaoId);
            Assert.AreNotEqual(Guid.Empty, movimentacoesRegistradas[0].TransacaoId);

            // Verifica os tipos: Envio (3) e Recebimento (4)
            Assert.AreEqual(3, movimentacoesRegistradas[0].TipoMovimentacaoId);
            Assert.AreEqual(4, movimentacoesRegistradas[1].TipoMovimentacaoId);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task TransferirProduto_SaldoInsuficienteNaOrigem_DeveLancarExcecao()
        {
            ConfigurarProdutoESetorValidos();

            _mockEstoqueRepo.Setup(r => r.ObterPorProdutoIdESetorId(1, 1))
                .ReturnsAsync(new EstoqueSetor { ProdutoId = 1, SetorId = 1, QuantidadeEstoque = 3 });

            var dto = new TransferenciaProdutoDto { ProdutoId = 1, SetorOrigemId = 1, SetorDestinoId = 2, Quantidade = 10, UsuarioId = 1 };
            await _servico.TransferirProduto(dto);
        }

        [TestMethod]
        public async Task TransferirProduto_DestinoSemEstoque_DeveCriarRegistroNoDestino()
        {
            ConfigurarProdutoESetorValidos();

            var estoqueOrigem = new EstoqueSetor { ProdutoId = 1, SetorId = 1, QuantidadeEstoque = 20 };
            _mockEstoqueRepo.Setup(r => r.ObterPorProdutoIdESetorId(1, 1)).ReturnsAsync(estoqueOrigem);
            _mockEstoqueRepo.Setup(r => r.ObterPorProdutoIdESetorId(1, 2)).ReturnsAsync((EstoqueSetor)null);
            _mockEstoqueRepo.Setup(r => r.Adicionar(It.IsAny<EstoqueSetor>())).ReturnsAsync(1);
            _mockMovRepo.Setup(r => r.Adicionar(It.IsAny<MovimentacaoEstoque>())).ReturnsAsync(1);

            var dto = new TransferenciaProdutoDto { ProdutoId = 1, SetorOrigemId = 1, SetorDestinoId = 2, Quantidade = 5, UsuarioId = 1 };
            await _servico.TransferirProduto(dto);

            // Verifica que criou novo registro no destino com a quantidade transferida
            _mockEstoqueRepo.Verify(r => r.Adicionar(It.Is<EstoqueSetor>(e => e.SetorId == 2 && e.ProdutoId == 1 && e.QuantidadeEstoque == 5)), Times.Once);

            // Verifica débito na origem: 20 - 5 = 15
            _mockEstoqueRepo.Verify(r => r.Atualizar(It.Is<EstoqueSetor>(e => e.SetorId == 1 && e.QuantidadeEstoque == 15)), Times.Once);
        }
    }
}



