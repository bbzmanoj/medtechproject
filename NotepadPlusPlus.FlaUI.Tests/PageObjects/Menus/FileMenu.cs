using FlaUI.Core.Definitions;
using NotepadPlusPlus.FlaUI.Tests.Infrastructure;
using NotepadPlusPlus.FlaUI.Tests.PageObjects.Dialogs;
using NotepadPlusPlus.FlaUI.Tests.Support;

namespace NotepadPlusPlus.FlaUI.Tests.PageObjects.Menus;

public sealed class FileMenu
{
    private readonly NotepadPlusPlusSession session;

    public FileMenu(NotepadPlusPlusSession session)
    {
        this.session = session;
    }

    public NativeFileDialog OpenSaveAsDialog()
    {
        session.BringToFront();
        if (!TryClickFileMenuItem("Save As..."))
        {
            KeyboardShortcuts.Send(ShortcutKeys.SaveAs);
        }

        return NativeFileDialog.Attach(session, "Save As");
    }

    public NativeFileDialog OpenOpenDialog()
    {
        session.BringToFront();
        if (!TryClickFileMenuItem("Open..."))
        {
            KeyboardShortcuts.Send(ShortcutKeys.Open);
        }

        return NativeFileDialog.Attach(session, "Open");
    }

    private bool TryClickFileMenuItem(string itemName)
    {
        var fileMenu = session.MainWindow.FindFirstDescendant(cf =>
            cf.ByControlType(ControlType.MenuItem).And(cf.ByName("File")));

        if (fileMenu is null)
        {
            return false;
        }

        fileMenu.Click();
        Thread.Sleep(150);

        var menuItem = session.Automation.GetDesktop().FindFirstDescendant(cf =>
            cf.ByControlType(ControlType.MenuItem).And(cf.ByName(itemName)));

        if (menuItem is null)
        {
            KeyboardShortcuts.Send(ShortcutKeys.EscapeDialog);
            return false;
        }

        menuItem.Click();
        return true;
    }
}