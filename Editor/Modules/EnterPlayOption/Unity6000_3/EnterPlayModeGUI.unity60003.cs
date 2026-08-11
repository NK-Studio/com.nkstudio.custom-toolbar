#if UNITY_6000_3_OR_NEWER
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace NKStudio.CustomToolbar
{
    internal sealed partial class EnterPlayModeToolbarModule
    {
        [MainToolbarElement(ElementPath, defaultDockPosition = MainToolbarDockPosition.Left, defaultDockIndex = 13)]
        private static MainToolbarElement CreateElement()
        {
            string iconPath = AssetDatabase.GUIDToAssetPath("c0b301a9175a74304992d875f40f670d");
            var image = AssetDatabase.LoadAssetAtPath<Texture2D>($"{iconPath}/{ToolbarStyleUtility.DarkModePrefix}rush.png");
            var content = new MainToolbarContent("Enter Play Mode", tooltip: string.Empty, image: image);
       
            return new MainToolbarToggle(content, EditorSettings.enterPlayModeOptionsEnabled,
                value => EditorSettings.enterPlayModeOptionsEnabled = value)
            {
                displayed = CustomToolbarSettings.instance.ShowEnterPlayMode,
                enabled = ToolbarAvailability.InEditMode
            };
        }
    }
}
#endif
