using FluentValidation;

namespace Application.Commands.Documents.DownloadDocumentCommand
{
    public class DownloadDocumentCommandValidator : AbstractValidator<DownloadDocumentCommand>
    {
        public DownloadDocumentCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Document ID must be greater than 0");
        }
    }
}