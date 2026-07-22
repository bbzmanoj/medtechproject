using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;
using NotepadPlusPlus.FlaUI.Tests.Configuration;
using NUnit.Framework.Interfaces;

namespace NotepadPlusPlus.FlaUI.Tests.Reporting;

public static class ExtentReportManager
{
    private const string ReportTitle = "Notepad++ UI Automation Report";
    private const string ReportName = "Desktop Scenario Results";
    private const string CustomCss = """
        :root {
            --accent: #136f63;
            --accent-soft: #e6f4f1;
            --ink: #1f2937;
            --muted: #667085;
            --surface: #fffdf8;
            --surface-strong: #f7f2e8;
            --border: #e4dccf;
            --success: #13795b;
            --danger: #b42318;
        }

        body,
        .app,
        .test-wrapper,
        .card,
        .left-panel,
        .right-panel {
            background: var(--surface) !important;
            color: var(--ink) !important;
        }

        .nav-wrapper,
        .brand-logo,
        nav {
            background: linear-gradient(135deg, #11324d 0%, #1f5c7a 55%, #2c7a7b 100%) !important;
        }

        .brand-logo,
        .nav-right a,
        .side-nav a,
        .dropdown-content li > a,
        .dropdown-content li > span {
            color: #f8fafc !important;
        }

        .test-list-item,
        .card-panel,
        .test-content,
        .detail-body,
        .category-container,
        .author-container,
        .device-container,
        .exception-container {
            border-radius: 16px !important;
            border: 1px solid var(--border) !important;
            box-shadow: 0 16px 40px rgba(17, 50, 77, 0.08) !important;
            background: #ffffff !important;
        }

        .test-list-item {
            margin: 10px 12px !important;
            padding: 8px 10px !important;
        }

        .test-name,
        .name,
        .test-detail,
        .exception-name {
            color: var(--ink) !important;
            font-weight: 600 !important;
        }

        .test-time,
        .text-muted,
        .category-label,
        .author-label,
        .device-label {
            color: var(--muted) !important;
        }

        .badge,
        .test-status,
        .chip {
            border-radius: 999px !important;
            font-weight: 700 !important;
            letter-spacing: 0.02em;
        }

        .badge.pass,
        .test-status.pass,
        .green,
        .green-text {
            background: #e8f7f0 !important;
            color: var(--success) !important;
        }

        .badge.fail,
        .test-status.fail,
        .red,
        .red-text {
            background: #fdeceb !important;
            color: var(--danger) !important;
        }

        .badge.info,
        .blue,
        .blue-text {
            background: var(--accent-soft) !important;
            color: var(--accent) !important;
        }

        table.highlight > tbody > tr:hover {
            background-color: #f7fbfa !important;
        }

        th {
            color: var(--muted) !important;
            text-transform: uppercase;
            letter-spacing: 0.04em;
            font-size: 0.78rem !important;
        }

        .test-body,
        .detail-body {
            padding: 20px 24px !important;
        }
    """;

    private static readonly object Sync = new();
    private static readonly AsyncLocal<ExtentTest?> CurrentTest = new();
    private static ExtentReports? report;
    private static string? reportDirectory;
    private static string? reportPath;

    public static string ReportPath => reportPath ?? throw new InvalidOperationException("The report path has not been initialized.");
    public static string ReportDirectory => reportDirectory ?? throw new InvalidOperationException("The report directory has not been initialized.");

    public static void Initialize()
    {
        lock (Sync)
        {
            if (report is not null)
            {
                return;
            }

            Directory.CreateDirectory(TestSettings.ArtifactsRoot);
            reportDirectory = TestSettings.ReportRoot;
            Directory.CreateDirectory(reportDirectory);
            reportPath = Path.Combine(reportDirectory, "index.html");

            var reporter = new ExtentSparkReporter(reportPath);
            ConfigureReporter(reporter);

            report = new ExtentReports();
            report.AttachReporter(reporter);
            AttachRunMetadata(report);
        }
    }

    public static void StartTest(string testName, int attempt)
    {
        Initialize();
        CurrentTest.Value = report!.CreateTest($"{testName} (Attempt {attempt})").Info($"Attempt {attempt}");
    }

    public static void CompleteTest(TestStatus status, string? message, string? screenshotPath)
    {
        if (CurrentTest.Value is null)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(screenshotPath) && File.Exists(screenshotPath))
        {
            CurrentTest.Value.AddScreenCaptureFromPath(screenshotPath);
        }

        switch (status)
        {
            case TestStatus.Passed:
                CurrentTest.Value.Pass("Passed");
                break;
            case TestStatus.Failed:
                CurrentTest.Value.Fail(message ?? "Failed");
                break;
            default:
                CurrentTest.Value.Skip(message ?? status.ToString());
                break;
        }

        report?.Flush();
        CurrentTest.Value = null;
    }

    private static void ConfigureReporter(ExtentSparkReporter reporter)
    {
        reporter.Config.Theme = Theme.Standard;
        reporter.Config.DocumentTitle = ReportTitle;
        reporter.Config.ReportName = ReportName;
        reporter.Config.CSS = CustomCss;
        reporter.Config.Encoding = "utf-8";
        reporter.Config.TimeStampFormat = "dd MMM yyyy HH:mm:ss";
    }

    private static void AttachRunMetadata(ExtentReports extentReport)
    {
        extentReport.AddSystemInfo("OS", Environment.OSVersion.ToString());
    }
}