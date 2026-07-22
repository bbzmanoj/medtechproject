namespace NotepadPlusPlus.FlaUI.Tests.Support;

public static class UiTiming
{
    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);
    public static readonly TimeSpan ShortTimeout = TimeSpan.FromSeconds(2);
    public static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(100);
    public static readonly TimeSpan WindowSearchRetryDelay = TimeSpan.FromMilliseconds(200);
    public static readonly TimeSpan DialogNavigationDelay = TimeSpan.FromMilliseconds(300);
}