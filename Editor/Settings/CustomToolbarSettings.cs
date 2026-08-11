using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NKStudio.CustomToolbar
{
    [FilePath("ProjectSettings/CustomToolbarSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public sealed class CustomToolbarSettings : ScriptableSingleton<CustomToolbarSettings>
    {
        [SerializeField] private bool showEnterPlayMode = true;
        [SerializeField] private bool showSceneSwitcher = true;
        [SerializeField] private bool showTimeScale = true;
        [SerializeField] private bool showLocalization = true;
        [SerializeField] private bool showFmodDebug = true;
        [SerializeField] private string[] sceneSearchFolders = { "Assets/Scenes" };
        [SerializeField] private float minimumTimeScale;
        [SerializeField] private float maximumTimeScale = 5f;
        [SerializeField] private float resetTimeScale = 1f;

        public bool ShowEnterPlayMode => showEnterPlayMode;
        public bool ShowSceneSwitcher => showSceneSwitcher;
        public bool ShowTimeScale => showTimeScale;
        public bool ShowLocalization => showLocalization;
        public bool ShowFmodDebug => showFmodDebug;
        public IReadOnlyList<string> SceneSearchFolders => sceneSearchFolders;
        public float MinimumTimeScale => minimumTimeScale;
        public float MaximumTimeScale => maximumTimeScale;
        public float ResetTimeScale => resetTimeScale;

        internal void ValidateAndSave()
        {
            sceneSearchFolders = NormalizeFolders(sceneSearchFolders);
            NormalizeTimeScale(ref minimumTimeScale, ref maximumTimeScale, ref resetTimeScale);
            Save(true);
            ToolbarModuleRegistry.NotifySettingsChanged();
        }

        internal static void NormalizeTimeScale(ref float minimum, ref float maximum, ref float reset)
        {
            if (maximum < minimum)
                maximum = minimum;

            reset = Mathf.Clamp(reset, minimum, maximum);
        }

        internal static string[] NormalizeFolders(IEnumerable<string> folders)
        {
            if (folders == null)
                return Array.Empty<string>();

            return folders
                .Where(folder => !string.IsNullOrWhiteSpace(folder))
                .Select(folder => folder.Replace('\\', '/').Trim().TrimEnd('/'))
                .Where(folder => folder == "Assets" || folder.StartsWith("Assets/", StringComparison.Ordinal))
                .Where(AssetDatabase.IsValidFolder)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }
    }
}
