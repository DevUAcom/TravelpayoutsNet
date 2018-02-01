using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevUa.TravelpayoutsNet.TicketsCacheApi.ApiStrings;
using RichardSzalay.MockHttp;
using Shouldly;
using Xunit;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Tests.QueryStringTests
{
    public class GetLatestAsyncQueryStringTests
    {
        private const string ApiToken = "API_TOKEN";

        [Fact]
        public async Task GetLatestAsyncShouldNotContainQueryStrings()
        {

            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .Expect(ApiEndPoints.ApiBaseUrl + ApiEndPoints.Latest)
                .WithHeaders(new Dictionary<string, string>
                {
                    { RequestStrings.AccessToken, ApiToken },
                    { "Accept", RequestStrings.ApplicationJson }
                })
                .Respond(RequestStrings.ApplicationJson, JsonResponseHelper.GetJsonResponse("LatestSuccess"))
            ;
            var apiClient = new TicketsCacheApiClient(ApiToken, mockHttp.ToHttpClient(), false, false);

            await apiClient.GetLatestAsync();

            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GetLatestAsyncShouldContainTokenQueryString()
        {

            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .Expect(ApiEndPoints.ApiBaseUrl + ApiEndPoints.Latest)
                .WithExactQueryString(new Dictionary<string, string>()
                {
                    { QueryParams.Token, ApiToken },
                })
                .Respond(RequestStrings.ApplicationJson, JsonResponseHelper.GetJsonResponse("LatestSuccess"))
            ;
            var apiClient = new TicketsCacheApiClient(ApiToken, mockHttp.ToHttpClient(), false, true);

            await apiClient.GetLatestAsync();

            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GetLatestAsyncShouldContainOriginAndDestinationQueryStrings()
        {

            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .Expect(ApiEndPoints.ApiBaseUrl + ApiEndPoints.Latest)
                .WithExactQueryString(new Dictionary<string, string>()
                {
                    { QueryParams.Origin, "KBP" },
                    { QueryParams.Destination, "BKK" },
                    { QueryParams.PeriodType, "year" },
                })
                .Respond(RequestStrings.ApplicationJson, JsonResponseHelper.GetJsonResponse("LatestSuccess"))
            ;
            var httpClient = mockHttp.ToHttpClient();
            var apiClient = new TicketsCacheApiClient(ApiToken, httpClient, false, false);

            var tickets = await apiClient.GetLatestAsync(originIata: "KBP", desinationIata: "BKK", periodTypeMonth: false);

            tickets.ShouldNotBeNull();
            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GetLatestAsyncShouldContainAllQueryStrings()
        {
            DateTime beginningOfPeriod = new DateTime(2017, 12, 21);
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .Expect(ApiEndPoints.ApiBaseUrl + ApiEndPoints.Latest)
                .WithExactQueryString(new Dictionary<string, string>()
                {
                    { QueryParams.Currency, "Usd" },
                    { QueryParams.Origin, "KBP" },
                    { QueryParams.Destination, "BKK" },
                    { QueryParams.BeginningOfPeriod, beginningOfPeriod.ToString("yyyy-MM-dd") },
                    { QueryParams.PeriodType, "month" },
                    { QueryParams.OneWay, "true" },
                    { QueryParams.Page, "1" },
                    { QueryParams.Limit, "10" },
                    { QueryParams.ShowToAffiliates, "false" },
                    { QueryParams.Sorting, "route" },
                    { QueryParams.TripDuration, "2" },
                })
                .WithHeaders(new Dictionary<string, string>
                {
                    { RequestStrings.AccessToken, ApiToken },
                    { "Accept", RequestStrings.ApplicationJson }
                })
                .Respond(RequestStrings.ApplicationJson, JsonResponseHelper.GetJsonResponse("LatestSuccess"))
            ;
            var httpClient = mockHttp.ToHttpClient();
            var apiClient = new TicketsCacheApiClient(ApiToken, httpClient, false, false);

            var tickets = await apiClient.GetLatestAsync(
                currency: Enums.Currency.Usd,
                originIata: "KBP", 
                desinationIata: "BKK",
                beginningOfPeriod: beginningOfPeriod,
                periodTypeMonth: true,
                oneWay: true,
                page: 1,
                limit: 10,
                showToAffiliates: false,
                sorting: Enums.Sorting.Route,
                tripDurationInWeeks: 2
            );

            tickets.ShouldNotBeNull();
            mockHttp.VerifyNoOutstandingExpectation();
        }

    }
}
