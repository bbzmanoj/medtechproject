using NotepadPlusPlus.FlaUI.Tests.Reporting;
using NotepadPlusPlus.FlaUI.Tests.Support;

namespace NotepadPlusPlus.FlaUI.Tests;

[SetUpFixture]
public sealed class TestRunHooks
{
    [OneTimeSetUp]
    public void InitializeReporting()
    {
        ExtentReportManager.Initialize();
    }

    [OneTimeTearDown]
    public void CleanupGeneratedFiles()
    {
        TestRunCleanup.DeleteRegisteredFiles();
    }
}