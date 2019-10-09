/*
 * This file is part of the cv4pve-api-dotnet https://github.com/Corsinvest/cv4pve-api-dotnet,
 * Copyright (C) 2016 Corsinvest Srl
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

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