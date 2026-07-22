using NotepadPlusPlus.FlaUI.Tests.Abstractions;
using NotepadPlusPlus.FlaUI.Tests.PageObjects;
using NotepadPlusPlus.FlaUI.Tests.Reporting;
using NotepadPlusPlus.FlaUI.Tests.Support;
using NUnit.Framework.Interfaces;
using System.Threading;

namespace NotepadPlusPlus.FlaUI.Tests.Infrastructure;

[Apartment(ApartmentState.STA)]
public abstract class UiTestBase
{
    protected NotepadPlusPlusSession Session { get; private set; } = null!;

    protected IEditorApplication Page { get; private set; } = null!;

    protected string TestArtifactDirectory { get; private set; } = null!;

    protected int CurrentAttempt { get; private set; }

    [SetUp]
    public void SetUpUiTest()
    {
        CurrentAttempt = TestAttemptTracker.StartAttempt(TestContext.CurrentContext.Test.ID);
        TestArtifactDirectory = ArtifactPaths.CreateForCurrentTest();
        ExtentReportManager.StartTest(TestContext.CurrentContext.Test.Name, CurrentAttempt);
        TestContext.Progress.WriteLine($"[TEST ATTEMPT {CurrentAttempt} STARTED] {TestContext.CurrentContext.Test.Name}");
        LaunchSession();
    }

    [TearDown]
    public void TearDownUiTest()
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

    protected string RegisterCleanupFile(string filePath)
    {
        TestRunCleanup.RegisterFile(filePath);
        return filePath;
    }

    protected void Relaunch(string? filePath = null)
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