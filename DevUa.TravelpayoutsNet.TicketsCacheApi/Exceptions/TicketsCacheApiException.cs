using System;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Exceptions
{
    public class TicketsCacheApiException : Exception
    {
        public TicketsCacheApiException()
        {}

        public TicketsCacheApiException(string message) : base(message)
        {}
    }
}
