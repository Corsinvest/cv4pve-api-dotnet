/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

#nullable enable

using System.Text;
using Corsinvest.ProxmoxVE.Api.Metadata;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;

namespace Corsinvest.ProxmoxVE.Api.Extension.Shell;

/// <summary>
/// The data of <see cref="ApiSchema"/> as text, for a terminal or a chat. Optional: a caller that wants another
/// format uses <see cref="ApiSchema"/> directly.
/// </summary>
public static class ApiSchemaText
{
    private const int DescriptionWidth = 45;
    private const int TypeWidth = 18;

    /// <summary>One USAGE line per method; verbose adds the description and the parameters, returns the answer fields.</summary>
    /// <param name="resource">Path shown in the USAGE line.</param>
    /// <param name="methods">Methods to show, in this order (see <see cref="ApiSchema.GetMethods"/>).</param>
    /// <param name="verbose">Add the description and the table of the parameters.</param>
    /// <param name="returns">Add the table of the fields of the answer.</param>
    /// <param name="output">Format of the tables.</param>
    /// <param name="optionStyle">Required parameters as <c>--name &lt;type&gt;</c>; otherwise <c>name:&lt;type&gt;</c>.</param>
    public static string Usage(string resource,
                               IEnumerable<ApiMethodInfo> methods,
                               bool verbose,
                               bool returns,
                               TableGenerator.Output output,
                               bool optionStyle = true)
    {
        ArgumentNullException.ThrowIfNull(resource);
        ArgumentNullException.ThrowIfNull(methods);

        var ret = new StringBuilder();
        foreach (var method in methods)
        {
            ret.Append($"USAGE: {method.Method.ToString().ToLower()} {resource}");
            ret.Append(string.Concat(method.Parameters.Where(a => !a.Optional).Select(a => optionStyle ? $" --{a.Name} <{a.Type}>" : $" {a.Name}:<{a.Type}>")));
            if (method.Parameters.Any(a => a.Optional)) { ret.Append(" [OPTIONS]"); }
            ret.AppendLine();

            if (verbose)
            {
                ret.AppendLine().AppendLine("  " + method.Description);
                AppendTable(ret, method.Parameters, output);
            }

            if (returns)
            {
                ret.AppendLine("RETURNS:");
                AppendTable(ret, method.Returns, output);
            }

            if (verbose) { ret.AppendLine(); }
        }

        return ret.ToString();
    }

    /// <summary>Rows param/type/description, with long types and descriptions split on more lines.</summary>
    public static List<string[]> ParameterRows(IEnumerable<ParameterApi> parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        var rows = new List<string[]>();
        foreach (var parameter in parameters)
        {
            var description = SplitLines(parameter.Description.Replace("\n", " ").Trim().Split(' '), DescriptionWidth, " ");
            var type = !string.IsNullOrWhiteSpace(parameter.TypeText)
                        ? SplitLines(parameter.TypeText.Split(' '), TypeWidth, string.Empty)
                        : parameter.EnumValues.Length > 0
                            ? SplitLines(parameter.EnumValues, TypeWidth, ",")
                            : [parameter.Type];

            for (var i = 0; i < Math.Max(type.Length, description.Length); i++)
            {
                rows.Add([i == 0 ? parameter.Name : string.Empty,
                          i < type.Length ? type[i] : string.Empty,
                          i < description.Length ? description[i] : string.Empty]);
            }
        }

        return rows;
    }

    /// <summary>One line per child: attributes (D children, r read, c create) and name; then the error, if any.</summary>
    public static string List(ApiChildrenResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var ret = new StringBuilder();
        foreach (var child in result.Children)
        {
            ret.AppendLine($"{(child.HasChildren ? "D" : "-")}r--{(child.AcceptsCreate ? "c" : "-")}        {child.Name}");
        }

        if (!string.IsNullOrWhiteSpace(result.Error)) { ret.AppendLine(result.Error); }
        return ret.ToString();
    }

    private static void AppendTable(StringBuilder ret, IReadOnlyList<ParameterApi> parameters, TableGenerator.Output output)
    {
        if (parameters.Count == 0) { return; }
        ret.Append(new TableGenerator("param", "type", "description").AddRows(ParameterRows(parameters)).To(output));
    }

    private static string[] SplitLines(IEnumerable<string> words, int width, string separator)
    {
        var lines = new List<string>();
        var line = new StringBuilder();
        foreach (var word in words)
        {
            if (!string.IsNullOrWhiteSpace(line.ToString())) { line.Append(separator); }
            line.Append(word);
            if (line.Length >= width)
            {
                lines.Add(line.ToString().Trim());
                line.Clear();
            }
        }

        if (!string.IsNullOrWhiteSpace(line.ToString())) { lines.Add(line.ToString().Trim()); }
        return [.. lines];
    }
}
