using FlaUI.Core.Definitions;
using NotepadPlusPlus.FlaUI.Tests.Infrastructure;

namespace NotepadPlusPlus.FlaUI.Tests.PageObjects.Menus;

public sealed class FormatMenu
{
    private readonly NotepadPlusPlusSession session;

    public FormatMenu(NotepadPlusPlusSession session)
    {
        this.session = session;
    }

    public void Open()
    {
        session.BringToFront();
        var formatMenu = session.MainWindow.FindFirstDescendant(cf =>
            cf.ByControlType(ControlType.MenuItem).And(cf.ByName("Format")));

        formatMenu?.Click();
    }
}