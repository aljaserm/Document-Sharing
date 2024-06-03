using FluentValidation;

namespace Application.Commands.Documents.DownloadMultipleDocumentsCommand
{
    public class DownloadMultipleDocumentsCommandValidator : AbstractValidator<DownloadMultipleDocumentsCommand>
    {
        public DownloadMultipleDocumentsCommandValidator()
        {
            RuleFor(x => x.DocumentIds)
                .NotNull().WithMessage("DocumentIds must not be null.")
                .NotEmpty().WithMessage("DocumentIds must not be empty.")
                .Must(ids => ids.All(id => id > 0)).WithMessage("All DocumentIds must be greater than zero.");
        }
    }
}