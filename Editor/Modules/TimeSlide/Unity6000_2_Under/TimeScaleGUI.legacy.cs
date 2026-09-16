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
        private Label _valueLabel;
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

            if (_valueLabel != null)
                _valueLabel.text = FormatTimeScale(Time.timeScale);
#endif
        }

        public void BuildLegacy(LegacyToolbarBuilder builder)
        {
#if !UNITY_6000_3_OR_NEWER
            _slider = null;
            _valueLabel = null;
            CustomToolbarSettings settings = CustomToolbarSettings.instance;
            if (!settings.ShowTimeScale)
                return;

            var container = new VisualElement { name = "TimeScaleContainer" };
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;

            var slider = new Slider("Time Scale", settings.MinimumTimeScale, settings.MaximumTimeScale)
            {
                name = "TimeScaleSlider",
                value = Mathf.Clamp(Time.timeScale, settings.MinimumTimeScale, settings.MaximumTimeScale),
                tooltip = "Time Scale"
            };
            slider.style.width = 120;

            // BaseField 라벨 기본 min-width(120px)가 트랙을 짓눌러 툴바 밖으로 밀어내므로 텍스트 폭에 맞춘다.
            slider.labelElement.style.minWidth = 0;
            slider.labelElement.style.width = StyleKeyword.Auto;
            slider.labelElement.style.marginRight = 4;

            var valueLabel = new Label(FormatTimeScale(Time.timeScale))
            {
                name = "TimeScaleValue",
                tooltip = "Time Scale (double-click to reset)"
            };
            valueLabel.style.minWidth = 32;
            valueLabel.style.marginLeft = 2;
            valueLabel.style.unityTextAlign = TextAnchor.MiddleRight;
            ToolbarStyleUtility.ApplyStyleSheet(valueLabel);
            valueLabel.AddToClassList("nk-toolbar-clickable");
            valueLabel.RegisterCallback<ClickEvent>(evt =>
            {
                if (evt.clickCount != 2)
                    return;

                ResetTimeScale();
                evt.StopPropagation();
            });

            slider.RegisterValueChangedCallback(evt =>
            {
                Time.timeScale = evt.newValue;
                valueLabel.text = FormatTimeScale(evt.newValue);
            });
            container.AddManipulator(new ContextualMenuManipulator(evt =>
                evt.menu.AppendAction("Reset", _ => ResetTimeScale())));

            container.Add(slider);
            container.Add(valueLabel);
            builder.Add(LegacyToolbarZone.Right, container);
            _slider = slider;
            _valueLabel = valueLabel;
#endif
        }

#if !UNITY_6000_3_OR_NEWER
        private static string FormatTimeScale(float value)
        {
            return value.ToString("0.00");
        }
#endif

        private static void ResetTimeScale()
        {
            Time.timeScale = CustomToolbarSettings.instance.ResetTimeScale;
            ToolbarModuleRegistry.Refresh(ModuleId);
        }
    }
}
