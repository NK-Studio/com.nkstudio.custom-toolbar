using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace NKStudio.CustomToolbar
{
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
}
