# Tooling Selection: Windows Desktop UI Automation

## Recommended tool(s)

The primary recommendation is **FlaUI** for Windows desktop UI automation.

FlaUI is a strong fit because it is a C# wrapper over Windows UI Automation, which means it works naturally with a .NET-based test stack and gives direct access to native Windows controls, dialogs, and window state. For applications like Notepad++ and similar Windows desktop products, that direct access matters more than generic cross-platform tooling because the main challenge is interacting reliably with rich desktop behavior rather than browser-like navigation.

FlaUI also keeps the automation stack relatively lean. It does not require a separate driver service, it models windows and controls in a way that maps well to page objects, and it supports the kinds of interactions that commonly become fragile in desktop automation: modal dialogs, focus changes, native file pickers, and mixed standard/custom controls.

## Factors influencing the recommendation

- **Technology stack alignment:** The application under test and the automation project are both in the Microsoft/.NET ecosystem, so using a C#-native automation framework reduces friction.
- **Windows desktop specialization:** The target problem is Windows desktop UI automation, and FlaUI is built specifically for that environment rather than treating Windows as one driver target among many.
- **Maintainability:** A smaller stack with fewer external moving parts is easier to debug, upgrade, and support over time.
- **Team skills:** A team already comfortable with C# and .NET can work in one language, use familiar tooling, and avoid context switching into a different automation model.
- **Locator and dialog reliability:** Windows UI Automation access gives better control over windows, menus, file dialogs, and accessibility-backed elements.
- **Long-term supportability:** Tests are easier to reason about when the framework is close to the platform and the application technology, especially for a suite expected to grow over time.
- **Debugging speed:** Local troubleshooting is simpler when failures can be traced directly through the application, automation layer, and test code without an additional driver process in the middle.

## Options compared

| Option | Strengths | Weaknesses | Fit for Medtech |
| --- | --- | --- | --- |
| FlaUI | Native C# API, direct Windows UI Automation access, no separate server, good control over windows and dialogs | Windows only, still sensitive to desktop-session CI limits | Best fit for a .NET-heavy Windows desktop estate |
| WinAppDriver + Appium | Familiar WebDriver model, broader ecosystem, easier for teams already invested in Appium | Requires a separate driver service, more moving parts, slower setup/debug loop, less direct control for rich Windows desktop behavior | Acceptable, but heavier than needed for a Windows-first .NET suite |

## Decision

FlaUI is the recommended default for the Windows desktop automation lane.

## Alternatives considered and why they were set aside

### WinAppDriver + Appium

WinAppDriver + Appium was the main alternative considered. It has clear strengths: a familiar WebDriver model, broader ecosystem visibility, and some appeal for teams that already standardize on Appium. It was set aside because it introduces extra infrastructure and operational overhead without solving the core problem better for this context.

For a Windows-first .NET suite, the additional driver layer makes setup, execution, and debugging heavier. The team still has to solve the hard parts of desktop automation, such as native dialogs, focus changes, and control discovery, but now with an additional service boundary to manage. Even when license cost is low or effectively free, the operational cost is still higher because more components have to be installed, versioned, and supported. That tradeoff is less attractive when the expected value is maintainability and directness rather than cross-platform uniformity.

### Other approaches set aside

- **Raw UI Automation libraries:** These provide flexibility, but they place more low-level plumbing burden on the test framework and the team.
- **Commercial desktop automation tools:** These can be capable, but they often add licensing cost, proprietary workflows, and less natural integration with a code-first .NET test suite.
- **Cross-platform-first frameworks:** These are less compelling when the target application is explicitly Windows desktop and the critical requirement is deep Windows UI interaction.

## Summary recommendation

Use **FlaUI** as the default framework for Windows desktop UI automation in this codebase. It best matches the current .NET stack, the team's likely skills, and the long-term maintenance needs of a Windows-native test suite. WinAppDriver + Appium remains a viable alternative, but it was not chosen because it adds more infrastructure without providing a better fit for the core automation problem.
