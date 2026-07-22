# Tool Decision: Why FlaUI for the Patient Inbox User Story

## Context

Medtech's products are Windows desktop applications built primarily in .NET, so the framework choice should favor strong Windows desktop support, good C# integration, and maintainable UI locators. For this repository the target application is Notepad++, which exposes a mixture of standard Windows UI, native dialogs, and custom editor surfaces. That makes the quality of the Windows automation layer more important than nominal cross-platform reach.

For the user story "Patient Inbox email multiple items", the automation problem is not just clicking through a simple form. The likely path includes selecting multiple patient inbox records, invoking an email action, validating the correct items are included, handling modal windows or confirmation prompts, and checking that the final state remains correct after the action completes or is cancelled. That is exactly the kind of Windows desktop behavior where the automation layer needs strong control over windows, focus changes, dialogs, and accessibility-backed locators.

## Options compared

| Option | Strengths | Weaknesses | Fit for Medtech |
| --- | --- | --- | --- |
| FlaUI | Native C# API, direct Windows UI Automation access, no separate server, good control over windows and dialogs | Windows only, still sensitive to desktop-session CI limits | Best fit for a .NET-heavy Windows desktop estate and story-driven inbox workflows |
| WinAppDriver + Appium | Familiar WebDriver model, broader ecosystem, easier for teams already invested in Appium | Requires a separate driver service, more moving parts, slower setup/debug loop, less direct control for rich Windows desktop behavior | Acceptable, but heavier than needed for a Windows-first .NET suite |

## Decision

FlaUI is the better choice for this Medtech-focused Windows lane, especially for the Patient Inbox email-multiple-items workflow.

## Why FlaUI is useful for this user story

This user story depends on reliable interaction with desktop UI states, not just page navigation. A patient inbox flow often includes grid selection, toolbar or context-menu actions, modal email composition windows, file or attachment pickers, and confirmation dialogs. FlaUI is well suited to that because it talks directly to Windows UI Automation and lets the tests reason about windows, controls, focus, and dialog ownership more explicitly.

That matters when validating business behavior such as:

- selecting several inbox items and confirming the right records stay selected
- opening the email action and asserting the compose window appears with the expected context
- verifying attachments, recipients, or summary text associated with the chosen items
- handling warnings, cancel actions, and confirmation dialogs without losing application state
- checking the UI after completion to confirm the workflow succeeded or safely rolled back

For a Medtech user story, those checks need to be trustworthy because the value is in validating operator workflow, not in proving that a driver service can launch. FlaUI keeps the test close to the real Windows UI and reduces the layers between the scenario and the assertion.

## Why FlaUI won

- **Engineering fit:** FlaUI stays in C# for a Windows desktop application, which keeps the framework aligned with the .NET stack and easier for the team to own.
- **Direct UI Automation:** FlaUI works directly with Windows UI Automation, making dialogs, file pickers, and custom controls easier to locate and handle.
- **Simpler debugging:** FlaUI avoids the extra driver lifecycle and session-management overhead that comes with WinAppDriver + Appium.
- **Better scenario modeling:** The inbox window, email dialog, and confirmation prompts can be represented cleanly in page objects and dialog objects.
- **Transparent dialog handling:** Direct automation access makes modal windows and confirmation flows easier to inspect and stabilize.
- **Workflow coverage:** Multi-selection, confirmation behavior, and final-state validation map well to FlaUI's object model.
- **Lower maintenance:** One language and one runtime reduce operational complexity for a .NET-focused team.
- **Faster failure analysis:** A thinner framework stack makes issues easier to trace and debug locally.

## Why not WinAppDriver + Appium

- **Extra infrastructure:** WinAppDriver + Appium introduces a separate driver service and more moving parts without improving this workflow's business validation.
- **Weaker fit for this app type:** Its WebDriver-style model is less compelling when the product is a Windows-native .NET desktop application.
- **Same desktop challenges anyway:** The team would still need to handle multi-selection, modal dialogs, focus changes, compose windows, and confirmation prompts.
- **Higher maintenance cost:** More runtime dependencies and session-management concerns make the setup heavier to support.
- **Lower practical value here:** For Medtech, maintainability matters more than broader ecosystem reach, which makes FlaUI the stronger default choice.