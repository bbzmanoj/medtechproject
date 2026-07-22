namespace NotepadPlusPlus.FlaUI.Tests.Configuration;

public static class TestSettings
{
    public const string DefaultNotepadPlusPlusPath = @"C:\Program Files\Notepad++\notepad++.exe";

    public static string NotepadPlusPlusPath =>
        Environment.GetEnvironmentVariable("NOTEPAD_PLUS_PLUS_PATH")
        ?? DefaultNotepadPlusPlusPath;

    public static string ArtifactsRoot =>
        Environment.GetEnvironmentVariable("NOTEPAD_PLUS_PLUS_ARTIFACTS")
        ?? Path.Combine(TestContext.CurrentContext.WorkDirectory, "TestArtifacts");

    public static string ProjectRoot =>
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));

    public static string ReportRoot =>
        Environment.GetEnvironmentVariable("NOTEPAD_PLUS_PLUS_REPORTS")
        ?? Path.Combine(ProjectRoot, "Reporting", "report");
}