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
    public class UsuarioServicoTestes
    {
        private Mock<IUsuarioRepositorio> _mockRepo;
        private UsuarioServico _servico;
        private static readonly string SenhaHashAdmin = BCrypt.Net.BCrypt.HashPassword("admin123");

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IUsuarioRepositorio>();
            _servico = new UsuarioServico(_mockRepo.Object, new global::Servicos.Validacoes.LoginDtoValidator(), new global::Servicos.Validacoes.UsuarioCadastroDtoValidator(), new global::Servicos.Validacoes.UsuarioAtualizacaoDtoValidator());
        }

        [TestMethod]
        public async Task RealizarLogin_CredenciaisValidas_DeveRetornarUsuarioDto()
        {
            var usuario = new Usuario { Id = 1, Nome = "Admin", Cpf = "00000000000", SenhaHash = SenhaHashAdmin, Ativo = true };
            _mockRepo.Setup(r => r.ObterPorCpfComSenha("00000000000")).ReturnsAsync(usuario);
            var loginDto = new LoginDto { Cpf = "00000000000", Senha = "admin123" };
            var resultado = await _servico.RealizarLogin(loginDto);
            Assert.IsNotNull(resultado);
            Assert.AreEqual(1, resultado.Id);
            Assert.AreEqual("Admin", resultado.Nome);
            Assert.AreEqual("00000000000", resultado.Cpf);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public async Task RealizarLogin_SenhaIncorreta_DeveLancarExcecao()
        {
            var usuario = new Usuario { Id = 1, Nome = "Admin", Cpf = "00000000000", SenhaHash = SenhaHashAdmin, Ativo = true };
            _mockRepo.Setup(r => r.ObterPorCpfComSenha("00000000000")).ReturnsAsync(usuario);
            var loginDto = new LoginDto { Cpf = "00000000000", Senha = "senhaerrada" };
            await _servico.RealizarLogin(loginDto);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public async Task RealizarLogin_CpfInexistente_DeveLancarExcecao()
        {
            _mockRepo.Setup(r => r.ObterPorCpfComSenha("00000000000")).ReturnsAsync((Usuario)null);
            var loginDto = new LoginDto { Cpf = "00000000000", Senha = "admin123" };
            await _servico.RealizarLogin(loginDto);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public async Task RealizarLogin_UsuarioInativo_DeveLancarExcecao()
        {
            var usuario = new Usuario { Id = 1, Nome = "Admin", Cpf = "00000000000", SenhaHash = SenhaHashAdmin, Ativo = false };
            _mockRepo.Setup(r => r.ObterPorCpfComSenha("00000000000")).ReturnsAsync(usuario);
            var loginDto = new LoginDto { Cpf = "00000000000", Senha = "admin123" };
            await _servico.RealizarLogin(loginDto);
        }

        [TestMethod]
        public async Task Adicionar_DadosValidos_DeveRetornarId()
        {
            _mockRepo.Setup(r => r.ObterPorCpf("52998224725")).ReturnsAsync((Usuario)null);
            _mockRepo.Setup(r => r.Adicionar(It.IsAny<Usuario>())).ReturnsAsync(1);
            var dto = new UsuarioCadastroDto { Nome = "Joao Silva", Cpf = "52998224725", Senha = "senha123" };
            int id = await _servico.Adicionar(dto);
            Assert.AreEqual(1, id);
            _mockRepo.Verify(r => r.Adicionar(It.Is<Usuario>(u => u.Nome == "Joao Silva" && u.Cpf == "52998224725" && !string.IsNullOrEmpty(u.SenhaHash))), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Adicionar_CpfDuplicado_DeveLancarExcecao()
        {
            var usuarioExistente = new Usuario { Id = 1, Cpf = "52998224725", Ativo = true };
            _mockRepo.Setup(r => r.ObterPorCpf("52998224725")).ReturnsAsync(usuarioExistente);
            var dto = new UsuarioCadastroDto { Nome = "Outro Joao", Cpf = "52998224725", Senha = "senha123" };
            await _servico.Adicionar(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(FluentValidation.ValidationException))]
        public async Task Adicionar_CpfInvalido_DeveLancarExcecao()
        {
            var dto = new UsuarioCadastroDto { Nome = "Joao", Cpf = "12345678900", Senha = "senha123" };
            await _servico.Adicionar(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(FluentValidation.ValidationException))]
        public async Task Adicionar_SenhaCurta_DeveLancarExcecao()
        {
            var dto = new UsuarioCadastroDto { Nome = "Joao", Cpf = "52998224725", Senha = "123" };
            await _servico.Adicionar(dto);
        }

        [TestMethod]
        public async Task Atualizar_SemInformarSenha_DevePreservarSenhaExistente()
        {
            var usuarioExistente = new Usuario { Id = 1, Nome = "Admin", Cpf = "00000000000", SenhaHash = SenhaHashAdmin, Ativo = true };
            _mockRepo.Setup(r => r.ObterPorCpf("00000000000")).ReturnsAsync(usuarioExistente);
            _mockRepo.Setup(r => r.ObterPorIdComSenha(1)).ReturnsAsync(usuarioExistente);
            var dto = new UsuarioAtualizacaoDto { Id = 1, Nome = "Admin Atualizado", Cpf = "00000000000", Senha = null };
            await _servico.Atualizar(dto);
            _mockRepo.Verify(r => r.Atualizar(It.Is<Usuario>(u => u.Nome == "Admin Atualizado" && u.SenhaHash == SenhaHashAdmin)), Times.Once);
        }

        [TestMethod]
        public async Task Atualizar_ComNovaSenha_DeveAtualizarHash()
        {
            var usuarioExistente = new Usuario { Id = 1, Nome = "Admin", Cpf = "00000000000", SenhaHash = SenhaHashAdmin, Ativo = true };
            _mockRepo.Setup(r => r.ObterPorCpf("00000000000")).ReturnsAsync(usuarioExistente);
            _mockRepo.Setup(r => r.ObterPorIdComSenha(1)).ReturnsAsync(usuarioExistente);
            var dto = new UsuarioAtualizacaoDto { Id = 1, Nome = "Admin", Cpf = "00000000000", Senha = "novasenha123" };
            await _servico.Atualizar(dto);
            _mockRepo.Verify(r => r.Atualizar(It.Is<Usuario>(u => u.SenhaHash != SenhaHashAdmin && !string.IsNullOrEmpty(u.SenhaHash))), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Atualizar_CpfDeOutroUsuario_DeveLancarExcecao()
        {
            var outroUsuario = new Usuario { Id = 2, Cpf = "52998224725", Ativo = true };
            _mockRepo.Setup(r => r.ObterPorCpf("52998224725")).ReturnsAsync(outroUsuario);
            var dto = new UsuarioAtualizacaoDto { Id = 1, Nome = "Admin", Cpf = "52998224725", Senha = "senha123" };
            await _servico.Atualizar(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task Atualizar_UsuarioInexistente_DeveLancarExcecao()
        {
            _mockRepo.Setup(r => r.ObterPorCpf("00000000000")).ReturnsAsync((Usuario)null);
            _mockRepo.Setup(r => r.ObterPorIdComSenha(999)).ReturnsAsync((Usuario)null);
            var dto = new UsuarioAtualizacaoDto { Id = 999, Nome = "Inexistente", Cpf = "00000000000", Senha = "senha123" };
            await _servico.Atualizar(dto);
        }

        [TestMethod]
        public async Task Remover_UsuarioAtivo_DeveInativarComSucesso()
        {
            var usuario = new Usuario { Id = 1, Ativo = true };
            _mockRepo.Setup(r => r.ObterPorId(1)).ReturnsAsync(usuario);
            await _servico.Remover(1);
            _mockRepo.Verify(r => r.Remover(1), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Remover_UsuarioJaInativo_DeveLancarExcecao()
        {
            var usuario = new Usuario { Id = 1, Ativo = false };
            _mockRepo.Setup(r => r.ObterPorId(1)).ReturnsAsync(usuario);
            await _servico.Remover(1);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task Remover_UsuarioInexistente_DeveLancarExcecao()
        {
            _mockRepo.Setup(r => r.ObterPorId(999)).ReturnsAsync((Usuario)null);
            await _servico.Remover(999);
        }

        [TestMethod]
        public async Task Restaurar_UsuarioInativo_DeveRestaurarComSucesso()
        {
            var usuario = new Usuario { Id = 1, Ativo = false };
            _mockRepo.Setup(r => r.ObterPorId(1)).ReturnsAsync(usuario);
            await _servico.Restaurar(1);
            _mockRepo.Verify(r => r.Restaurar(1), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Restaurar_UsuarioJaAtivo_DeveLancarExcecao()
        {
            var usuario = new Usuario { Id = 1, Ativo = true };
            _mockRepo.Setup(r => r.ObterPorId(1)).ReturnsAsync(usuario);
            await _servico.Restaurar(1);
        }
    }
}


