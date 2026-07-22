using System.Collections.Concurrent;

namespace NotepadPlusPlus.FlaUI.Tests.Support;

public static class TestAttemptTracker
{
    private static readonly ConcurrentDictionary<string, int> Attempts = new();

    public static int StartAttempt(string testId)
    {
        return Attempts.AddOrUpdate(testId, 1, (_, attempt) => attempt + 1);
    }

    public static int Peek(string testId)
    {
        return Attempts.TryGetValue(testId, out var attempt) ? attempt : 1;
    }
}