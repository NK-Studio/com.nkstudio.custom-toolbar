#if UNITY_6000_3_OR_NEWER
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace NKStudio.CustomToolbar.FMOD
{
    internal sealed partial class FmodDebugToolbarModule
    {
        [MainToolbarElement(ElementPath, defaultDockPosition = MainToolbarDockPosition.Left, defaultDockIndex = 13)]
        private static MainToolbarElement CreateElement()
        {
            string iconPath = AssetDatabase.GUIDToAssetPath("f0de615af202e44a190b20daca134fc4");
            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>($"{iconPath}/{ToolbarStyleUtility.DarkModePrefix}fmod.png");
            var content = new MainToolbarContent("FMOD Debug", tooltip: string.Empty, image: icon);

            return new MainToolbarToggle(content, IsFmodDebugActive, SetFmodDebugOverlay)
            {
                displayed = CustomToolbarSettings.instance.ShowFmodDebug,
                enabled = ToolbarAvailability.InEditMode
            };
        }
    }
}
#endif
