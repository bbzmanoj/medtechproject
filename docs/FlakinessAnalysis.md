# Flakiness Analysis

The most common desktop UI flakiness drivers are focus loss, timing races between the test runner and the UI thread, modal dialogs appearing later than expected, and environmental drift such as display scaling or background notifications. For that reason, I would treat flakiness as an engineering signal rather than a nuisance to hide.

In practice, I would track flaky behavior in three ways. First, every failure should retain artifacts: screenshot, test name, attempt number, and the specific step that failed. Second, retries should be selective and visible. In this suite the flaky demonstration test is explicitly tagged and retried, while normal failures still fail immediately. Third, I would trend retry counts over time. A test that passes only on retry is still unhealthy and should be visible in dashboards and release discussions.

My escalation rule would be simple. If a test flakes twice in one week, it gets triaged. If it blocks releases or hides a suspected product defect, it becomes a priority fix. The possible outcomes are to harden the locator or wait logic, isolate an environmental dependency, quarantine the test temporarily with an owner and expiry date, or convert the check to a lower layer if UI is not the right place to verify it.

The key principle is that retries are a containment measure, not a success criterion. A suite that needs frequent retries is telling the team something useful about either the product or the framework.