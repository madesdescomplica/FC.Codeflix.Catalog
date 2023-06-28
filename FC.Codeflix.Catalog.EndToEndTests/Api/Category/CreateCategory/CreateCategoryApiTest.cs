using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FluentAssertions;
using Xunit;
using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.CreateCategory;

[Collection(nameof(CreateCategoryApiTestFixture))]
public class CreateCategoryApiTest
{
    private readonly CreateCategoryApiTestFixture _fixture;

    public CreateCategoryApiTest(CreateCategoryApiTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("EndToEnd/API", "Category - Endpoints")]
    public async Task CreateCategory()
    {
        // Arrange
        var input = _fixture.GetExampleInput();

        // Act
        var (response, output) = await _fixture
            .ApiClient
            .Post<CategoryModelOutput>(
                "/categories",
                input
            );
        var dbCategory = await _fixture
            .Persistence
            .GetById(output!.Id);

        // Assert
        response!.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.Created);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBeSameDateAs(default);

        dbCategory.Should().NotBeNull();
        dbCategory!.Id.Should().NotBeEmpty();
        dbCategory.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(input.IsActive);
        dbCategory.CreatedAt.Should().NotBeSameDateAs(default);
    }
}
