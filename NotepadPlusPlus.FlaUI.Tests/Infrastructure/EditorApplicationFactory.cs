using NotepadPlusPlus.FlaUI.Tests.Abstractions;
using NotepadPlusPlus.FlaUI.Tests.Platforms.Windows;

namespace NotepadPlusPlus.FlaUI.Tests.Infrastructure;

public static class EditorApplicationFactory
{
    public static IEditorApplication Create(NotepadPlusPlusSession session)
    {
        return new NotepadPlusPlusApplication(session);
    }
}