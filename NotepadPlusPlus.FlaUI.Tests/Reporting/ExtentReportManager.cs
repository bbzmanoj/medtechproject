using NotepadPlusPlus.FlaUI.Tests.Configuration;
using NUnit.Framework.Interfaces;
using System.Text;

namespace NotepadPlusPlus.FlaUI.Tests.Reporting;

public static class ExtentReportManager
{
    private static readonly object Sync = new();
    private static readonly AsyncLocal<TestExecutionRecord?> CurrentTest = new();
    private static readonly List<TestExecutionRecord> CompletedTests = new();
    private static string? reportDirectory;
    private static string? reportPath;
    private static DateTimeOffset? runStartedAt;

    public static string ReportPath => reportPath ?? throw new InvalidOperationException("The report path has not been initialized.");
    public static string ReportDirectory => reportDirectory ?? throw new InvalidOperationException("The report directory has not been initialized.");

    public static void Initialize()
    {
        lock (Sync)
        {
            if (reportPath is not null)
            {
                return;
            }

            Directory.CreateDirectory(TestSettings.ArtifactsRoot);
            reportDirectory = TestSettings.ReportRoot;
            Directory.CreateDirectory(reportDirectory);
            reportPath = Path.Combine(reportDirectory, "index.html");
            runStartedAt = DateTimeOffset.Now;
            WriteHtmlReport();
        }
    }

    public static void StartTest(string testName, int attempt)
    {
        Initialize();
      CurrentTest.Value = new TestExecutionRecord
      {
        TestName = testName,
        Attempt = attempt,
        StartedAt = DateTimeOffset.Now,
        Status = "running",
        Details = "Running"
      };
    }

    public static void CompleteTest(TestStatus status, string? message, string? screenshotPath)
    {
        if (CurrentTest.Value is null)
        {
            return;
        }

        var completed = new TestExecutionRecord
        {
          TestName = CurrentTest.Value.TestName,
          Attempt = CurrentTest.Value.Attempt,
          StartedAt = CurrentTest.Value.StartedAt,
          Status = NormalizeStatus(status),
          Details = string.IsNullOrWhiteSpace(message) ? status.ToString() : message!,
          ScreenshotPath = File.Exists(screenshotPath) ? screenshotPath : null,
          DurationMs = (long)Math.Round((DateTimeOffset.Now - CurrentTest.Value.StartedAt).TotalMilliseconds)
        };

        lock (Sync)
        {
            CompletedTests.Add(completed);
            WriteHtmlReport();
        }

        CurrentTest.Value = null;
    }

    private static void WriteHtmlReport()
    {
        if (reportPath is null)
        {
            return;
        }

        var results = CompletedTests
            .OrderBy(result => result.StartedAt)
            .ToList();

        File.WriteAllText(reportPath, GenerateHtmlReport(results), Encoding.UTF8);
    }

