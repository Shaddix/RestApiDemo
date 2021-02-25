using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RestApiDemo.WebApi.Serialization
{
    /// <summary>
    /// Converter to be used on fields which should contain value that is the same across time zones.
    /// E.g. BirthDate (which should be the same in Tomsk and Berlin)
    /// </summary>
    public class TimezoneIndependentDateTimeConverter : IsoDateTimeConverter
    {
        public TimezoneIndependentDateTimeConverter()
        {
            DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
            JsonSerializer serializer)
        {
            object? baseValue =
                base.ReadJson(reader, typeof(DateTimeOffset?), existingValue, serializer);
            if (baseValue is DateTimeOffset dateTimeOffset)
            {
                return dateTimeOffset.DateTime;
            }

            return baseValue;
        }
    }
}