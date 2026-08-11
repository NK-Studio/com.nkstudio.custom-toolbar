using UnityEditor;

#if !UNITY_6000_3_OR_NEWER
using UnityEditor.Toolbars;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
#endif

namespace NKStudio.CustomToolbar
{
    [InitializeOnLoad]
    internal sealed partial class SceneSwitcherToolbarModule : IToolbarModule, ILegacyToolbarModule
    {
        internal const string ModuleId = "NKStudio.CustomToolbar.SceneSwitcher";
        internal const string FirstScenePath = "NK Toolbar/Move to First Scene";
        internal const string SceneMenuPath = "NK Toolbar/Move To Scene";

#if !UNITY_6000_3_OR_NEWER
        private ToolbarButton _firstSceneButton;
        private EditorToolbarDropdown _sceneDropdown;
#endif

        static SceneSwitcherToolbarModule()
        {
            ToolbarModuleRegistry.Register(new SceneSwitcherToolbarModule());
        }

        public string Id => ModuleId;

        public void Refresh(ToolbarRefreshReason reason)
        {
#if UNITY_6000_3_OR_NEWER
            UnityEditor.Toolbars.MainToolbar.Refresh(FirstScenePath);
            UnityEditor.Toolbars.MainToolbar.Refresh(SceneMenuPath);
#else
            if (_firstSceneButton != null)
            {
                _firstSceneButton.text = $"Start '{SceneHelper.FirstSceneName}'";
                _firstSceneButton.SetEnabled(ToolbarUtility.IsNotPlaying && SceneHelper.HasFirstScene);
            }

            _sceneDropdown?.SetEnabled(ToolbarUtility.IsNotPlaying);
#endif
        }

        public void BuildLegacy(LegacyToolbarBuilder builder)
        {
#if !UNITY_6000_3_OR_NEWER
            _firstSceneButton = null;
            _sceneDropdown = null;
            if (!CustomToolbarSettings.instance.ShowSceneSwitcher)
                return;

            var firstSceneButton = new ToolbarButton(SceneHelper.MoveFirstScene)
            {
                name = "InitSceneButton",
                text = $"Start '{SceneHelper.FirstSceneName}'",
                tooltip = Application.systemLanguage == SystemLanguage.Korean
                    ? "최초 세팅 씬부터 게임을 진입합니다."
                    : "Start from the first scene."
            };
            firstSceneButton.style.marginRight = 3;
            firstSceneButton.style.unityTextAlign = TextAnchor.MiddleCenter;
            firstSceneButton.SetEnabled(ToolbarUtility.IsNotPlaying && SceneHelper.HasFirstScene);

            var sceneDropdown = new EditorToolbarDropdown
            {
                name = "MoveSceneDropdown",
                text = "씬 선택 이동"
            };
            sceneDropdown.style.marginLeft = 3;
            sceneDropdown.SetEnabled(ToolbarUtility.IsNotPlaying);
            sceneDropdown.clicked += () => SceneHelper.ShowSceneMenu(sceneDropdown);

            builder.Add(LegacyToolbarZone.CenterLeft, firstSceneButton);
            builder.Add(LegacyToolbarZone.CenterRight, sceneDropdown);
            _firstSceneButton = firstSceneButton;
            _sceneDropdown = sceneDropdown;
#endif
        }
    }
}
