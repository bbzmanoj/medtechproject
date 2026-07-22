using FlaUI.Core.Definitions;
using FlaUI.Core.AutomationElements;
using NotepadPlusPlus.FlaUI.Tests.Infrastructure;

namespace NotepadPlusPlus.FlaUI.Tests.PageObjects.Menus;

public sealed class MenuBar
{
    private readonly NotepadPlusPlusSession session;

    public MenuBar(NotepadPlusPlusSession session)
    {
        this.session = session;
        File = new FileMenu(session);
        Edit = new EditMenu(session);
        Format = new FormatMenu(session);
    }

    public FileMenu File { get; }

    public EditMenu Edit { get; }

    public FormatMenu Format { get; }

    public void EnsureUtf8Encoding()
    {
        session.BringToFront();

        var encodingMenu = session.MainWindow.FindFirstDescendant(cf =>
            cf.ByControlType(ControlType.MenuItem).And(cf.ByName("Encoding")));

        encodingMenu?.Click();

        var utf8Item = session.TryFindTopLevelWindow(
            window => window.Title.IndexOf("Notepad++", StringComparison.OrdinalIgnoreCase) >= 0,
            TimeSpan.FromMilliseconds(500));

        var menuItem = utf8Item?.FindFirstDescendant(cf =>
            cf.ByControlType(ControlType.MenuItem)
                .And(cf.ByName("UTF-8")));

        menuItem?.Click();
    }
}