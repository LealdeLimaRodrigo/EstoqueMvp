using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servicos.Dtos;
using Servicos.Validacoes;

namespace EstoqueMvp.Testes.Validacoes
{
    /// <summary>
    /// Testes unitários para validação de movimentações de estoque (entrada e consumo).
    /// </summary>
    [TestClass]
    public class MovimentacaoEstoqueDtoValidatorTestes
    {
        private readonly MovimentacaoEstoqueDtoValidator _validator = new MovimentacaoEstoqueDtoValidator();

        [TestMethod]
        public void Validar_DadosValidos_NaoDeveLancarExcecao()
        {
            var dto = new MovimentacaoEstoqueDto { ProdutoId = 1, SetorId = 1, Quantidade = 10, UsuarioId = 1 };
            _validator.ValidateAndThrow(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void Validar_ProdutoIdZero_DeveLancarExcecao()
        {
            var dto = new MovimentacaoEstoqueDto { ProdutoId = 0, SetorId = 1, Quantidade = 10, UsuarioId = 1 };
            _validator.ValidateAndThrow(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void Validar_SetorIdZero_DeveLancarExcecao()
        {
            var dto = new MovimentacaoEstoqueDto { ProdutoId = 1, SetorId = 0, Quantidade = 10, UsuarioId = 1 };
            _validator.ValidateAndThrow(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void Validar_QuantidadeZero_DeveLancarExcecao()
        {
            var dto = new MovimentacaoEstoqueDto { ProdutoId = 1, SetorId = 1, Quantidade = 0, UsuarioId = 1 };
            _validator.ValidateAndThrow(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void Validar_QuantidadeNegativa_DeveLancarExcecao()
        {
            var dto = new MovimentacaoEstoqueDto { ProdutoId = 1, SetorId = 1, Quantidade = -5, UsuarioId = 1 };
            _validator.ValidateAndThrow(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void Validar_UsuarioIdZero_DeveLancarExcecao()
        {
            var dto = new MovimentacaoEstoqueDto { ProdutoId = 1, SetorId = 1, Quantidade = 10, UsuarioId = 0 };
            _validator.ValidateAndThrow(dto);
        }
    }
}
