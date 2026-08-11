# Unity 6000.3+ Module Guide

Use Unity's public main toolbar API. Do not implement `ILegacyToolbarModule`, inspect `MainToolbarWindow`, or query generated toolbar `VisualElement` names.

## Example: Open Project Folder

Create the following Editor-only module in the project rather than the package:

```csharp
using System.IO;
using NKStudio.CustomToolbar;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

[InitializeOnLoad]
internal sealed class OpenProjectFolderToolbarModule : IToolbarModule
{
    private const string ModuleId = "Game.Toolbar.OpenProjectFolder";
    private const string ElementPath = "Game Toolbar/Open Project Folder";

    static OpenProjectFolderToolbarModule()
    {
        ToolbarModuleRegistry.Register(new OpenProjectFolderToolbarModule());
    }

    public string Id => ModuleId;

    public void Refresh(ToolbarRefreshReason reason)
    {
        MainToolbar.Refresh(ElementPath);
    }

    [MainToolbarElement(
        ElementPath,
        defaultDockPosition = MainToolbarDockPosition.Right,
        defaultDockIndex = 20)]
    private static MainToolbarElement CreateElement()
    {
        var content = new MainToolbarContent(
            "Project Folder",
            "Open the current project folder.");

        return new MainToolbarButton(content, OpenProjectFolder)
        {
            enabled = ToolbarUtility.IsNotPlaying
        };
    }

    private static void OpenProjectFolder()
    {
        string projectPath = Path.GetDirectoryName(Application.dataPath);
        EditorUtility.RevealInFinder(projectPath);
    }
}
```

## Rules

- The `MainToolbar.Refresh` argument must exactly equal the `MainToolbarElement` path.
- Recreate content and state in the attributed factory; Unity calls it again after refresh.
- Set `displayed` in the returned element when the module owns a visibility setting.
- Use the appropriate `MainToolbarButton`, `MainToolbarToggle`, `MainToolbarDropdown`, or `MainToolbarSlider` descriptor.
- Do not retain or query the generated `VisualElement`.
