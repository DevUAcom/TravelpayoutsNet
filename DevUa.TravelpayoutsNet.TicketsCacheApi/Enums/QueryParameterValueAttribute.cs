using System;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Enums
{
    [AttributeUsage(AttributeTargets.Field)]
    public class QueryParameterValueAttribute : Attribute
    {
        public QueryParameterValueAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}
