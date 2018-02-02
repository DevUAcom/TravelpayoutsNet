using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevUa.TravelpayoutsNet.TicketsCacheApi.ApiStrings;
using DevUa.TravelpayoutsNet.TicketsCacheApi.Exceptions;
using Shouldly;
using Xunit;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Tests.ApiGetMethodTests
{
    public class GetCheapAsyncTests : ApiGetMethodTestsBase
    {
        [Fact]
        public async Task GetCheapAsyncShouldReturnTicketArray()
        {
            SetupMockHttp(ApiEndPoints.Cheap, "CheapSuccess");
            var apiClient = new TicketsCacheApiClient(GetTokenFixture(), mockHttp.ToHttpClient(), false, false);

            var data = await apiClient.GetCheapAsync("IEV");

            data.Count.ShouldBe(1);
            data.Keys.First().ShouldBe("HKT");
            var destinationDictionary = data.Values.First();
            var firstTicket = destinationDictionary["0"];
            firstTicket.Price.ShouldBe(35443);
            firstTicket.Airline.ShouldBe("UN");
            firstTicket.FlightNumber.ShouldBe(571);
            firstTicket.DepartureAt.ShouldBe(new DateTime(2015, 6, 9, 21, 20, 0));
            firstTicket.ReturnAt.ShouldBe(new DateTime(2015, 7, 15, 12, 40, 0));
            firstTicket.ExpiresAt.ShouldBe(new DateTime(2015, 1, 8, 18, 30, 40));

        }

        [Fact]
        public void GetCheapAsyncShouldThrowTicketsCacheApiException()
        {
            SetupMockHttp(ApiEndPoints.Cheap, "Unauthorized");

            var apiClient = new TicketsCacheApiClient(GetTokenFixture(), mockHttp.ToHttpClient(), false, false);

            apiClient.GetCheapAsync("IEV").ShouldThrow(typeof(TicketsCacheApiException));

        }

        [Fact]
        public void GetCheapAsyncShouldThrowArgumentException()
        {
            var apiClient = new TicketsCacheApiClient(GetTokenFixture(), mockHttp.ToHttpClient(), false, false);

            apiClient.GetCheapAsync(null).ShouldThrow(typeof(ArgumentException));
        }

    }
}
