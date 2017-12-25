﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevUa.TravelpayoutsNet.TicketsCacheApi.ApiStrings;
using RichardSzalay.MockHttp;
using Shouldly;
using Xunit;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Test
{
    public class TicketsCacheApiClientQueryStringTests
    {
        private const string ApiToken = "API_TOKEN";

        [Fact]
        public async Task GetLatestAsyncShouldContainPeroidTypeQueryStrings()
        {

            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .Expect(ApiEndPoints.ApiBaseUrl + ApiEndPoints.Latest)
                .WithExactQueryString(new Dictionary<string, string>()
                {
                    { QueryParams.PeriodType, "year" },
                })
                .WithHeaders(new Dictionary<string, string>
                {
                    { "X-Access-Token", ApiToken },
                    { "Accept", "application/json" }
                })
                .Respond("application/json", JsonResponseHelper.GetJsonResponse("LatestSuccess"))
            ;
            var apiClient = new TicketsCacheApiClient(ApiToken, mockHttp.ToHttpClient()) {AcceptGzip = false};

            await apiClient.GetLatestAsync();

            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GetLatestAsyncShouldContainPeroidTypeAndTokenQueryStrings()
        {

            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .Expect(ApiEndPoints.ApiBaseUrl + ApiEndPoints.Latest)
                .WithExactQueryString(new Dictionary<string, string>()
                {
                    { QueryParams.PeriodType, "year" },
                    { QueryParams.Token, ApiToken },
                })
                .Respond("application/json", JsonResponseHelper.GetJsonResponse("LatestSuccess"))
            ;
            var apiClient = new TicketsCacheApiClient(ApiToken, mockHttp.ToHttpClient()) {AcceptGzip = false, SendTokenInQueryString = true};

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
                .Respond("application/json", JsonResponseHelper.GetJsonResponse("LatestSuccess"))
            ;
            var httpClient = mockHttp.ToHttpClient();
            var apiClient = new TicketsCacheApiClient(ApiToken, httpClient) {AcceptGzip = false};

            var tickets = await apiClient.GetLatestAsync(originIata: "KBP", desinationIata: "BKK");

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
                    { "X-Access-Token", ApiToken },
                    { "Accept", "application/json" }
                })
                .Respond("application/json", JsonResponseHelper.GetJsonResponse("LatestSuccess"))
            ;
            var httpClient = mockHttp.ToHttpClient();
            var apiClient = new TicketsCacheApiClient(ApiToken, httpClient) {AcceptGzip = false};

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