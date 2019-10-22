/*
 * This file is part of the cv4pve-api-dotnet https://github.com/Corsinvest/cv4pve-api-dotnet,
 *
 * This source file is available under two different licenses:
 * - GNU General Public License version 3 (GPLv3)
 * - Corsinvest Enterprise License (CEL)
 * Full copyright and license information is available in
 * LICENSE.md which is distributed with this source code.
 *
 * Copyright (C) 2016 Corsinvest Srl	GPLv3 and CEL
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