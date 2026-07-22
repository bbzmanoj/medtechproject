namespace NotepadPlusPlus.FlaUI.Tests.Abstractions;

public interface IEditorApplication
{
    IEditorSurface Editor { get; }

    bool IsOpen { get; }

    void SaveFile(string filePath);

    void OpenFile(string filePath);

    void SaveFileAsUtf8(string filePath);

    IReplaceDialog OpenReplaceDialog();

    IUnsavedChangesDialog AttemptToClose();
}