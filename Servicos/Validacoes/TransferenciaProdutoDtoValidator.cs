using FluentValidation;
using Servicos.Dtos;

namespace Servicos.Validacoes
{
    /// <summary>
    /// Regras de validação para transferência de produto entre setores.
    /// Garante que origem e destino sejam diferentes e que a quantidade seja positiva.
    /// </summary>
    public class TransferenciaProdutoDtoValidator : AbstractValidator<TransferenciaProdutoDto>
    {
        public TransferenciaProdutoDtoValidator()
        {
            RuleFor(x => x.ProdutoId).GreaterThan(0).WithMessage("O ID do produto deve ser maior que zero.");
            RuleFor(x => x.SetorOrigemId).GreaterThan(0).WithMessage("O ID do setor de origem deve ser maior que zero.");
            RuleFor(x => x.SetorDestinoId).GreaterThan(0).WithMessage("O ID do setor de destino deve ser maior que zero.");
            RuleFor(x => x.Quantidade).GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");
            RuleFor(x => x.UsuarioId).GreaterThan(0).WithMessage("O ID do usuário deve ser maior que zero.");

            RuleFor(x => x.SetorDestinoId).NotEqual(x => x.SetorOrigemId).WithMessage("O setor de destino deve ser diferente do setor de origem.");
        }
    }
}