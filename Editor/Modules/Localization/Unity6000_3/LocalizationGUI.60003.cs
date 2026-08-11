#if USE_LOCALIZATION && UNITY_6000_3_OR_NEWER
using UnityEditor.Toolbars;

namespace NKStudio.CustomToolbar.Localization
{
    internal sealed partial class LocalizationToolbarModule
    {
        [MainToolbarElement(ElementPath, defaultDockPosition = MainToolbarDockPosition.Left, defaultDockIndex = 14)]
        private static MainToolbarElement CreateElement()
        {
            var content = new MainToolbarContent(CreateLabel(), tooltip: string.Empty, image: LoadLocaleIcon());
            return new MainToolbarDropdown(content, ShowLocaleMenu)
            {
                displayed = CustomToolbarSettings.instance.ShowLocalization
            };
        }
    }
}
#endif
