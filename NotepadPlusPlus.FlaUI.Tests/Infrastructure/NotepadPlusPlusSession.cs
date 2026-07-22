using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using NotepadPlusPlus.FlaUI.Tests.Configuration;
using NotepadPlusPlus.FlaUI.Tests.Support;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using FlaUiApplication = FlaUI.Core.Application;

namespace NotepadPlusPlus.FlaUI.Tests.Infrastructure;

public sealed class NotepadPlusPlusSession : IDisposable
{
    private readonly UIA3Automation automation;

    private NotepadPlusPlusSession(FlaUiApplication application, Window mainWindow, UIA3Automation automation)
    {
        Application = application;
        MainWindow = mainWindow;
        this.automation = automation;
    }

    public FlaUiApplication Application { get; }

    public Window MainWindow { get; }

    public UIA3Automation Automation => automation;

    public static NotepadPlusPlusSession Launch(string? filePath = null)
    {
        if (!File.Exists(TestSettings.NotepadPlusPlusPath))
        {
            throw new FileNotFoundException("Notepad++ executable was not found.", TestSettings.NotepadPlusPlusPath);
        }

        var arguments = filePath is null
            ? "-multiInst -nosession"
            : $"-multiInst -nosession \"{filePath}\"";

        var application = FlaUiApplication.Launch(TestSettings.NotepadPlusPlusPath, arguments);
        var automation = new UIA3Automation();
        var mainWindow = application.GetMainWindow(automation, TimeSpan.FromSeconds(10));

        if (mainWindow is null)
        {
            automation.Dispose();
            application.Dispose();
            throw new InvalidOperationException("Notepad++ main window was not detected within the timeout.");
        }

        return new NotepadPlusPlusSession(application, mainWindow, automation);
    }

    public void BringToFront()
    {
        MainWindow.Focus();
        MainWindow.SetForeground();
    }

    public Window WaitForTopLevelWindow(Func<Window, bool> predicate, TimeSpan timeout)
    {
        var window = TryFindTopLevelWindow(predicate, timeout);
        if (window is null)
        {
            throw new TimeoutException("Expected top-level window was not found within the timeout.");
        }

        return window;
    }

    public Window WaitForDesktopWindow(Func<Window, bool> predicate, TimeSpan timeout)
    {
        var window = TryFindDesktopWindow(predicate, timeout);
        if (window is null)
        {
            throw new TimeoutException("Expected desktop window was not found within the timeout.");
        }

        return window;
    }

    public Window? TryFindTopLevelWindow(Func<Window, bool> predicate, TimeSpan timeout)
    {
        var stopwatch = Stopwatch.StartNew();

        while (stopwatch.Elapsed < timeout)
        {
            if (Application.HasExited)
            {
                return null;
            }

            foreach (var window in Application.GetAllTopLevelWindows(automation))
            {
                if (predicate(window))
                {
                    return window;
                }
            }

            Thread.Sleep(UiTiming.WindowSearchRetryDelay);
        }

        return null;
    }

    public Window? TryFindDesktopWindow(Func<Window, bool> predicate, TimeSpan timeout)
    {
        var stopwatch = Stopwatch.StartNew();

        while (stopwatch.Elapsed < timeout)
        {
            try
            {
                var desktop = automation.GetDesktop();

                foreach (var window in FindMatchingWindows(desktop.FindAllChildren(cf => cf.ByControlType(ControlType.Window)), predicate))
                {
                    return window;
                }

                foreach (var window in FindMatchingWindows(desktop.FindAllDescendants(cf => cf.ByControlType(ControlType.Window)), predicate))
                {
                    return window;
                }
            }
            catch (COMException)
            {
                Thread.Sleep(UiTiming.WindowSearchRetryDelay);
                continue;
            }

            Thread.Sleep(UiTiming.WindowSearchRetryDelay);
        }

        return null;
    }

    private static IEnumerable<Window> FindMatchingWindows(IEnumerable<AutomationElement> elements, Func<Window, bool> predicate)
    {
        foreach (var element in elements)
        {
            Window window;

            try
            {
                window = element.AsWindow();
            }
            catch (COMException)
            {
                continue;
            }

            if (predicate(window))
            {
                yield return window;
            }
        }
    }

    public void CaptureScreenshot(string filePath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        using var bitmap = MainWindow.Capture();
        bitmap?.Save(filePath, ImageFormat.Png);
    }

    public void Dispose()
    {
        try
        {
            if (!Application.HasExited)
            {
                Application.Close();

                if (!Application.HasExited)
                {
                    Application.Kill();
                }
            }
        }
        finally
        {
            automation.Dispose();
            Application.Dispose();
        }
    }
}