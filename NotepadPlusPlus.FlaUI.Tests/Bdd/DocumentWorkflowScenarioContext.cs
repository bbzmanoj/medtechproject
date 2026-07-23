using NotepadPlusPlus.FlaUI.Tests.Abstractions;
using NotepadPlusPlus.FlaUI.Tests.Infrastructure;
using NotepadPlusPlus.FlaUI.Tests.Reporting;
using NotepadPlusPlus.FlaUI.Tests.Support;
using NUnit.Framework.Interfaces;

namespace NotepadPlusPlus.FlaUI.Tests.Bdd;

public sealed class DocumentWorkflowScenarioContext
{
    public NotepadPlusPlusSession Session { get; private set; } = null!;

    public IEditorApplication Page { get; private set; } = null!;

    public string TestArtifactDirectory { get; private set; } = null!;

    public int CurrentAttempt { get; private set; }

    public string ExpectedText { get; set; } = string.Empty;

    public string PreparedFilePath { get; set; } = string.Empty;

    public void Initialize()
    {
        CurrentAttempt = TestAttemptTracker.StartAttempt(TestContext.CurrentContext.Test.ID);
        TestArtifactDirectory = ArtifactPaths.CreateForCurrentTest();
        ExtentReportManager.StartTest(TestContext.CurrentContext.Test.Name, CurrentAttempt);
        TestContext.Progress.WriteLine($"[TEST ATTEMPT {CurrentAttempt} STARTED] {TestContext.CurrentContext.Test.Name}");
        LaunchSession();
    }

    public void Complete()
    {
        var outcome = TestContext.CurrentContext.Result.Outcome.Status;
        string? screenshotPath = null;

        if (outcome == TestStatus.Failed && Session is not null)
        {
            screenshotPath = Path.Combine(TestArtifactDirectory, "failure.png");
            Session.CaptureScreenshot(screenshotPath);
        }

        ExtentReportManager.CompleteTest(
            outcome,
            TestContext.CurrentContext.Result.Message,
            screenshotPath);

        TestContext.Progress.WriteLine($"[TEST ATTEMPT {CurrentAttempt} {FormatOutcome(outcome)}] {TestContext.CurrentContext.Test.Name}");

        Session?.Dispose();
    }

    public string RegisterCleanupFile(string filePath)
    {
        TestRunCleanup.RegisterFile(filePath);
        return filePath;
    }

    public void Relaunch(string? filePath = null)
    {
        Session.Dispose();
        LaunchSession(filePath);
    }

    private void LaunchSession(string? filePath = null)
    {
        Session = NotepadPlusPlusSession.Launch(filePath);
        Page = EditorApplicationFactory.Create(Session);
    }

    private static string FormatOutcome(TestStatus outcome)
    {
        switch (outcome)
        {
            case TestStatus.Passed:
                return "PASSED";
            case TestStatus.Failed:
                return "FAILED";
            case TestStatus.Skipped:
                return "SKIPPED";
            case TestStatus.Inconclusive:
                return "INCONCLUSIVE";
            default:
                return outcome.ToString().ToUpperInvariant();
        }
    }
}