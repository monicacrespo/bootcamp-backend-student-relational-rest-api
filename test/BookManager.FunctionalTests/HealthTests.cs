namespace BookManager.FunctionalTests {

    using System.Net;
    using BookManager.FunctionalTests.TestSupport;
    using FluentAssertions;

    public class HealthTests : IntegrationTest
    {
        [Fact]
        public async Task Given_Health_Endpoint_When_Sending_Get_Request_It_Returns_200_Ok()
        {
            // Given
            var healthPath = "api/health";

            // When
            var result = await HttpClient.GetAsync(healthPath);

            // Then
            result.StatusCode.Should().Be(HttpStatusCode.OK);                      
        }
    }
}