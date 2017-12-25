using System;
using System.Reflection;
using DevUa.TravelpayoutsNet.TicketsCacheApi.Enums;

namespace DevUa.TravelpayoutsNet.TicketsCacheApi.Helpers
{
    public static class QueryParameterValueHelper
    {
        public static string GetQueryParameterValue(this Enum enumValue)
        {
            Type enumType = enumValue.GetType();
            FieldInfo fi = enumType.GetField(enumValue.ToString());
            QueryParameterValueAttribute[] attrs = fi.GetCustomAttributes(typeof(QueryParameterValueAttribute), false) as QueryParameterValueAttribute[];
            if (attrs != null && attrs.Length > 0)
            {
                return attrs[0].Value;
            }
            else
            {
                return enumValue.ToString();
            }
        }
    }
}
