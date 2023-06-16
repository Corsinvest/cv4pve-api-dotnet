/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Corsinvest.ProxmoxVE.Api
{
    /// <summary>
    /// Result extension
    /// </summary>
    public static class ResultExtension
    {
        /// <summary>
        /// Enumerable result for Linq.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static IEnumerable<dynamic> ToEnumerable(this Result result) => (IEnumerable<dynamic>)result.ToData();

        /// <summary>
        /// Enumerable result data.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static dynamic ToData(this Result result) => result.Response.data;

        /// <summary>
        /// Enumerable result data.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static T ToData<T>(this Result result) => (T)Convert.ChangeType(result.ToData(), typeof(T));

        /// <summary>
        /// Check result in error.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool InError(this Result result) => result != null && result.ResponseInError;

        /// <summary>
        /// Convert result to model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public static T ToModel<T>(this Result result)
            => result.InError() || !result.IsSuccessStatusCode //check exists error
                ? throw new PveExceptionResult(result, !result.IsSuccessStatusCode ? result.ReasonPhrase : result.GetError())
                : JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(result.ToData()), new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Converters = new JsonConverter[] { new CustomBooleanJsonConverter() }.ToList()
                });

        class CustomBooleanJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType) => objectType == typeof(bool);

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.ValueType == typeof(string))
                {
                    return !string.IsNullOrWhiteSpace(reader.Value + "") && Convert.ToBoolean(Convert.ToByte(reader.Value));
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
}