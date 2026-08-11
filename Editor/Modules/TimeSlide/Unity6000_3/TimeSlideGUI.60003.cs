#if UNITY_6000_3_OR_NEWER
using UnityEditor.Toolbars;
using UnityEngine;

namespace NKStudio.CustomToolbar
{
    internal sealed partial class TimeScaleToolbarModule
    {
        [MainToolbarElement(ElementPath, defaultDockPosition = MainToolbarDockPosition.Right, defaultDockIndex = 7)]
        private static MainToolbarElement CreateElement()
        {
            CustomToolbarSettings settings = CustomToolbarSettings.instance;
            var content = new MainToolbarContent("Time Scale", "Time Scale");
            var slider = new MainToolbarSlider(content, Time.timeScale, settings.MinimumTimeScale,
                settings.MaximumTimeScale, value => Time.timeScale = value)
            {
                displayed = settings.ShowTimeScale,
                populateContextMenu = menu => menu.AppendAction("Reset", _ => ResetTimeScale())
            };

            return slider;
        }
    }
}
#endif