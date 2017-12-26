using Newtonsoft.Json;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Dtos
{
    internal class ApiDetailResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public DetailDataResponse Data { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
