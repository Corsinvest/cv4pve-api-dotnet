using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;

namespace Corsinvest.ProxmoxVE.Api
{
    /// <summary>
    /// Result request API
    /// </summary>
    public class Result
    {
        internal Result(dynamic response, HttpStatusCode statusCode, string reasonPhrase, bool isSuccessStatusCode)
        {
            Response = response;
            StatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
            IsSuccessStatusCode = isSuccessStatusCode;
        }

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
        public IDictionary<String, object> ResponseToDictionary => (IDictionary<String, object>)Response;

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
            var ret = "";
            if (ResponseInError)
            {
                foreach (var item in (IDictionary<string, object>)Response.errors)
                {
                    if (!string.IsNullOrWhiteSpace(ret)) { ret += Environment.NewLine; }
                    ret += $"{item.Key} : {item.Value}";
                }
            }
            return ret;
        }
    }
}