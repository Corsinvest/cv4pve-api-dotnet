/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Corsinvest.ProxmoxVE.Api.Metadata;

/// <summary>
/// Generator class Api
/// </summary>
public static class GeneratorClassApi
{
    /// <summary>
    /// Generate class Api
    /// </summary>
    /// <param name="host"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    public static async Task<ClassApi> GenerateAsync(string host = "pve.proxmox.com", int port = 443)
    {
        var classApi = new ClassApi();
        foreach (var token in JArray.Parse(await GetJsonSchemaFromApiDocAsync(host, port))) { _ = new ClassApi(token, classApi); }
        return classApi;
    }

    /// <summary>
    /// Fetches the JSON schema from the Proxmox VE API documentation.
    /// </summary>
    /// <param name="host">The Proxmox VE host address.</param>
    /// <param name="port">The port number.</param>
    /// <returns>The JSON schema string extracted from the API documentation.</returns>
    public static async Task<string> GetJsonSchemaFromApiDocAsync(string host, int port)
    {
        var url = $"https://{host}:{port}/pve-docs/api-viewer/apidoc.js";
        var json = new StringBuilder();

#pragma warning disable S4830 // Server certificates should be verified during SSL/TLS connections
        using (var httpClientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
#pragma warning restore S4830 // Server certificates should be verified during SSL/TLS connections
        })
        using (var client = new HttpClient(httpClientHandler))
        using (var response = await client.GetAsync(url))
        {
            var data = await response.Content.ReadAsStringAsync();
            //start Json API
            data = data[data.IndexOf('[')..];

            foreach (var line in data.Split('\n'))
            {
                json.Append(line);
                //end Json API
                if (line[..1] == "]") { break; }
            }
        }

        return json.ToString();
    }

    private static readonly JsonSerializerOptions FlatWriteOpts = new()
    {
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly JsonSerializerOptions FlatReadOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Build a compact flat JSON cache from a ClassApi tree.
    /// </summary>
    public static string BuildFlatCache(ClassApi root)
    {
        var dict = new Dictionary<string, FlatResourceInfo>();
        Traverse(root, dict);
        return JsonSerializer.Serialize(dict, FlatWriteOpts);
    }

    /// <summary>
    /// Load a flat cache from a JSON string previously built with BuildFlatCache.
    /// </summary>
    public static Dictionary<string, FlatResourceInfo>? LoadFlatCache(string json)
        => JsonSerializer.Deserialize<Dictionary<string, FlatResourceInfo>>(json, FlatReadOpts);

    /// <summary>
    /// Rebuild a ClassApi tree from a flat cache dictionary.
    /// </summary>
    public static ClassApi BuildClassApiFromFlat(Dictionary<string, FlatResourceInfo> flat)
    {
        var root = new ClassApi();
        // Sort by path depth so parents are created before children
        foreach (var kv in flat.OrderBy(kv => kv.Key.Count(c => c == '/')))
        {
            var path = kv.Key;
            var info = kv.Value;
            var segments = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var name     = segments.Last();
            var isIndexed = name.StartsWith("{");

            // Find or create parent node
            var parentPath = "/" + string.Join("/", segments.Take(segments.Length - 1).ToArray());
            var parent = segments.Length == 1
                ? root
                : ClassApi.GetFromResource(root, parentPath) ?? root;

            var node = new ClassApi(path, name, isIndexed, parent);

            if (info.Methods != null)
            {
                foreach (var mkv in info.Methods)
                {
                    node.Methods.Add(new MethodApi(mkv.Key, mkv.Value, node));
                }
            }
        }
        return root;
    }

    private static FlatParamInfo ToFlatParam(ParameterApi p) => new(
        p.Name,
        string.IsNullOrEmpty(p.Type)        ? null : p.Type,
        string.IsNullOrEmpty(p.TypeText)    ? null : p.TypeText,
        string.IsNullOrEmpty(p.Description) ? null : p.Description,
        p.Optional ? true : null,
        string.IsNullOrEmpty(p.Default)     ? null : p.Default,
        p.Minimum,
        p.Maximum,
        p.EnumValues.Length > 0 ? p.EnumValues
            : string.Equals(p.Type, "boolean", StringComparison.OrdinalIgnoreCase) ? ["0", "1"]
            : null);

    private static void Traverse(ClassApi node, Dictionary<string, FlatResourceInfo> dict)
    {
        if (!node.IsRoot)
        {
            var methods = new Dictionary<string, FlatMethodInfo>();
            foreach (var method in node.Methods)
            {
                var ps  = method.Parameters.Where(p => !node.Keys.Contains(p.Name)).ToArray();
                var rps = method.ReturnParameters.ToArray();
                methods[method.MethodType.ToLower()] = new FlatMethodInfo(
                    string.IsNullOrEmpty(method.Comment)        ? null : method.Comment,
                    string.IsNullOrEmpty(method.ReturnType)     ? null : method.ReturnType,
                    string.IsNullOrEmpty(method.ReturnLinkHRef) ? null : method.ReturnLinkHRef,
                    ps.Length  > 0 ? ps.Select(ToFlatParam).ToArray()  : null,
                    rps.Length > 0 ? rps.Select(ToFlatParam).ToArray() : null);
            }

            var children = node.SubClasses.Select(c => new FlatChildInfo( 
                c.Name,
                c.IsIndexed ? true : null,
                c.SubClasses.Count > 0 ? true : null)).ToArray();

            dict[node.Resource] = new FlatResourceInfo(
                node.Keys.Count > 0 ? [.. node.Keys] : null,
                children.Length > 0 ? children : null,
                methods.Count > 0 ? methods : null);
        }
        foreach (var sub in node.SubClasses) { Traverse(sub, dict); }
    }
}