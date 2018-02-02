using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevUa.TravelpayoutsNet.TicketsCacheApi.ApiStrings;
using RichardSzalay.MockHttp;
using Shouldly;
using Xunit;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Tests.QueryStringTests
{
    public class GetCheapAsyncQueryStringTests
    {
        private const string ApiToken = "API_TOKEN";
        private const string Origin = "IEV";

        [Fact]
        public async Task GetCheapAsyncShouldContainOriginQueryStrings()
        {

            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .Expect(ApiEndPoints.ApiBaseUrl + ApiEndPoints.Cheap)
                .WithExactQueryString(new Dictionary<string, string>()
                {
                    { QueryParams.Origin, Origin },
                })
                .WithHeaders(new Dictionary<string, string>
                {
                    { RequestStrings.AccessToken, ApiToken },
                    { "Accept", RequestStrings.ApplicationJson }
                })
                .Respond(RequestStrings.ApplicationJson, JsonResponseHelper.GetJsonResponse("CheapSuccess"))
            ;
            var apiClient = new TicketsCacheApiClient(ApiToken, mockHttp.ToHttpClient(), false, false);

            await apiClient.GetCheapAsync(originIata:Origin);

            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GetCheapAsyncShouldContainTokenQueryString()
        {

            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .Expect(ApiEndPoints.ApiBaseUrl + ApiEndPoints.Cheap)
                .WithExactQueryString(new Dictionary<string, string>()
                {
                    { QueryParams.Token, ApiToken },
                    { QueryParams.Origin, Origin },
                })
                .Respond(RequestStrings.ApplicationJson, JsonResponseHelper.GetJsonResponse("CheapSuccess"))
            ;
            var apiClient = new TicketsCacheApiClient(ApiToken, mockHttp.ToHttpClient(), false, true);

            await apiClient.GetCheapAsync(originIata: Origin);

            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GetCheapAsyncShouldContainOriginAndDestinationQueryStrings()
        {

            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .Expect(ApiEndPoints.ApiBaseUrl + ApiEndPoints.Cheap)
                .WithExactQueryString(new Dictionary<string, string>()
                {
                    { QueryParams.Origin, "KBP" },
                    { QueryParams.Destination, "BKK" },
                })
                .Respond(RequestStrings.ApplicationJson, JsonResponseHelper.GetJsonResponse("CheapSuccess"))
            ;
            var httpClient = mockHttp.ToHttpClient();
            var apiClient = new TicketsCacheApiClient(ApiToken, httpClient, false, false);

            var tickets = await apiClient.GetCheapAsync(originIata: "KBP", destinationIata: "BKK");

            tickets.ShouldNotBeNull();
            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GetCheapAsyncShouldContainAllQueryStrings()
        {
            DateTime departDate = new DateTime(2017, 12, 21);
            DateTime returnDate = new DateTime(2017, 12, 25);
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .Expect(ApiEndPoints.ApiBaseUrl + ApiEndPoints.Cheap)
                .WithExactQueryString(new Dictionary<string, string>()
                {
                    { QueryParams.Currency, "Usd" },
                    { QueryParams.Origin, "KBP" },
                    { QueryParams.Destination, "BKK" },
                    { QueryParams.DepartDate, departDate.ToString("yyyy-MM-dd") },
                    { QueryParams.ReturnDate, returnDate.ToString("yyyy-MM-dd") },
                    { QueryParams.Page, "1" },
                })
                .WithHeaders(new Dictionary<string, string>
                {
                    { RequestStrings.AccessToken, ApiToken },
                    { "Accept", RequestStrings.ApplicationJson }
                })
                .Respond(RequestStrings.ApplicationJson, JsonResponseHelper.GetJsonResponse("CheapSuccess"))
            ;
            var httpClient = mockHttp.ToHttpClient();
            var apiClient = new TicketsCacheApiClient(ApiToken, httpClient, false, false);

            var tickets = await apiClient.GetCheapAsync(
                currency: Enums.Currency.Usd,
                originIata: "KBP", 
                destinationIata: "BKK",
                departDate: departDate,
                returnDate: returnDate,
                page: 1
            );

            tickets.ShouldNotBeNull();
            mockHttp.VerifyNoOutstandingExpectation();
        }

    }
}
