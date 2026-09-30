/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

#nullable enable

using System.Globalization;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Utils;

/// <summary>
/// A table of columns and rows, written as Text, Markdown, Html or Json.
/// Build it with <see cref="From{T}(IEnumerable{T})"/> and <c>Column(...)</c>, or with the constructor and
/// <see cref="AddRow"/>. Numbers are aligned right, everything else left, unless a column says otherwise.
/// </summary>
public sealed partial class TableGenerator
{
    /// <summary>Format of the table.</summary>
    public enum Output
    {
        /// <summary>Text with borders.</summary>
        Text,

        /// <summary>Html table.</summary>
        Html,

        /// <summary>Markdown table.</summary>
        Markdown,

        /// <summary>Json array of objects.</summary>
        Json,

        /// <summary>Indented Json array of objects.</summary>
        JsonPretty,
    }

    /// <summary>Alignment of a column.</summary>
    public enum Align
    {
        /// <summary>Left.</summary>
        Left,

        /// <summary>Right.</summary>
        Right,
    }

    private static readonly HashSet<Type> NumberTypes =
    [
        typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint),
        typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal),
    ];

    private static readonly string[] NewLines = ["\r\n", "\n", "\r"];

    private readonly List<string> _columns = [];
    private readonly List<Align?> _aligns = [];
    private readonly List<object?[]> _rows = [];

    /// <summary>Table with these columns; the alignment of each column comes from its values.</summary>
    public TableGenerator(params string[] columns)
    {
        foreach (var column in columns ?? []) { AddColumn(column); }
    }

    /// <summary>Titles of the columns.</summary>
    public IReadOnlyList<string> Columns => _columns;

    /// <summary>Values of the rows, one per column.</summary>
    public IReadOnlyList<IReadOnlyList<object?>> Rows => _rows;

    /// <summary>Adds a column; <paramref name="align"/> null means from the values.</summary>
    public TableGenerator AddColumn(string title, Align? align = null)
    {
        ArgumentNullException.ThrowIfNull(title);
        if (_rows.Count > 0) { throw new ArgumentException("Columns cannot be added after rows."); }
        if (_columns.Contains(title)) { throw new ArgumentException($"Column '{title}' is already defined."); }

        _columns.Add(title);
        _aligns.Add(align);
        return this;
    }

    /// <summary>Adds a row, one value per column.</summary>
    public TableGenerator AddRow(params object?[] values)
    {
        values ??= [null];
        if (values.Length != _columns.Count)
        {
            throw new ArgumentException($"Row has {values.Length} values, the table has {_columns.Count} columns.");
        }

        _rows.Add(values);
        return this;
    }

    /// <summary>Adds rows, one value per column each.</summary>
    public TableGenerator AddRows(IEnumerable<IEnumerable<object?>> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        foreach (var row in rows) { AddRow([.. row]); }
        return this;
    }

    /// <summary>The table in <paramref name="output"/> format.</summary>
    public string To(Output output)
        => output switch
        {
            Output.Text => ToText(),
            Output.Html => ToHtml(),
            Output.Markdown => ToMarkdown(),
            Output.Json => ToJson(false),
            Output.JsonPretty => ToJson(true),
            _ => throw new ArgumentOutOfRangeException(nameof(output), output, "Unknown output"),
        };

    /// <summary>Text with borders; a value with newlines takes more lines.</summary>
    public string ToText()
    {
        if (_columns.Count == 0) { return string.Empty; }

        static string[] Lines(string text) => text.Split(NewLines, StringSplitOptions.None);

        var header = _columns.Select(Lines).ToArray();
        var cells = _rows.Select(row => row.Select(a => Lines(CellText(a))).ToArray()).ToList();
        var widths = Widths(cells.Prepend(header).Select(row => row.Select(lines => lines.Max(a => a.Length)).ToArray()));
        var aligns = ResolveAligns();
        var divider = "+" + string.Join("+", widths.Select(a => new string('-', a + 2))) + "+";

        var ret = new StringBuilder();
        void AppendRow(string[][] row)
        {
            for (var i = 0; i < row.Max(a => a.Length); i++)
            {
                ret.AppendLine(Line(row.Select(lines => i < lines.Length ? lines[i] : string.Empty).ToList(), widths, aligns));
            }
        }

        ret.AppendLine(divider);
        AppendRow(header);
        ret.AppendLine(divider);
        foreach (var row in cells) { AppendRow(row); }
        ret.AppendLine(divider);
        return ret.ToString();
    }

    /// <summary>Markdown table; <c>|</c> escaped, newlines as <c>&lt;br&gt;</c>.</summary>
    public string ToMarkdown()
    {
        if (_columns.Count == 0) { return string.Empty; }

        static string Escape(string text) => string.Join("<br>", text.Replace("|", "\\|").Split(NewLines, StringSplitOptions.None));

        var header = _columns.Select(Escape).ToList();
        var cells = _rows.Select(row => row.Select(a => Escape(CellText(a))).ToList()).ToList();
        var widths = Widths(cells.Prepend(header).Select(row => row.Select(a => a.Length).ToArray()));
        var aligns = ResolveAligns();

        var ret = new StringBuilder();
        ret.AppendLine(Line(header, widths, aligns));
        ret.AppendLine("|" + string.Join("|", widths.Select((a, i) => new string('-', a + 1)
                                                                        + (aligns[i] == Align.Right ? ":" : "-"))) + "|");
        foreach (var row in cells) { ret.AppendLine(Line(row, widths, aligns)); }
        return ret.ToString();
    }

    /// <summary>Html table; values escaped, newlines as <c>&lt;br&gt;</c>.</summary>
    public string ToHtml()
    {
        if (_columns.Count == 0) { return string.Empty; }

        var aligns = ResolveAligns();
        string Row(string tag, IEnumerable<object?> values)
            => "<tr>"
               + string.Concat(values.Select((a, i) => $"<{tag} style='border: 1px solid black;"
                                                       + (aligns[i] == Align.Right ? "text-align: right;" : string.Empty)
                                                       + $"'>{HtmlText(a)}</{tag}>"))
               + "</tr>";

        return "<table style='width: 100%;border-collapse: collapse;border: 1px solid black;'>"
               + $"<thead>{Row("th", _columns)}</thead>"
               + $"<tbody>{string.Concat(_rows.Select(a => Row("td", a)))}</tbody>"
               + "</table>";
    }

    /// <summary>Json array of objects {column: value}; values keep their type.</summary>
    public string ToJson(bool pretty = false)
    {
        if (_columns.Count == 0) { return "[]"; }

        var data = _rows.Select(row => _columns.Select((column, i) => (column, value: row[i]))
                                               .ToDictionary(a => a.column, a => a.value))
                        .ToList();
        return JsonConvert.SerializeObject(data, pretty ? Formatting.Indented : Formatting.None);
    }

    /// <summary>
    /// Table from a list of items: declare each column with <c>Column(key)</c> (dictionaries, <c>ExpandoObject</c>,
    /// API answers, or properties by name) or <c>Column(a =&gt; a.Member)</c> (typed items).
    /// Dynamic items use the key form: expressions cannot contain dynamic operations.
    /// </summary>
    public static Builder<T> From<T>(IEnumerable<T> items) => new(items);

    /// <summary>Columns of a table built from a list of items.</summary>
    public sealed class Builder<T>
    {
        private readonly IEnumerable<T> _items;
        private readonly List<ColumnDefinition> _columns = [];

        internal Builder(IEnumerable<T> items)
        {
            ArgumentNullException.ThrowIfNull(items);
            _items = items;
        }

        /// <summary>Column read by key; a missing key or property is an empty cell. Title: the key.</summary>
        public ColumnBuilder<T> Column(string key)
        {
            ArgumentNullException.ThrowIfNull(key);
            return Add(new ColumnDefinition(key, a => ReadKey(a, key), null, key));
        }

        /// <summary>Several columns read by key, with default title and alignment.</summary>
        public Builder<T> Columns(params string[] keys)
        {
            ArgumentNullException.ThrowIfNull(keys);
            foreach (var key in keys) { Column(key); }
            return this;
        }

        /// <summary>Column from an expression. Title: the member name, or <c>.Title()</c> when it is not a member.</summary>
        public ColumnBuilder<T> Column(Expression<Func<T, object?>> value)
        {
            ArgumentNullException.ThrowIfNull(value);

            var body = value.Body is UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked } convert
                        ? convert.Operand
                        : value.Body;
            TableGenerator.Align? typeAlign = body.Type == typeof(object)
                                                ? null
                                                : IsNumberType(body.Type) ? TableGenerator.Align.Right : TableGenerator.Align.Left;
            var compiled = value.Compile();
            return Add(new ColumnDefinition((body as MemberExpression)?.Member.Name, a => compiled(a), typeAlign, value.ToString()));
        }

        /// <summary>The table.</summary>
        public TableGenerator Build()
        {
            var columns = _columns.Where(a => a.When).ToList();
            var table = new TableGenerator();
            foreach (var column in columns)
            {
                if (column.Title == null) { throw new ArgumentException($"Column title is required for '{column.Source}'."); }
                table.AddColumn(column.Title, column.Align ?? (column.Format == null ? column.TypeAlign : null));
            }

            foreach (var item in _items)
            {
                table.AddRow([.. columns.Select(a => a.Format == null ? a.Value(item) : a.Format(a.Value(item)))]);
            }

            return table;
        }

        /// <inheritdoc cref="TableGenerator.To"/>
        public string To(Output output) => Build().To(output);

        /// <inheritdoc cref="TableGenerator.ToText"/>
        public string ToText() => Build().ToText();

        /// <inheritdoc cref="TableGenerator.ToMarkdown"/>
        public string ToMarkdown() => Build().ToMarkdown();

        /// <inheritdoc cref="TableGenerator.ToHtml"/>
        public string ToHtml() => Build().ToHtml();

        /// <inheritdoc cref="TableGenerator.ToJson"/>
        public string ToJson(bool pretty = false) => Build().ToJson(pretty);

        private ColumnBuilder<T> Add(ColumnDefinition column)
        {
            _columns.Add(column);
            return new(this, column);
        }

        private static object? ReadKey(T item, string key)
        {
            switch (item)
            {
                case null: return null;
                case IDictionary<string, object?> dictionary: return dictionary.TryGetValue(key, out var value) ? value : null;
            }

            // Exact name first, then ignoring case; the first declared wins when two differ only by case.
            var properties = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var property = properties.FirstOrDefault(a => a.Name == key)
                           ?? properties.FirstOrDefault(a => string.Equals(a.Name, key, StringComparison.OrdinalIgnoreCase));
            return property?.GetValue(item);
        }

        internal sealed class ColumnDefinition(string? title, Func<T, object?> value, TableGenerator.Align? typeAlign, string source)
        {
            public string? Title { get; set; } = title;
            public Func<T, object?> Value { get; } = value;
            public TableGenerator.Align? TypeAlign { get; } = typeAlign;
            public string Source { get; } = source;
            public Func<object?, object?>? Format { get; set; }
            public TableGenerator.Align? Align { get; set; }
            public bool When { get; set; } = true;
        }
    }

    /// <summary>Options of the last declared column; the chain goes on with the next <c>Column</c>.</summary>
    public sealed class ColumnBuilder<T>
    {
        private readonly Builder<T> _builder;
        private readonly Builder<T>.ColumnDefinition _column;

        internal ColumnBuilder(Builder<T> builder, Builder<T>.ColumnDefinition column)
        {
            _builder = builder;
            _column = column;
        }

        /// <summary>Title of the column.</summary>
        public ColumnBuilder<T> Title(string title)
        {
            ArgumentNullException.ThrowIfNull(title);
            _column.Title = title;
            return this;
        }

        /// <summary>Changes the value: receives the raw value (or null) and returns the cell value.</summary>
        public ColumnBuilder<T> Format(Func<object?, object?> format)
        {
            ArgumentNullException.ThrowIfNull(format);
            _column.Format = format;
            return this;
        }

        /// <summary>Alignment of the column, instead of the one from the type.</summary>
        public ColumnBuilder<T> Align(TableGenerator.Align align)
        {
            _column.Align = align;
            return this;
        }

        /// <summary>False leaves the column out.</summary>
        public ColumnBuilder<T> When(bool condition)
        {
            _column.When = condition;
            return this;
        }

        /// <inheritdoc cref="Builder{T}.Column(string)"/>
        public ColumnBuilder<T> Column(string key) => _builder.Column(key);

        /// <inheritdoc cref="Builder{T}.Column(Expression{Func{T, object}})"/>
        public ColumnBuilder<T> Column(Expression<Func<T, object?>> value) => _builder.Column(value);

        /// <inheritdoc cref="Builder{T}.Columns"/>
        public Builder<T> Columns(params string[] keys) => _builder.Columns(keys);

        /// <inheritdoc cref="Builder{T}.Build"/>
        public TableGenerator Build() => _builder.Build();

        /// <inheritdoc cref="TableGenerator.To"/>
        public string To(Output output) => _builder.To(output);

        /// <inheritdoc cref="TableGenerator.ToText"/>
        public string ToText() => _builder.ToText();

        /// <inheritdoc cref="TableGenerator.ToMarkdown"/>
        public string ToMarkdown() => _builder.ToMarkdown();

        /// <inheritdoc cref="TableGenerator.ToHtml"/>
        public string ToHtml() => _builder.ToHtml();

        /// <inheritdoc cref="TableGenerator.ToJson"/>
        public string ToJson(bool pretty = false) => _builder.ToJson(pretty);
    }

    internal static bool IsNumberType(Type type) => NumberTypes.Contains(Nullable.GetUnderlyingType(type) ?? type);

    private static bool IsNumber(object value) => NumberTypes.Contains(value.GetType());

    private static string CellText(object? value)
        => value switch
        {
            null => string.Empty,
            IFormattable formattable => formattable.ToString(null, CultureInfo.CurrentCulture),
            _ => value.ToString() ?? string.Empty,
        };

    private static string HtmlText(object? value)
        => string.Join("<br>", WebUtility.HtmlEncode(CellText(value).Trim()).Split(NewLines, StringSplitOptions.None));

    private int[] Widths(IEnumerable<int[]> rowWidths)
    {
        var widths = new int[_columns.Count];
        foreach (var row in rowWidths)
        {
            for (var i = 0; i < widths.Length; i++) { widths[i] = Math.Max(widths[i], row[i]); }
        }

        return widths;
    }

    private Align[] ResolveAligns()
        => [.. _aligns.Select((align, i) =>
        {
            if (align != null) { return align.Value; }
            var values = _rows.Select(a => a[i]).Where(a => a != null).ToList();
            return values.Count > 0 && values.All(a => IsNumber(a!)) ? Align.Right : Align.Left;
        })];

    private static string Line(IReadOnlyList<string> cells, int[] widths, Align[] aligns)
        => "| " + string.Join(" | ", cells.Select((a, i) => aligns[i] == Align.Right
                                                              ? a.PadLeft(widths[i])
                                                              : a.PadRight(widths[i]))) + " |";
}
