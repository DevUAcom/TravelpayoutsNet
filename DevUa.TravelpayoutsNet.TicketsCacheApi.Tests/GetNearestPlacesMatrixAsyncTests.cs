using System.Threading.Tasks;
using AutoFixture;
using DevUa.TravelpayoutsNet.TicketsCacheApi.ApiStrings;
using DevUa.TravelpayoutsNet.TicketsCacheApi.Exceptions;
using RichardSzalay.MockHttp;
using Shouldly;
using Xunit;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Tests
{
    public class GetNearestPlacesMatrixAsync
    {
        [Fact]
        public async Task GetNearestPlacesMatrixAsyncShouldReturnTicketArray()
        {
            Fixture fixture = new Fixture();
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When(ApiEndPoints.ApiBaseUrl + ApiEndPoints.NearestPlacesMatrix)
                .Respond("application/json", JsonResponseHelper.GetJsonResponse("NearestPlacesMatrixSuccess"))
            ;
            var httpClient = mockHttp.ToHttpClient();
            var apiClient = new TicketsCacheApiClient(fixture.Create<string>(), false, false, httpClient);

            var data = await apiClient.GetNearestPlacesMatrixAsync();

            data.Prices.Length.ShouldBe(4);
            var ticket = data.Prices[0];
            ticket.Value.ShouldBe(51155);
        }

        [Fact]
        public void GetNearestPlacesMatrixAsyncShouldThrowTicketsCacheApiException()
        {
            Fixture fixture = new Fixture();
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When(ApiEndPoints.ApiBaseUrl + ApiEndPoints.NearestPlacesMatrix)
                .Respond("application/json", JsonResponseHelper.GetJsonResponse("Unauthorized"))
                ;
            var httpClient = mockHttp.ToHttpClient();
            var apiClient = new TicketsCacheApiClient(fixture.Create<string>(), false, false, httpClient);

            apiClient.GetNearestPlacesMatrixAsync().ShouldThrow(typeof(TicketsCacheApiException));

        }
    }
}
