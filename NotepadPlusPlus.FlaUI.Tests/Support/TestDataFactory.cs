using System.Text;

namespace NotepadPlusPlus.FlaUI.Tests.Support;

public static class TestDataFactory
{
    public static string CreateLargeLoremIpsum(int minimumWords)
    {
        const string sentence = "lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua";
        var words = sentence.Split(' ');
        var builder = new StringBuilder();

        while (builder.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length < minimumWords)
        {
            builder.Append(sentence).Append(' ');
        }

        return builder.ToString().Trim();
    }

    public static string CreateRepeatedWordDocument(string repeatedWord, int occurrences, string replacement)
    {
        var parts = Enumerable.Range(1, occurrences)
            .Select(index => $"Line {index}: {repeatedWord} appears in this sentence.");

        return string.Join(Environment.NewLine, parts) + Environment.NewLine + $"Replacement marker: {replacement}.";
    }

    public static string CreateUnicodeDocument()
    {
        return string.Join(
            Environment.NewLine,
            "Plain ASCII line for baseline validation.",
            "Emoji line: testing UTF-8 persistence with 😀 and 🚑.",
            "Arabic line: مرحبا بكم في اختبار واجهة المستخدم.",
            "Chinese line: 这是一个用于验证编码的自动化测试。",
            "Trailing delimiter line to detect truncation.");
    }
}