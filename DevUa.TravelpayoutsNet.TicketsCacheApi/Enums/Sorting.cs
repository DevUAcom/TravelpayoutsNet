namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Enums
{
    public enum Sorting
    {
        [QueryParameterValue("price")]
        Price,

        [QueryParameterValue("route")]
        Route,

        [QueryParameterValue("distance_unit_price")]
        DistanceUnitPrice
    }
}
