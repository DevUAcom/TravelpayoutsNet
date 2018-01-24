using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Dtos
{
    internal class ApiTicketResponse : ApiResponse
    {
        [JsonProperty("data")]
        public Ticket[] Data { get; set; }
    }
}
