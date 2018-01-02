using Newtonsoft.Json;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Dtos
{
    public class DetailDataResponse
    {
        [JsonProperty("prices")]
        public Ticket[] Prices { get; set; }

        [JsonProperty("errors")]
        public dynamic Errors { get; set; }

        [JsonProperty("origins")]
        public string[] Origins { get; set; }

        [JsonProperty("destinations")]
        public string[] Destinations { get; set; }

    }
}
