/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

#nullable enable

using System.Text;

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils;

public static partial class NetworkDiagramBuilder
{
    private record SvgNode(string Id, string Label, string Tooltip, string Fill, string TextColor, int Col, int Row);
    private record SvgEdge(string FromId, string ToId, string Label = "");
    private record SvgStorage(string Id, string Label, string Tooltip, string? FromBridgeId, bool IsDisabled);

    private record NodeSection(string NodeName, List<SvgNode> Nodes, List<SvgEdge> Edges, List<SvgStorage> Storages)
    {
        private const int SvgBoxLineH = 13;
        private const int SvgBoxPadY = 8;
        private const int SvgBoxPadX = 10;
        // Approx glyph width in px for Segoe UI at the sizes used below (11pt title, 9pt body).
        // Used to size boxes to the widest label line without measuring text in SVG.
        private const double SvgCharWBody = 5.3;
        private const double SvgCharWTitle = 6.4;
        private const int SvgStorageStripGapY = 40;
        private const int SvgStorageGapX = 20;

        private static int BoxHeight(string label)
            => Math.Max(SvgBoxH, (Math.Max(1, label.Split('\n').Length) * SvgBoxLineH) + SvgBoxPadY);

        private static int BoxWidth(string label)
        {
            var lines = label.Split('\n');
            double maxPx = 0;
            for (var i = 0; i < lines.Length; i++)
            {
                var w = lines[i].Length * (i == 0 ? SvgCharWTitle : SvgCharWBody);
                if (w > maxPx) { maxPx = w; }
            }
            return Math.Max(SvgBoxW, (int)Math.Ceiling(maxPx) + SvgBoxPadX);
        }

        private int RowHeight(int row)
            => Nodes.Where(n => n.Row == row)
                    .Select(n => BoxHeight(n.Label))
                    .DefaultIfEmpty(SvgBoxH)
                    .Max();

        private int ColWidth(int col)
            => Nodes.Where(n => n.Col == col)
                    .Select(n => BoxWidth(n.Label))
                    .DefaultIfEmpty(SvgBoxW)
                    .Max();

        private int RowYOffset(int row)
        {
            var y = 0;
            for (var r = 0; r < row; r++) { y += RowHeight(r) + SvgRowGap; }
            return y;
        }

        private int ColXOffset(int col)
        {
            var x = 0;
            for (var c = 0; c < col; c++) { x += ColWidth(c) + SvgColGap; }
            return x;
        }

        private int StorageStripHeight
            => Storages.Count == 0
                ? 0
                : Storages.Max(s => BoxHeight(s.Label)) + SvgStorageStripGapY + 32;

        private int StorageStripWidth
            => Storages.Count == 0
                ? 0
                : Storages.Sum(s => BoxWidth(s.Label)) + (Math.Max(0, Storages.Count - 1) * SvgStorageGapX);

        public int Width
            => Math.Max(Nodes.Count == 0
                            ? 200
                            : Enumerable.Range(0, Nodes.Max(n => n.Col) + 1).Sum(c => ColWidth(c) + SvgColGap) - SvgColGap,
                        StorageStripWidth);

        public int Height
        {
            get
            {
                var topologyH = 60;
                if (Nodes.Count > 0)
                {
                    var maxRow = Nodes.Max(n => n.Row);
                    var total = 0;
                    for (var i = 0; i <= maxRow; i++) { total += RowHeight(i) + SvgRowGap; }
                    topologyH = total + 40;
                }
                return topologyH + StorageStripHeight;
            }
        }

        private static void RenderBox(StringBuilder sb, int x, int y, int w, int h,
                                      string label, string tooltip, string fill, string textColor)
        {
            sb.AppendLine($"""
                <g>
                  <title>{Escape(tooltip)}</title>
                  <rect x="{x}" y="{y}" width="{w}" height="{h}" rx="4" fill="{fill}" stroke="{SvgColBorderStrong}" stroke-width="1"/>
                """);

            var lines = label.Split('\n');
            var blockH = lines.Length * SvgBoxLineH;
            var startY = y + ((h - blockH) / 2) + (SvgBoxLineH / 2);
            for (var i = 0; i < lines.Length; i++)
            {
                var ty = startY + (i * SvgBoxLineH);
                var bold = i == 0;
                var fontSize = bold ? 11 : 9;
                var fontWeight = bold ? "bold" : "normal";
                sb.AppendLine($"""  <text x="{x + (w / 2)}" y="{ty}" font-size="{fontSize}" font-weight="{fontWeight}" fill="{textColor}" text-anchor="middle" dominant-baseline="middle">{Escape(lines[i])}</text>""");
            }
            sb.AppendLine("</g>");
        }

