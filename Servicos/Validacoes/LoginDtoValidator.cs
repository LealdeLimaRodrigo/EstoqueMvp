using FluentValidation;
using Servicos.Dtos;

namespace Servicos.Validacoes
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator() 
        {
            RuleFor(x => x.Cpf)
                .NotEmpty().WithMessage("CPF é obrigatório.")
                .Must(CpfValidacao.ValidarCpf).WithMessage("CPF inválido.");

            RuleFor(x => x.Senha)
                .NotEmpty().WithMessage("Senha é obrigatória.")
                .MinimumLength(6).WithMessage("Senha deve conter no mínimo 6 caracteres.");
        }
    }
}
