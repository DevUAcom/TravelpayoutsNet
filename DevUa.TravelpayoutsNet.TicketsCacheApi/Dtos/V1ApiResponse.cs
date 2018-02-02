using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Dtos
{
    internal class V1ApiResponse : ApiResponse
    {
        [JsonProperty("data")]
        public Dictionary<string, Dictionary<string, V1ApiTicket>> Data { get; set; }

    }
}
