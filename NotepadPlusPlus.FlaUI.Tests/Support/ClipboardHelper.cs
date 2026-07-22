using System.Windows.Forms;

namespace NotepadPlusPlus.FlaUI.Tests.Support;

public static class ClipboardHelper
{
    public static void PasteText(string text)
    {
        WithRetries(() => Clipboard.SetText(text));
        KeyboardShortcuts.Send(ShortcutKeys.Paste);
    }

    public static string ReadText()
    {
        return WithRetries(() => Clipboard.ContainsText() ? Clipboard.GetText() : string.Empty);
    }

    private static T WithRetries<T>(Func<T> operation)
    {
        Exception? lastError = null;

        for (var attempt = 0; attempt < 5; attempt++)
        {
            try
            {
                return operation();
            }
            catch (Exception error)
            {
                lastError = error;
                Thread.Sleep(UiTiming.PollInterval);
            }
        }

        throw lastError ?? new InvalidOperationException("Clipboard operation failed.");
    }

    private static void WithRetries(Action operation)
    {
        WithRetries(() =>
        {
            operation();
            return true;
        });
    }
}