using NotepadPlusPlus.FlaUI.Tests.Infrastructure;
using NotepadPlusPlus.FlaUI.Tests.Support;

namespace NotepadPlusPlus.FlaUI.Tests.Tests;

[TestFixture]
public sealed class FlakinessHandlingTests : UiTestBase
{
    [Test]
    [Category("Flaky")]
    [Retry(2)]
    public void TextPropagation_UsesSelectiveRetry_WhenEditorStabilizationIsSlow()
    {
        var expected = TestDataFactory.CreateLargeLoremIpsum(120);
        Page.Editor.ReplaceAllText(expected);

        var stabilized = Page.Editor.WaitForText(expected, TimeSpan.FromMilliseconds(150));

        Assert.That(
            stabilized,
            Is.True,
            "The editor did not stabilize within the intentionally aggressive timeout for this flaky demonstration test.");
    }
}