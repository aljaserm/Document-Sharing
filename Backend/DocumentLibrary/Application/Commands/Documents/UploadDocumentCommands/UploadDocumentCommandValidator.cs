using FluentValidation;

namespace Application.Commands.Documents.UploadDocumentCommands
{
    public class UploadDocumentCommandValidator : AbstractValidator<UploadDocumentCommand>
    {
        public UploadDocumentCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Document name is required.");
            RuleFor(x => x.FileType).NotEmpty().WithMessage("File type is required.");
            RuleFor(x => x.Content).NotEmpty().WithMessage("File content is required.");
        }
    }
}