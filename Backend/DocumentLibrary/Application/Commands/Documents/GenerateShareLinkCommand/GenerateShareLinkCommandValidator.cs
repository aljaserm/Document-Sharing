using Application.Enums;
using FluentValidation;

namespace Application.Commands.Documents.GenerateShareLinkCommand
{
    public class GenerateShareLinkCommandValidator : AbstractValidator<GenerateShareLinkCommand>
    {
        public GenerateShareLinkCommandValidator()
        {
            RuleFor(x => x.DocumentId).GreaterThan(0).WithMessage("Document ID must be greater than 0");
            RuleFor(x => x.Duration).GreaterThan(0).WithMessage("Duration must be greater than 0");
            RuleFor(x => x.Unit).IsInEnum().WithMessage("Invalid time unit");
        }
    }
}