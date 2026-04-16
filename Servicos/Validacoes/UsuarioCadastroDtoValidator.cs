using FluentValidation;
using Servicos.Dtos;

namespace Servicos.Validacoes
{
    /// <summary>
    /// Regras de validação para cadastro de usuário.
    /// Valida nome, CPF (com algoritmo de dígitos verificadores) e senha com mínimo de 6 caracteres.
    /// </summary>
    public class UsuarioCadastroDtoValidator : AbstractValidator<UsuarioCadastroDto>
    {
        public UsuarioCadastroDtoValidator()
        {
            RuleFor(p => p.Nome)
                .NotEmpty().WithMessage("O nome do usuário é obrigatório.")
                .MaximumLength(100).WithMessage("O nome do usuário deve ter no máximo 100 caracteres.");

            RuleFor(c => c.Cpf)
                .NotEmpty().WithMessage("O CPF do usuário é obrigatório.")
                .Must(cpf => CpfValidacao.ValidarCpf(cpf)).WithMessage("O CPF informado é inválido.");

            RuleFor(s => s.Senha)
                .NotEmpty().WithMessage("A senha do usuário é obrigatória.")
                .MinimumLength(6).WithMessage("A senha do usuário deve ter no mínimo 6 caracteres.");
        }
    }
}
