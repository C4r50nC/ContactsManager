using Fizzler.Systems.HtmlAgilityPack;
using FluentAssertions;
using HtmlAgilityPack;

namespace ContactsManager.IntegrationTests
{
    public class PersonsControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public PersonsControllerIntegrationTest(CustomWebApplicationFactory customWebApplicationFactory)
        {
            _client = customWebApplicationFactory.CreateClient();
        }

        #region Index

        [Fact]
        public async Task Index_ToReturnView()
        {
            HttpResponseMessage response = await _client.GetAsync("/Persons/Index");

            response.Should().BeSuccessful();

            string responseBody = await response.Content.ReadAsStringAsync();
            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml(responseBody);

            HtmlNode? document = htmlDocument.DocumentNode;
            document.QuerySelectorAll("table.persons").Should().NotBeNull();
        }

        #endregion
    }
}
