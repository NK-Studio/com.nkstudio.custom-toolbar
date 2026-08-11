using UnityEngine.UIElements;

namespace NKStudio.CustomToolbar
{
    internal static class Unity6CaptionGUI
    {
        internal static void RemoveCaption(VisualElement element)
        {
            if (element  == null )
                return;
            
            var caption = element.Q<VisualElement>("ToolbarProductCaption");

            caption?.RemoveFromHierarchy();
        }
    }
}
