using FluentValidation;

namespace Application.Queries.Documents.GetDocumentByShareLinkQuery
{
    public class GetDocumentByShareLinkQueryValidator : AbstractValidator<GetDocumentByShareLinkQuery>
    {
        public GetDocumentByShareLinkQueryValidator()
        {
            RuleFor(x => x.ShareLink).NotEmpty().WithMessage("Share link must not be empty");
        }
    }
}