using System.Globalization;
using System.Security;
using System.Text;
using PartnerAppPortal.Core.Models.Assessments;

namespace PartnerAppPortal.Portal.Services
{
    public static class AssessmentScoreChartSvg
    {
        public const double GoalPercent = 90.0;

        public sealed class HistoryPoint
        {
            public string Label { get; set; } = "";
            public double Score { get; set; }
        }

        public static string BuildSectionChart(
            IReadOnlyList<CategoryScoreResult> sections,
            decimal overallBaseline)
        {
            if (sections.Count == 0)
                return "";

            var n = sections.Count;
            const double w = 720, h = 320;
            const double padL = 52, padR = 24, padT = 28, padB = 72;
            var plotW = w - padL - padR;
            var plotH = h - padT - padB;

            double X(int i) => padL + (n == 1 ? plotW / 2 : i * (plotW / Math.Max(n - 1, 1)));
            double Y(double pct) => padT + plotH * (1 - Math.Clamp(pct, 0, 100) / 100.0);

            var sb = new StringBuilder();
            sb.Append(CultureInfo.InvariantCulture, $"<svg viewBox=\"0 0 {w} {h}\" class=\"score-chart-svg\" xmlns=\"http://www.w3.org/2000/svg\">");
            sb.Append(CultureInfo.InvariantCulture, $"<rect x=\"{padL}\" y=\"{padT}\" width=\"{plotW}\" height=\"{plotH}\" class=\"score-chart-plot\"/>");

            foreach (var tick in new[] { 0, 25, 50, 75, 100 })
            {
                var y = Y(tick);
                sb.Append(CultureInfo.InvariantCulture, $"<line x1=\"{padL}\" y1=\"{y}\" x2=\"{padL + plotW}\" y2=\"{y}\" class=\"score-chart-grid\"/>");
                sb.Append(CultureInfo.InvariantCulture, $"<text x=\"{padL - 8}\" y=\"{y + 4}\" text-anchor=\"end\" class=\"score-chart-axis-text\">{tick}%</text>");
            }

            sb.Append(CultureInfo.InvariantCulture, $"<text x=\"16\" y=\"{padT + plotH / 2}\" class=\"score-chart-axis-title\" transform=\"rotate(-90 16,{padT + plotH / 2})\">Scores</text>");
            sb.Append(CultureInfo.InvariantCulture, $"<text x=\"{padL + plotW / 2}\" y=\"{h - 10}\" text-anchor=\"middle\" class=\"score-chart-axis-title\">Sections</text>");

            var yGoal = Y(GoalPercent);
            sb.Append(CultureInfo.InvariantCulture, $"<line x1=\"{padL}\" y1=\"{yGoal}\" x2=\"{padL + plotW}\" y2=\"{yGoal}\" class=\"score-chart-goal\"/>");
            sb.Append(CultureInfo.InvariantCulture, $"<text x=\"{padL + plotW - 4}\" y=\"{yGoal - 6}\" text-anchor=\"end\" class=\"score-chart-goal-label\">Goal 90%</text>");

            var hasSectionBaseline = sections.Any(s => s.BaselinePercentage is not null);
            if (hasSectionBaseline)
            {
                var basePts = new List<string>();
                for (var i = 0; i < n; i++)
                {
                    var bp = sections[i].BaselinePercentage ?? sections[i].Percentage;
                    basePts.Add(string.Create(CultureInfo.InvariantCulture, $"{X(i):0.##},{Y(bp):0.##}"));
                }

                sb.Append(CultureInfo.InvariantCulture, $"<polyline points=\"{string.Join(' ', basePts)}\" class=\"score-chart-baseline-line\" fill=\"none\"/>");
                for (var i = 0; i < n; i++)
                {
                    var bp = sections[i].BaselinePercentage ?? sections[i].Percentage;
                    sb.Append(CultureInfo.InvariantCulture, $"<circle cx=\"{X(i):0.##}\" cy=\"{Y(bp):0.##}\" r=\"4\" class=\"score-chart-baseline-dot\"/>");
                }
            }
            else
            {
                var yBase = Y((double)overallBaseline);
                sb.Append(CultureInfo.InvariantCulture, $"<line x1=\"{padL}\" y1=\"{yBase}\" x2=\"{padL + plotW}\" y2=\"{yBase}\" class=\"score-chart-baseline-line\"/>");
                sb.Append(CultureInfo.InvariantCulture, $"<text x=\"{padL + 4}\" y=\"{yBase - 6}\" class=\"score-chart-baseline-label\">Baseline {overallBaseline}%</text>");
            }

            var curPts = new List<string>();
            for (var i = 0; i < n; i++)
                curPts.Add(string.Create(CultureInfo.InvariantCulture, $"{X(i):0.##},{Y(sections[i].Percentage):0.##}"));

            sb.Append(CultureInfo.InvariantCulture, $"<polyline points=\"{string.Join(' ', curPts)}\" class=\"score-chart-current-line\" fill=\"none\"/>");

            for (var i = 0; i < n; i++)
            {
                var s = sections[i];
                var cx = X(i);
                var cy = Y(s.Percentage);
                sb.Append(CultureInfo.InvariantCulture, $"<circle cx=\"{cx:0.##}\" cy=\"{cy:0.##}\" r=\"5\" class=\"score-chart-current-dot\"/>");
                sb.Append(CultureInfo.InvariantCulture, $"<text x=\"{cx:0.##}\" y=\"{cy - 10}\" text-anchor=\"middle\" class=\"score-chart-value\">{Math.Round(s.Percentage):0}%</text>");
                sb.Append(CultureInfo.InvariantCulture, $"<rect x=\"{cx - 14}\" y=\"{padT + plotH}\" width=\"28\" height=\"8\" class=\"score-chart-tick\"/>");
                sb.Append(CultureInfo.InvariantCulture, $"<text x=\"{cx:0.##}\" y=\"{padT + plotH + 28}\" text-anchor=\"middle\" class=\"score-chart-axis-text\">{SecurityElement.Escape(ShortSectionName(s.CategoryName))}</text>");
            }

            sb.Append("</svg>");
            return sb.ToString();
        }

