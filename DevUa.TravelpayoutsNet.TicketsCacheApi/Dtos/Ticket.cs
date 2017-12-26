using System;
using Newtonsoft.Json;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Dtos
{
    public class Ticket
    {
        [JsonProperty("show_to_affiliates")]
        public bool ShowToAffiliates { get; set; }

        [JsonProperty("trip_class")]
        public int TripClass { get; set; }

        [JsonProperty("origin")]
        public string OriginIata { get; set; }

        [JsonProperty("destination")]
        public string DestinationIata { get; set; }

        [JsonProperty("depart_date")]
        public DateTime DepartDate { get; set; }

        [JsonProperty("return_date")]
        public DateTime? ReturnDate { get; set; }

        [JsonProperty("number_of_changes")]
        public int NumberOfChanges { get; set; }

        [JsonProperty("value")]
        public decimal Value { get; set; }

        [JsonProperty("found_at")]
        public DateTime FoundAt { get; set; }

        [JsonProperty("distance")]
        public int Distance { get; set; }

        [JsonProperty("actual")]
        public bool Actual { get; set; }

        [JsonProperty("duration")]
        public int? Duration { get; set; }

        [JsonProperty("gate")]
        public string Gate { get; set; }
    }
}
