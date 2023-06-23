using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;

public interface ICreateCategory 
    : IRequestHandler<CreateCategoryInput, CreateCategoryOutput>   
{
    public new Task<CreateCategoryOutput> Handle(
        CreateCategoryInput input,
        CancellationToken cancellationToken
    );
}
