using System.Collections.Concurrent;

namespace NotepadPlusPlus.FlaUI.Tests.Support;

public static class TestRunCleanup
{
    private static readonly ConcurrentDictionary<string, byte> Files = new(StringComparer.OrdinalIgnoreCase);

    public static void RegisterFile(string filePath)
    {
        Files.TryAdd(filePath, 0);
    }

    public static void DeleteRegisteredFiles()
    {
        foreach (var filePath in Files.Keys)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        Files.Clear();
    }
}