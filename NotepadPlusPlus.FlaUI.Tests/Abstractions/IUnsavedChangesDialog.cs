namespace NotepadPlusPlus.FlaUI.Tests.Abstractions;

public interface IUnsavedChangesDialog
{
    string Title { get; }

    void Cancel();

    void DiscardChanges();
}