#if UNITY_6000_3_OR_NEWER
using UnityEditor;

namespace NKStudio.CustomToolbar
{
    [InitializeOnLoad]
    internal static class ModernToolbarHost
    {
        static ModernToolbarHost()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorBuildSettings.sceneListChanged -= OnSceneListChanged;
            EditorBuildSettings.sceneListChanged += OnSceneListChanged;
            EditorApplication.delayCall += ToolbarModuleRegistry.Initialize;
        }

        private static void OnSceneListChanged()
        {
            ToolbarModuleRegistry.RefreshAll(ToolbarRefreshReason.SceneListChanged);
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange _)
        {
            ToolbarModuleRegistry.RefreshAll(ToolbarRefreshReason.PlayModeChanged);
        }
    }
}
#endif
