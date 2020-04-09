using FluentValidation;

namespace PokerClient.Web.Commands
{
    public class JoinGameCommandValidator : AbstractValidator<JoinGameCommand>
    {
        public JoinGameCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
            RuleFor(x => x.BuyIn).NotNull().WithMessage("Buy in is required");
            RuleFor(x => x.BuyIn).GreaterThanOrEqualTo(1).WithMessage("Buy in must be more than 0");
        }
    }
}
