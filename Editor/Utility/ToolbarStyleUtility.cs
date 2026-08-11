using UnityEditor;
using UnityEngine.UIElements;

namespace NKStudio.CustomToolbar
{
    public static class ToolbarStyleUtility
    {
        private const string ToolbarDarkStyleSheetPathGuid = "70d5ed84aacee41e8b5ad32f631a10fb";
        private const string ToolbarLightStyleSheetPathGuid = "f7504feab7f654b218f493aad2f15192";

        public static string DarkModePrefix => EditorGUIUtility.isProSkin ? "d_" : "";

        public static void Apply2022ButtonStyle(VisualElement element)
        {
            string path = AssetDatabase.GUIDToAssetPath(EditorGUIUtility.isProSkin
                ? ToolbarDarkStyleSheetPathGuid
                : ToolbarLightStyleSheetPathGuid);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
            element.styleSheets.Add(styleSheet);
            element.AddToClassList("unity-toolbar-frame");
        }
    }
}
