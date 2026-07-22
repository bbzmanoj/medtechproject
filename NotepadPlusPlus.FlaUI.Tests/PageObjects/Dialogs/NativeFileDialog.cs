using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using NotepadPlusPlus.FlaUI.Tests.Infrastructure;
using NotepadPlusPlus.FlaUI.Tests.Support;
using System.Diagnostics;
using FlaButton = FlaUI.Core.AutomationElements.Button;

namespace NotepadPlusPlus.FlaUI.Tests.PageObjects.Dialogs;

public sealed class NativeFileDialog
{
    private readonly Window window;

    private NativeFileDialog(NotepadPlusPlusSession session, Window window)
    {
        Session = session;
        this.window = window;
    }

    public NotepadPlusPlusSession Session { get; }

    public static NativeFileDialog Attach(NotepadPlusPlusSession session, string title)
    {
        var dialog = session.WaitForDesktopWindow(
            window => window.Title.IndexOf(title, StringComparison.OrdinalIgnoreCase) >= 0,
            UiTiming.DefaultTimeout);

        return new NativeFileDialog(session, dialog);
    }

    public void Save(string filePath)
    {
        NavigateToDirectory(filePath);
        SetFileName(filePath);
        Confirm();
        WaitForFileToBeCreated(filePath);
        ConfirmOverwriteIfPresent();
        WaitUntilClosed();
    }

    public void Open(string filePath)
    {
        NavigateToDirectory(filePath);
        SetFileName(filePath);
        Confirm();
        WaitUntilClosed();
    }

    private void NavigateToDirectory(string filePath)
    {
        var directoryPath = Path.GetDirectoryName(filePath);
        if (string.IsNullOrWhiteSpace(directoryPath))
        {
            throw new InvalidOperationException($"The file path '{filePath}' does not contain a directory.");
        }

        window.SetForeground();
        KeyboardShortcuts.Send("%d");
        ClipboardHelper.PasteText(directoryPath);
        KeyboardShortcuts.Send(ShortcutKeys.ConfirmDialog);
        WaitForNavigationToSettle();
    }

    private void SetFileName(string filePath)
    {
        window.SetForeground();

        var fileName = Path.GetFileName(filePath);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new InvalidOperationException($"The file path '{filePath}' does not contain a file name.");
        }

        var fileNameField = window.FindFirstDescendant(cf =>
            cf.ByControlType(ControlType.Edit).And(cf.ByName("File name:")))
            ?? window.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit));

        if (fileNameField is null)
        {
            throw new InvalidOperationException("The native file dialog file-name input was not found.");
        }

        fileNameField.Focus();
        fileNameField.Click();
        KeyboardShortcuts.Send(ShortcutKeys.SelectAll);
        ClipboardHelper.PasteText(fileName);
    }

    private static void WaitForNavigationToSettle()
    {
        Thread.Sleep(UiTiming.DialogNavigationDelay);
    }

    private void Confirm()
    {
        window.SetForeground();

        var button = FindActionButton("Save") ?? FindActionButton("Open");
        if (button is not null)
        {
            button.Focus();

            var invokePattern = button.Patterns.Invoke.PatternOrDefault;
            if (invokePattern is not null)
            {
                invokePattern.Invoke();
                return;
            }

            button.Click();
            return;
        }

        KeyboardShortcuts.Send(ShortcutKeys.ConfirmDialog);
    }

    private FlaButton? FindActionButton(string actionName)
    {
        foreach (var candidate in window.FindAllDescendants(cf => cf.ByControlType(ControlType.Button)))
        {
            if (candidate is not FlaButton button)
            {
                continue;
            }

            var name = button.Name;
            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            if (name.IndexOf(actionName, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return button;
            }
        }

        return null;
    }

    private void ConfirmOverwriteIfPresent()
    {
        var overwriteDialog = Session.TryFindDesktopWindow(
            dialog => dialog.Title.IndexOf("Confirm Save As", StringComparison.OrdinalIgnoreCase) >= 0
                || dialog.Title.IndexOf("Confirm", StringComparison.OrdinalIgnoreCase) >= 0,
            UiTiming.ShortTimeout);

        if (overwriteDialog is null)
        {
            return;
        }

        overwriteDialog.SetForeground();
        var yesButton = overwriteDialog.FindFirstDescendant(cf =>
            cf.ByControlType(ControlType.Button).And(cf.ByName("Yes")));

        if (yesButton is not null)
        {
            yesButton.Click();
        }
        else
        {
            KeyboardShortcuts.Send(ShortcutKeys.ConfirmDialog);
        }

        WaitForWindowToClose(overwriteDialog, UiTiming.ShortTimeout);
    }

    private void WaitForFileToBeCreated(string filePath)
    {
        var stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed < UiTiming.DefaultTimeout)
        {
            if (File.Exists(filePath))
            {
                return;
            }

            TryConfirmCreatePrompt();

            Thread.Sleep(UiTiming.PollInterval);
        }

        if (!File.Exists(filePath))
        {
            throw new InvalidOperationException($"The file '{filePath}' was not created after confirming Save As.");
        }
    }

    private void TryConfirmCreatePrompt()
    {
        var desktop = Session.Automation.GetDesktop();
        var prompt = desktop.FindFirstDescendant(cf => cf.ByName("Create new file"))
            ?? Session.MainWindow.FindFirstDescendant(cf => cf.ByName("Create new file"));

        if (prompt is null)
        {
            return;
        }

        var container = prompt.Parent;
        while (container is not null)
        {
            var yesButton = container.FindFirstDescendant(cf =>
                cf.ByControlType(ControlType.Button).And(cf.ByName("Yes")));

            if (yesButton is not null)
            {
                yesButton.Focus();
                yesButton.Click();
                return;
            }

            container = container.Parent;
        }

        var desktopYesButton = desktop.FindFirstDescendant(cf =>
            cf.ByControlType(ControlType.Button).And(cf.ByName("Yes")));

        if (desktopYesButton is not null)
        {
            desktopYesButton.Focus();
            desktopYesButton.Click();
            return;
        }

        KeyboardShortcuts.Send(ShortcutKeys.ConfirmDialog);
    }

    private static void WaitForWindowToClose(Window dialog, TimeSpan timeout)
    {
        var stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed < timeout)
        {
            if (!dialog.IsAvailable)
            {
                return;
            }

            Thread.Sleep(UiTiming.PollInterval);
        }
    }

    private void WaitUntilClosed()
    {
        var stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed < UiTiming.DefaultTimeout)
        {
            if (!window.IsAvailable)
            {
                return;
            }

            Thread.Sleep(UiTiming.PollInterval);
        }
    }
}