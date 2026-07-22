# Tool Selection Decision

## Context

The target application is Notepad++ on Windows and the requested implementation here is explicitly FlaUI-based. The assessment itself asks for a cross-platform approach, so the tooling decision needs to be honest about where FlaUI fits and where it does not.

## Candidate frameworks considered

| Framework | Cross-platform support | CI/CD friendliness | Maintainability | Community / docs | Licensing cost |
| --- | --- | --- | --- | --- | --- |
| FlaUI | Windows only | Good on self-hosted Windows runners with interactive desktop access; weak on hosted desktop CI | Strong for Windows desktop apps because it maps cleanly to UI Automation and C# | Mature enough, especially for .NET teams | Free / open source |
| WinAppDriver + Appium | Windows only | Usable in CI, but adds WebDriver server overhead and another moving part | Moderate; extra infrastructure and more brittle session setup | Larger ecosystem than FlaUI, but slower project momentum | Free |
| PyAutoGUI | Windows and macOS | Weak in headless or scaled CI environments because image recognition is sensitive to resolution, theme, and focus | Lower for long-term desktop regression suites because locators are visual rather than semantic | Broad community, but not ideal for accessibility-driven desktop testing | Free |

## Final choice for this repo

I chose FlaUI for the Notepad++ implementation in this repository.

## Why FlaUI

- It aligns with a .NET-heavy Windows desktop ecosystem, which is close to the Medtech product context.
- It uses Windows UI Automation rather than image matching, so locators and interactions are more maintainable than screen-coordinate automation.
- It keeps the stack simple: one language, one test framework, and no separate automation server.
- It supports the Page Object style cleanly and allows UI-aware failure capture.

## Trade-offs accepted

- FlaUI does not cover macOS. That means this repo does not fully satisfy the assessment's original cross-platform requirement on its own.
- Hosted CI for desktop UI remains difficult because interactive desktop automation is fundamentally different from API or browser automation.
- Some native dialogs and custom editor controls require fallback keyboard interactions when the accessibility tree is thin.

## Why the alternatives were set aside

### WinAppDriver + Appium

This remains a valid Windows option, but it adds a service dependency and a remote-driver architecture that is unnecessary for a local Notepad++ proof of concept. For a first framework in a .NET desktop estate, FlaUI is simpler to own.

### PyAutoGUI

PyAutoGUI solves the cross-platform constraint better than FlaUI, but it pays for that with image-based fragility. In a CI pipeline with display scaling, focus changes, or theme differences, the maintenance cost rises quickly. For a clinical desktop product, semantic locators are preferable wherever possible.

## Practical recommendation

For the exact Medtech assessment, I would present this as the Windows reference implementation and propose a separate macOS adapter behind the same test-facing abstraction. That preserves good Windows maintainability while keeping the cross-platform architecture honest.