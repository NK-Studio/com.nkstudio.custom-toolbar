#if !UNITY_6000_3_OR_NEWER
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NKStudio.CustomToolbar
{
    [InitializeOnLoad]
    internal static class LegacyToolbarHost
    {
        private static readonly Dictionary<LegacyToolbarZone, VisualElement> Zones =
            new Dictionary<LegacyToolbarZone, VisualElement>();

        private static VisualElement _toolbarRoot;
        private static bool _registryInitialized;
        private static double _nextToolbarProbeTime;

        static LegacyToolbarHost()
        {
            EditorApplication.update -= TryInitialize;
            EditorApplication.update += TryInitialize;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorBuildSettings.sceneListChanged -= OnSceneListChanged;
            EditorBuildSettings.sceneListChanged += OnSceneListChanged;
            ToolbarModuleRegistry.LegacyRebuildRequested -= Rebuild;
            ToolbarModuleRegistry.LegacyRebuildRequested += Rebuild;
        }

        private static void TryInitialize()
        {
            if (EditorApplication.timeSinceStartup < _nextToolbarProbeTime)
                return;

            _nextToolbarProbeTime = EditorApplication.timeSinceStartup + 1d;

            if (!TryGetToolbarRoot(out VisualElement root))
                return;

            if (_toolbarRoot != root || Zones.Count == 0)
            {
                CreateZones(root);
                Rebuild();
            }

            if (_registryInitialized)
                return;

            _registryInitialized = true;
            ToolbarModuleRegistry.Initialize();
        }

        private static void CreateZones(VisualElement root)
        {
            _toolbarRoot = root;
            Zones.Clear();

            VisualElement left = root.Q("ToolbarZoneLeftAlign");
            VisualElement right = root.Q("ToolbarZoneRightAlign");
            VisualElement playMode = root.Q("ToolbarZonePlayMode")?.Q<VisualElement>("PlayMode");

            if (left == null || right == null || playMode == null)
                throw new InvalidOperationException("Unity toolbar zones could not be located.");

            Zones.Add(LegacyToolbarZone.Left, CreateContainer(left, "NKToolbarLeft", FlexDirection.Row));
            Zones.Add(LegacyToolbarZone.Right, CreateContainer(right, "NKToolbarRight", FlexDirection.RowReverse));

            var centerLeft = new VisualElement { name = "NKToolbarCenterLeft" };
            centerLeft.style.flexDirection = FlexDirection.RowReverse;
            playMode.Insert(0, centerLeft);
            Zones.Add(LegacyToolbarZone.CenterLeft, centerLeft);

            var centerRight = new VisualElement { name = "NKToolbarCenterRight" };
            centerRight.style.flexDirection = FlexDirection.Row;
            playMode.Add(centerRight);
            Zones.Add(LegacyToolbarZone.CenterRight, centerRight);

#if UNITY_6000
            Unity6CaptionGUI.RemoveCaption(root);
#endif
        }

        private static VisualElement CreateContainer(VisualElement parent, string name, FlexDirection direction)
        {
            VisualElement existing = parent.Q<VisualElement>(name);
            existing?.RemoveFromHierarchy();

            var container = new VisualElement { name = name };
            container.style.flexDirection = direction;
            parent.Add(container);
            return container;
        }

        private static void Rebuild()
        {
            if (Zones.Count == 0)
                return;

            foreach (VisualElement zone in Zones.Values)
                zone.Clear();

            ToolbarModuleRegistry.BuildLegacy(new LegacyToolbarBuilder(Zones));
            ToolbarModuleRegistry.RefreshAll(ToolbarRefreshReason.Initialized);
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange _)
        {
            ToolbarModuleRegistry.RefreshAll(ToolbarRefreshReason.PlayModeChanged);
        }

        private static void OnSceneListChanged()
        {
            ToolbarModuleRegistry.RefreshAll(ToolbarRefreshReason.SceneListChanged);
        }

        private static bool TryGetToolbarRoot(out VisualElement root)
        {
            Type toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
            FieldInfo getField = toolbarType?.GetField("get", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            object toolbar = getField?.GetValue(null);
            FieldInfo rootField = toolbarType?.GetField("m_Root", BindingFlags.Instance | BindingFlags.NonPublic);
            root = rootField?.GetValue(toolbar) as VisualElement;
            return root != null;
        }
    }
}
#endif
