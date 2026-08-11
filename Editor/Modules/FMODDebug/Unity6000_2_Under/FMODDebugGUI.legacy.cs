using UnityEditor;

#if !UNITY_6000_3_OR_NEWER
using UnityEngine;
using UnityEngine.UIElements;
#endif

namespace NKStudio.CustomToolbar.FMOD
{
    [InitializeOnLoad]
    internal sealed partial class FmodDebugToolbarModule : IToolbarModule, ILegacyToolbarModule
    {
        internal const string ModuleId = "NKStudio.CustomToolbar.FMODDebug";
        internal const string ElementPath = "NK Toolbar/FMOD Debugger Toggle";

#if !UNITY_6000_3_OR_NEWER
        private Toggle _toggle;
#endif

        static FmodDebugToolbarModule()
        {
            ToolbarModuleRegistry.Register(new FmodDebugToolbarModule());
        }

        public string Id => ModuleId;

        public void Refresh(ToolbarRefreshReason reason)
        {
#if UNITY_6000_3_OR_NEWER
            UnityEditor.Toolbars.MainToolbar.Refresh(ElementPath);
#else
            if (_toggle != null)
            {
                _toggle.SetValueWithoutNotify(IsFmodDebugActive);
                _toggle.SetEnabled(ToolbarAvailability.InEditMode);
            }
#endif
        }

        public void BuildLegacy(LegacyToolbarBuilder builder)
        {
#if !UNITY_6000_3_OR_NEWER
            _toggle = null;
            if (!CustomToolbarSettings.instance.ShowFmodDebug)
                return;

            var toggle = new Toggle
            {
                name = "FMODDebugToggle",
                text = "FMOD Debug",
                value = IsFmodDebugActive
            };

#if UNITY_6000
            var icon = new Image();
            icon.style.width = 54;
            icon.style.height = 16;
#else
            var icon = new Image();
            icon.style.width = 16;
            icon.style.height = 16;
#endif
            icon.style.alignSelf = Align.Center;
            string iconPath = AssetDatabase.GUIDToAssetPath("f0de615af202e44a190b20daca134fc4");
            icon.image = AssetDatabase.LoadAssetAtPath<Texture2D>($"{iconPath}/{ToolbarStyleUtility.DarkModePrefix}fmod.png");
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
            toggle.RegisterValueChangedCallback(evt => SetFmodDebugOverlay(evt.newValue));
            builder.Add(LegacyToolbarZone.Left, toggle);
            _toggle = toggle;
#endif
        }
    }
}
