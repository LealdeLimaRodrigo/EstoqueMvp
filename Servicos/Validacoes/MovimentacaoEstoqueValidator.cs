using Dominio.Entidades;
using FluentValidation;

namespace Servicos.Validacoes
{
    public class MovimentacaoEstoqueValidator : AbstractValidator<MovimentacaoEstoque>
    {
        public MovimentacaoEstoqueValidator() 
        {
            RuleFor(q => q.Quantidade)
                .NotEmpty().WithMessage("A quantidade de estoque é obrigatória.")
                .GreaterThan(0).WithMessage("A quantidade de estoque deve ser maior que zero.");
        }
    }
}
