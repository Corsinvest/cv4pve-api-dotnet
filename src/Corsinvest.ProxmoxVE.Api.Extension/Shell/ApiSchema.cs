/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

#nullable enable

using System.Collections;
using System.Dynamic;
using Corsinvest.ProxmoxVE.Api.Metadata;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Extension.Shell;

/// <summary>A method a path accepts.</summary>
/// <param name="Method">Method.</param>
/// <param name="Description">What the call does, from the API schema.</param>
/// <param name="Parameters">Parameters of the call, without the ones already in the path.</param>
/// <param name="Returns">Fields of the answer the schema describes.</param>
public sealed record ApiMethodInfo(MethodType Method,
                                   string Description,
                                   IReadOnlyList<ParameterApi> Parameters,
                                   IReadOnlyList<ParameterApi> Returns);

/// <summary>Something under a path: a fixed name, or a value read from the cluster (a node, a VM ID…).</summary>
/// <param name="Name">Name or value.</param>
/// <param name="HasChildren">It has paths under it.</param>
/// <param name="AcceptsCreate">It accepts <c>create</c> (POST).</param>
public sealed record ApiChild(string Name, bool HasChildren, bool AcceptsCreate);

/// <summary>What is under a path, or why it could not be read.</summary>
public sealed record ApiChildrenResult(IReadOnlyList<ApiChild> Children, string? Error);

/// <summary>How <see cref="ApiSchema.ToTable(object?, ClassApi, string, ApiTableOptions?)"/> shows the data.</summary>
/// <param name="AllColumns">A list shows every key of its items, not only those the schema describes.</param>
/// <param name="HumanReadable">Values rendered as the schema says (sizes, percentages, durations, dates), as pvesh
/// does; false keeps them as the API returns them, e.g. for Json.</param>
public sealed record ApiTableOptions(bool AllColumns = false, bool HumanReadable = true);

/// <summary>The API schema as data: methods, parameters and children of a path.</summary>
public static class ApiSchema
{
    /// <summary>Methods <paramref name="resource"/> accepts; null when the path is not in the API.</summary>
    /// <param name="root">API schema.</param>
    /// <param name="resource">Path, with real values or placeholders.</param>
    public static IReadOnlyList<ApiMethodInfo>? GetMethods(ClassApi root, string resource)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(resource);

