using NotepadPlusPlus.FlaUI.Tests.Infrastructure;
using NotepadPlusPlus.FlaUI.Tests.PageObjects.Dialogs;
using NotepadPlusPlus.FlaUI.Tests.PageObjects.Menus;
using NotepadPlusPlus.FlaUI.Tests.Support;

namespace NotepadPlusPlus.FlaUI.Tests.PageObjects;

public sealed class EditorPage
{
    private readonly NotepadPlusPlusSession session;

    public EditorPage(NotepadPlusPlusSession session)
    {
        this.session = session;
        Editor = new EditorArea(session);
        MenuBar = new MenuBar(session);
    }

    public EditorArea Editor { get; }

    public MenuBar MenuBar { get; }

    public void SaveFile(string filePath)
    {
        var dialog = MenuBar.File.OpenSaveAsDialog();
        dialog.Save(filePath);
    }

    public void OpenFile(string filePath)
    {
        var dialog = MenuBar.File.OpenOpenDialog();
        dialog.Open(filePath);
    }

    public void SaveFileAsUtf8(string filePath)
    {
        MenuBar.EnsureUtf8Encoding();
        SaveFile(filePath);
    }

    public ReplaceDialog OpenReplaceDialog()
    {
        return MenuBar.Edit.OpenReplaceDialog();
    }

    public UnsavedChangesDialog AttemptToClose()
    {
        session.BringToFront();
        KeyboardShortcuts.Send(ShortcutKeys.CloseWindow);
        return UnsavedChangesDialog.Attach(session);
    }
}