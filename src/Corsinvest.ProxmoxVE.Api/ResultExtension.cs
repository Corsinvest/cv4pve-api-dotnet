/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;
using System.Dynamic;

namespace Corsinvest.ProxmoxVE.Api;

/// <summary>
/// Result extension
/// </summary>
public static class ResultExtension
{
    /// <summary>
    /// Enumerable result for Linq. Returns an empty sequence when the
    /// endpoint has no data (data is null or absent).
    /// </summary>
    public static IEnumerable<dynamic> ToEnumerable(this Result result)
        => result?.ToData() is IEnumerable<dynamic> seq ? seq : [];

    /// <summary>
    /// Enumerable result data.
    /// </summary>
    public static dynamic ToData(this Result result) => result.Response.data;

    /// <summary>
    /// Enumerable result data.
    /// </summary>
    public static T ToData<T>(this Result result) => (T)Convert.ChangeType(result.ToData(), typeof(T));

    /// <summary>
    /// Convert result (t and n) to logs. Returns an empty sequence when the
    /// endpoint has no data (e.g. firewall log when logging is disabled or empty).
    /// </summary>
    public static IEnumerable<string> ToLogs(this Result result)
    {
        var data = result?.ToData();
        if (data == null) { return []; }
        if (data is ExpandoObject) { return [((dynamic)data).t as string]; }
        return ((IEnumerable<dynamic>)data).OrderBy(a => a.n).Select(a => a.t as string);
    }

    /// <summary>
    /// Check result in error.
    /// </summary>
    public static bool InError(this Result result) => result?.ResponseInError is true;

    /// <summary>
    /// Convert result to model
    /// </summary>
    public static T ToModel<T>(this Result result)
        => result.InError() || !result.IsSuccessStatusCode //check exists error
            ? throw new PveResultException(result, !result.IsSuccessStatusCode ? result.ReasonPhrase : result.GetError())
            : JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(result.ToData()), new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = [new CustomBooleanJsonConverter()]
            });

    class CustomBooleanJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(bool);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.ValueType == typeof(string))
            {
                var value = reader.Value + string.Empty;
                if (string.IsNullOrWhiteSpace(value)) { return false; }
                if (byte.TryParse(value, out var b)) { return Convert.ToBoolean(b); }
                return true; // non-empty non-numeric string (e.g. fingerprint) => true
            }
            else
            {
                return Convert.ToBoolean(reader.Value);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => throw new NotImplementedException();
    }
}