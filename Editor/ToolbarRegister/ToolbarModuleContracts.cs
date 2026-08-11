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
}
