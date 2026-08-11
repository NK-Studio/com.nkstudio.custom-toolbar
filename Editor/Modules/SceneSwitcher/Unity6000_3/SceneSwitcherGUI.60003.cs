#if UNITY_6000_3_OR_NEWER
using UnityEditor.Toolbars;
using UnityEngine;

namespace NKStudio.CustomToolbar
{
    internal sealed partial class SceneSwitcherToolbarModule
    {
        [MainToolbarElement(FirstScenePath, defaultDockPosition = MainToolbarDockPosition.Middle, defaultDockIndex = -1)]
        private static MainToolbarElement CreateFirstSceneButton()
        {
            var content = new MainToolbarContent(
                $"Start '{SceneHelper.FirstSceneName}'",
                Application.systemLanguage == SystemLanguage.Korean
                    ? "최초 세팅 씬부터 게임을 진입합니다."
                    : "Start from the first scene.");

            return new MainToolbarButton(content, SceneHelper.MoveFirstScene)
            {
                displayed = CustomToolbarSettings.instance.ShowSceneSwitcher,
                enabled = ToolbarAvailability.InEditMode && SceneHelper.HasFirstScene
            };
        }

        [MainToolbarElement(SceneMenuPath, defaultDockPosition = MainToolbarDockPosition.Middle, defaultDockIndex = 1)]
        private static MainToolbarElement CreateSceneMenu()
        {
            var content = new MainToolbarContent("씬 선택 이동");
            return new MainToolbarDropdown(content, SceneHelper.ShowSceneMenu)
            {
                displayed = CustomToolbarSettings.instance.ShowSceneSwitcher,
                enabled = ToolbarAvailability.InEditMode
            };
        }
    }
}
#endif
