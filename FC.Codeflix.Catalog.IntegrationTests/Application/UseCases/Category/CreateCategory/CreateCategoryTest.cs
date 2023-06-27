using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using ApplicationUseCases = FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using Xunit;
using FluentAssertions;
using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.CreateCategory;

[Collection(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTest
{
    private readonly CreateCategoryTestFixture _fixture;

    public CreateCategoryTest(CreateCategoryTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("Integration/Application", "CreateCategory - UseCases")]
    public async void CreateCategory()
    {
        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var unitOfWorkMock = new UnitOfWork(dbContext);
        var useCase = new ApplicationUseCases.CreateCategory(
            repository,
            unitOfWorkMock
        );
        var input = _fixture.GetInput();
        var output = await useCase.Handle(
            input,
            CancellationToken.None
        );

        var dbCategory = await (_fixture.CreateDbContext(true))
            .Categories.FindAsync(output.Id);

        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(input.IsActive);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
    }

    [Fact(DisplayName = nameof(CreateCategoryWithOnlyNameAndDescription))]
    [Trait("Integration/Application", "CreateCategory - UseCases")]
    public async void CreateCategoryWithOnlyNameAndDescription()
    {
        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var unitOfWorkMock = new UnitOfWork(dbContext);
        var useCase = new ApplicationUseCases.CreateCategory(
            repository,
            unitOfWorkMock
        );
        var exampleInput = _fixture.GetInput();
        var input = new CreateCategoryInput(
            exampleInput.Name,
            exampleInput.Description
        );
        var output = await useCase.Handle(
            input,
            CancellationToken.None
        );

        var dbCategory = await (_fixture.CreateDbContext(true))
            .Categories.FindAsync(output.Id);

        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(true);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(true);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
    }

    [Theory(DisplayName = nameof(ThrowWhenCantInstantiateCategory))]
    [Trait("Integration/Application", "CreateCategory - UseCases")]
    [MemberData(
        nameof(CreateCategoryTestDataGenerator.GetInvalidInputs),
        parameters: 4,
        MemberType = typeof(CreateCategoryTestDataGenerator)
    )]
    public async void ThrowWhenCantInstantiateCategory(
        CreateCategoryInput input,
        string exceptionMesssage
    )
    {
        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var unitOfWorkMock = new UnitOfWork(dbContext);
        var useCase = new ApplicationUseCases.CreateCategory(
            repository,
            unitOfWorkMock
        );
        var task = async ()
            => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<EntityValidationException>()
            .WithMessage(exceptionMesssage);

        var dbCategoriesList = _fixture.CreateDbContext(true).Categories
            .AsNoTracking().ToList();
        dbCategoriesList.Should().HaveCount(0);
    }
}
