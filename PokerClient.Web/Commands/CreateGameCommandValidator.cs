using FluentValidation;

namespace PokerClient.Web.Commands
{
    public class CreateGameCommandValidator : AbstractValidator<CreateGameCommand>
    {
        public CreateGameCommandValidator()
        {
            RuleFor(x => x.AdminName).NotEmpty().WithMessage("Admin name required");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name required");
            RuleFor(x => x.Name).MinimumLength(3).WithMessage("Name must be at least 3 characters long");
            RuleFor(x => x.Name).MaximumLength(20).WithMessage("Name must be at most 20 characters long");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password required");
            RuleFor(x => x.AdminBuyIn).NotNull().WithMessage("Admin buy in required");
            RuleFor(x => x.MaxPlayers).NotNull().WithMessage("Max players required");
            RuleFor(x => x.MaxPlayers).LessThanOrEqualTo(8).WithMessage("8 players maximum");
            RuleFor(x => x.MaxPlayers).GreaterThanOrEqualTo(2).WithMessage("2 players minimum");
            RuleFor(x => x.SmallBlind).NotNull().WithMessage("Small Blind required");
            RuleFor(x => x.BigBlind).NotNull().WithMessage("Big Blind required");
            RuleFor(x => x.BigBlind).GreaterThan(x => x.SmallBlind).WithMessage("8 players maximum");
        }
    }
}
