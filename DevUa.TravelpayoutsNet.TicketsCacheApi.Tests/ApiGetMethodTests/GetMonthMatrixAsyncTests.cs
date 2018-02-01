using System.Threading.Tasks;
using AutoFixture;
using DevUa.TravelpayoutsNet.TicketsCacheApi.ApiStrings;
using DevUa.TravelpayoutsNet.TicketsCacheApi.Exceptions;
using RichardSzalay.MockHttp;
using Shouldly;
using Xunit;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Tests.ApiGetMethodTests
{
    public class GetMonthMatrixAsyncTests : ApiGetMethodTestsBase
    {
        [Fact]
        public async Task GetMonthMatrixAsyncShouldReturnTicketArray()
        {
            SetupMockHttp(ApiEndPoints.MonthMatrix, "MonthMatrixSuccess");
            var apiClient = new TicketsCacheApiClient(GetTokenFixture(), mockHttp.ToHttpClient(), false, false);

            var tickets = await apiClient.GetMonthMatrixAsync();

            tickets.Length.ShouldBe(1);
            var ticket = tickets[0];
            ticket.Value.ShouldBe(29127);
        }

        [Fact]
        public void GetMonthMatrixAsyncShouldThrowTicketsCacheApiException()
        {
            SetupMockHttp(ApiEndPoints.MonthMatrix, "Unauthorized");

            var apiClient = new TicketsCacheApiClient(GetTokenFixture(), mockHttp.ToHttpClient(), false, false);

            apiClient.GetMonthMatrixAsync().ShouldThrow(typeof(TicketsCacheApiException));

        }
    }
}
