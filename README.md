# Notepad++ FlaUI Automation Suite

This repository scaffolds a Windows desktop UI automation suite for Notepad++ using C#, NUnit, and FlaUI. It is designed to support Medtech's Windows desktop applications, which are built primarily in .NET, with a framework shape that supports future platform expansion.

## What this repo includes

- A .NET Framework 4.8 NUnit test project that launches a dedicated Notepad++ instance for each test.
- A desktop-oriented Page Object Model for the main editor, menu bar, Replace dialog, native file dialogs, and unsaved-changes dialog, wrapped by higher-level editor abstractions.
- Windows-focused automated scenarios for file round-trip, replace/undo, Unicode handling, dialog behavior, shortcut abstraction, and a deliberately flaky retry example.
- Extent HTML reporting with screenshot capture on failure, published to an openable `index.html`.
- A GitHub Actions workflow with a working Windows execution lane and a macOS TextEdit lane executed through AppleScript on a macOS runner.
- Supporting design and implementation docs under `docs/`.

## Future macOS implementation

The current implementation runs the Windows lane through FlaUI and Notepad++.

The framework is already structured so a future macOS lane can be added without rewriting the scenario tests:

- `Abstractions/` defines the platform-neutral editor contracts.
- `Platforms/Windows/` contains the working Windows adapter that wraps the existing FlaUI implementation.
- `Platforms/Mac/` contains the TextEdit adapter contract and generated Mac-lane outputs.

That means the existing Windows suite remains fully operational today, while the project structure already demonstrates how a macOS implementation would plug into the same scenario-facing API later.

The repository also includes a GitHub Actions macOS runner lane that executes a TextEdit scenario runner through AppleScript. The Windows lane continues to run the real FlaUI-based tests.

## Prerequisites

- Windows 10 or later
- .NET SDK installed
- Notepad++ installed at `C:\Program Files\Notepad++\notepad++.exe`

Optional environment variables:

- `NOTEPAD_PLUS_PLUS_PATH`: override the Notepad++ executable path
- `NOTEPAD_PLUS_PLUS_ARTIFACTS`: override the per-test artifact output directory
- `NOTEPAD_PLUS_PLUS_REPORTS`: override the HTML report output directory

## Install and run

```powershell
dotnet restore .\NotepadPlusPlus.FlaUI.sln
dotnet test .\NotepadPlusPlus.FlaUI.Tests\NotepadPlusPlus.FlaUI.Tests.csproj
```

You can also use npm as a thin wrapper around `dotnet test`:

```powershell
npm test
npm run test:smoke
npm run test:file -- DialogBehaviorTests.cs
npm run test:tests
npm run test:one -- "Name~Launches_NotepadPlusPlus_MainWindow"
```

Run a focused smoke test:

```powershell
dotnet test .\NotepadPlusPlus.FlaUI.Tests\NotepadPlusPlus.FlaUI.Tests.csproj --filter Launches_NotepadPlusPlus_MainWindow
```

Artifacts are written under `NotepadPlusPlus.FlaUI.Tests\TestArtifacts` by default.
The HTML report is generated on every run at `NotepadPlusPlus.FlaUI.Tests\Reporting\report\index.html` by default, or under `%NOTEPAD_PLUS_PLUS_REPORTS%\index.html` when the report root is overridden.

## Recommended VS Code extensions

- C# Dev Kit or the C# extension for editing, project loading, debugging, and test discovery

The extension is recommended for productivity, but not required for FlaUI itself to run.

## Project structure

```text
NotepadPlusPlus.FlaUI.Tests/
  Abstractions/
  Configuration/
  Infrastructure/
  PageObjects/
    Dialogs/
    Menus/
  Platforms/
    Mac/
    Windows/
  Reporting/
  Support/
  Tests/
    SmokeTests/
docs/
.github/workflows/
```

## Cross-platform structure

The framework is shaped so another platform can be added without rewriting the scenarios.

- `Abstractions/` defines platform-neutral editor contracts used by the scenario tests.
- `Platforms/Windows/` adapts those contracts to the existing FlaUI-based Notepad++ implementation.
- `Platforms/Mac/` is a placeholder for a future TextEdit adapter.

That means the current Windows suite still runs exactly as before, while the design demonstrates how a macOS lane would plug into the same scenario-facing API later.

## Known limitations

- The Notepad++ UI tree can vary slightly across versions and plugins, so dialog locators may need minor adjustments on another machine.
- The UTF-8 save path assumes a standard Notepad++ installation and keeps encoding selection best-effort at the POM layer.
- GitHub-hosted Windows runners are not a reliable place for interactive desktop automation. The provided workflow is best suited to a self-hosted Windows runner with an interactive session.

## AI usage note

AI assistance was used to accelerate initial scaffolding, documentation drafting, and framework structuring. The generated output still requires human review, locator refinement, and execution validation against the target Notepad++ version.