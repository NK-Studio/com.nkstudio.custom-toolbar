#if USE_LOCALIZATION
using UnityEditor;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

#if !UNITY_6000_3_OR_NEWER
using UnityEditor.Toolbars;
using UnityEditor.UIElements;
#endif

namespace NKStudio.CustomToolbar.Localization
{
    [InitializeOnLoad]
    internal sealed partial class LocalizationToolbarModule : IToolbarModule, ILegacyToolbarModule
    {
        internal const string ModuleId = "NKStudio.CustomToolbar.Localization";
        internal const string ElementPath = "NK Toolbar/Localization Switcher";

#if !UNITY_6000_3_OR_NEWER
        private EditorToolbarDropdown _dropdown;
#endif

        static LocalizationToolbarModule()
        {
            if (LocalizationSettings.HasSettings)
            {
                LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;
                LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;
            }

            ToolbarModuleRegistry.Register(new LocalizationToolbarModule());
        }

        public string Id => ModuleId;

        public void Refresh(ToolbarRefreshReason reason)
        {
#if UNITY_6000_3_OR_NEWER
            UnityEditor.Toolbars.MainToolbar.Refresh(ElementPath);
#else
            if (_dropdown != null)
                _dropdown.text = CreateLabel();
#endif
        }

        public void BuildLegacy(LegacyToolbarBuilder builder)
        {
#if !UNITY_6000_3_OR_NEWER
            _dropdown = null;
            if (!CustomToolbarSettings.instance.ShowLocalization)
                return;

            var dropdown = new EditorToolbarDropdown
            {
                name = "LocalizationDropdown",
                text = CreateLabel(),
                icon = LoadLocaleIcon()
            };
            dropdown.clicked += () => ShowLocaleMenu(dropdown.worldBound);
            builder.Add(LegacyToolbarZone.Left, dropdown);
            _dropdown = dropdown;
#endif
        }

        private static void OnSelectedLocaleChanged(Locale _)
        {
            ToolbarModuleRegistry.Refresh(ModuleId);
        }
    }
}
#endif
