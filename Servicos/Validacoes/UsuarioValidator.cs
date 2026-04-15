using Dominio.Entidades;
using FluentValidation;

namespace Servicos.Validacoes
{
    public class UsuarioValidator : AbstractValidator<Usuario>
    {
        public UsuarioValidator() 
        {
            RuleFor(p => p.Nome)
                .NotEmpty().WithMessage("O nome do usuário é obrigatório.")
                .MaximumLength(100).WithMessage("O nome do usuário deve ter no máximo 100 caracteres.");
            
            RuleFor(c => c.Cpf)
                .NotEmpty().WithMessage("O CPF do usuário é obrigatório.")
                .Length(11).WithMessage("O CPF do usuário deve ter 11 caracteres.");

            RuleFor(s => s.SenhaHash)
                .NotEmpty().WithMessage("A senha do usuário é obrigatória.")
                .MinimumLength(6).WithMessage("A senha do usuário deve ter no mínimo 6 caracteres.");
        }
    }
}
