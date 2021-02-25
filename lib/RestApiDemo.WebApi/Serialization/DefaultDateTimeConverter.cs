using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RestApiDemo.WebApi.Serialization
{
    /// <summary>
    /// Default converter for date time.
    /// Expects client to send date in local time with offset, converts it to UTC.
    /// Behaviour is identical to settings JsonSerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc.
    /// Unfortunately, if we set DateTimeZoneHandling to UTC globally, we won't be able to use <see cref="TimezoneIndependentDateTimeConverter"/>.
    /// </summary>
    public class DefaultDateTimeConverter : IsoDateTimeConverter
    {
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
            JsonSerializer serializer)
        {
            object? baseValue =
                base.ReadJson(reader, typeof(DateTimeOffset?), existingValue, serializer);
            if (baseValue is DateTimeOffset dateTimeOffset)
            {
                return dateTimeOffset.UtcDateTime;
            }

            return baseValue;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            // This override is needed when ServerTime = DatabaseTime = UTC.
            // Otherwise in that scenario timezone isn't added to response.
            if (value is DateTime dateTime)
            {
                value = new DateTime(dateTime.Ticks, DateTimeKind.Utc);
            }

            base.WriteJson(writer, value, serializer);
        }
    }
}