using System.Threading.Tasks;
using AutoFixture;
using DevUa.TravelpayoutsNet.TicketsCacheApi.ApiStrings;
using DevUa.TravelpayoutsNet.TicketsCacheApi.Exceptions;
using RichardSzalay.MockHttp;
using Shouldly;
using Xunit;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Tests.ApiGetMethodTests
{
    public class GetNearestPlacesMatrixAsync : ApiGetMethodTestsBase
    {
        [Fact]
        public async Task GetNearestPlacesMatrixAsyncShouldReturnTicketArray()
        {
            SetupMockHttp(ApiEndPoints.NearestPlacesMatrix, "NearestPlacesMatrixSuccess");
            var apiClient = new TicketsCacheApiClient(GetTokenFixture(), false, false, mockHttp.ToHttpClient());

            var data = await apiClient.GetNearestPlacesMatrixAsync();

            data.Prices.Length.ShouldBe(4);
            var ticket = data.Prices[0];
            ticket.Value.ShouldBe(51155);
        }

        [Fact]
        public void GetNearestPlacesMatrixAsyncShouldThrowTicketsCacheApiException()
        {
            SetupMockHttp(ApiEndPoints.NearestPlacesMatrix, "Unauthorized");

            var apiClient = new TicketsCacheApiClient(GetTokenFixture(), false, false, mockHttp.ToHttpClient());

            apiClient.GetNearestPlacesMatrixAsync().ShouldThrow(typeof(TicketsCacheApiException));

        }
    }
}
