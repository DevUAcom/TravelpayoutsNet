using System.Threading.Tasks;
using AutoFixture;
using DevUa.TravelpayoutsNet.TicketsCacheApi.ApiStrings;
using DevUa.TravelpayoutsNet.TicketsCacheApi.Exceptions;
using RichardSzalay.MockHttp;
using Shouldly;
using Xunit;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Tests
{
    public class GetLatestAsyncTests
    {
        [Fact]
        public async Task GetLatestAsyncShouldReturnTicketArray()
        {
            Fixture fixture = new Fixture();
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When(ApiEndPoints.ApiBaseUrl + ApiEndPoints.Latest)
                .Respond("application/json", JsonResponseHelper.GetJsonResponse("LatestSuccess"))
            ;
            var httpClient = mockHttp.ToHttpClient();
            var apiClient = new TicketsCacheApiClient(fixture.Create<string>(), false, false, httpClient);

            var tickets = await apiClient.GetLatestAsync();

            tickets.Length.ShouldBe(1);
            var ticket = tickets[0];
            ticket.Value.ShouldBe(1183);
        }

        //[Fact]
        //public async Task GetLatestAsyncGzipShouldReturnTicketArray()
        //{
        //    Fixture fixture = new Fixture();
        //    var mockHttp = new MockHttpMessageHandler();
        //    mockHttp
        //        .When(ApiEndPoints.ApiBaseUrl + ApiEndPoints.Latest)
        //        .Respond("application/json", JsonResponseHelper.GetStreamResponse("LatestSuccess.gzip"))
        //    ;
        //    var httpClient = mockHttp.ToHttpClient();
        //    var apiClient = new TicketsCacheApiClient(fixture.Create<string>(), httpClient);

        //    var tickets = await apiClient.GetLatestAsync();

        //    tickets.Length.ShouldBe(1);
        //    var ticket = tickets[0];
        //    ticket.Value.ShouldBe(1183);
        //}

        [Fact]
        public void GetLatestAsyncShouldThrowTicketsCacheApiException()
        {
            Fixture fixture = new Fixture();
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When(ApiEndPoints.ApiBaseUrl + ApiEndPoints.Latest)
                .Respond("application/json", JsonResponseHelper.GetJsonResponse("Unauthorized"))
                ;
            var httpClient = mockHttp.ToHttpClient();
            var apiClient = new TicketsCacheApiClient(fixture.Create<string>(), false, false, httpClient);

            apiClient.GetLatestAsync().ShouldThrow(typeof(TicketsCacheApiException));

        }
    }
}