    private static string GenerateHtmlReport(IReadOnlyCollection<TestExecutionRecord> results)
    {
        var passedCount = results.Count(result => result.Status == "passed");
        var failedCount = results.Count(result => result.Status == "failed");
        var skippedCount = results.Count(result => result.Status == "skipped");
        var totalCount = results.Count;
        var totalDurationMs = results.Sum(result => result.DurationMs);
        var generatedAt = DateTimeOffset.Now;

        var html = new StringBuilder();
        html.AppendLine("<!doctype html>");
        html.AppendLine("<html lang=\"en\">");
        html.AppendLine("<head>");
        html.AppendLine("  <meta charset=\"utf-8\">");
        html.AppendLine("  <title>Windows Desktop Scenario Report</title>");
        html.AppendLine("  <style>");
        html.AppendLine("    body { font-family: 'Segoe UI', Arial, sans-serif; margin: 0; background: #f5f7fb; color: #1f2937; }");
        html.AppendLine("    header { padding: 32px; background: linear-gradient(135deg, #0f3d56, #1b6b7b); color: #fff; }");
        html.AppendLine("    main { padding: 24px 32px; }");
        html.AppendLine("    .summary, .charts { display: flex; gap: 16px; margin: 0 0 24px; flex-wrap: wrap; }");
        html.AppendLine("    .card, .chart-card { background: #fff; border-radius: 16px; padding: 16px 20px; box-shadow: 0 10px 30px rgba(15, 61, 86, 0.08); min-width: 160px; }");
        html.AppendLine("    .chart-card { flex: 1 1 320px; }");
        html.AppendLine("    .label { font-size: 12px; text-transform: uppercase; letter-spacing: 0.04em; color: #6b7280; }");
        html.AppendLine("    .value { font-size: 28px; font-weight: 700; margin-top: 6px; }");
        html.AppendLine("    .subtle { color: #475467; margin-top: 8px; }");
        html.AppendLine("    .chart-row { display: grid; grid-template-columns: 90px 1fr 48px; gap: 12px; align-items: center; margin: 12px 0; }");
        html.AppendLine("    .chart-track { background: #e5e7eb; border-radius: 999px; overflow: hidden; height: 14px; }");
        html.AppendLine("    .chart-fill { height: 100%; border-radius: 999px; }");
        html.AppendLine("    .chart-fill.passed { background: #22a06b; }");
        html.AppendLine("    .chart-fill.failed { background: #d92d20; }");
        html.AppendLine("    .chart-fill.skipped { background: #1570ef; }");
        html.AppendLine("    table { width: 100%; border-collapse: collapse; background: #fff; border-radius: 16px; overflow: hidden; box-shadow: 0 10px 30px rgba(15, 61, 86, 0.08); }");
        html.AppendLine("    th, td { padding: 14px 16px; border-bottom: 1px solid #e5e7eb; text-align: left; vertical-align: top; }");
        html.AppendLine("    th { font-size: 12px; text-transform: uppercase; letter-spacing: 0.04em; color: #6b7280; background: #f9fafb; }");
        html.AppendLine("    tr:last-child td { border-bottom: 0; }");
        html.AppendLine("    .status-pill { display: inline-block; padding: 4px 10px; border-radius: 999px; font-size: 12px; font-weight: 700; text-transform: uppercase; letter-spacing: 0.04em; }");
        html.AppendLine("    .status-pill.passed { background: #e8f7f0; color: #13795b; }");
        html.AppendLine("    .status-pill.failed { background: #fdeceb; color: #b42318; }");
        html.AppendLine("    .status-pill.skipped { background: #e8f1fd; color: #175cd3; }");
        html.AppendLine("    a { color: #175cd3; text-decoration: none; font-weight: 600; }");
        html.AppendLine("    a:hover { text-decoration: underline; }");
        html.AppendLine("    .note { margin-top: 16px; color: #475467; }");
        html.AppendLine("  </style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");
        html.AppendLine("  <header>");
        html.AppendLine("    <h1>Windows Desktop Scenario Report</h1>");
        html.AppendLine($"    <p>Automation: FlaUI on Windows UI Automation | Generated: {EscapeHtml(generatedAt.ToString("O"))}</p>");
        html.AppendLine("  </header>");
        html.AppendLine("  <main>");
        html.AppendLine("    <section class=\"summary\">");
        html.AppendLine($"      <div class=\"card\"><div class=\"label\">Passed</div><div class=\"value\">{passedCount}</div></div>");
        html.AppendLine($"      <div class=\"card\"><div class=\"label\">Failed</div><div class=\"value\">{failedCount}</div></div>");
        html.AppendLine($"      <div class=\"card\"><div class=\"label\">Skipped</div><div class=\"value\">{skippedCount}</div></div>");
        html.AppendLine("      <div class=\"card\"><div class=\"label\">OS</div><div class=\"value\">Windows</div></div>");
        html.AppendLine("    </section>");
        html.AppendLine("    <section class=\"charts\">");
        html.AppendLine("      <div class=\"chart-card\">");
        html.AppendLine("        <div class=\"label\">Status Distribution</div>");
        html.AppendLine(BuildChartRow("Passed", passedCount, totalCount, "passed"));
        html.AppendLine(BuildChartRow("Failed", failedCount, totalCount, "failed"));
        html.AppendLine(BuildChartRow("Skipped", skippedCount, totalCount, "skipped"));
        html.AppendLine("      </div>");
        html.AppendLine("      <div class=\"chart-card\">");
        html.AppendLine("        <div class=\"label\">Run Summary</div>");
        html.AppendLine($"        <div class=\"value\">{totalCount}</div>");
        html.AppendLine("        <div class=\"subtle\">Total tests in this report</div>");
        html.AppendLine($"        <div class=\"subtle\">Run started: {EscapeHtml((runStartedAt ?? generatedAt).ToString("dd MMM yyyy HH:mm:ss"))}</div>");
        html.AppendLine($"        <div class=\"subtle\">Total duration: {totalDurationMs} ms</div>");
        html.AppendLine("      </div>");
        html.AppendLine("    </section>");
        html.AppendLine("    <table>");
        html.AppendLine("      <thead>");
        html.AppendLine("        <tr>");
        html.AppendLine("          <th>Test</th>");
        html.AppendLine("          <th>Status</th>");
        html.AppendLine("          <th>Attempt</th>");
        html.AppendLine("          <th>Duration (ms)</th>");
        html.AppendLine("          <th>Details</th>");
        html.AppendLine("          <th>Screenshot</th>");
        html.AppendLine("        </tr>");
        html.AppendLine("      </thead>");
        html.AppendLine("      <tbody>");

        foreach (var result in results)
        {
            html.AppendLine("        <tr>");
            html.AppendLine($"          <td>{EscapeHtml(result.TestName)}</td>");
            html.AppendLine($"          <td><span class=\"status-pill {result.Status}\">{EscapeHtml(result.Status)}</span></td>");
            html.AppendLine($"          <td>{result.Attempt}</td>");
            html.AppendLine($"          <td>{result.DurationMs}</td>");
            html.AppendLine($"          <td>{FormatDetails(result.Details)}</td>");
            html.AppendLine($"          <td>{FormatScreenshotCell(result.ScreenshotPath)}</td>");
            html.AppendLine("        </tr>");
        }

        html.AppendLine("      </tbody>");
        html.AppendLine("    </table>");
        html.AppendLine("    <p class=\"note\">This report is generated for Windows UI runs and includes the same summary style as the macOS report, with status charts for quick review.</p>");
        html.AppendLine("  </main>");
        html.AppendLine("</body>");
        html.AppendLine("</html>");

        return html.ToString();
    }

