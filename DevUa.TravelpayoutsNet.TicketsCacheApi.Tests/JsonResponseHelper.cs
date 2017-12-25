using System.IO;
using System.Reflection;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Test
{
    public static class JsonResponseHelper
    {
        public static string GetJsonResponse(string responseName)
        {
            using(var stream = GetStreamResponse(responseName))
            {
                using (var sr = new StreamReader(stream))
                {
                    return sr.ReadToEnd();
                }
            }

        }

        public static Stream GetStreamResponse(string responseName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream($"DevUa.TravelpayoutsNet.TicketsCacheApi.Tests.ApiResponses.{responseName}.json");
        }


    }
}
