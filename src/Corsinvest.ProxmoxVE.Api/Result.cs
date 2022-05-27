/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Text;

namespace Corsinvest.ProxmoxVE.Api
{
    /// <summary>
    /// Result request API
    /// </summary>
    public class Result
    {
        internal Result(dynamic response,
                        HttpStatusCode statusCode,
                        string reasonPhrase,
                        bool isSuccessStatusCode,
                        string requestResource,
                        IDictionary<string, object> requestParameters,
                        MethodType methodType,
                        ResponseType responseType)
        {
            Response = response;
            StatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
            IsSuccessStatusCode = isSuccessStatusCode;
            RequestResource = requestResource;
            RequestParameters = requestParameters;
            MethodType = methodType;
            ResponseType = responseType;
        }

        /// <summary>
        /// Method type
        /// </summary>
        /// <value></value>
        public MethodType MethodType { get; }

        /// <summary>
        /// Response type
        /// </summary>
        /// <value></value>
        public ResponseType ResponseType { get; }

        /// <summary>
        /// Resource request
        /// </summary>
        /// <value></value>
        public string RequestResource { get; }

        /// <summary>
        /// Request parameter
        /// </summary>
        /// <value></value>
        public IDictionary<string, object> RequestParameters { get; }

        /// <summary>
        /// Get if response Proxmox VE contain errors
        /// </summary>
        /// <returns></returns>
        public bool ResponseInError => Response is ExpandoObject && ResponseToDictionary.ContainsKey("errors");

        /// <summary>
        /// Proxmox VE response.
        /// </summary>
        /// <returns></returns>
        public dynamic Response { get; }

        /// <summary>
        /// Proxmox VE response to dictionary.
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, object> ResponseToDictionary => (IDictionary<string, object>)Response;

        /// <summary>
        /// Contains the values of status codes defined for HTTP.
        /// </summary>
        /// <returns></returns>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Gets the reason phrase which typically is sent by servers together with the status code.
        /// </summary>
        /// <returns></returns>
        public string ReasonPhrase { get; }

        /// <summary>
        /// Gets a value that indicates if the HTTP response was successful.
        /// </summary>
        /// <returns></returns>
        public bool IsSuccessStatusCode { get; }

        /// <summary>
        /// Get error
        /// </summary>
        /// <returns></returns>
        public string GetError()
        {
            var ret = new StringBuilder();
            if (ResponseInError)
            {
                foreach (var item in (IDictionary<string, object>)Response.errors)
                {
                    if (!string.IsNullOrWhiteSpace(ret.ToString())) { ret.AppendLine(); }
                    ret.Append($"{item.Key} : {item.Value}");
                }
            }
            return ret.ToString();
        }
    }
}