    private static string BuildChartRow(string label, int count, int totalCount, string cssClass)
    {
        var percentage = totalCount == 0 ? 0 : (int)Math.Round((double)count * 100 / totalCount);
        return $"<div class=\"chart-row\"><span>{EscapeHtml(label)}</span><div class=\"chart-track\"><div class=\"chart-fill {cssClass}\" style=\"width: {percentage}%\"></div></div><span>{percentage}%</span></div>";
    }

    private static string FormatDetails(string details)
    {
        return EscapeHtml(details).Replace(Environment.NewLine, "<br>").Replace("\n", "<br>");
    }

    private static string FormatScreenshotCell(string? screenshotPath)
    {
        if (string.IsNullOrWhiteSpace(screenshotPath))
        {
            return "-";
        }

        return $"<a href=\"{EscapeHtml(new Uri(screenshotPath).AbsoluteUri)}\">Open screenshot</a>";
    }

    private static string EscapeHtml(string value)
    {
        return value
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#39;");
    }

    private static string NormalizeStatus(TestStatus status)
    {
        return status switch
        {
            TestStatus.Passed => "passed",
            TestStatus.Failed => "failed",
            TestStatus.Skipped => "skipped",
            TestStatus.Inconclusive => "skipped",
            _ => "skipped"
        };
    }

    private sealed class TestExecutionRecord
    {
        public string TestName { get; set; } = string.Empty;

        public int Attempt { get; set; }

        public DateTimeOffset StartedAt { get; set; }

        public string Status { get; set; } = string.Empty;

        public string Details { get; set; } = string.Empty;

        public string? ScreenshotPath { get; set; }

        public long DurationMs { get; set; }
    }
}