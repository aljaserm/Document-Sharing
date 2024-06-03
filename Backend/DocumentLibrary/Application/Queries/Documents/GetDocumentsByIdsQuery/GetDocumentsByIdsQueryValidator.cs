using FluentValidation;

namespace Application.Queries.Documents.GetDocumentsByIdsQuery;

public class GetDocumentsByIdsQueryValidator : AbstractValidator<GetDocumentsByIdsQuery>
{
    public GetDocumentsByIdsQueryValidator()
    {
        RuleFor(x => x.Ids).NotEmpty().WithMessage("Document IDs must be provided");
    }
}