---
name: extend-custom-toolbar
description: Create or modify extension modules for NKStudio Custom Toolbar in Unity Editor projects. Use when adding toolbar buttons, toggles, dropdowns, sliders, module registration, or toolbar lifecycle behavior, especially when the implementation must inspect ProjectSettings/ProjectVersion.txt and select the Unity 2022.3–6000.2 LegacyToolbarBuilder API or the Unity 6000.3+ MainToolbarElement API.
---

# Extend Custom Toolbar

Create project-specific Custom Toolbar modules using the API supported by the project's actual Unity version.

## Workflow

1. Locate the Unity project root containing `Assets`, `Packages`, and `ProjectSettings`.
2. Read `ProjectSettings/ProjectVersion.txt` and extract `m_EditorVersion`.
3. Confirm `Packages/com.nkstudio.custom-toolbar/package.json` and the public types in `Editor/ToolbarRegister/ToolbarModuleRegistry.cs` exist. If the package uses another location, locate it before editing.
4. Select exactly one implementation guide:
   - Unity 2022.3 through Unity 6000.2: read [references/unity-2022-6000-2.md](references/unity-2022-6000-2.md).
   - Unity 6000.3 or newer: read [references/unity-6000-3-plus.md](references/unity-6000-3-plus.md).
   - Earlier than Unity 2022.3: report that the package does not support the project and do not invent a fallback.
   - Only when the user explicitly requires one source to support both branches: read both references and combine them with `UNITY_6000_3_OR_NEWER` guards.
5. Inspect the nearest parent `.asmdef` for the destination. Add an Editor-only asmdef reference to `NKStudio.CustomToolbar.Editor` only when the extension assembly does not already see it.
6. Implement and validate the module.

Do not infer the Unity version from package folders, compiler symbols in unrelated code, or memory. Use `ProjectVersion.txt` as the source of truth.

## Placement

Create project-specific modules outside the package by default:

```text
Assets/Editor/ToolbarModules/<FeatureName>ToolbarModule.cs
```

Do not edit `Packages/com.nkstudio.custom-toolbar` for a project-specific feature. Package contents can be replaced by an update. Edit the package only when the user explicitly requests a built-in module intended for distribution with the package.

## Required Conventions

- Use a stable, globally unique module ID such as `Company.Product.Toolbar.Feature`.
- Register exactly once from an `[InitializeOnLoad]` static constructor with `ToolbarModuleRegistry.Register(...)`.
- Implement `IToolbarModule.Refresh(ToolbarRefreshReason)` instead of subscribing to `EditorApplication.playModeStateChanged` or `EditorBuildSettings.sceneListChanged` inside the module.
- Never use anonymous Editor lifecycle subscriptions to update toolbar state.
- Keep module actions Editor-only and place source under an Editor folder or Editor-only asmdef.
- Use `ToolbarAvailability.InEditMode` for edit-only controls.
- Do not add the external module to `CustomToolbarSettings`; manage feature-specific settings separately unless changing the distributed package itself.
- Preserve existing package public APIs. Do not change the registry merely to add one module.

## Assembly Definition

If the destination has its own asmdef, use this minimum shape and choose a project-specific assembly name:

```json
{
  "name": "Game.CustomToolbarExtensions.Editor",
  "references": [
    "NKStudio.CustomToolbar.Editor"
  ],
  "includePlatforms": [
    "Editor"
  ]
}
```

Do not create another asmdef when an existing parent Editor asmdef is the correct owner. When no asmdef owns a script under `Assets/Editor`, the package core is auto-referenced.

## Validation

1. Confirm the module ID is not duplicated anywhere in `Assets` or `Packages`.
2. Confirm modern `MainToolbar.Refresh(path)` uses the exact path declared by `MainToolbarElement`.
3. Confirm Legacy code adds each element through `LegacyToolbarBuilder` and retains references only for later refresh.
4. Search the new module for direct lifecycle subscriptions and remove them.
5. Compile with the Unity Editor version recorded in `ProjectVersion.txt` when that Editor is installed.
6. Verify creation, action execution, and state changes across Edit Mode and Play Mode as applicable.

Never claim compatibility with a Unity branch that was not compiled or otherwise verified.
