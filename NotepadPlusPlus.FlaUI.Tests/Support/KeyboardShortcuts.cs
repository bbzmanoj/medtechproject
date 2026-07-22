using System.Windows.Forms;

namespace NotepadPlusPlus.FlaUI.Tests.Support;

public static class KeyboardShortcuts
{
    public static void Send(string shortcut)
    {
        SendKeys.SendWait(shortcut);
        Thread.Sleep(UiTiming.PollInterval);
    }
}

public static class ShortcutKeys
{
    public const string Copy = "^c";
    public const string Paste = "^v";
    public const string SelectAll = "^a";
    public const string Undo = "^z";
    public const string SaveAs = "^+s";
    public const string Open = "^o";
    public const string Replace = "^h";
    public const string CloseWindow = "%{F4}";
    public const string ConfirmDialog = "{ENTER}";
    public const string EscapeDialog = "{ESC}";
}