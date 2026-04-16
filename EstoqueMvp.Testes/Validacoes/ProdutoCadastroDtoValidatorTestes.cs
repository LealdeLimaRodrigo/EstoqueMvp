using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servicos.Dtos;
using Servicos.Validacoes;

namespace EstoqueMvp.Testes.Validacoes
{
    /// <summary>
    /// Testes unitários para as regras de validação do cadastro de produto.
    /// </summary>
    [TestClass]
    public class ProdutoCadastroDtoValidatorTestes
    {
        private readonly ProdutoCadastroDtoValidator _validator = new ProdutoCadastroDtoValidator();

        [TestMethod]
        public void Validar_DadosValidos_NaoDeveLancarExcecao()
        {
            var dto = new ProdutoCadastroDto { Nome = "Mouse Óptico", Descricao = "Mouse sem fio", Preco = 45.90m };
            _validator.ValidateAndThrow(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void Validar_NomeVazio_DeveLancarExcecao()
        {
            var dto = new ProdutoCadastroDto { Nome = "", Descricao = "Teste", Preco = 10m };
            _validator.ValidateAndThrow(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void Validar_NomeMaiorQue100Caracteres_DeveLancarExcecao()
        {
            var dto = new ProdutoCadastroDto { Nome = new string('A', 101), Preco = 10m };
            _validator.ValidateAndThrow(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void Validar_PrecoZero_DeveLancarExcecao()
        {
            var dto = new ProdutoCadastroDto { Nome = "Produto", Preco = 0 };
            _validator.ValidateAndThrow(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void Validar_PrecoNegativo_DeveLancarExcecao()
        {
            var dto = new ProdutoCadastroDto { Nome = "Produto", Preco = -10m };
            _validator.ValidateAndThrow(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void Validar_DescricaoMaiorQue500Caracteres_DeveLancarExcecao()
        {
            var dto = new ProdutoCadastroDto { Nome = "Produto", Preco = 10m, Descricao = new string('A', 501) };
            _validator.ValidateAndThrow(dto);
        }

        [TestMethod]
        public void Validar_DescricaoNula_NaoDeveLancarExcecao()
        {
            var dto = new ProdutoCadastroDto { Nome = "Produto", Preco = 10m, Descricao = null };
            _validator.ValidateAndThrow(dto);
        }
    }
}
