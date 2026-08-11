using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace NKStudio.CustomToolbar
{
    internal sealed class CustomToolbarSettingsProvider : SettingsProvider
    {
        private SerializedObject _serializedSettings;

        private CustomToolbarSettingsProvider(string path, SettingsScope scope)
            : base(path, scope, new HashSet<string>
            {
                "toolbar", "scene", "FMOD", "localization", "time scale"
            })
        {
        }

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            return new CustomToolbarSettingsProvider("Project/NKStudio/Custom Toolbar", SettingsScope.Project);
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            _serializedSettings = new SerializedObject(CustomToolbarSettings.instance);
        }

        public override void OnGUI(string searchContext)
        {
            _serializedSettings ??= new SerializedObject(CustomToolbarSettings.instance);
            _serializedSettings.Update();

            EditorGUILayout.LabelField("Visible Modules", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_serializedSettings.FindProperty("showEnterPlayMode"));
            EditorGUILayout.PropertyField(_serializedSettings.FindProperty("showSceneSwitcher"));
            EditorGUILayout.PropertyField(_serializedSettings.FindProperty("showTimeScale"));
            EditorGUILayout.PropertyField(_serializedSettings.FindProperty("showLocalization"));
            EditorGUILayout.PropertyField(_serializedSettings.FindProperty("showFmodDebug"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Scene Switcher", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_serializedSettings.FindProperty("sceneSearchFolders"), true);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Time Scale", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_serializedSettings.FindProperty("minimumTimeScale"));
            EditorGUILayout.PropertyField(_serializedSettings.FindProperty("maximumTimeScale"));
            EditorGUILayout.PropertyField(_serializedSettings.FindProperty("resetTimeScale"));

            if (_serializedSettings.ApplyModifiedProperties())
                CustomToolbarSettings.instance.ValidateAndSave();
        }
    }
}
