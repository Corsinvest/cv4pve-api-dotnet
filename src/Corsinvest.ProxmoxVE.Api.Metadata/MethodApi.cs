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
        MethodType = httpMethod.ToUpper();
        MethodName = httpMethod;
        Comment = flat.Comment ?? string.Empty;
        ReturnType = flat.ReturnType ?? string.Empty;
        ReturnLinkHRef = flat.ReturnLinkHRef ?? string.Empty;
        ReturnIsArray = ReturnType == "array";
        ReturnIsNull = ReturnType == "null";
        ClassApi = classApi;
        if (flat.Params != null) { Parameters.AddRange(flat.Params.Select(p => new ParameterApi(p))); }
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

        var parameters = MergeProperties(token["parameters"]);
        Parameters.AddRange([.. parameters.Properties().Select(a => new ParameterApi(a.Value))]);

        ReturnIsArray = ReturnType == "array";
        ReturnIsNull = ReturnType == "null";
    }

    /// <summary>
    /// Parameters of a schema as one object, name to definition: its 'properties', also inside 'allOf'
    /// (all parts apply) and 'oneOf' (one variant applies, chosen by 'type-property'), as in
    /// POST /cluster/ha/rules. A parameter required in only some 'oneOf' variants becomes optional; the
    /// 'type-property' that chooses the variant is added, required, with its allowed values.
    /// </summary>
    /// <param name="schema">'parameters' of a method, or a part of it.</param>
    private static JObject MergeProperties(JToken schema)
    {
        var ret = new JObject();
        if (schema == null || schema.Type != JTokenType.Object) { return ret; }

        void Add(string name, JToken definition)
        {
            if (ret[name] == null) { ret[name] = definition.DeepClone(); }
        }

        if (schema["properties"] is JObject properties)
        {
            foreach (var item in properties.Properties()) { Add(item.Name, item.Value); }
        }

        if (schema["allOf"] is JArray allOf)
        {
            foreach (var part in allOf)
            {
                foreach (var item in MergeProperties(part).Properties()) { Add(item.Name, item.Value); }
            }
        }

        if (schema["oneOf"] is JArray oneOf)
        {
            var variants = oneOf.Select(MergeProperties).ToList();

            //the parameter that chooses the variant
            var typeProperty = schema["type-property"]?.ToString();
            if (!string.IsNullOrEmpty(typeProperty) && schema["type-property-schema"] is JObject typeSchema)
            {
                var definition = (JObject)typeSchema.DeepClone();
                definition["optional"] = 0;
                Add(typeProperty, definition);
            }

            foreach (var name in variants.SelectMany(a => a.Properties().Select(b => b.Name)).Distinct())
            {
                var definition = (JObject)variants.First(a => a[name] != null)[name].DeepClone();

                //required only if required in every variant
                var requiredEverywhere = variants.All(a => a[name] != null && (a[name]["optional"] ?? 0).ToString() != "1");
                if (!requiredEverywhere) { definition["optional"] = 1; }

                Add(name, definition);
            }
        }

        return ret;
    }

    /// <summary>
    /// Href
    /// </summary>
    public string ReturnLinkHRef { get; }

    /// <summary>
    /// Rel
    /// </summary>
    public string ReturnLinkRel { get; }

    /// <summary>
    /// Parameter
    /// </summary>
    public List<ParameterApi> Parameters { get; } = [];

    /// <summary>
    /// Return parameter
    /// </summary>
    public List<ParameterApi> ReturnParameters { get; } = [];

    /// <summary>
    /// Method Type
    /// </summary>
    public string MethodType { get; }

    /// <summary>
    /// Is Get
    /// </summary>
    public bool IsGet => string.Equals(MethodType, "get", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Is Post
    /// </summary>
    public bool IsPost => string.Equals(MethodType, "post", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Is Post
    /// </summary>
    public bool IsPut => string.Equals(MethodType, "put", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Get Method Type Humanized
    /// </summary>
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
    public ClassApi ClassApi { get; }
}
