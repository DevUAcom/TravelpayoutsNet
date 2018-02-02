using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        private readonly string token;
        private readonly bool acceptGzip;
        private readonly bool sendTokenInQueryString;
        private readonly HttpClient client;

        /// <summary>
        /// The full constructor
        /// </summary>
        /// <see href="https://www.travelpayouts.com/developers/api"/>
        /// <param name="token">Your API token.</param>
        /// <param name="client">(optional) HttpClient object</param>
        /// <param name="acceptGzip">Whether accept the GZip format. The default is true</param>
        /// <param name="sendTokenInQueryString">If false (default) the token will be send in the header (recommended). If true the token will be send in the query string.</param>
        public TicketsCacheApiClient(string token, HttpClient client, bool acceptGzip = true, bool sendTokenInQueryString = false)
        {
            if (String.IsNullOrEmpty(token))
            {
                throw new ArgumentException("You must pass your token!", nameof(token));
            }

            if (client == null)
            {
                throw new ArgumentNullException(nameof(client), "You must pass instance of HttpClient!");
            }

            this.token = token;
            this.acceptGzip = acceptGzip;
            this.sendTokenInQueryString = sendTokenInQueryString;

            this.client = client;
            this.client.BaseAddress = new Uri(ApiEndPoints.ApiBaseUrl);
            this.client.DefaultRequestHeaders.Accept.Clear();
            this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(RequestStrings.ApplicationJson));
            if(acceptGzip)
            {
                this.client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue(RequestStrings.Gzip));
            }
            if(!sendTokenInQueryString)
            {
                this.client.DefaultRequestHeaders.Add(RequestStrings.AccessToken, this.token);
            }
        }

        /// <summary>
        /// Query the Flight price history API which brings back the list of prices found by users 
        /// during the most recent 48 hours according to the filters used.
        /// All parameters are optional. If a parameter is skipped the API default will be used.
        /// </summary>
        /// <see href="https://support.travelpayouts.com/hc/en-us/articles/203956163-Travel-insights-with-Travelpayouts-Data-API#the_prices_for_the_airline_tickets"/>
        /// <param name="currency">The currency of the airline ticket</param>
        /// <param name="originIata">The point of departure. The IATA city code or the country code. The length - from 2 to 3 symbols.</param>
        /// <param name="destinationIata">The point of destination. The IATA city code or the country code. The length - from 2 to 3 symbols.</param>
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
            string destinationIata = null,
            DateTime? beginningOfPeriod = null,
            bool? periodTypeMonth = null,
            bool? oneWay = null,
            int? page = null,
            int? limit = null,
            bool? showToAffiliates = null,
            Sorting? sorting = null,
            int? tripDurationInWeeks = null
        )
        {
            if (periodTypeMonth.HasValue && periodTypeMonth.Value && !beginningOfPeriod.HasValue)
            {
                throw new ArgumentException("For the PeriodType = Month the BeginningOfPeriod must be specified!");
            }

            var query = GetQueryString();

            query
                .AddValueIfNotNull(QueryParams.Currency, currency)
                .AddValueIfNotNull(QueryParams.Origin, originIata)
                .AddValueIfNotNull(QueryParams.Destination, destinationIata)
                .AddValueIfNotNull(QueryParams.BeginningOfPeriod, beginningOfPeriod)
                .AddValueIfNotNull(QueryParams.PeriodType, periodTypeMonth.HasValue ? periodTypeMonth.Value ? "month" : "year" : null)
                .AddValueIfNotNull(QueryParams.OneWay, oneWay)
                .AddValueIfNotNull(QueryParams.Page, page)
                .AddValueIfNotNull(QueryParams.Limit, limit)
                .AddValueIfNotNull(QueryParams.ShowToAffiliates, showToAffiliates)
                .AddValueIfNotNull(QueryParams.Sorting, sorting)
                .AddValueIfNotNull(QueryParams.TripDuration, tripDurationInWeeks)
            ;

            var apiResponse = await GetApiResponse<ApiTicketResponse>(ApiEndPoints.Latest, query);

            return apiResponse.Data;
        }

        /// <summary>
        /// Brings back the prices for each day of a month, grouped together by number of transfers.
        /// </summary>
        /// <see href="https://support.travelpayouts.com/hc/en-us/articles/203956163#the_calendar_of_prices_for_a_month"/>
        /// <param name="currency">The currency of the airline ticket</param>
        /// <param name="originIata">The point of departure. The IATA city code or the country code. The length - from 2 to 3 symbols.</param>
        /// <param name="destinationIata">The point of destination. The IATA city code or the country code. The length - from 2 to 3 symbols.</param>
        /// <param name="showToAffiliates">False - all the prices, true - just the prices, found using the partner marker (recommended). </param>
        /// <param name="month">The beginning of the month</param>
        /// <returns>Array of Ticket objects</returns>
        public async Task<Ticket[]> GetMonthMatrixAsync(
            Currency? currency = null,
            string originIata = null,
            string destinationIata = null,
            bool? showToAffiliates = null,
            DateTime? month = null
        )
        {
            var query = GetQueryString();

            query
                .AddValueIfNotNull(QueryParams.Currency, currency)
                .AddValueIfNotNull(QueryParams.Origin, originIata)
                .AddValueIfNotNull(QueryParams.Destination, destinationIata)
                .AddValueIfNotNull(QueryParams.ShowToAffiliates, showToAffiliates)
                .AddValueIfNotNull(QueryParams.Month, month)
                ;

            var apiResponse = await GetApiResponse<ApiTicketResponse>(ApiEndPoints.MonthMatrix, query);

            return apiResponse.Data;
        }

        /// <summary>
        /// Brings back the prices for the directions between the nearest to the target cities.
        /// </summary>
        /// <see href="https://support.travelpayouts.com/hc/en-us/articles/203956163#the_prices_for_the_alternative_directions"/>
        /// <param name="currency">The currency of the airline ticket</param>
        /// <param name="originIata">The point of departure. The IATA city code or the country code. The length - from 2 to 3 symbols.</param>
        /// <param name="destinationIata">The point of destination. The IATA city code or the country code. The length - from 2 to 3 symbols.</param>
        /// <param name="showToAffiliates">False - all the prices, true - just the prices, found using the partner marker (recommended). </param>
        /// <param name="departDate">The month of departure</param>
        /// <param name="returnDate"></param>
        /// <param name="distance"></param>
        /// <param name="limit"></param>
        /// <param name="flexibility"></param>
        /// <returns></returns>
        public async Task<DetailDataResponse> GetNearestPlacesMatrixAsync(
            Currency? currency = null,
            string originIata = null,
            string destinationIata = null,
            bool? showToAffiliates = null,
            DateTime? departDate = null,
            DateTime? returnDate = null,
            int? distance = null,
            int? limit = null,
            int? flexibility = null
        )
        {
            var query = GetQueryString();

            query
                .AddValueIfNotNull(QueryParams.Currency, currency)
                .AddValueIfNotNull(QueryParams.Origin, originIata)
                .AddValueIfNotNull(QueryParams.Destination, destinationIata)
                .AddValueIfNotNull(QueryParams.ShowToAffiliates, showToAffiliates)
                .AddValueIfNotNull(QueryParams.DepartDate, departDate)
                .AddValueIfNotNull(QueryParams.ReturnDate, returnDate)
                .AddValueIfNotNull(QueryParams.Distance, distance)
                .AddValueIfNotNull(QueryParams.Limit, limit)
                .AddValueIfNotNull(QueryParams.Flexibilty, flexibility)
                ;

            var apiResponse = await GetApiResponse<ApiDetailResponse>(ApiEndPoints.NearestPlacesMatrix, query);

            return apiResponse.Data;
        }

        /// <summary>
        /// Returns the cheapest non-stop tickets, as well as tickets with 1 or 2 stops, for the selected route with departure/return date filters.
        /// </summary>
        /// <see href="https://support.travelpayouts.com/hc/en-us/articles/203956163-Travel-insights-with-Travelpayouts-Data-API#cheapest_tickets"/>
        /// <param name="originIata">The point of departure. The IATA city code or the country code. The length - from 2 to 3 symbols.</param>
        /// <param name="destinationIata">The point of destination. The IATA city code or the country code. The length - from 2 to 3 symbols.</param>
        /// <param name="departDate">The month of departure</param>
        /// <param name="returnDate"></param>
        /// <param name="currency">The currency of the airline ticket</param>
        /// <param name="page">The page number</param>
        public async Task<Dictionary<string, Dictionary<string, V1ApiTicket>>> GetCheapAsync(
            string originIata,
            string destinationIata = null,
            DateTime? departDate = null,
            DateTime? returnDate = null,
            Currency? currency = null,
            int? page = null
        )
        {
            if (String.IsNullOrEmpty(originIata))
            {
                throw new ArgumentException($"{nameof(originIata)} must be specified!", nameof(originIata));
            }

            var query = GetQueryString();

            query
                .AddValueIfNotNull(QueryParams.Currency, currency)
                .AddValueIfNotNull(QueryParams.Origin, originIata)
                .AddValueIfNotNull(QueryParams.Destination, destinationIata)
                .AddValueIfNotNull(QueryParams.DepartDate, departDate)
                .AddValueIfNotNull(QueryParams.ReturnDate, returnDate)
                .AddValueIfNotNull(QueryParams.Page, page)
                ;

            var apiResponse = await GetApiResponse<V1ApiResponse>(ApiEndPoints.Cheap, query);

            return apiResponse.Data;
        }

        private async Task<T> GetApiResponse<T>(string apiMethodName, NameValueCollection query)
            where T : ApiResponse
        {
            HttpResponseMessage response = await client.GetAsync($"{apiMethodName}?{query}");
            response.EnsureSuccessStatusCode();

            string jsonString = await GetJsonString(response);
            var apiResponse = JsonConvert.DeserializeObject<T>(jsonString);
            EnsureSuccessResponse(apiResponse);

            return apiResponse;
        }

        private void EnsureSuccessResponse<T>(T apiResponse) where T : ApiResponse
        {
            if (!apiResponse.Success)
            {
                throw new TicketsCacheApiException(apiResponse.Message);
            }
        }

        private async Task<string> GetJsonString(HttpResponseMessage response)
        {
            string jsonString;
            using (Stream stream = await response.Content.ReadAsStreamAsync())
            using (Stream decompressed = acceptGzip ? new GZipStream(stream, CompressionMode.Decompress) : stream)
            using (StreamReader reader = new StreamReader(decompressed))
            {
                jsonString = reader.ReadToEnd();
            }

            return jsonString;
        }


        private NameValueCollection GetQueryString()
        {
            var query = HttpUtility.ParseQueryString(String.Empty);
            if (sendTokenInQueryString)
            {
                query.Add(QueryParams.Token, token);
            }

            return query;
        }

    }
}
