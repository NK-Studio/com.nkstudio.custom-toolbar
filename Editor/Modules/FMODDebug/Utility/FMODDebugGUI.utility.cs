using System.Reflection;
using FMODUnity;
using UnityEditor;

namespace NKStudio.CustomToolbar.FMOD
{
    internal sealed partial class FmodDebugToolbarModule
    {
        private static bool IsFmodDebugActive => Settings.Instance.PlayInEditorPlatform.Overlay == TriStateBool.Enabled;

        private static void SetFmodDebugOverlay(bool isEnabled)
        {
            var editorPlatform = Settings.Instance.PlayInEditorPlatform;
            FieldInfo properties = editorPlatform.GetType().GetField("Properties",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            object editorPlatformSettings = properties?.GetValue(editorPlatform);
            FieldInfo overlay = editorPlatformSettings?.GetType().GetField("Overlay",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            object overlayValue = overlay?.GetValue(editorPlatformSettings);
            FieldInfo value = overlayValue?.GetType().GetField("Value",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (value == null)
                return;

            value.SetValue(overlayValue, isEnabled ? TriStateBool.Enabled : TriStateBool.Disabled);
            EditorUtility.SetDirty(editorPlatform);
        }
    }
}
