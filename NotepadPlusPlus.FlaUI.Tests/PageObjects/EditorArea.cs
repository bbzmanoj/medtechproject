using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.AutomationElements;
using NotepadPlusPlus.FlaUI.Tests.Abstractions;
using NotepadPlusPlus.FlaUI.Tests.Infrastructure;
using NotepadPlusPlus.FlaUI.Tests.Support;
using System.Diagnostics;

namespace NotepadPlusPlus.FlaUI.Tests.PageObjects;

public sealed class EditorArea : IEditorSurface
{
    private readonly NotepadPlusPlusSession session;

    public EditorArea(NotepadPlusPlusSession session)
    {
        this.session = session;
    }

    public void ReplaceAllText(string text)
    {
        Focus();
        SelectAll();
        ClipboardHelper.PasteText(text);
    }

    public void AppendText(string text)
    {
        Focus();
        ClipboardHelper.PasteText(text);
    }

    public string ReadAllText()
    {
        Focus();
        SelectAll();
        KeyboardShortcuts.Send(ShortcutKeys.Copy);
        return ClipboardHelper.ReadText();
    }

    public void SelectAll()
    {
        Focus();
        KeyboardShortcuts.Send(ShortcutKeys.SelectAll);
    }

    public void Undo()
    {
        Focus();
        KeyboardShortcuts.Send(ShortcutKeys.Undo);
    }

    public void OverwriteSelection(string text)
    {
        ClipboardHelper.PasteText(text);
    }

    public bool WaitForText(string expectedText, TimeSpan timeout)
    {
        var stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed < timeout)
        {
            if (ReadAllText() == expectedText)
            {
                return true;
            }

            Thread.Sleep(UiTiming.PollInterval);
        }

        return false;
    }

    public void Focus()
    {
        session.BringToFront();
        var editor = FindEditorElement();
        editor.Focus();
        editor.Click();
    }

    private AutomationElement FindEditorElement()
    {
        var editor = session.MainWindow.FindFirstDescendant(cf =>
            cf.ByClassName("Scintilla")
                .Or(cf.ByControlType(ControlType.Document))
                .Or(cf.ByControlType(ControlType.Edit)));

        if (editor is null)
        {
            throw new InvalidOperationException("The Notepad++ editor element was not found.");
        }

        return editor;
    }
}