        var classApi = ClassApi.GetFromResource(root, resource);
        return classApi == null
                ? null
                : [.. classApi.Methods
                              .Select(a => new ApiMethodInfo(ToMethodType(a.MethodType),
                                                             a.Comment ?? string.Empty,
                                                             [.. a.Parameters.Where(p => !classApi.Keys.Contains(p.Name))],
                                                             [.. a.ReturnParameters]))
                              .OrderBy(a => a.Method)];
    }

    /// <summary>One method of <paramref name="resource"/>, or null.</summary>
    public static ApiMethodInfo? GetMethod(ClassApi root, string resource, MethodType method)
        => GetMethods(root, resource)?.FirstOrDefault(a => a.Method == method);

    /// <summary>Values a parameter accepts: its list of values, or 0 and 1 for a boolean; empty otherwise.</summary>
    public static IReadOnlyList<string> GetAllowedValues(ParameterApi parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter);
        return parameter.EnumValues.Length > 0
                ? parameter.EnumValues
                : string.Equals(parameter.Type, "boolean", StringComparison.OrdinalIgnoreCase)
                    ? ["0", "1"]
                    : [];
    }

    /// <summary>
    /// What is under <paramref name="resource"/>, sorted by name. Fixed names come from the schema; indexed
    /// children (nodes, VM IDs, storages…) are read from the cluster with a GET on the path.
    /// </summary>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was cancelled.</exception>
    public static async Task<ApiChildrenResult> GetChildrenAsync(PveClient client,
                                                                 ClassApi root,
                                                                 string resource,
                                                                 CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(resource);
        cancellationToken.ThrowIfCancellationRequested();

        var classApi = ClassApi.GetFromResource(root, resource);
        if (classApi == null) { return new([], $"no such resource '{resource}'"); }
        if (classApi.SubClasses.Count == 0) { return new([], $"resource '{resource}' does not define child links"); }

        var children = new List<ApiChild>();
        string? error = null;
        foreach (var subClass in classApi.SubClasses.OrderBy(a => a.Name))
        {
            var hasChildren = subClass.SubClasses.Count > 0;
            var acceptsCreate = subClass.Methods.Any(a => a.IsPost);

            if (!subClass.IsIndexed)
            {
                children.Add(new(subClass.Name, hasChildren, acceptsCreate));
                continue;
            }

            var (values, readError) = await ReadIndexedValuesAsync(client, classApi, resource);
            error ??= readError;
            children.AddRange(values.Select(a => new ApiChild(a, hasChildren, acceptsCreate)));
        }

        return new(children, error);
    }

    /// <inheritdoc cref="ToTable(object?, ClassApi, string, ApiTableOptions?, out IReadOnlyList{string})"/>
    public static TableGenerator? ToTable(object? data, ClassApi root, string resource, ApiTableOptions? options = null)
        => ToTable(data, root, resource, options, out _);

    /// <summary>
    /// The "data" of an API answer as a table, as pvesh shows it: an object as key/value rows with every key; a list
    /// as one row per item with the columns the schema names (numbered keys such as net0 match <c>net[n]</c>),
    /// or every column with <see cref="ApiTableOptions.AllColumns"/>, sorted by the first column; a list of strings
    /// as one "value" column. With <see cref="ApiTableOptions.HumanReadable"/> values are rendered as the schema says
    /// and those columns are aligned right. Null for no data or a single value: print it as it is. An empty object or
    /// list is a table with no columns.
    /// </summary>
    /// <param name="data">The "data" of the answer (<see cref="ApiResponse.Data"/>).</param>
    /// <param name="root">API schema.</param>
    /// <param name="resource">Path of the call.</param>
    /// <param name="options">How to show the data; null for the defaults.</param>
    /// <param name="hiddenColumns">Keys of a list left out because the schema does not name them.</param>
    public static TableGenerator? ToTable(object? data,
                                          ClassApi root,
                                          string resource,
                                          ApiTableOptions? options,
                                          out IReadOnlyList<string> hiddenColumns)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(resource);
        options ??= new();
        var allColumns = options.AllColumns;
        hiddenColumns = [];

        var returns = ClassApi.GetFromResource(root, resource)?.Methods.FirstOrDefault(a => a.IsGet)?.ReturnParameters ?? [];
        var schemaKeys = returns.OrderBy(a => a.Optional).ThenBy(a => a.Name).Select(a => a.Name).ToList();

        // "net[n]" in the schema stands for net0, net1…
        static bool IsNumbered(string schemaKey, string key)
            => schemaKey.EndsWith("[n]", StringComparison.Ordinal)
               && key.Length > schemaKey.Length - 3
               && key.StartsWith(schemaKey[..^3], StringComparison.Ordinal)
               && key[(schemaKey.Length - 3)..].All(char.IsAsciiDigit);

        ParameterApi? Parameter(string key)
            => returns.FirstOrDefault(a => a.Name == key) ?? returns.FirstOrDefault(a => IsNumbered(a.Name, key));

        object? Render(object? value, string key)
        {
            if (value == null) { return null; }
            if (value is ExpandoObject or IList) { return JsonConvert.SerializeObject(value); }
            return options.HumanReadable && Parameter(key) is { } parameter ? parameter.RendererValue(value) : value;
        }

        // A rendered value is text: the column keeps the alignment of what it shows (sizes, percentages… right).
        TableGenerator.Align? AlignOf(string key)
            => options.HumanReadable && Parameter(key) is { Renderer.Length: > 0 } parameter
                ? parameter.GetAlignmentValue() == "R" ? TableGenerator.Align.Right : TableGenerator.Align.Left
                : null;

        switch (data)
        {
            case IDictionary<string, object?> values:
                {
                    var keys = values.Keys.Order().ToList();
                    if (keys.Count == 0) { return new TableGenerator(); }

                    var table = new TableGenerator("key", "value");
                    foreach (var key in keys) { table.AddRow(key, Render(values[key], key)); }
                    return table;
                }

            case IList list when list.Count > 0 && list[0] is string:
                return TableGenerator.From(list.Cast<string>()).Column(a => a).Title("value").Build();

            case IList list:
                {
                    var items = list.OfType<IDictionary<string, object?>>().ToList();
                    var dataKeys = items.SelectMany(a => a.Keys).Distinct().Order().ToList();

                    // Schema columns in schema order, a numbered one expanded to the keys of the data (net0, net1, net10).
                    var schemaColumns = schemaKeys.SelectMany<string, string>(schemaKey => schemaKey.EndsWith("[n]", StringComparison.Ordinal)
                                                                             ? dataKeys.Where(a => IsNumbered(schemaKey, a))
                                                                                       .OrderBy(a => long.Parse(a[(schemaKey.Length - 3)..]))
                                                                             : [schemaKey])
                                                  .Distinct()
                                                  .ToList();
                    var others = dataKeys.Where(a => !schemaColumns.Contains(a)).ToList();
                    var columns = schemaKeys.Count == 0 || allColumns
                                    ? [.. schemaColumns, .. others]
                                    : schemaColumns;
                    if (schemaKeys.Count > 0 && !allColumns) { hiddenColumns = others; }
                    if (items.Count == 0 || columns.Count == 0) { return new TableGenerator(); }

                    string Shown(IDictionary<string, object?> item)
                        => item.TryGetValue(columns[0], out var value) ? Render(value, columns[0]) + string.Empty : string.Empty;

                    var builder = TableGenerator.From(items.OrderBy(Shown));
                    foreach (var column in columns)
                    {
                        var added = builder.Column(column).Format(value => Render(value, column));
                        if (AlignOf(column) is { } align) { added.Align(align); }
                    }
                    return builder.Build();
                }

            default:
                return null;
        }
    }

    private static async Task<(IEnumerable<string> Values, string? Error)> ReadIndexedValuesAsync(PveClient client, ClassApi classApi, string resource)
    {
        var result = await client.GetAsync(resource);
        if (!result.IsSuccessStatusCode || result.ResponseInError)
        {
            var detail = result.ResponseInError ? result.GetError() : string.Empty;
            return ([], string.IsNullOrWhiteSpace(detail) ? result.ReasonPhrase : detail);
        }

        // The field holding the index is named by the link of the GET, e.g. "{node}" → "node".
        var key = classApi.Methods.FirstOrDefault(a => a.IsGet)?.ReturnLinkHRef?.Trim('{', '}');
        var answer = result.Response as IDictionary<string, object>;
        if (string.IsNullOrWhiteSpace(key)
            || answer == null
            || !answer.TryGetValue("data", out var data)
            || data is not IEnumerable<object> items)
        {
            return ([], null);
        }

        var values = items.OfType<IDictionary<string, object>>()
                          .Select(a => a.TryGetValue(key, out var value) ? value : null)
                          .OfType<object>()
                          .ToList();

        // VM IDs are numbers: 99 before 100.
        var ordered = values.All(a => a is byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal)
                        ? values.OrderBy(a => Convert.ToDecimal(a, System.Globalization.CultureInfo.InvariantCulture))
                        : values.OrderBy(a => a + string.Empty);
        return ([.. ordered.Select(a => a + string.Empty)], null);
    }

    private static MethodType ToMethodType(string method)
        => method.ToLowerInvariant() switch
        {
            "get" => MethodType.Get,
            "put" => MethodType.Set,
            "post" => MethodType.Create,
            "delete" => MethodType.Delete,
            _ => throw new ArgumentOutOfRangeException(nameof(method), method, "Unknown method"),
        };
}
