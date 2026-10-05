/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Dynamic;
using Newtonsoft.Json;

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
        => GetData(result) is IEnumerable<dynamic> seq ? seq : [];

    /// <summary>
    /// 'data' of the answer as an object, not dynamic: null when the answer has none
    /// (a failed call, an empty body, a request that got no answer).
    /// </summary>
    private static object GetData(Result result)
        => result is { ResponseHasData: true } ? result.ResponseToDictionary["data"] : null;

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
        var data = GetData(result);
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
            //the data as object, not dynamic: a dynamic call cannot return a type that is not public (a model of the caller)
            : JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(GetData(result)), new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = [new CustomBooleanJsonConverter()]
            });

    private class CustomBooleanJsonConverter : JsonConverter
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