        public static string BuildOverallHistoryChart(IReadOnlyList<HistoryPoint> points)
        {
            if (points.Count < 2)
                return "";

            var n = points.Count;
            const double w = 720, h = 300;
            const double padL = 52, padR = 24, padT = 28, padB = 64;
            var plotW = w - padL - padR;
            var plotH = h - padT - padB;

            double X(int i) => padL + (n == 1 ? plotW / 2 : i * (plotW / Math.Max(n - 1, 1)));
            double Y(double pct) => padT + plotH * (1 - Math.Clamp(pct, 0, 100) / 100.0);

            var sb = new StringBuilder();
            sb.Append(CultureInfo.InvariantCulture, $"<svg viewBox=\"0 0 {w} {h}\" class=\"score-chart-svg\" xmlns=\"http://www.w3.org/2000/svg\">");
            sb.Append(CultureInfo.InvariantCulture, $"<rect x=\"{padL}\" y=\"{padT}\" width=\"{plotW}\" height=\"{plotH}\" class=\"score-chart-plot\"/>");

            foreach (var tick in new[] { 0, 25, 50, 75, 100 })
            {
                var y = Y(tick);
                sb.Append(CultureInfo.InvariantCulture, $"<line x1=\"{padL}\" y1=\"{y}\" x2=\"{padL + plotW}\" y2=\"{y}\" class=\"score-chart-grid\"/>");
                sb.Append(CultureInfo.InvariantCulture, $"<text x=\"{padL - 8}\" y=\"{y + 4}\" text-anchor=\"end\" class=\"score-chart-axis-text\">{tick}%</text>");
            }

            var yGoal = Y(GoalPercent);
            sb.Append(CultureInfo.InvariantCulture, $"<line x1=\"{padL}\" y1=\"{yGoal}\" x2=\"{padL + plotW}\" y2=\"{yGoal}\" class=\"score-chart-goal\"/>");
            sb.Append(CultureInfo.InvariantCulture, $"<text x=\"{padL + plotW - 4}\" y=\"{yGoal - 6}\" text-anchor=\"end\" class=\"score-chart-goal-label\">Goal 90%</text>");

            var yBase = Y(points[0].Score);
            sb.Append(CultureInfo.InvariantCulture, $"<line x1=\"{padL}\" y1=\"{yBase}\" x2=\"{padL + plotW}\" y2=\"{yBase}\" class=\"score-chart-baseline-line\"/>");

            var linePts = string.Join(' ', Enumerable.Range(0, n)
                .Select(i => string.Create(CultureInfo.InvariantCulture, $"{X(i):0.##},{Y(points[i].Score):0.##}")));
            sb.Append(CultureInfo.InvariantCulture, $"<polyline points=\"{linePts}\" class=\"score-chart-current-line\" fill=\"none\"/>");

            for (var i = 0; i < n; i++)
            {
                var p = points[i];
                var cx = X(i);
                var cy = Y(p.Score);
                sb.Append(CultureInfo.InvariantCulture, $"<circle cx=\"{cx:0.##}\" cy=\"{cy:0.##}\" r=\"5\" class=\"score-chart-current-dot\"/>");
                sb.Append(CultureInfo.InvariantCulture, $"<text x=\"{cx:0.##}\" y=\"{cy - 10}\" text-anchor=\"middle\" class=\"score-chart-value\">{Math.Round(p.Score):0}%</text>");
                sb.Append(CultureInfo.InvariantCulture, $"<text x=\"{cx:0.##}\" y=\"{padT + plotH + 28}\" text-anchor=\"middle\" class=\"score-chart-axis-text\">{SecurityElement.Escape(p.Label)}</text>");
            }

            sb.Append(CultureInfo.InvariantCulture, $"<text x=\"{padL + plotW / 2}\" y=\"{h - 8}\" text-anchor=\"middle\" class=\"score-chart-axis-title\">Assessment checkpoints</text>");
            sb.Append("</svg>");
            return sb.ToString();
        }

        private static string ShortSectionName(string name) => name switch
        {
            "Solutions and Specializations" => "Solutions",
            "Customers & CSP" => "CSP",
            "Co-Sell & Referrals" => "Co-Sell",
            "Marketplace Offers" => "Marketplace",
            "Incentives Usage" => "Incentives",
            _ => name.Length <= 14 ? name : name[..12] + "…"
        };
    }
}
