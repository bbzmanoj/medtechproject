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

First, it matches the engineering stack. The suite is written in C#, the application domain is Windows desktop, and the team context is .NET-oriented. FlaUI keeps the framework in one language and one runtime, which reduces operational complexity and lowers the barrier to ownership for developers and test engineers.

Second, FlaUI talks directly to Windows UI Automation instead of adding a WebDriver server between the tests and the application. That matters for desktop products with modal dialogs, native file pickers, multi-step desktop workflows, and custom controls. In practice it gives better transparency when locating windows, handling accessibility surfaces, and capturing failures.

Third, FlaUI is simpler to debug locally. WinAppDriver + Appium introduces extra lifecycle concerns such as server startup, port management, capability negotiation, and remote session stability. Those concerns are reasonable in some estates, but they are overhead for a Medtech Windows desktop framework where the main goal is maintainable, trustworthy UI coverage.

Fourth, FlaUI fits better when the team wants tests to read like business scenarios. For the Patient Inbox email-multiple-items story, the automation can be expressed in page objects and dialog objects that map closely to what a user does in the desktop application. That produces clearer test intent and simpler maintenance when the workflow changes.

## Why not WinAppDriver + Appium

WinAppDriver + Appium is still a legitimate Windows automation option, but it is not the best fit here. Its biggest advantage is consistency with teams already standardized on Appium or WebDriver. That advantage is weaker when the target system is a Windows-native .NET application and the automation team can stay fully in C#.

For this user story specifically, WinAppDriver + Appium adds infrastructure that does not directly improve the validation. The team would need to manage the driver process and session lifecycle while still solving the same desktop problems: multi-selection, modal dialogs, focus changes, and workflow assertions. In other words, it adds more moving parts without giving a clear testing benefit for this scenario.

For Medtech, the deciding factor is maintainability over infrastructure breadth. FlaUI gives a thinner stack, fewer runtime dependencies, and a more direct path to stable Windows desktop automation. That makes it the stronger default choice for this project.