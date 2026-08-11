using UnityEditor;

#if !UNITY_6000_3_OR_NEWER
using UnityEngine;
using UnityEngine.UIElements;
#endif

namespace NKStudio.CustomToolbar
{
    [InitializeOnLoad]
    internal sealed partial class EnterPlayModeToolbarModule : IToolbarModule, ILegacyToolbarModule
    {
        internal const string ModuleId = "NKStudio.CustomToolbar.EnterPlayMode";
        internal const string ElementPath = "NK Toolbar/Enter Play Mode";

#if !UNITY_6000_3_OR_NEWER
        private Toggle _toggle;
#endif

        static EnterPlayModeToolbarModule()
        {
            ToolbarModuleRegistry.Register(new EnterPlayModeToolbarModule());
        }

        public string Id => ModuleId;

        public void Refresh(ToolbarRefreshReason reason)
        {
#if UNITY_6000_3_OR_NEWER
            UnityEditor.Toolbars.MainToolbar.Refresh(ElementPath);
#else
            if (_toggle != null)
            {
                _toggle.SetValueWithoutNotify(EditorSettings.enterPlayModeOptionsEnabled);
                _toggle.SetEnabled(ToolbarAvailability.InEditMode);
            }
#endif
        }

        public void BuildLegacy(LegacyToolbarBuilder builder)
        {
#if !UNITY_6000_3_OR_NEWER
            _toggle = null;
            if (!CustomToolbarSettings.instance.ShowEnterPlayMode)
                return;

            var toggle = new Toggle
            {
                name = "EnterPlayModeOption",
                text = "Enter Play Mode",
                value = EditorSettings.enterPlayModeOptionsEnabled
            };

            var icon = new Image();
            icon.style.alignSelf = Align.Center;
            icon.style.width = 16;
            icon.style.height = 16;
            string iconPath = AssetDatabase.GUIDToAssetPath("c0b301a9175a74304992d875f40f670d");
            icon.image = AssetDatabase.LoadAssetAtPath<Texture2D>($"{iconPath}/{ToolbarStyleUtility.DarkModePrefix}rush.png");
            toggle[0].Insert(0, icon);

#if !UNITY_6000
            ToolbarStyleUtility.Apply2022ButtonStyle(toggle);
#endif

            VisualElement checkmark = toggle[0].Q<VisualElement>("unity-checkmark");
            if (checkmark != null)
            {
                checkmark.RemoveFromHierarchy();
                toggle[0].Add(checkmark);
            }

            Label label = toggle[0].Q<Label>();
            if (label != null)
            {
                label.style.marginLeft = 4;
                label.style.marginRight = 4;
            }

            toggle.SetEnabled(ToolbarAvailability.InEditMode);
            toggle.RegisterValueChangedCallback(evt => EditorSettings.enterPlayModeOptionsEnabled = evt.newValue);
            builder.Add(LegacyToolbarZone.Left, toggle);
            _toggle = toggle;
#endif
        }
    }
}
