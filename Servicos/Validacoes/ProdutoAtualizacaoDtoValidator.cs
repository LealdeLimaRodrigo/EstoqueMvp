using FluentValidation;
using Servicos.Dtos;

namespace Servicos.Validacoes
{
    /// <summary>
    /// Regras de validação para atualização de produto. Inclui validação do ID.
    /// </summary>
    public class ProdutoAtualizacaoDtoValidator : AbstractValidator<ProdutoAtualizacaoDto>
    {
        public ProdutoAtualizacaoDtoValidator()
        {
            RuleFor(p => p.Id)
                .GreaterThan(0).WithMessage("O ID do produto deve ser maior que zero.");

            RuleFor(p => p.Nome)
                .NotEmpty().WithMessage("O nome do produto é obrigatório.")
                .MaximumLength(100).WithMessage("O nome do produto deve ter no máximo 100 caracteres.");

            RuleFor(x => x.Descricao)
                .MaximumLength(500).WithMessage("A descrição não pode exceder 500 caracteres.");

            RuleFor(p => p.Preco)
                .GreaterThan(0).WithMessage("O preço do produto deve ser maior que zero.");
        }
    }
}
