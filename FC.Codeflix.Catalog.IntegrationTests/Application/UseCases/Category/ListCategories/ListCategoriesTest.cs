﻿using ApplicationUseCases = FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Infra.Data.EF;
using Xunit;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FluentAssertions;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Domain.SeedWork;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.ListCategories;

[Collection(nameof(ListCategoriesTestFixture))]
public class ListCategoriesTest
{
    private readonly ListCategoriesTestFixture _fixture;

    public ListCategoriesTest(ListCategoriesTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(SearchReturnsListAndTotal))]
    [Trait("Integration/Application", "ListCategories - UseCases")]
    public async Task SearchReturnsListAndTotal()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(10);
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var input = new ListCategoriesInput(1, 20);
        var useCase = new ApplicationUseCases.ListCategories(categoryRepository);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleCategoriesList.Count);
        output.Items.Should().HaveCount(exampleCategoriesList.Count);
        foreach (CategoryModelOutput outputItem in output.Items)
        {
            var exampleItem = exampleCategoriesList.Find(
                category => category.Id == outputItem.Id
            );

            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

    [Fact(DisplayName = nameof(SearchReturnsEmptyWhenEmpty))]
    [Trait("Integration/Application", "ListCategories - UseCases")]
    public async Task SearchReturnsEmptyWhenEmpty()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var categoryRepository = new CategoryRepository(dbContext);
        var input = new ListCategoriesInput(1, 20);
        var useCase = new ApplicationUseCases.ListCategories(categoryRepository);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
    }

    [Theory(DisplayName = nameof(SearchRetursPaginated))]
    [Trait("Integration/Application", "ListCategories - UseCases")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchRetursPaginated(
        int quantityCategoriesToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems
    )
    {

        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(
            quantityCategoriesToGenerate
        );
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var input = new ListCategoriesInput(page, perPage);
        var useCase = new ApplicationUseCases.ListCategories(categoryRepository);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleCategoriesList.Count);
        output.Items.Should().HaveCount(expectedQuantityItems);
        foreach (CategoryModelOutput outputItem in output.Items)
        {
            var exampleItem = exampleCategoriesList.Find(
                category => category.Id == outputItem.Id
            );

            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("Integration/Application", "ListCategories - UseCases")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-Fi", 1, 5, 4, 4)]
    [InlineData("Sci-Fi", 1, 2, 2, 4)]
    [InlineData("Sci-Fi", 2, 3, 1, 4)]
    [InlineData("Sci-Fi Other", 1, 3, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task SearchByText(
        string search,
        int page,
        int perPage,
        int expectedQuantityItemsReturned,
        int expectedQuantityTotalItems
    )
    {
        var categoryNamesList = new List<string>()
        {
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-Fi AI",
            "Sci-Fi Space",
            "Sci-Fi Robots",
            "Sci-Fi Future",
        };
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategoriesList = _fixture.GetExampleCategoriesListWithNames(
            categoryNamesList);
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var input = new ListCategoriesInput(page, perPage, search);
        var useCase = new ApplicationUseCases.ListCategories(categoryRepository);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(expectedQuantityTotalItems);
        output.Items.Should().HaveCount(expectedQuantityItemsReturned);
        foreach (CategoryModelOutput outputItem in output.Items)
        {
            var exampleItem = exampleCategoriesList.Find(
                category => category.Id == outputItem.Id
            );

            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

    [Theory(DisplayName = nameof(SearchOrdered))]
    [Trait("Integration/Application", "ListCategories - UseCases")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "asc")]
    public async Task SearchOrdered(
        string orderBy,
        string order
    )
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(10);
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var useCaseOrder = 
            order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var input = new ListCategoriesInput(1, 20, "", orderBy, useCaseOrder);
        var useCase = new ApplicationUseCases.ListCategories(categoryRepository);

        var output = await useCase.Handle(input, CancellationToken.None);

        var expectedOrderedList = _fixture.CloneCategoriesListOrdered(
            exampleCategoriesList,
            input.Sort,
            input.Dir
        );
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleCategoriesList.Count);
        output.Items.Should().HaveCount(exampleCategoriesList.Count);
        for ( int i = 0; i < expectedOrderedList.Count; i++ )
        {
            var outputItem = output.Items[i];
            var exampleItem = expectedOrderedList[i];

            exampleItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }
}