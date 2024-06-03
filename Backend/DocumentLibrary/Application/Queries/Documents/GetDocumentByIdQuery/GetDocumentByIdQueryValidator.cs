using FluentValidation;

namespace Application.Queries.Documents.GetDocumentByIdQuery;
public class GetDocumentByIdQueryValidator : AbstractValidator<GetDocumentByIdQuery>
{
    public GetDocumentByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Document ID must be greater than 0");
    }
}
