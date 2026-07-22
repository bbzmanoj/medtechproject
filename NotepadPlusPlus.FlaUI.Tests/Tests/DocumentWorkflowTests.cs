using NotepadPlusPlus.FlaUI.Tests.Infrastructure;
using NotepadPlusPlus.FlaUI.Tests.Support;

namespace NotepadPlusPlus.FlaUI.Tests.Tests;

[TestFixture]
public sealed class DocumentWorkflowTests : UiTestBase
{
    [Test]
    public void ScenarioA_FileRoundTrip_PreservesLargeText()
    {
        var expected = TestDataFactory.CreateLargeLoremIpsum(550);
        var filePath = RegisterCleanupFile(ArtifactPaths.CreateFilePath(TestArtifactDirectory, "round-trip.txt"));

        Page.Editor.ReplaceAllText(expected);
        Page.SaveFile(filePath);

        Relaunch(filePath);

        Assert.That(Page.Editor.ReadAllText(), Is.EqualTo(expected));
    }

    [Test]
    public void OpenDialog_LoadsExistingFile()
    {
        var expected = "Pre-existing file content for open dialog validation.";
        var filePath = RegisterCleanupFile(ArtifactPaths.CreateFilePath(TestArtifactDirectory, "open-dialog.txt"));
        File.WriteAllText(filePath, expected);

        Page.OpenFile(filePath);

        Assert.That(Page.Editor.ReadAllText(), Is.EqualTo(expected));
    }

    [Test]
    public void ScenarioC_UnicodeAndEncoding_PreservesUtf8Characters()
    {
        var expected = TestDataFactory.CreateUnicodeDocument();
        var filePath = RegisterCleanupFile(ArtifactPaths.CreateFilePath(TestArtifactDirectory, "unicode-round-trip.txt"));

        Page.Editor.ReplaceAllText(expected);
        Page.SaveFileAsUtf8(filePath);

        Relaunch(filePath);

        Assert.That(Page.Editor.ReadAllText(), Is.EqualTo(expected));
    }
}