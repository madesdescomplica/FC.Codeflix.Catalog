using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Exceptions;

using UseCases = FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;

using FC.Codeflix.Catalog.UnitTests.Application.CreateCategory;

using FluentAssertions;
using Moq;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category;

[Collection(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTest
{
    private readonly CreateCategoryTestFixture _fixture;

    public CreateCategoryTest(CreateCategoryTestFixture fixture)
        => _fixture = fixture;
    

    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("Application", "CreateCategory - Use Cases")]
    public async void CreateCategory()
    {
        var repositoryMock = CreateCategoryTestFixture.GetRepositoryMock();
        var unitOfWorkMock = CreateCategoryTestFixture.GetUnitOfWorkMock();
        var useCase = new UseCases.CreateCategory(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );
        var input = _fixture.GetInput();
        var output = await useCase.Handle(
            input, 
            CancellationToken.None
        );

        repositoryMock.Verify(
            repository => repository.Insert(
                It.IsAny<DomainEntity.Category>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        unitOfWorkMock.Verify(
            uow => uow.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
    }

    [Fact(DisplayName = nameof(CreateCategoryWithOnlyName))]
    [Trait("Application", "CreateCategory - Use Cases")]
    public async void CreateCategoryWithOnlyName()
    {
        var repositoryMock = CreateCategoryTestFixture.GetRepositoryMock();
        var unitOfWorkMock = CreateCategoryTestFixture.GetUnitOfWorkMock();
        var useCase = new UseCases.CreateCategory(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );
        var input = new CreateCategoryInput(
            _fixture.GetValidCategoryName()
        );
        var output = await useCase.Handle(
            input, 
            CancellationToken.None
        );

        repositoryMock.Verify(
            repository => repository.Insert(
                It.IsAny<DomainEntity.Category>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        unitOfWorkMock.Verify(
            uow => uow.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be("");
        output.IsActive.Should().BeTrue();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
    }

    [Fact(DisplayName = nameof(CreateCategoryWithOnlyName))]
    [Trait("Application", "CreateCategory - Use Cases")]
    public async void CreateCategoryWithOnlyNameAndDescription()
    {
        var repositoryMock = CreateCategoryTestFixture.GetRepositoryMock();
        var unitOfWorkMock = CreateCategoryTestFixture.GetUnitOfWorkMock();
        var useCase = new UseCases.CreateCategory(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );
        var input = new CreateCategoryInput(
            _fixture.GetValidCategoryName(),
            _fixture.GetValidCategoryDescription()
        );
        var output = await useCase.Handle(
            input, 
            CancellationToken.None
        );

        repositoryMock.Verify(
            repository => repository.Insert(
                It.IsAny<DomainEntity.Category>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        unitOfWorkMock.Verify(
            uow => uow.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().BeTrue();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
    }

    [Theory(DisplayName = nameof(ThrowWhenCantInstantiateCategory))]
    [Trait("Application", "CreateCategory - Use Cases")]
    [MemberData(
        nameof(CreateCategoryTestDataGenerator.GetInvalidInputs),
        parameters: 24,
        MemberType = typeof(CreateCategoryTestDataGenerator)
    )]
    public async void ThrowWhenCantInstantiateCategory(
        CreateCategoryInput input,
        string exceptionMesssage     
    )
    {
        var useCase = new UseCases.CreateCategory(
            CreateCategoryTestFixture.GetRepositoryMock().Object,
            CreateCategoryTestFixture.GetUnitOfWorkMock().Object
        );
        
        Func<Task> task = async ()
            => await useCase.Handle(
                input,
                CancellationToken.None
            );

        await task.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage(exceptionMesssage);
    }
}
