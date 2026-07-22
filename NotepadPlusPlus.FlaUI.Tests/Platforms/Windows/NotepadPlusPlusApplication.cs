using NotepadPlusPlus.FlaUI.Tests.Abstractions;
using NotepadPlusPlus.FlaUI.Tests.Infrastructure;
using NotepadPlusPlus.FlaUI.Tests.PageObjects;

namespace NotepadPlusPlus.FlaUI.Tests.Platforms.Windows;

public sealed class NotepadPlusPlusApplication : IEditorApplication
{
    private readonly EditorPage page;
    private readonly NotepadPlusPlusSession session;

    public NotepadPlusPlusApplication(NotepadPlusPlusSession session)
    {
        this.session = session;
        page = new EditorPage(session);
    }

    public IEditorSurface Editor => page.Editor;

    public bool IsOpen => !session.Application.HasExited;

    public void SaveFile(string filePath)
    {
        page.SaveFile(filePath);
    }

    public void OpenFile(string filePath)
    {
        page.OpenFile(filePath);
    }

    public void SaveFileAsUtf8(string filePath)
    {
        page.SaveFileAsUtf8(filePath);
    }

    public IReplaceDialog OpenReplaceDialog()
    {
        return page.OpenReplaceDialog();
    }

    public IUnsavedChangesDialog AttemptToClose()
    {
        return page.AttemptToClose();
    }
}