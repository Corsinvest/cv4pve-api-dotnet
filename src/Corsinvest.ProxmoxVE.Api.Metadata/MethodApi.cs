/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json.Linq;

namespace Corsinvest.ProxmoxVE.Api.Metadata;

/// <summary>
/// Method Api
/// </summary>
public class MethodApi
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="token"></param>
    /// <param name="classApi"></param>
    public MethodApi(JToken token, ClassApi classApi)
    {
        MethodType = token["method"].ToString();
        MethodName = token["name"].ToString();
        Comment = token["description"] + "";
        ClassApi = classApi;

        var returns = token["returns"];
        if (returns != null)
        {
            ReturnType = returns["type"] + "";

            if (returns["properties"] != null)
            {
                ReturnParameters.AddRange([.. returns["properties"].Select(a => new ParameterApi(a.Parent[((JProperty)a).Name]))]);
            }
            else if (returns["items"]?["properties"] != null)
            {
                ReturnParameters.AddRange([.. returns["items"]["properties"].Select(a => new ParameterApi(a.Parent[((JProperty)a).Name]))]);
            }

            if (returns["links"] != null)
            {
                ReturnLinkHRef = returns["links"][0]["href"].ToString();
                ReturnLinkRel = returns["links"][0]["rel"].ToString();
            }
        }

        if (token["parameters"]?["properties"] != null)
        {
            Parameters.AddRange([.. token["parameters"]["properties"].Select(a => new ParameterApi(a.Parent[((JProperty)a).Name]))]);
        }

        ReturnIsArray = ReturnType == "array";
        ReturnIsNull = ReturnType == "null";
    }

    /// <summary>
    /// Href
    /// </summary>
    /// <value></value>
    public string ReturnLinkHRef { get; }

    /// <summary>
    /// Rel
    /// </summary>
    /// <value></value>
    public string ReturnLinkRel { get; }

    /// <summary>
    /// Parameter
    /// </summary>
    /// <returns></returns>
    public List<ParameterApi> Parameters { get; } = [];

    /// <summary>
    /// Return parameter
    /// </summary>
    /// <returns></returns>
    public List<ParameterApi> ReturnParameters { get; } = [];

    /// <summary>
    /// Method Type
    /// </summary>
    /// <value></value>
    public string MethodType { get; }

    /// <summary>
    /// Is Get
    /// </summary>
    /// <returns></returns>
    public bool IsGet => string.Equals(MethodType, "get", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Is Post
    /// </summary>
    /// <returns></returns>
    public bool IsPost => string.Equals(MethodType, "post", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Is Post
    /// </summary>
    /// <returns></returns>
    public bool IsPut => string.Equals(MethodType, "put", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Get Method Type Humanized
    /// </summary>
    /// <returns></returns>
    public string GetMethodTypeHumanized()
    {
        var name = MethodType.ToLower();
        return name switch
        {
            "post" => "create",
            "put" => "set",
            _ => name,
        };
    }

    /// <summary>
    /// Return type
    /// </summary>
    public string ReturnType { get; }

    /// <summary>
    /// Return Is Array
    /// </summary>
    public bool ReturnIsArray { get; }

    /// <summary>
    /// Return Is Null
    /// </summary>
    public bool ReturnIsNull { get; }

    /// <summary>
    /// Method name
    /// </summary>
    public string MethodName { get; }

    /// <summary>
    /// Comment
    /// </summary>
    public string Comment { get; }

    /// <summary>
    /// Class Api
    /// </summary>
    /// <value></value>
    public ClassApi ClassApi { get; }
}