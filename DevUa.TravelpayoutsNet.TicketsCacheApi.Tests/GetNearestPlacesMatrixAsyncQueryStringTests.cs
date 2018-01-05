using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevUa.TravelpayoutsNet.TicketsCacheApi.ApiStrings;
using RichardSzalay.MockHttp;
using Shouldly;
using Xunit;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Tests
{
    public class GetNearestPlacesMatrixAsyncQueryStringTests
    {
        private const string ApiToken = "API_TOKEN";

        [Fact]
        public async Task GetNearestPlacesMatrixAsyncShouldNotContainQueryStrings()
        {

            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .Expect(ApiEndPoints.ApiBaseUrl + ApiEndPoints.NearestPlacesMatrix)
                .WithHeaders(new Dictionary<string, string>
                {
                    { "X-Access-Token", ApiToken },
                    { "Accept", "application/json" }
                })
                .Respond("application/json", JsonResponseHelper.GetJsonResponse("NearestPlacesMatrixSuccess"))
            ;
            var apiClient = new TicketsCacheApiClient(ApiToken, mockHttp.ToHttpClient()) {AcceptGzip = false};

            await apiClient.GetNearestPlacesMatrixAsync();

            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GetNearestPlacesMatrixAsyncShouldContainTokenQueryString()
        {

            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .Expect(ApiEndPoints.ApiBaseUrl + ApiEndPoints.NearestPlacesMatrix)
                .WithExactQueryString(new Dictionary<string, string>()
                {
                    { QueryParams.Token, ApiToken },
                })
                .Respond("application/json", JsonResponseHelper.GetJsonResponse("NearestPlacesMatrixSuccess"))
            ;
            var apiClient = new TicketsCacheApiClient(ApiToken, mockHttp.ToHttpClient()) {AcceptGzip = false, SendTokenInQueryString = true};

            await apiClient.GetNearestPlacesMatrixAsync();

            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GettrixAsyncShouldContainOriginAndDestinationQueryStrings()
        {

            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .Expect(ApiEndPoints.ApiBaseUrl + ApiEndPoints.NearestPlacesMatrix)
                .WithExactQueryString(new Dictionary<string, string>()
                {
                    { QueryParams.Origin, "KBP" },
                    { QueryParams.Destination, "BKK" },
                })
                .Respond("application/json", JsonResponseHelper.GetJsonResponse("NearestPlacesMatrixSuccess"))
            ;
            var httpClient = mockHttp.ToHttpClient();
            var apiClient = new TicketsCacheApiClient(ApiToken, httpClient) {AcceptGzip = false};

            var tickets = await apiClient.GetNearestPlacesMatrixAsync(originIata: "KBP", desinationIata: "BKK");

            tickets.ShouldNotBeNull();
            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GetNearestPlacesMatrixAsyncShouldContainAllQueryStrings()
        {
            DateTime departDate = new DateTime(2017, 12, 21);
            DateTime returnDate = new DateTime(2018, 1, 21);
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .Expect(ApiEndPoints.ApiBaseUrl + ApiEndPoints.NearestPlacesMatrix)
                .WithExactQueryString(new Dictionary<string, string>()
                {
                    { QueryParams.Currency, "Usd" },
                    { QueryParams.Origin, "KBP" },
                    { QueryParams.Destination, "BKK" },
                    { QueryParams.ShowToAffiliates, "false" },
                    { QueryParams.DepartDate, departDate.ToString("yyyy-MM-dd") },
                    { QueryParams.ReturnDate, returnDate.ToString("yyyy-MM-dd") },
                    { QueryParams.Distance, "100" },
                    { QueryParams.Limit, "10" },
                    { QueryParams.Flexibilty, "7" },
                })
                .WithHeaders(new Dictionary<string, string>
                {
                    { "X-Access-Token", ApiToken },
                    { "Accept", "application/json" }
                })
                .Respond("application/json", JsonResponseHelper.GetJsonResponse("NearestPlacesMatrixSuccess"))
            ;
            var httpClient = mockHttp.ToHttpClient();
            var apiClient = new TicketsCacheApiClient(ApiToken, httpClient) {AcceptGzip = false};

            var tickets = await apiClient.GetNearestPlacesMatrixAsync(
                currency: Enums.Currency.Usd,
                originIata: "KBP", 
                desinationIata: "BKK",
                showToAffiliates: false,
                departDate: departDate,
                returnDate: returnDate,
                distance: 100,
                limit: 10,
                flexibility: 7
            );

            tickets.ShouldNotBeNull();
            mockHttp.VerifyNoOutstandingExpectation();
        }

    }
}
