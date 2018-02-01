using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using DevUa.TravelpayoutsNet.TicketsCacheApi.ApiStrings;
using Shouldly;
using Xunit;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Tests
{
    public class TicketsCacheApiClientTests
    {
        [Fact]
        public void AcceptEncodingShouldBeGzip()
        {
            Fixture fixture = new Fixture();
            HttpClient httpClient = new HttpClient();
            var apiClient = new TicketsCacheApiClient(fixture.Create<string>(), httpClient, true, false);

            httpClient.DefaultRequestHeaders.AcceptEncoding.Count.ShouldBe(1);
            httpClient.DefaultRequestHeaders.AcceptEncoding.First().Value.ShouldBe(RequestStrings.Gzip);
        }

        [Fact]
        public void AcceptEncodingShouldNotBeGzip()
        {
            Fixture fixture = new Fixture();
            HttpClient httpClient = new HttpClient();
            var apiClient = new TicketsCacheApiClient(fixture.Create<string>(), httpClient, false, false);

            httpClient.DefaultRequestHeaders.AcceptEncoding.Count.ShouldBe(0);
        }

        [Fact]
        public void PassingEmptyTokenShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new TicketsCacheApiClient("", null));
        }

        [Fact]
        public void PassingNullTokenShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentException>(() => new TicketsCacheApiClient("", null));
        }

        [Fact]
        public void PassingNullHttpClientShouldThrowArgumentNullException()
        {
            Fixture fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new TicketsCacheApiClient(fixture.Create<string>(), null));
        }

    }
}
