using System;
using UnityEngine;

namespace NKStudio.CustomToolbar
{
    public static partial class ToolbarModuleRegistry
    {
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
