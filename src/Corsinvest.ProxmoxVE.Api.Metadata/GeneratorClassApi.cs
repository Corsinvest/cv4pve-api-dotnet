/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace Corsinvest.ProxmoxVE.Api.Metadata;

/// <summary>
/// Generator class Api
/// </summary>
public static class GeneratorClassApi
{
    /// <summary>
    /// Generate class Api
    /// </summary>
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

        using (var httpClientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
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

    /// <summary>
    /// Format of the flat cache written by <see cref="BuildFlatCache"/>. A cache of another version is not loaded
    /// (<see cref="LoadFlatCache"/> returns null): build it again from the API schema.
    /// </summary>
    public const int FlatCacheFormatVersion = 2;

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
        return JsonSerializer.Serialize(new { formatVersion = FlatCacheFormatVersion, resources = dict }, FlatWriteOpts);
    }

    /// <summary>
    /// Load a flat cache from a JSON string previously built with BuildFlatCache.
    /// </summary>
    /// <returns>The resources; null for a cache of another <see cref="FlatCacheFormatVersion"/>. A JSON object of
    /// resources without version (written by hand, e.g. in tests) is read as it is.</returns>
    public static Dictionary<string, FlatResourceInfo>? LoadFlatCache(string json)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        if (root.TryGetProperty("formatVersion", out var version))
        {
            return version.TryGetInt32(out var number)
                   && number == FlatCacheFormatVersion
                   && root.TryGetProperty("resources", out var resources)
                    ? resources.Deserialize<Dictionary<string, FlatResourceInfo>>(FlatReadOpts)
                    : null;
        }

        return root.Deserialize<Dictionary<string, FlatResourceInfo>>(FlatReadOpts);
    }

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
            var segments = path.Split(['/'], StringSplitOptions.RemoveEmptyEntries);
            var name = segments[^1];
            var isIndexed = name.StartsWith('{');

            // Find or create parent node
            var parentPath = "/" + string.Join("/", [.. segments.Take(segments.Length - 1)]);
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

    private static string NullIfEmpty(string value) => string.IsNullOrEmpty(value) ? null : value;

    // A copy of every field: a field left out here is lost for every caller that reads the cache.
    private static FlatParamInfo ToFlatParam(ParameterApi p)
        => new(p.Name,
               NullIfEmpty(p.Type),
               NullIfEmpty(p.TypeText),
               NullIfEmpty(p.Description),
               p.Optional ? true : null,
               p.Default,
               p.Minimum,
               p.Maximum,
               p.EnumValues.Length > 0 ? p.EnumValues : null,
               NullIfEmpty(p.Renderer),
               NullIfEmpty(p.VerboseDescription),
               p.Formats.Count > 0 ? [.. p.Formats.Select(ToFlatFormat)] : null,
               p.Items.Count > 0 ? [.. p.Items.Select(ToFlatParam)] : null);

    private static FlatFormatInfo ToFlatFormat(ParameterFormatApi f)
        => new(f.Name,
               NullIfEmpty(f.Type),
               NullIfEmpty(f.Description),
               f.Optional ? true : null,
               f.Minimum,
               f.Maximum,
               NullIfEmpty(f.DefaultKey),
               NullIfEmpty(f.FormatDescription),
               NullIfEmpty(f.Format),
               NullIfEmpty(f.Alias),
               f.MaxLength,
               f.EnumValues.Length > 0 ? f.EnumValues : null);

    private static void Traverse(ClassApi node, Dictionary<string, FlatResourceInfo> dict)
    {
        if (!node.IsRoot)
        {
            var methods = new Dictionary<string, FlatMethodInfo>();
            foreach (var method in node.Methods)
            {
                var ps = method.Parameters.ToArray();
                var rps = method.ReturnParameters.ToArray();
                methods[method.MethodType.ToLower()] = new(NullIfEmpty(method.Comment),
                                                            method.ReturnType,
                                                            method.ReturnLinkHRef,
                                                            ps.Length > 0 ? [.. ps.Select(ToFlatParam)] : null,
                                                            rps.Length > 0 ? [.. rps.Select(ToFlatParam)] : null,
                                                            method.MethodName,
                                                            method.ReturnLinkRel);
            }

            var children = node.SubClasses.Select(c => new FlatChildInfo(c.Name,
                                                                         c.IsIndexed ? true : null,
                                                                         c.SubClasses.Count > 0 ? true : null)).ToArray();

            dict[node.Resource] = new(node.Keys.Count > 0 ? [.. node.Keys] : null,
                                      children.Length > 0 ? children : null,
                                      methods.Count > 0 ? methods : null);
        }
        foreach (var sub in node.SubClasses) { Traverse(sub, dict); }
    }
}