        public string Render(int offsetX, int offsetY)
        {
            var pos = Nodes.ToDictionary(n => n.Id,
                                         n => (x: offsetX + ColXOffset(n.Col),
                                               y: offsetY + 30 + RowYOffset(n.Row),
                                               w: ColWidth(n.Col),
                                               h: BoxHeight(n.Label)));

            var sb = new StringBuilder();

            sb.AppendLine($"""
                <rect x="{offsetX - 8}" y="{offsetY}" width="{Width + 16}" height="{Height}" rx="8" fill="{SvgColBg}" stroke="{SvgColBorder}" stroke-width="1"/>
                <text x="{offsetX}" y="{offsetY + 18}" font-size="13" font-weight="bold" fill="{SvgColTextHeader}">Node: {NodeName}</text>
                """);

            var edgesByTarget = Edges.Where(e => pos.ContainsKey(e.FromId) && pos.ContainsKey(e.ToId))
                                     .GroupBy(e => e.ToId)
                                     .ToDictionary(g => g.Key, g => g.OrderBy(e => pos[e.FromId].y).ToList());

            foreach (var edge in Edges)
            {
                if (!pos.TryGetValue(edge.FromId, out var from)) { continue; }
                if (!pos.TryGetValue(edge.ToId, out var to)) { continue; }

                var x1 = from.x + from.w;
                var y1 = from.y + (from.h / 2);
                var x2 = to.x;
                var y2 = to.y + (to.h / 2);

                // Route the edge as 3 segments (out, vertical, in). Spread the vertical
                // bend per-sibling so multiple edges arriving at the same target don't
                // overlap into a single line: each sibling is offset by `bendStrideX`
                // around the centre of the group.
                const int finalRunX = 50;
                const int bendStrideX = 8;
                var siblings = edgesByTarget[edge.ToId];
                var idx = siblings.IndexOf(edge);
                var totalIn = siblings.Count;
                var spreadOffset = (idx - ((totalIn - 1) / 2)) * bendStrideX;
                var mx = Math.Max(x1 + 10, x2 - finalRunX + spreadOffset);
                sb.AppendLine($"""<path d="M {x1} {y1} L {mx} {y1} L {mx} {y2} L {x2} {y2}" fill="none" stroke="{SvgColLine}" stroke-width="1.5" stroke-linejoin="miter" marker-end="url(#arrow)"/>""");

                if (!string.IsNullOrEmpty(edge.Label))
                {
                    var lx = (mx + x2) / 2;
                    var ly = y2 - 7;
                    sb.AppendLine($"""<text x="{lx}" y="{ly}" font-size="9" fill="{SvgColTextSecondary}" text-anchor="middle">{Escape(edge.Label)}</text>""");
                }
            }

            foreach (var node in Nodes)
            {
                var (bx, by, bw, bh) = pos[node.Id];
                RenderBox(sb, bx, by, bw, bh, node.Label, node.Tooltip, node.Fill, node.TextColor);
            }

            if (Storages.Count > 0)
            {
                var topologyEndY = offsetY + Height - StorageStripHeight;
                var dividerY = topologyEndY + 8;
                sb.AppendLine($"""<line x1="{offsetX - 8}" y1="{dividerY}" x2="{offsetX + Width + 8}" y2="{dividerY}" stroke="{SvgColBorderDashed}" stroke-width="1" stroke-dasharray="5,3"/>""");

                var stripHeaderY = dividerY + 16;
                sb.AppendLine($"""<text x="{offsetX}" y="{stripHeaderY}" font-size="12" font-weight="bold" fill="{SvgColTextSecondary}">Storages</text>""");

                var sy = stripHeaderY + 8;
                var sx = offsetX;
                foreach (var s in Storages)
                {
                    var sw = BoxWidth(s.Label);
                    var sh = BoxHeight(s.Label);

                    if (s.FromBridgeId != null && pos.TryGetValue(s.FromBridgeId, out var br))
                    {
                        var bx = br.x + (br.w / 2);
                        var by = br.y + br.h;
                        var tx = sx + (sw / 2);
                        var ty = sy;
                        var routeY = dividerY - 6;
                        sb.AppendLine($"""<path d="M {bx} {by} L {bx} {routeY} L {tx} {routeY} L {tx} {ty}" fill="none" stroke="{SvgColLine}" stroke-width="1.5" stroke-linejoin="miter" marker-end="url(#arrow)"/>""");
                    }

                    var sFill = s.IsDisabled ? SvgColDown : SvgColStorage;
                    RenderBox(sb, sx, sy, sw, sh, s.Label, s.Tooltip, sFill, SvgColWhite);
                    sx += sw + SvgStorageGapX;
                }
            }

            var arrowDefs = $"""
                <defs>
                  <marker id="arrow" markerWidth="8" markerHeight="8" refX="8" refY="3" orient="auto">
                    <path d="M0,0 L0,6 L8,3 z" fill="{SvgColLine}"/>
                  </marker>
                </defs>

                """;
            return arrowDefs + sb;
        }
    }
}
