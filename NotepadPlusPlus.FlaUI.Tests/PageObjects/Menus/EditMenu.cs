using NotepadPlusPlus.FlaUI.Tests.Infrastructure;
using NotepadPlusPlus.FlaUI.Tests.PageObjects.Dialogs;
using NotepadPlusPlus.FlaUI.Tests.Support;

namespace NotepadPlusPlus.FlaUI.Tests.PageObjects.Menus;

public sealed class EditMenu
{
    private readonly NotepadPlusPlusSession session;

    public EditMenu(NotepadPlusPlusSession session)
    {
        this.session = session;
    }

    public ReplaceDialog OpenReplaceDialog()
    {
        session.BringToFront();
        KeyboardShortcuts.Send(ShortcutKeys.Replace);
        return ReplaceDialog.Attach(session);
    }
}