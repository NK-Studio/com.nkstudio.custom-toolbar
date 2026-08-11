#if !UNITY_6000_3_OR_NEWER
using System;
using UnityEngine;

namespace NKStudio.CustomToolbar
{
    public static partial class ToolbarModuleRegistry
    {
        internal static event Action LegacyRebuildRequested;

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
    }
}
#endif
