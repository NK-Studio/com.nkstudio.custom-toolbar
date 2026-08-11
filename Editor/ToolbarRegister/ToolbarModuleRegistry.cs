using System;
using System.Collections.Generic;

namespace NKStudio.CustomToolbar
{
    public static partial class ToolbarModuleRegistry
    {
        private static readonly SortedDictionary<string, IToolbarModule> Modules =
            new SortedDictionary<string, IToolbarModule>(StringComparer.Ordinal);

        private static bool _isInitialized;

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

    }
}
