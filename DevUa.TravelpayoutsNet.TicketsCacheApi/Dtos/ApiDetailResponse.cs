using Newtonsoft.Json;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Dtos
{
    internal class ApiDetailResponse : ApiResponse
    {
        [JsonProperty("data")]
        public DetailDataResponse Data { get; set; }
    }
}
