using FluentValidation;
using Servicos.Dtos;

namespace Servicos.Validacoes
{
    /// <summary>
    /// Regras de validação para atualização de setor. Inclui validação do ID.
    /// </summary>
    public class SetorAtualizacaoDtoValidator : AbstractValidator<SetorAtualizacaoDto>
    {
        public SetorAtualizacaoDtoValidator()
        {
            RuleFor(p => p.Id)
                .GreaterThan(0).WithMessage("O ID do setor deve ser maior que zero.");

            RuleFor(p => p.Nome)
                .NotEmpty().WithMessage("O nome do setor é obrigatório.")
                .MaximumLength(100).WithMessage("O nome do setor deve ter no máximo 100 caracteres.");

            RuleFor(x => x.Descricao)
                .MaximumLength(500).WithMessage("A descrição do setor não pode exceder 500 caracteres.");
        }
    }
}
