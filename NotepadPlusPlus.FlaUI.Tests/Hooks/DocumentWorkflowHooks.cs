using NotepadPlusPlus.FlaUI.Tests.Bdd;
using Reqnroll;

namespace NotepadPlusPlus.FlaUI.Tests.Hooks;

[Binding]
public sealed class DocumentWorkflowHooks
{
    private readonly DocumentWorkflowScenarioContext context;

    public DocumentWorkflowHooks(DocumentWorkflowScenarioContext context)
    {
        this.context = context;
    }

    [BeforeScenario]
    public void BeforeScenario()
    {
        context.Initialize();
    }

    [AfterScenario]
    public void AfterScenario()
    {
        context.Complete();
    }
}