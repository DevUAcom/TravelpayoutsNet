﻿using System.Threading.Tasks;
using DevUa.TravelpayoutsNet.TicketsCacheApi.ApiStrings;
using DevUa.TravelpayoutsNet.TicketsCacheApi.Exceptions;
using DevUa.TravelpayoutsNet.TicketsCacheApi.Test;
using Ploeh.AutoFixture;
using RichardSzalay.MockHttp;
using Shouldly;
using Xunit;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Tests
{
    public class GetMonthMatrixAsyncTests
    {
        [Fact]
        public async Task GetMonthMatrixAsyncShouldReturnTicketArray()
        {
            Fixture fixture = new Fixture();
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When(ApiEndPoints.ApiBaseUrl + ApiEndPoints.MonthMatrix)
                .Respond("application/json", JsonResponseHelper.GetJsonResponse("MonthMatrixSuccess"))
            ;
            var httpClient = mockHttp.ToHttpClient();
            var apiClient = new TicketsCacheApiClient(fixture.Create<string>(), httpClient) {AcceptGzip = false};

            var tickets = await apiClient.GetMonthMatrixAsync();

            tickets.Length.ShouldBe(1);
            var ticket = tickets[0];
            ticket.Value.ShouldBe(29127);
        }

        [Fact]
        public void GetMonthMatrixAsyncShouldThrowTicketsCacheApiException()
        {
            Fixture fixture = new Fixture();
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When(ApiEndPoints.ApiBaseUrl + ApiEndPoints.MonthMatrix)
                .Respond("application/json", JsonResponseHelper.GetJsonResponse("Unauthorized"))
                ;
            var httpClient = mockHttp.ToHttpClient();
            var apiClient = new TicketsCacheApiClient(fixture.Create<string>(), httpClient) {AcceptGzip = false};

            apiClient.GetMonthMatrixAsync().ShouldThrow(typeof(TicketsCacheApiException));

        }
    }
}