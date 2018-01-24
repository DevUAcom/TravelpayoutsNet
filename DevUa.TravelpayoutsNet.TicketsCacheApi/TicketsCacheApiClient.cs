using System;
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
        private readonly StringWithQualityHeaderValue _gzipEncodingValue = new StringWithQualityHeaderValue("gzip");
        private readonly string _token;
        private readonly bool _acceptGzip;
        private readonly bool _sendTokenInQueryString;
        private readonly HttpClient _client;

        /// <summary>
        /// The constructor
        /// </summary>
        /// <see href="https://www.travelpayouts.com/developers/api"/>
        /// <param name="token">Your API token.</param>
        /// <param name="acceptGzip">Whether accept the GZip format. The default is true</param>
        /// <param name="sendTokenInQueryString">If false (default) the token will be send in the header (recommended). If true the token will be send in the query string.</param>
        /// <param name="client">(optional) HttpClient object</param>
        public TicketsCacheApiClient(string token, bool acceptGzip = true, bool sendTokenInQueryString = false, HttpClient client = null)
        {
            _token = token;
            _acceptGzip = acceptGzip;
            _sendTokenInQueryString = sendTokenInQueryString;

            _client = client ?? new HttpClient();
            _client.BaseAddress = new Uri(ApiEndPoints.ApiBaseUrl);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if(acceptGzip)
            {
                _client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            }
            if(!sendTokenInQueryString)
            {
                _client.DefaultRequestHeaders.Add("X-Access-Token", _token);
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
                .AddValueIfNotNull(QueryParams.Destination, desinationIata)
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
        /// <param name="desinationIata">The point of destination. The IATA city code or the country code. The length - from 2 to 3 symbols.</param>
        /// <param name="showToAffiliates">False - all the prices, true - just the prices, found using the partner marker (recommended). </param>
        /// <param name="month">The beginning of the month</param>
        /// <returns>Array of Ticket objects</returns>
        public async Task<Ticket[]> GetMonthMatrixAsync(
            Currency? currency = null,
            string originIata = null,
            string desinationIata = null,
            bool? showToAffiliates = null,
            DateTime? month = null
        )
        {
            var query = GetQueryString();

            query
                .AddValueIfNotNull(QueryParams.Currency, currency)
                .AddValueIfNotNull(QueryParams.Origin, originIata)
                .AddValueIfNotNull(QueryParams.Destination, desinationIata)
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
        /// <param name="desinationIata">The point of destination. The IATA city code or the country code. The length - from 2 to 3 symbols.</param>
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
            string desinationIata = null,
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
                .AddValueIfNotNull(QueryParams.Destination, desinationIata)
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

        private async Task<T> GetApiResponse<T>(string apiMethodName, NameValueCollection query)
            where T : ApiResponse
        {
            HttpResponseMessage response = await _client.GetAsync($"{apiMethodName}?{query}");
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
            using (Stream decompressed = _acceptGzip ? new GZipStream(stream, CompressionMode.Decompress) : stream)
            using (StreamReader reader = new StreamReader(decompressed))
            {
                jsonString = reader.ReadToEnd();
            }

            return jsonString;
        }


        private NameValueCollection GetQueryString()
        {
            var query = HttpUtility.ParseQueryString(String.Empty);
            if (_sendTokenInQueryString)
            {
                query.Add(QueryParams.Token, _token);
            }

            return query;
        }

    }
}
