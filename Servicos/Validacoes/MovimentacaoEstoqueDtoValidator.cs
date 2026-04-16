using FluentValidation;
using Servicos.Dtos;

namespace Servicos.Validacoes
{
    /// <summary>
    /// Regras de validação para movimentações de entrada e consumo.
    /// Garante que todos os IDs sejam válidos e a quantidade seja positiva.
    /// </summary>
    public class MovimentacaoEstoqueDtoValidator : AbstractValidator<MovimentacaoEstoqueDto>
    {
        public MovimentacaoEstoqueDtoValidator() 
        {
            RuleFor(x => x.ProdutoId).GreaterThan(0).WithMessage("O ID do produto deve ser maior que zero.");
            RuleFor(x => x.SetorId).GreaterThan(0).WithMessage("O ID do setor deve ser maior que zero.");
            RuleFor(x => x.Quantidade).GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");
            RuleFor(x => x.UsuarioId).GreaterThan(0).WithMessage("O ID do usuário deve ser maior que zero.");
        }
    }
}
