# Framework Proposal

## Assumptions

- The application under test is a Windows desktop product built on a .NET-friendly stack.
- The team wants a framework that one test engineer can stand up and other engineers or testers can extend without specialist tooling.
- The first automation targets should prove business-critical regression value rather than maximize test count.

## Tooling recommendation

I would recommend a layered Windows UI automation framework using NUnit for execution, FlaUI for Windows UI Automation, and an HTML reporter such as ExtentReports for evidence capture. This keeps the stack close to the product technology, reduces infrastructure overhead, and makes the suite easier to own long term.

If the team wants a BDD layer, I would add **Reqnroll** on top of NUnit and keep FlaUI underneath as the Windows automation engine. That adds readable feature files and step definitions without changing the core page-object and abstraction structure.

Alternatives such as WinAppDriver + Appium or image-based tools are worth considering, but I would set them aside initially. WinAppDriver introduces another runtime dependency and service layer. Image-based tools are more fragile under display scaling, theme changes, and CI session differences.

## Framework structure

The framework should separate test intent from UI mechanics.

- `Tests/`: scenario-driven test cases only
- `Abstractions/`: platform-neutral contracts that define what the test layer can do without depending on a concrete UI implementation
- `PageObjects/`: editor, menus, dialogs, and shared desktop flows
- `Platforms/`: platform-specific implementations that adapt the abstractions to Windows or another target environment
- `Infrastructure/`: app launch, session control, teardown, evidence capture
- `Support/`: clipboard, keyboard abstraction, test data, artifact paths, retry metadata
- `Reporting/`: HTML report lifecycle and screenshot attachment

The abstraction layer is required here because the tests already work against a platform-neutral application contract rather than directly against the Windows page objects. That gives the suite a stable API for common actions such as open, save, replace, and close, while keeping the concrete Windows automation details behind the implementation layer.

This separation is useful for three reasons:

- it keeps tests focused on scenario intent instead of FlaUI-specific interaction details
- it allows the implementation to change without rewriting test bodies
- it gives the framework a clear seam for future platform adapters or alternate dialog implementations

For patterns, I would use a Page Object Model with dialog abstractions and keep OS-specific behavior below the test layer. In the current structure, the factory and abstraction layers already provide that seam, even though the active implementation is still Windows-first.

## What to automate first

I would automate workflows that are stable, clinically relevant, and high-signal when they fail.

- File open/save round trips
- Search and replace behavior that risks document corruption
- Unicode and encoding persistence
- Unsaved changes prompts and destructive-close safeguards
- Core keyboard shortcuts used constantly by real users

I would explicitly avoid automating highly cosmetic behaviors early, low-value menu permutations, or unstable areas that the team does not yet understand. The initial goal is trustworthy regression coverage, not broad but noisy coverage.

## BDD pilot for the selected test

If the framework is extended with BDD, I would start with a single pilot conversion rather than rewriting the suite. The best initial candidate in the current code is `OpenDialog_LoadsExistingFile` from `DocumentWorkflowTests` because it is small, clear, and already maps directly to user behavior.

Example Reqnroll feature:

```gherkin
Feature: Document workflow

	Scenario: Open dialog loads an existing file
		Given Notepad++ is open
		And an existing file contains "Pre-existing file content for open dialog validation."
		When I open that file through the Open dialog
		Then the editor should display the existing file content
```

In this model, the Reqnroll step definitions should stay thin and call the same framework methods the NUnit test already uses. The step layer would prepare the file, call the application abstraction to open it, and assert against the editor text. FlaUI-specific locator logic should remain in page objects and dialogs, not in the BDD steps.

This approach keeps the existing framework intact while proving whether BDD adds value for stakeholder readability and test collaboration.

## CI/CD integration

Desktop UI suites should run in a dedicated Windows agent with interactive desktop access. For CI, I would run smoke coverage on each pull request and a fuller regression pack on a scheduled or controlled branch basis. A meaningful quality gate would require:

- all smoke tests green
- no new flaky-test additions without triage
- screenshots and logs attached for every failure
- trend visibility on pass rate and retry rate

## Risks and mitigations

- Desktop UI automation is sensitive to focus and timing. Mitigation: explicit waits, isolated test instances, and disciplined retry only for known flaky cases.
- Native dialogs differ from application-owned windows. Mitigation: keep dialog logic in dedicated abstractions, not in test bodies.
- Over-automation too early can create maintenance drag. Mitigation: prioritize by product risk, defect history, and workflow criticality.