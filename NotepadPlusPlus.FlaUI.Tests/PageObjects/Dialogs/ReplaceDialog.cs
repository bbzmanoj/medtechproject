using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using NotepadPlusPlus.FlaUI.Tests.Abstractions;
using NotepadPlusPlus.FlaUI.Tests.Infrastructure;
using NotepadPlusPlus.FlaUI.Tests.Support;

namespace NotepadPlusPlus.FlaUI.Tests.PageObjects.Dialogs;

public sealed class ReplaceDialog : IReplaceDialog
{
    private readonly Window window;

    private ReplaceDialog(Window window)
    {
        this.window = window;
    }

    public static ReplaceDialog Attach(NotepadPlusPlusSession session)
    {
        var dialog = session.WaitForDesktopWindow(
            window => window.Title.Equals("Replace", StringComparison.OrdinalIgnoreCase),
            UiTiming.DefaultTimeout);

        return new ReplaceDialog(dialog);
    }

    public void ReplaceAll(string findWhat, string replaceWith)
    {
        var editBoxes = window.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
        if (editBoxes.Length < 2)
        {
            throw new InvalidOperationException("Replace dialog did not expose the expected input fields.");
        }

        SetText(editBoxes[0], findWhat);
        SetText(editBoxes[1], replaceWith);

        var replaceAllButton = window.FindFirstDescendant(cf =>
            cf.ByControlType(ControlType.Button).And(cf.ByName("Replace All")));

        if (replaceAllButton is null)
        {
            throw new InvalidOperationException("Replace All button was not found.");
        }

        replaceAllButton.Click();
        KeyboardShortcuts.Send(ShortcutKeys.EscapeDialog);
    }

    private static void SetText(AutomationElement element, string value)
    {
        element.Focus();
        element.Click();
        KeyboardShortcuts.Send(ShortcutKeys.SelectAll);
        ClipboardHelper.PasteText(value);
    }
}