using NotepadPlusPlus.FlaUI.Tests.Infrastructure;

namespace NotepadPlusPlus.FlaUI.Tests.Tests;

[TestFixture]
public sealed class ReportDemoTests : UiTestBase
{
    [Test]
    [Category("Demo")]
    public void DuplicateOfLaunches_NotepadPlusPlus_MainWindow_FailsIntentionally_WhenEnabled()
    {
        Assert.That(
            Session.MainWindow.Title,
            Does.Contain("WordPad"),
            "Intentional demo failure: this test duplicates the launch check and uses an incorrect expected title so the HTML report shows a realistic failed UI assertion.");
    }
}