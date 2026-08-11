using UnityEditor;

namespace NKStudio.CustomToolbar
{
    /// <summary>
    /// 툴바 요소가 활성화되는 에디터 상태를 제공합니다.
    /// </summary>
    public static class ToolbarAvailability
    {
        /// <summary>
        /// Edit 모드에서만 활성화합니다.
        /// </summary>
        public static bool InEditMode => !EditorApplication.isPlayingOrWillChangePlaymode;

        /// <summary>
        /// Play 모드에서만 활성화합니다.
        /// </summary>
        public static bool InPlayMode => EditorApplication.isPlaying;

        /// <summary>
        /// 에디터 상태와 관계없이 항상 활성화합니다.
        /// </summary>
        public const bool Always = true;

        /// <summary>
        /// 에디터 상태와 관계없이 항상 비활성화합니다.
        /// </summary>
        public const bool Never = false;
    }
}
