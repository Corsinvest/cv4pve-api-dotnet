/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json.Linq;

namespace Corsinvest.ProxmoxVE.Api.Metadata;

/// <summary>
/// Method Api
/// </summary>
public class MethodApi
{
    /// <summary>Constructor from flat cache</summary>
    /// <param name="httpMethod">HTTP method (get/post/put/delete)</param>
    /// <param name="flat">Flat cache method info</param>
    /// <param name="classApi">Parent ClassApi node</param>
    internal MethodApi(string httpMethod, FlatMethodInfo flat, ClassApi classApi)
    {
        MethodType       = httpMethod.ToUpper();
        MethodName       = httpMethod;
        Comment          = flat.Comment ?? string.Empty;
        ReturnType       = flat.ReturnType ?? string.Empty;
        ReturnLinkHRef   = flat.ReturnLinkHRef ?? string.Empty;
        ReturnIsArray    = ReturnType == "array";
        ReturnIsNull     = ReturnType == "null";
        ClassApi         = classApi;
        if (flat.Params != null)     { Parameters.AddRange(flat.Params.Select(p => new ParameterApi(p))); }
        if (flat.ReturnParams != null) { ReturnParameters.AddRange(flat.ReturnParams.Select(p => new ParameterApi(p))); }
    }

    /// <summary>Constructor from JSON token</summary>
    public MethodApi(JToken token, ClassApi classApi)
    {
        MethodType = token["method"].ToString();
        MethodName = token["name"].ToString();
        Comment = token["description"] + string.Empty;
        ClassApi = classApi;

        var returns = token["returns"];
        if (returns != null)
        {
            ReturnType = returns["type"] + string.Empty;

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