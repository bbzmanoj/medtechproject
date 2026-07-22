using System.Threading;
using NotepadPlusPlus.FlaUI.Tests.Infrastructure;

namespace NotepadPlusPlus.FlaUI.Tests.SmokeTests;

[TestFixture]
[Apartment(ApartmentState.STA)]
public class LaunchTests
{
    [Test]
    public void Launches_NotepadPlusPlus_MainWindow()
    {
        using var session = NotepadPlusPlusSession.Launch();

        Assert.That(session.MainWindow.Title, Does.Contain("Notepad++"));
    }
}