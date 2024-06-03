using System.Text.Json.Serialization;

namespace Application.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]

    public enum TimeUnit
    {
        Minutes,
        Hours,
        Days,
        Weeks,
        Months,
        Years
    }
}