using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
