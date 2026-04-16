using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servicos.Dtos;
using Servicos.Validacoes;

namespace EstoqueMvp.Testes.Validacoes
{
    /// <summary>
    /// Testes unitários para as regras de validação do cadastro de usuário.
    /// Cobre validação de nome, CPF e senha.
    /// </summary>
    [TestClass]
    public class UsuarioCadastroDtoValidatorTestes
    {
        private readonly UsuarioCadastroDtoValidator _validator = new UsuarioCadastroDtoValidator();

        [TestMethod]
        public void Validar_DadosValidos_NaoDeveLancarExcecao()
        {
            var dto = new UsuarioCadastroDto { Nome = "João Silva", Cpf = "52998224725", Senha = "senha123" };
            _validator.ValidateAndThrow(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void Validar_NomeVazio_DeveLancarExcecao()
        {
            var dto = new UsuarioCadastroDto { Nome = "", Cpf = "52998224725", Senha = "senha123" };
            _validator.ValidateAndThrow(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void Validar_CpfInvalido_DeveLancarExcecao()
        {
            var dto = new UsuarioCadastroDto { Nome = "João", Cpf = "12345678900", Senha = "senha123" };
            _validator.ValidateAndThrow(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void Validar_CpfVazio_DeveLancarExcecao()
        {
            var dto = new UsuarioCadastroDto { Nome = "João", Cpf = "", Senha = "senha123" };
            _validator.ValidateAndThrow(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void Validar_SenhaVazia_DeveLancarExcecao()
        {
            var dto = new UsuarioCadastroDto { Nome = "João", Cpf = "52998224725", Senha = "" };
            _validator.ValidateAndThrow(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void Validar_SenhaMenorQue6Caracteres_DeveLancarExcecao()
        {
            var dto = new UsuarioCadastroDto { Nome = "João", Cpf = "52998224725", Senha = "12345" };
            _validator.ValidateAndThrow(dto);
        }

        [TestMethod]
        public void Validar_CpfComFormatacao_DeveAceitarComoValido()
        {
            var dto = new UsuarioCadastroDto { Nome = "João", Cpf = "529.982.247-25", Senha = "senha123" };
            _validator.ValidateAndThrow(dto);
        }
    }
}
