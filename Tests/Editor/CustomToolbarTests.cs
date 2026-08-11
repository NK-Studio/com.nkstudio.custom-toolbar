using System;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace NKStudio.CustomToolbar.Tests
{
    public sealed class CustomToolbarTests
    {
        [Test]
        public void Settings_DefaultValues_AreStable()
        {
            var settings = ScriptableObject.CreateInstance<CustomToolbarSettings>();
            try
            {
                Assert.That(settings.ShowEnterPlayMode, Is.True);
                Assert.That(settings.ShowSceneSwitcher, Is.True);
                Assert.That(settings.ShowTimeScale, Is.True);
                Assert.That(settings.ShowLocalization, Is.True);
                Assert.That(settings.ShowFmodDebug, Is.True);
                Assert.That(settings.SceneSearchFolders, Is.EqualTo(new[] { "Assets/Scenes" }));
                Assert.That(settings.MinimumTimeScale, Is.EqualTo(0f));
                Assert.That(settings.MaximumTimeScale, Is.EqualTo(5f));
                Assert.That(settings.ResetTimeScale, Is.EqualTo(1f));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(settings);
            }
        }

        [Test]
        public void NormalizeFolders_RemovesInvalidAndDuplicatePaths()
        {
            string[] result = CustomToolbarSettings.NormalizeFolders(new[]
            {
                "Assets/",
                "Assets",
                "Assets/DoesNotExist",
                "Packages/com.example"
            });

            Assert.That(result, Is.EqualTo(new[] { "Assets" }));
        }

        [TestCase(0f, 5f, 1f, 0f, 5f, 1f)]
        [TestCase(2f, 1f, 10f, 2f, 2f, 2f)]
        [TestCase(0f, 5f, -1f, 0f, 5f, 0f)]
        public void NormalizeTimeScale_ProducesValidRange(float minimum, float maximum, float reset,
            float expectedMinimum, float expectedMaximum, float expectedReset)
        {
            CustomToolbarSettings.NormalizeTimeScale(ref minimum, ref maximum, ref reset);
            Assert.That(minimum, Is.EqualTo(expectedMinimum));
            Assert.That(maximum, Is.EqualTo(expectedMaximum));
            Assert.That(reset, Is.EqualTo(expectedReset));
        }

        [Test]
        public void Registry_RejectsDuplicateModuleIds()
        {
            string id = $"test.duplicate.{Guid.NewGuid():N}";
            ToolbarModuleRegistry.Register(new FakeModule(id));
            Assert.Throws<InvalidOperationException>(() =>
                ToolbarModuleRegistry.Register(new FakeModule(id)));
        }

        [Test]
        public void LateRegistration_IsImmediatelyRefreshed()
        {
            ToolbarModuleRegistry.Initialize();
            var module = new FakeModule($"test.late.{Guid.NewGuid():N}");
            ToolbarModuleRegistry.Register(module);
            Assert.That(module.RefreshCount, Is.EqualTo(1));
            Assert.That(module.LastReason, Is.EqualTo(ToolbarRefreshReason.Initialized));
        }

        [Test]
        public void ToolbarAvailability_MatchesEditorState()
        {
            Assert.That(ToolbarAvailability.InEditMode,
                Is.EqualTo(!EditorApplication.isPlayingOrWillChangePlaymode));
            Assert.That(ToolbarAvailability.InPlayMode,
                Is.EqualTo(EditorApplication.isPlaying));
            Assert.That(ToolbarAvailability.Always, Is.True);
            Assert.That(ToolbarAvailability.Never, Is.False);
        }

        private sealed class FakeModule : IToolbarModule
        {
            public FakeModule(string id)
            {
                Id = id;
            }

            public string Id { get; }
            public int RefreshCount { get; private set; }
            public ToolbarRefreshReason LastReason { get; private set; }

            public void Refresh(ToolbarRefreshReason reason)
            {
                RefreshCount++;
                LastReason = reason;
            }
        }
    }
}
