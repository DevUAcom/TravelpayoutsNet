using System.Threading.Tasks;
using AutoFixture;
using DevUa.TravelpayoutsNet.TicketsCacheApi.ApiStrings;
using DevUa.TravelpayoutsNet.TicketsCacheApi.Exceptions;
using RichardSzalay.MockHttp;
using Shouldly;
using Xunit;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Tests.ApiGetMethodTests
{
    public class GetLatestAsyncTests : ApiGetMethodTestsBase
    {
        [Fact]
        public async Task GetLatestAsyncShouldReturnTicketArray()
        {
            SetupMockHttp(ApiEndPoints.Latest, "LatestSuccess");
            var apiClient = new TicketsCacheApiClient(GetTokenFixture(), mockHttp.ToHttpClient(), false, false);

            var tickets = await apiClient.GetLatestAsync();

            tickets.Length.ShouldBe(1);
            var ticket = tickets[0];
            ticket.Value.ShouldBe(1183);
        }

        [Fact]
        public async Task GetLatestAsyncGzipShouldReturnTicketArray()
        {
            mockHttp
                .When(ApiEndPoints.ApiBaseUrl + ApiEndPoints.Latest)
                .Respond("application/gzip", JsonResponseHelper.GetStreamResponse("LatestSuccess.gzip"))
            ;
            var apiClient = new TicketsCacheApiClient(GetTokenFixture(), mockHttp.ToHttpClient(), true, false);

            var tickets = await apiClient.GetLatestAsync();

            tickets.Length.ShouldBe(1);
            var ticket = tickets[0];
            ticket.Value.ShouldBe(1183);
        }

        [Fact]
        public void GetLatestAsyncShouldThrowTicketsCacheApiException()
        {
            SetupMockHttp(ApiEndPoints.Latest, "Unauthorized");

            var apiClient = new TicketsCacheApiClient(GetTokenFixture(), mockHttp.ToHttpClient(), false, false);

            apiClient.GetLatestAsync().ShouldThrow(typeof(TicketsCacheApiException));

        }
    }
}
