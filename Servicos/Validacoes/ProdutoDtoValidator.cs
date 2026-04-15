using FluentValidation;
using Servicos.Dtos;

namespace Servicos.Validacoes
{
    public class ProdutoDtoValidator : AbstractValidator<ProdutoDto>
    {
        public ProdutoDtoValidator() 
        {
            RuleFor(x => x.ProdutoId).GreaterThan(0).WithMessage("O ID do produto deve ser maior que zero.");
            RuleFor(x => x.SetorId).GreaterThan(0).WithMessage("O ID do setor deve ser maior que zero.");
            RuleFor(x => x.Quantidade).GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");
            RuleFor(x => x.UsuarioId).GreaterThan(0).WithMessage("O ID do usuário deve ser maior que zero.");
        }
    }
}
