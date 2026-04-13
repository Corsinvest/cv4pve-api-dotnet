/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace Corsinvest.ProxmoxVE.Api.Metadata;

/// <summary>
/// Class Api
/// </summary>
public partial class ClassApi
{
    /// <summary>
    /// Constructor
    /// </summary>
    public ClassApi() { IsRoot = true; }

    /// <summary>
    /// Constructor for rebuilding from flat cache
    /// </summary>
    internal ClassApi(string resource, string name, bool isIndexed, ClassApi parent)
    {
        Resource = resource;
        Name = name;
        IsIndexed = isIndexed;
        Parent = parent;
        parent.SubClasses.Add(this);
        Keys.AddRange(parent.Keys);
        if (isIndexed) { Keys.Add(name.Replace("{", string.Empty).Replace("}", string.Empty)); }
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public ClassApi(JToken token, ClassApi parent)
    {
        Name = token["text"].ToString().Replace("-", "_");
        IndexName = Name.Replace("{", string.Empty).Replace("}", string.Empty);
        Parent = parent;
        Resource = token["path"].ToString();

        Resource = ResourcePlaceholderRegex().Replace(Resource, match => "{" + match.Groups[1].Value.Replace('-', '_') + "}");

        Keys.AddRange(parent.Keys);
        parent.SubClasses.Add(this);

        IsIndexed = token["text"].ToString().StartsWith('{');
        if (IsIndexed) { Keys.Add(IndexName); }

        if (token["info"] != null)
        {
            Methods.AddRange([.. token["info"].Select(a => new MethodApi(a.Parent[((JProperty)a).Name], this))]);
        }

        //tree children
        if (token["children"] != null)
        {
            foreach (var child in token["children"]) { _ = new ClassApi(child, this); }
        }
    }

    /// <summary>
    /// Get From Resource
    /// </summary>
    public static async Task<ClassApi> GetFromResourceAsync(string host, int port, string resource)
        => GetFromResource(await GeneratorClassApi.GenerateAsync(host, port), resource);

    /// <summary>
    /// Get From Resource
    /// </summary>
    public static ClassApi GetFromResource(ClassApi classApiRoot, string resource)
    {
        var classApi = classApiRoot;
        foreach (var item in resource.Split('/'))
        {
            if (string.IsNullOrWhiteSpace(item)) { continue; }
            var oldClassApi = classApi;
            classApi = classApi.SubClasses.FirstOrDefault(a => a.Resource == classApi.Resource + "/" + item);
            classApi ??= oldClassApi.SubClasses.FirstOrDefault(a => a.IsIndexed);
            if (classApi == null) { break; }
        }

        return classApi;
    }

    /// <summary>
    /// Is Root
    /// </summary>
    public bool IsRoot { get; }

    /// <summary>
    /// Name of class
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Index Name
    /// </summary>
    public string IndexName { get; }

    /// <summary>
    /// Parent class
    /// </summary>
    public ClassApi Parent { get; }

    /// <summary>
    /// Sub classes
    /// </summary>
    public IList<ClassApi> SubClasses { get; } = [];

    /// <summary>
    /// Methods
    /// </summary>
    public List<MethodApi> Methods { get; } = [];

    /// <summary>
    /// Keys class
    /// </summary>
    public List<string> Keys { get; } = [];

    /// <summary>
    /// Is Index
    /// </summary>
    public bool IsIndexed { get; }

    /// <summary>
    /// Path resource
    /// </summary>
    public string Resource { get; }

    [GeneratedRegex(@"\{([^}]*)\}")]
    private static partial Regex ResourcePlaceholderRegex();
}