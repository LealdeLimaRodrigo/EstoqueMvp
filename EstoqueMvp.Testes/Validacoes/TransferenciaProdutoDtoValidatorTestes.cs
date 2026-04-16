using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servicos.Dtos;
using Servicos.Validacoes;

namespace EstoqueMvp.Testes.Validacoes
{
    /// <summary>
    /// Testes unitários para validação de transferência de produto entre setores.
    /// Verifica que origem e destino devem ser diferentes e dados válidos.
    /// </summary>
    [TestClass]
    public class TransferenciaProdutoDtoValidatorTestes
    {
        private readonly TransferenciaProdutoDtoValidator _validator = new TransferenciaProdutoDtoValidator();

        [TestMethod]
        public void Validar_DadosValidos_NaoDeveLancarExcecao()
        {
            var dto = new TransferenciaProdutoDto
            {
                ProdutoId = 1,
                SetorOrigemId = 1,
                SetorDestinoId = 2,
                Quantidade = 5,
                UsuarioId = 1
            };
            _validator.ValidateAndThrow(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void Validar_SetorOrigemIgualDestino_DeveLancarExcecao()
        {
            var dto = new TransferenciaProdutoDto
            {
                ProdutoId = 1,
                SetorOrigemId = 1,
                SetorDestinoId = 1,
                Quantidade = 5,
                UsuarioId = 1
            };
            _validator.ValidateAndThrow(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void Validar_QuantidadeZero_DeveLancarExcecao()
        {
            var dto = new TransferenciaProdutoDto
            {
                ProdutoId = 1,
                SetorOrigemId = 1,
                SetorDestinoId = 2,
                Quantidade = 0,
                UsuarioId = 1
            };
            _validator.ValidateAndThrow(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void Validar_ProdutoIdZero_DeveLancarExcecao()
        {
            var dto = new TransferenciaProdutoDto
            {
                ProdutoId = 0,
                SetorOrigemId = 1,
                SetorDestinoId = 2,
                Quantidade = 5,
                UsuarioId = 1
            };
            _validator.ValidateAndThrow(dto);
        }
    }
}
