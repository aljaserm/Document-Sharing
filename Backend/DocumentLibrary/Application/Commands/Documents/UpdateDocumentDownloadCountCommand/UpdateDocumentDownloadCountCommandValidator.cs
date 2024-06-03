using FluentValidation;

namespace Application.Commands.Documents.UpdateDocumentDownloadCountCommand
{
    public class UpdateDocumentDownloadCountCommandValidator : AbstractValidator<UpdateDocumentDownloadCountCommand>
    {
        public UpdateDocumentDownloadCountCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Document ID must be greater than 0");
        }
    }
}