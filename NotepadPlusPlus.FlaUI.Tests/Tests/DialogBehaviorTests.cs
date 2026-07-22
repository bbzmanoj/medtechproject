using NotepadPlusPlus.FlaUI.Tests.Infrastructure;
using NotepadPlusPlus.FlaUI.Tests.Support;

namespace NotepadPlusPlus.FlaUI.Tests.Tests;

[TestFixture]
public sealed class DialogBehaviorTests : UiTestBase
{
    [Test]
    public void ScenarioD_UnsavedChangesDialog_CancelKeepsApplicationOpenAndLeavesFileUnchanged()
    {
        const string originalContent = "Original saved file content.";
        const string modifiedContent = "Modified editor content that should remain unsaved after cancel.";
        var filePath = RegisterCleanupFile(ArtifactPaths.CreateFilePath(TestArtifactDirectory, "dialog-behavior.txt"));
        File.WriteAllText(filePath, originalContent);

        Relaunch(filePath);
        Page.Editor.ReplaceAllText(modifiedContent);

        var dialog = Page.AttemptToClose();

        Assert.That(dialog.Title, Is.EqualTo("Save"));

        dialog.Cancel();

        Assert.Multiple(() =>
        {
            Assert.That(Page.IsOpen, Is.True);
            Assert.That(Page.Editor.ReadAllText(), Is.EqualTo(modifiedContent));
            Assert.That(File.ReadAllText(filePath), Is.EqualTo(originalContent));
        });

        Page.AttemptToClose().DiscardChanges();
    }
}