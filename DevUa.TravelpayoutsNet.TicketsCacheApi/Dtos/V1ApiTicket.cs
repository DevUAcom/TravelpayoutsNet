using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Dtos
{
    public class V1ApiTicket
    {
        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("airline")]
        public string Airline { get; set; }

        [JsonProperty("flight_number")]
        public int FlightNumber { get; set; }

        [JsonProperty("departure_at")]
        public DateTime DepartureAt { get; set; }

        [JsonProperty("return_at")]
        public DateTime ReturnAt { get; set; }

        [JsonProperty("expires_at")]
        public DateTime ExpiresAt { get; set; }
    }
}
