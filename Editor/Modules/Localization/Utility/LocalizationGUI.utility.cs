#if USE_LOCALIZATION
using System.Collections.ObjectModel;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace NKStudio.CustomToolbar.Localization
{
    internal sealed partial class LocalizationToolbarModule
    {
        private static string CreateLabel()
        {
            return LocalizationSettings.HasSettings && LocalizationSettings.SelectedLocale != null
                ? $"Localization : {LocalizationSettings.SelectedLocale}"
                : "Localization : None";
        }

        private static Texture2D LoadLocaleIcon()
        {
            return AssetDatabase.LoadAssetAtPath<Texture2D>(
                $"Packages/com.unity.localization/Editor/Icons/Locale/{ToolbarUtility.DarkModePrefix}Locale.png");
        }

        private static void ShowLocaleMenu(Rect rect)
        {
            var menu = new GenericMenu();
            ReadOnlyCollection<Locale> locales = LocalizationEditorSettings.GetLocales();

            if (locales.Count == 0)
            {
                menu.AddDisabledItem(new GUIContent("No locales found"));
            }
            else
            {
                foreach (Locale locale in locales)
                {
                    Locale selectedLocale = locale;
                    menu.AddItem(new GUIContent(locale.ToString()),
                        locale == LocalizationSettings.SelectedLocale,
                        () => LocalizationSettings.SelectedLocale = selectedLocale);
                }
            }

            menu.DropDown(rect);
        }
    }
}
#endif
