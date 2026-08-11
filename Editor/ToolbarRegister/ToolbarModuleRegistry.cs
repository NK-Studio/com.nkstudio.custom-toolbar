using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NKStudio.CustomToolbar
{
    public enum ToolbarRefreshReason
    {
        Initialized,
        PlayModeChanged,
        SceneListChanged,
        SettingsChanged,
        ExternalStateChanged
    }

    public enum LegacyToolbarZone
    {
        Left,
        CenterLeft,
        CenterRight,
        Right
    }

    public interface IToolbarModule
    {
        string Id { get; }
        void Refresh(ToolbarRefreshReason reason);
    }

    public interface ILegacyToolbarModule : IToolbarModule
    {
        void BuildLegacy(LegacyToolbarBuilder builder);
    }

    public sealed class LegacyToolbarBuilder
    {
        private readonly IReadOnlyDictionary<LegacyToolbarZone, VisualElement> _zones;

        internal LegacyToolbarBuilder(IReadOnlyDictionary<LegacyToolbarZone, VisualElement> zones)
        {
            _zones = zones;
        }

        public void Add(LegacyToolbarZone zone, VisualElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (!_zones.TryGetValue(zone, out VisualElement parent) || parent == null)
                throw new InvalidOperationException($"Toolbar zone '{zone}' is not available.");

            parent.Add(element);
        }
    }

    public static class ToolbarModuleRegistry
    {
        private static readonly SortedDictionary<string, IToolbarModule> Modules =
            new SortedDictionary<string, IToolbarModule>(StringComparer.Ordinal);

        private static bool _isInitialized;

#if !UNITY_6000_3_OR_NEWER
        internal static event Action LegacyRebuildRequested;
#endif

        public static void Register(IToolbarModule module)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));
            if (string.IsNullOrWhiteSpace(module.Id))
                throw new ArgumentException("Toolbar module ID must not be empty.", nameof(module));
            if (Modules.ContainsKey(module.Id))
                throw new InvalidOperationException($"A toolbar module with ID '{module.Id}' is already registered.");

            Modules.Add(module.Id, module);

            if (!_isInitialized)
                return;

#if UNITY_6000_3_OR_NEWER
            module.Refresh(ToolbarRefreshReason.Initialized);
#else
            LegacyRebuildRequested?.Invoke();
#endif
        }

        public static void Refresh(string moduleId, ToolbarRefreshReason reason = ToolbarRefreshReason.ExternalStateChanged)
        {
            if (string.IsNullOrWhiteSpace(moduleId))
                throw new ArgumentException("Toolbar module ID must not be empty.", nameof(moduleId));

            if (Modules.TryGetValue(moduleId, out IToolbarModule module))
                module.Refresh(reason);
        }

        internal static void Initialize()
        {
            _isInitialized = true;
            RefreshAll(ToolbarRefreshReason.Initialized);
        }

        internal static void RefreshAll(ToolbarRefreshReason reason)
        {
            foreach (IToolbarModule module in Modules.Values)
            {
                try
                {
                    module.Refresh(reason);
                }
                catch (Exception exception)
                {
                    Debug.LogException(new InvalidOperationException(
                        $"Failed to refresh toolbar module '{module.Id}'.", exception));
                }
            }
        }

        internal static void BuildLegacy(LegacyToolbarBuilder builder)
        {
            foreach (IToolbarModule module in Modules.Values)
            {
                if (!(module is ILegacyToolbarModule legacyModule))
                    continue;

                try
                {
                    legacyModule.BuildLegacy(builder);
                }
                catch (Exception exception)
                {
                    Debug.LogException(new InvalidOperationException(
                        $"Failed to build legacy toolbar module '{module.Id}'.", exception));
                }
            }
        }

        internal static void NotifySettingsChanged()
        {
#if UNITY_6000_3_OR_NEWER
            RefreshAll(ToolbarRefreshReason.SettingsChanged);
#else
            LegacyRebuildRequested?.Invoke();
#endif
        }
    }
}
