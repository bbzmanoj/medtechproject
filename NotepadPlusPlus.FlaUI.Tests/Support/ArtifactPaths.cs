using NotepadPlusPlus.FlaUI.Tests.Configuration;
using System.Text;

namespace NotepadPlusPlus.FlaUI.Tests.Support;

public static class ArtifactPaths
{
    public static string CreateForCurrentTest()
    {
        var folder = Path.Combine(
            TestSettings.ArtifactsRoot,
            Sanitize(TestContext.CurrentContext.Test.Name),
            $"attempt-{TestAttemptTracker.Peek(TestContext.CurrentContext.Test.ID)}");

        Directory.CreateDirectory(folder);
        return folder;
    }

    public static string CreateFilePath(string directory, string fileName)
    {
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, fileName);
    }

    private static string Sanitize(string value)
    {
        var builder = new StringBuilder(value.Length);
        foreach (var character in value)
        {
            builder.Append(Path.GetInvalidFileNameChars().Contains(character) ? '_' : character);
        }

        return builder.ToString();
    }
}