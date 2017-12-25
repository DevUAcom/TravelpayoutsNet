﻿using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using DevUa.TravelpayoutsNet.TicketsCacheApi.ApiStrings;
using DevUa.TravelpayoutsNet.TicketsCacheApi.Dtos;
using DevUa.TravelpayoutsNet.TicketsCacheApi.Enums;
using DevUa.TravelpayoutsNet.TicketsCacheApi.Exceptions;
using DevUa.TravelpayoutsNet.TicketsCacheApi.Helpers;
using Newtonsoft.Json;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi
{
    public class TicketsCacheApiClient
    {
        private readonly string _token;
        private readonly HttpClient _client;

        /// <summary>
        /// The constructor
        /// </summary>
        /// <see cref="https://www.travelpayouts.com/developers/api"/>
        /// <param name="token">Your API token.</param>
        /// <param name="client">(optional) HttpClient object</param>
        public TicketsCacheApiClient(string token, HttpClient client = null)
        {
            _token = token;

            _client = client ?? new HttpClient();
            _client.BaseAddress = new Uri(ApiEndPoints.ApiBaseUrl);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Whether accept the GZip format. The default is true.
        /// </summary>
        public bool AcceptGzip { get; set; } = true;

        /// <summary>
        /// If false (default) the token will be send in the header (recommended). 
        /// If true the token will be send in the query string.
        /// </summary>
        public bool SendTokenInQueryString { get; set; }

        /// <summary>
        /// Query the Flight price history API which brings back the list of prices found by users 
        /// during the most recent 48 hours according to the filters used.
        /// All parameters are optional. If a parameter is skipped the API default will be used.
        /// </summary>
        /// <see cref="https://support.travelpayouts.com/hc/en-us/articles/203956163-Travel-insights-with-Travelpayouts-Data-API#the_prices_for_the_airline_tickets"/>
        /// <param name="currency">The currency of the airline ticket</param>
        /// <param name="originIata">The point of departure. The IATA city code or the country code. The length - from 2 to 3 symbols.</param>
        /// <param name="desinationIata">The point of destination. The IATA city code or the country code. The length - from 2 to 3 symbols.</param>
        /// <param name="beginningOfPeriod">The beginning of the period, within which the dates of departure fall. Must be specified if period_type is equal to month.</param>
        /// <param name="periodTypeMonth">The period for which the tickets have been found. If true - for a month, else - for the whole time</param>
        /// <param name="oneWay">If true - one way tickets, else - round trip</param>
        /// <param name="page">The page number</param>
        /// <param name="limit">The total number of records in the papge</param>
        /// <param name="showToAffiliates">False - all the prices, true - just the prices, found using the partner marker (recommended). </param>
        /// <param name="sorting">Sorting parameter</param>
        /// <param name="tripDurationInWeeks">The duration of trip</param>
        /// <returns>Array of Ticket objects</returns>
        public async Task<Ticket[]> GetLatestAsync(
            Currency? currency = null,
            string originIata = null,
            string desinationIata = null,
            DateTime? beginningOfPeriod = null,
            bool periodTypeMonth = false,
            bool? oneWay = null,
            int? page = null,
            int? limit = null,
            bool? showToAffiliates = null,
            Sorting? sorting = null,
            int? tripDurationInWeeks = null
        )
        {
            if (periodTypeMonth && !beginningOfPeriod.HasValue)
            {
                throw new ArgumentException("For the PeriodType = Month the BeginningOfPeriod must be specified!");
            }

            SetHeaders();

            var query = HttpUtility.ParseQueryString(String.Empty);
            if (SendTokenInQueryString)
            {
                query.Add(QueryParams.Token, _token);
            }
            query.Add(QueryParams.PeriodType, periodTypeMonth ? "month" : "year");

            query
                .AddValueIfNotNull(QueryParams.Currency, currency)
                .AddValueIfNotNull(QueryParams.Origin, originIata)
                .AddValueIfNotNull(QueryParams.Destination, desinationIata)
                .AddValueIfNotNull(QueryParams.BeginningOfPeriod, beginningOfPeriod)
                .AddValueIfNotNull(QueryParams.OneWay, oneWay)
                .AddValueIfNotNull(QueryParams.Page, page)
                .AddValueIfNotNull(QueryParams.Limit, limit)
                .AddValueIfNotNull(QueryParams.ShowToAffiliates, showToAffiliates)
                .AddValueIfNotNull(QueryParams.Sorting, sorting)
                .AddValueIfNotNull(QueryParams.TripDuration, tripDurationInWeeks)
            ;

            HttpResponseMessage response = await _client.GetAsync($"{ApiEndPoints.Latest}?{query}");
            response.EnsureSuccessStatusCode();
            string jsonString;

            using(Stream stream = await response.Content.ReadAsStreamAsync())
            using(Stream decompressed = AcceptGzip ? new GZipStream(stream, CompressionMode.Decompress) : stream)
            using(StreamReader reader = new StreamReader(decompressed))
            {
                jsonString = reader.ReadToEnd();
            }

            //string jsonString = await response.Content.ReadAsStringAsync();
            var apiResponse  = JsonConvert.DeserializeObject<ApiResponse>(jsonString);
            if (!apiResponse.Success)
            {
                throw new TicketsCacheApiException(apiResponse.Message);
            }

            return apiResponse.Data;
        }

        private void SetHeaders()
        {
            if(AcceptGzip)
            {
                _client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            }
            if(!SendTokenInQueryString)
            {
                _client.DefaultRequestHeaders.Add("X-Access-Token", _token);
            }
        }
    }
}
