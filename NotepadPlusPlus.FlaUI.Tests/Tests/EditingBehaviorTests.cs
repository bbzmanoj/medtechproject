using NotepadPlusPlus.FlaUI.Tests.Infrastructure;
using NotepadPlusPlus.FlaUI.Tests.Support;

namespace NotepadPlusPlus.FlaUI.Tests.Tests;

[TestFixture]
public sealed class EditingBehaviorTests : UiTestBase
{
    [Test]
    public void ScenarioB_FindAndReplace_ReplacesExpectedCount_AndUndoRestoresOriginal()
    {
        const string target = "alpha";
        const string replacement = "beta";
        var original = TestDataFactory.CreateRepeatedWordDocument(target, 12, replacement);
        var originalTargetCount = CountOccurrences(original, target);
        var originalReplacementCount = CountOccurrences(original, replacement);

        Page.Editor.ReplaceAllText(original);

        var replaceDialog = Page.OpenReplaceDialog();
        replaceDialog.ReplaceAll(target, replacement);

        var updated = Page.Editor.ReadAllText();

        Assert.Multiple(() =>
        {
            Assert.That(originalTargetCount, Is.GreaterThanOrEqualTo(10));
            Assert.That(CountOccurrences(updated, target), Is.EqualTo(0));
            Assert.That(CountOccurrences(updated, replacement), Is.EqualTo(originalReplacementCount + originalTargetCount));
        });

        Page.Editor.Undo();

        Assert.That(Page.Editor.WaitForText(original, UiTiming.ShortTimeout), Is.True,
            "Undo did not fully restore the original buffer within the timeout.");
        Assert.That(Page.Editor.ReadAllText(), Is.EqualTo(original));
    }

    [Test]
    public void ScenarioE_KeyboardShortcutParity_SelectAllAndUndoWorkWithoutOsBranching()
    {
        Page.Editor.ReplaceAllText("first version of the note");
        Page.Editor.SelectAll();
        Page.Editor.OverwriteSelection("replacement");

        Assert.That(Page.Editor.ReadAllText(), Is.EqualTo("replacement"));

        Page.Editor.Undo();

        Assert.That(Page.Editor.ReadAllText(), Is.EqualTo("first version of the note"));
    }

    private static int CountOccurrences(string source, string value)
    {
        return source.Split(new[] { value }, StringSplitOptions.None).Length - 1;
    }
}