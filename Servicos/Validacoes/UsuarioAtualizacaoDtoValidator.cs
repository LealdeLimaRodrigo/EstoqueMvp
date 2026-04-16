using FluentValidation;
using Servicos.Dtos;

namespace Servicos.Validacoes
{
    /// <summary>
    /// Regras de validação para atualização de usuário. Senha é validada apenas quando informada.
    /// </summary>
    public class UsuarioAtualizacaoDtoValidator : AbstractValidator<UsuarioAtualizacaoDto>
    {
        public UsuarioAtualizacaoDtoValidator()
        {
            RuleFor(p => p.Id)
                .GreaterThan(0).WithMessage("O ID do usuário deve ser maior que zero.");

            RuleFor(p => p.Nome)
                .NotEmpty().WithMessage("O nome do usuário é obrigatório.")
                .MaximumLength(100).WithMessage("O nome do usuário deve ter no máximo 100 caracteres.");

            RuleFor(c => c.Cpf)
                .NotEmpty().WithMessage("O CPF do usuário é obrigatório.")
                .Must(cpf => CpfValidacao.ValidarCpf(cpf)).WithMessage("O CPF informado é inválido.");

            RuleFor(s => s.Senha)
                .MinimumLength(6).WithMessage("A senha do usuário deve ter no mínimo 6 caracteres.")
                .When(u => !string.IsNullOrWhiteSpace(u.Senha));
        }
    }
}
