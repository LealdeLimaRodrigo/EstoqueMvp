using FluentValidation;
using Servicos.Dtos;

namespace Servicos.Validacoes
{
    /// <summary>
    /// Regras de validação para cadastro de produto.
    /// Garante que nome não esteja vazio, descrição dentro do limite e preço positivo.
    /// </summary>
    public class ProdutoCadastroDtoValidator : AbstractValidator<ProdutoCadastroDto>
    {
        public ProdutoCadastroDtoValidator()
        {
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
