# Framework Proposal

## Assumptions

- The application under test is a Windows desktop product built on a .NET-friendly stack.
- The team wants a framework that one test engineer can stand up and other engineers or testers can extend without specialist tooling.
- The first automation targets should prove business-critical regression value rather than maximize test count.

## Tooling recommendation

I would recommend a layered Windows UI automation framework using NUnit for execution, FlaUI for Windows UI Automation, and an HTML reporter such as ExtentReports for evidence capture. This keeps the stack close to the product technology, reduces infrastructure overhead, and makes the suite easier to own long term.

Alternatives such as WinAppDriver + Appium or image-based tools are worth considering, but I would set them aside initially. WinAppDriver introduces another runtime dependency and service layer. Image-based tools are more fragile under display scaling, theme changes, and CI session differences.

## Framework structure

The framework should separate test intent from UI mechanics.

- `Tests/`: scenario-driven test cases only
- `PageObjects/`: editor, menus, dialogs, and shared desktop flows
- `Infrastructure/`: app launch, session control, teardown, evidence capture
- `Support/`: clipboard, keyboard abstraction, test data, artifact paths, retry metadata
- `Reporting/`: HTML report lifecycle and screenshot attachment

For patterns, I would use a Page Object Model with dialog abstractions and keep OS-specific behavior below the test layer. If the suite later needs macOS support, I would introduce a factory or strategy layer that returns the correct file dialog and shortcut implementation per platform.

## What to automate first

I would automate workflows that are stable, clinically relevant, and high-signal when they fail.

- File open/save round trips
- Search and replace behavior that risks document corruption
- Unicode and encoding persistence
- Unsaved changes prompts and destructive-close safeguards
- Core keyboard shortcuts used constantly by real users

I would explicitly avoid automating highly cosmetic behaviors early, low-value menu permutations, or unstable areas that the team does not yet understand. The initial goal is trustworthy regression coverage, not broad but noisy coverage.

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