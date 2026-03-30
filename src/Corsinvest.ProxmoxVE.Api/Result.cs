/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Dynamic;
using System.Net;
using System.Text;

namespace Corsinvest.ProxmoxVE.Api;

/// <summary>
/// Result request API
/// </summary>
public class Result(dynamic response,
                    HttpStatusCode statusCode,
                    string reasonPhrase,
                    bool isSuccessStatusCode,
                    string requestResource,
                    IDictionary<string, object> requestParameters,
                    MethodType methodType,
                    ResponseType responseType)
{
    /// <summary>
    /// Method type
    /// </summary>
    public MethodType MethodType { get; } = methodType;

    /// <summary>
    /// Response type
    /// </summary>
    public ResponseType ResponseType { get; } = responseType;

    /// <summary>
    /// Resource request
    /// </summary>
    public string RequestResource { get; } = requestResource;

    /// <summary>
    /// Request parameter
    /// </summary>
    public IDictionary<string, object> RequestParameters { get; } = requestParameters;

    /// <summary>
    /// Get if response Proxmox VE contain errors
    /// </summary>
    public bool ResponseInError => Response is ExpandoObject && ResponseToDictionary.ContainsKey("errors");

    /// <summary>
    /// Proxmox VE response.
    /// </summary>
    public dynamic Response { get; } = response;

    /// <summary>
    /// Proxmox VE response to dictionary.
    /// </summary>
    public IDictionary<string, object> ResponseToDictionary => (IDictionary<string, object>)Response;

    /// <summary>
    /// Contains the values of status codes defined for HTTP.
    /// </summary>
    public HttpStatusCode StatusCode { get; } = statusCode;

    /// <summary>
    /// Gets the reason phrase which typically is sent by servers together with the status code.
    /// </summary>
    public string ReasonPhrase { get; } = reasonPhrase;

    /// <summary>
    /// Gets a value that indicates if the HTTP response was successful.
    /// </summary>
    public bool IsSuccessStatusCode { get; } = isSuccessStatusCode;

    /// <summary>
    /// Get error
    /// </summary>
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