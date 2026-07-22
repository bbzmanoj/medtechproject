This folder contains the macOS TextEdit adapter contract and generated outputs for the GitHub Actions macOS lane.

- `adapter-contract.json` defines the current runner shape, artifact locations, and scenario implementation status.
- `artifacts/` is generated at runtime with JSON results and any Mac-specific outputs.
- `report/` is generated at runtime with the macOS HTML summary.

The current Windows suite still runs through `Platforms/Windows`, while the Mac lane executes through `scripts/run-macos-tests.js` and `scripts/macos/run-textedit-scenarios.applescript` on a macOS runner.