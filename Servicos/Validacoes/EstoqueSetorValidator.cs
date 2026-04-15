using Dominio.Entidades;
using FluentValidation;

namespace Servicos.Validacoes
{
    public class EstoqueSetorValidator : AbstractValidator<EstoqueSetor>
    {
        public EstoqueSetorValidator() 
        {
            RuleFor(q => q.QuantidadeEstoque)
                .NotEmpty().WithMessage("A quantidade de estoque é obrigatória.")
                .GreaterThan(0).WithMessage("A quantidade de estoque deve ser maior que zero.");
        }
    }
}
