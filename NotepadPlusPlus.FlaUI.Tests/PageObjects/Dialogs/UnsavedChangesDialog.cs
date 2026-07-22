using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using NotepadPlusPlus.FlaUI.Tests.Abstractions;
using NotepadPlusPlus.FlaUI.Tests.Infrastructure;
using NotepadPlusPlus.FlaUI.Tests.Support;

namespace NotepadPlusPlus.FlaUI.Tests.PageObjects.Dialogs;

public sealed class UnsavedChangesDialog : IUnsavedChangesDialog
{
    private readonly Window window;

    private UnsavedChangesDialog(Window window)
    {
        this.window = window;
    }

    public string Title => window.Title;

    public static UnsavedChangesDialog Attach(NotepadPlusPlusSession session)
    {
        var dialog = session.WaitForDesktopWindow(
            window => window.Title.Equals("Save", StringComparison.OrdinalIgnoreCase),
            UiTiming.DefaultTimeout);

        return new UnsavedChangesDialog(dialog);
    }

    public void Cancel()
    {
        var cancelButton = window.FindFirstDescendant(cf =>
            cf.ByControlType(ControlType.Button).And(cf.ByName("Cancel")));

        if (cancelButton is not null)
        {
            cancelButton.Click();
            return;
        }

        KeyboardShortcuts.Send(ShortcutKeys.EscapeDialog);
    }

    public void DiscardChanges()
    {
        var noButton = window.FindFirstDescendant(cf =>
            cf.ByControlType(ControlType.Button).And(cf.ByName("No")));

        if (noButton is not null)
        {
            noButton.Click();
            return;
        }

        KeyboardShortcuts.Send("%n");
    }
}