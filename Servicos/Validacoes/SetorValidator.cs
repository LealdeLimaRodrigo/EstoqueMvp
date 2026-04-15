using Dominio.Entidades;
using FluentValidation;

namespace Servicos.Validacoes
{
    public class SetorValidator : AbstractValidator<Setor>
    {
        public SetorValidator() 
        {
            RuleFor(p => p.Nome)
                .NotEmpty().WithMessage("O nome do setor é obrigatório.")
                .MaximumLength(100).WithMessage("O nome do setor deve ter no máximo 100 caracteres.");

            RuleFor(x => x.Descricao)
                .MaximumLength(500).WithMessage("A descrição do setor não pode exceder 500 caracteres.");
        }
    }
}
