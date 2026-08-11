using UnityEditor;
using UnityEngine;

#if !UNITY_6000_3_OR_NEWER
using UnityEngine.UIElements;
#endif

namespace NKStudio.CustomToolbar
{
    [InitializeOnLoad]
    internal sealed partial class TimeScaleToolbarModule : IToolbarModule, ILegacyToolbarModule
    {
        internal const string ModuleId = "NKStudio.CustomToolbar.TimeScale";
        internal const string ElementPath = "NK Toolbar/TimeSlider";

#if !UNITY_6000_3_OR_NEWER
        private Slider _slider;
#endif

        static TimeScaleToolbarModule()
        {
            ToolbarModuleRegistry.Register(new TimeScaleToolbarModule());
        }

        public string Id => ModuleId;

        public void Refresh(ToolbarRefreshReason reason)
        {
#if UNITY_6000_3_OR_NEWER
            UnityEditor.Toolbars.MainToolbar.Refresh(ElementPath);
#else
            if (_slider != null)
                _slider.SetValueWithoutNotify(Time.timeScale);
#endif
        }

        public void BuildLegacy(LegacyToolbarBuilder builder)
        {
#if !UNITY_6000_3_OR_NEWER
            _slider = null;
            CustomToolbarSettings settings = CustomToolbarSettings.instance;
            if (!settings.ShowTimeScale)
                return;

            var slider = new Slider("Time Scale", settings.MinimumTimeScale, settings.MaximumTimeScale)
            {
                name = "TimeScaleSlider",
                value = Mathf.Clamp(Time.timeScale, settings.MinimumTimeScale, settings.MaximumTimeScale),
                showInputField = true
            };
            slider.style.width = 190;
            slider.RegisterValueChangedCallback(evt => Time.timeScale = evt.newValue);
            slider.AddManipulator(new ContextualMenuManipulator(evt =>
                evt.menu.AppendAction("Reset", _ => ResetTimeScale())));
            builder.Add(LegacyToolbarZone.Right, slider);
            _slider = slider;
#endif
        }

        private static void ResetTimeScale()
        {
            Time.timeScale = CustomToolbarSettings.instance.ResetTimeScale;
            ToolbarModuleRegistry.Refresh(ModuleId);
        }
    }
}
