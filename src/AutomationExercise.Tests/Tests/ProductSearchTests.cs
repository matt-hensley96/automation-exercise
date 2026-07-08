using AutomationExercise.Tests.Fixtures;
using AutomationExercise.Tests.Pages;
using FluentAssertions;

namespace AutomationExercise.Tests.Tests;

public class ProductSearchTests : PageTestBase
{
    public ProductSearchTests(TestRunFixture fixture) : base(fixture)
    {
    }

    // Confirmed against the live site: an empty search term is treated as "no filter" and
    // returns the full catalog, not zero results or a validation error.
    [Fact]
    public async Task Search_WithEmptyTerm_ShowsAllProducts()
        => await RunStepAsync(async () =>
        {
            var productsPage = new ProductsPage(Page);
            await productsPage.GotoAsync();
            await productsPage.SearchAsync("");

            var count = await productsPage.GetProductCountAsync();
            count.Should().BeGreaterThan(10, "an empty search term should behave like 'no filter applied', not like 'no matches'");
        });

    // Confirmed against the live site: a term with no matches renders an empty product grid
    // with no explicit "no results" message - this test pins that as the expected, non-crashing behavior.
    [Fact]
    public async Task Search_WithNoMatchingTerm_ShowsNoResults()
        => await RunStepAsync(async () =>
        {
            var productsPage = new ProductsPage(Page);
            await productsPage.GotoAsync();
            await productsPage.SearchAsync("zzzxcvqqq123nonexistent");

            var count = await productsPage.GetProductCountAsync();
            count.Should().Be(0);
        });
}
