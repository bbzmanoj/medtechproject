using NotepadPlusPlus.FlaUI.Tests.Bdd;
using NotepadPlusPlus.FlaUI.Tests.Support;
using Reqnroll;

namespace NotepadPlusPlus.FlaUI.Tests.Steps;

[Binding]
public sealed class DocumentWorkflowSteps
{
    private readonly DocumentWorkflowScenarioContext context;

    public DocumentWorkflowSteps(DocumentWorkflowScenarioContext context)
    {
        this.context = context;
    }

    [Given(@"^Notepad\+\+ is open$")]
    public void GivenNotepadPlusPlusIsOpen()
    {
        Assert.That(context.Page.IsOpen, Is.True);
    }

    [Given("I have prepared large document content")]
    public void GivenIHavePreparedLargeDocumentContent()
    {
        context.ExpectedText = TestDataFactory.CreateLargeLoremIpsum(550);
    }

    [Given("I have prepared unicode document content")]
    public void GivenIHavePreparedUnicodeDocumentContent()
    {
        context.ExpectedText = TestDataFactory.CreateUnicodeDocument();
    }

    [Given("an existing file contains {string}")]
    public void GivenAnExistingFileContains(string expectedText)
    {
        context.ExpectedText = expectedText;
        context.PreparedFilePath = context.RegisterCleanupFile(
            ArtifactPaths.CreateFilePath(context.TestArtifactDirectory, "open-dialog.txt"));

        File.WriteAllText(context.PreparedFilePath, expectedText);
    }

    [When("I replace the editor content with the prepared text")]
    public void WhenIReplaceTheEditorContentWithThePreparedText()
    {
        context.Page.Editor.ReplaceAllText(context.ExpectedText);
    }

    [When("I save the document as {string}")]
    public void WhenISaveTheDocumentAs(string fileName)
    {
        context.PreparedFilePath = context.RegisterCleanupFile(
            ArtifactPaths.CreateFilePath(context.TestArtifactDirectory, fileName));

        context.Page.SaveFile(context.PreparedFilePath);
    }

    [When("I save the document as UTF-8 file {string}")]
    public void WhenISaveTheDocumentAsUtf8File(string fileName)
    {
        context.PreparedFilePath = context.RegisterCleanupFile(
            ArtifactPaths.CreateFilePath(context.TestArtifactDirectory, fileName));

        context.Page.SaveFileAsUtf8(context.PreparedFilePath);
    }

    [When("I reopen the saved file")]
    public void WhenIReopenTheSavedFile()
    {
        context.Relaunch(context.PreparedFilePath);
    }

    [When("I open the prepared file through the Open dialog")]
    public void WhenIOpenThePreparedFileThroughTheOpenDialog()
    {
        context.Page.OpenFile(context.PreparedFilePath);
    }

    [Then("the editor should display the prepared text")]
    public void ThenTheEditorShouldDisplayThePreparedText()
    {
        Assert.That(context.Page.Editor.ReadAllText(), Is.EqualTo(context.ExpectedText));
    }
}