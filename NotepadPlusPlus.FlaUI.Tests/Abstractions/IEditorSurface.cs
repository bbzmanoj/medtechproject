namespace NotepadPlusPlus.FlaUI.Tests.Abstractions;

public interface IEditorSurface
{
    void ReplaceAllText(string text);

    void AppendText(string text);

    string ReadAllText();

    void SelectAll();

    void Undo();

    void OverwriteSelection(string text);

    bool WaitForText(string expectedText, TimeSpan timeout);
}