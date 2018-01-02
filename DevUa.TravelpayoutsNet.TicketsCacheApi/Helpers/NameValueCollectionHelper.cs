using System;
using System.Collections.Specialized;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Helpers
{
    public static class NameValueCollectionHelper
    {
        public static NameValueCollection AddValueIfNotNull(this NameValueCollection nameValueCollection, string name, string value)
        {
            if (value != null)
            {
                nameValueCollection.Add(name, value);
            }
            return nameValueCollection;
        }

        public static NameValueCollection AddValueIfNotNull(this NameValueCollection nameValueCollection, string name, DateTime? value)
        {
            if (value != null)
            {
                nameValueCollection.Add(name, value.Value.ToString("yyyy-MM-dd"));
            }
            return nameValueCollection;
        }

        public static NameValueCollection AddValueIfNotNull<T>(this NameValueCollection nameValueCollection, string name, T? value)
            where T : struct 
        {
            if (value.HasValue)
            {
                Enum enumValue = value.Value as Enum;
                nameValueCollection.Add(name,
                    enumValue != null ? enumValue.GetQueryParameterValue() : value.Value.ToString().ToLower()
                );
            }
            return nameValueCollection;
        }
    }
}
