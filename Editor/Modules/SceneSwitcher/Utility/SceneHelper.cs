using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
#if !UNITY_6000_3_OR_NEWER
using UnityEngine.UIElements;
#endif

namespace NKStudio.CustomToolbar
{
    internal readonly struct SceneMenuEntry
    {
        public SceneMenuEntry(string label, string assetPath)
        {
            Label = label;
            AssetPath = assetPath;
        }

        public string Label { get; }
        public string AssetPath { get; }
    }

    internal static class SceneHelper
    {
        private static string _sceneToOpen;
        private static bool _playAfterOpen;

        public static IReadOnlyList<SceneMenuEntry> FindAllScenes()
        {
            string[] folders = CustomToolbarSettings.NormalizeFolders(
                CustomToolbarSettings.instance.SceneSearchFolders);

            if (folders.Length == 0)
                return Array.Empty<SceneMenuEntry>();

            var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string guid in AssetDatabase.FindAssets("t:Scene", folders))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!string.IsNullOrEmpty(path))
                    paths.Add(path);
            }

            var entries = paths
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .Select(path => new SceneMenuEntry(CreateLabel(path, folders), path))
                .ToList();

            var duplicateLabels = entries
                .GroupBy(entry => entry.Label, StringComparer.OrdinalIgnoreCase)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (duplicateLabels.Count == 0)
                return entries;

            return entries
                .Select(entry => duplicateLabels.Contains(entry.Label)
                    ? new SceneMenuEntry(Path.ChangeExtension(entry.AssetPath, null), entry.AssetPath)
                    : entry)
                .ToArray();
        }

        public static string FirstScenePath
        {
            get
            {
                foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
                {
                    if (scene.enabled && !string.IsNullOrEmpty(scene.path))
                        return scene.path;
                }

                return null;
            }
        }

        public static string FirstSceneName => string.IsNullOrEmpty(FirstScenePath)
            ? "None"
            : Path.GetFileNameWithoutExtension(FirstScenePath);

        public static bool HasFirstScene => !string.IsNullOrEmpty(FirstScenePath);

        public static void StartScene(string scenePath, bool playAfterOpen)
        {
            if (string.IsNullOrEmpty(scenePath) || !File.Exists(scenePath))
            {
                Debug.LogError($"Scene does not exist: '{scenePath ?? "<null>"}'.");
                return;
            }

            if (EditorApplication.isPlaying)
                EditorApplication.isPlaying = false;

            _sceneToOpen = scenePath;
            _playAfterOpen = playAfterOpen;
            EditorApplication.update -= OpenPendingScene;
            EditorApplication.update += OpenPendingScene;
        }

        public static void MoveFirstScene()
        {
            if (HasFirstScene)
                StartScene(FirstScenePath, true);
        }

        private static void OpenPendingScene()
        {
            if (string.IsNullOrEmpty(_sceneToOpen) || EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            EditorApplication.update -= OpenPendingScene;
            string scenePath = _sceneToOpen;
            bool playAfterOpen = _playAfterOpen;
            _sceneToOpen = null;
            _playAfterOpen = false;

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            EditorSceneManager.OpenScene(scenePath);
            EditorApplication.isPlaying = playAfterOpen;
        }

        private static string CreateLabel(string scenePath, IEnumerable<string> folders)
        {
            foreach (string folder in folders.OrderByDescending(folder => folder.Length))
            {
                string prefix = folder + "/";
                if (scenePath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    return Path.ChangeExtension(scenePath.Substring(prefix.Length), null);
            }

            return Path.ChangeExtension(scenePath, null);
        }

        private static GenericMenu CreateSceneMenu()
        {
            var menu = new GenericMenu();
            IReadOnlyList<SceneMenuEntry> scenes = FindAllScenes();

            if (scenes.Count == 0)
            {
                menu.AddDisabledItem(new GUIContent("No scenes found"));
                return menu;
            }

            foreach (SceneMenuEntry entry in scenes)
            {
                string path = entry.AssetPath;
                menu.AddItem(new GUIContent(entry.Label), false, () => StartScene(path, false));
            }

            return menu;
        }

#if UNITY_6000_3_OR_NEWER
        public static void ShowSceneMenu(Rect rect)
        {
            CreateSceneMenu().DropDown(rect);
        }
#else
        public static void ShowSceneMenu(VisualElement dropdown)
        {
            CreateSceneMenu().DropDown(dropdown.worldBound);
        }
#endif
    }
}
