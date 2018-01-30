using AutoFixture;
using RichardSzalay.MockHttp;
using DevUa.TravelpayoutsNet.TicketsCacheApi.ApiStrings;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Tests.ApiGetMethodTests
{
    public class ApiGetMethodTestsBase
    {
        private readonly Fixture fixture;
        protected MockHttpMessageHandler mockHttp;

        public ApiGetMethodTestsBase()
        {
            fixture = new Fixture();
            mockHttp = new MockHttpMessageHandler();
        }

        protected void SetupMockHttp(string apiEndPoing, string jsonResponseFile)
        {
            mockHttp
                .When(ApiEndPoints.ApiBaseUrl + apiEndPoing)
                .Respond("application/json", JsonResponseHelper.GetJsonResponse(jsonResponseFile))
                ;
        }

        protected string GetTokenFixture()
        {
            return fixture.Create<string>();
        }
    }
}