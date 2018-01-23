using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
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
            var apiClient = new TicketsCacheApiClient(fixture.Create<string>(), true, false, httpClient);

            httpClient.DefaultRequestHeaders.AcceptEncoding.Count.ShouldBe(1);
            httpClient.DefaultRequestHeaders.AcceptEncoding.First().Value.ShouldBe("gzip");
        }

        [Fact]
        public void AcceptEncodingShouldNotBeGzip()
        {
            Fixture fixture = new Fixture();
            HttpClient httpClient = new HttpClient();
            var apiClient = new TicketsCacheApiClient(fixture.Create<string>(), false, false, httpClient);

            httpClient.DefaultRequestHeaders.AcceptEncoding.Count.ShouldBe(0);
        }
    }
}
