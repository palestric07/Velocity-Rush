using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VelocityRush.Core
{
    /// <summary>
    /// Loads scenes asynchronously with observable progress and reports completion through <see cref="EventBus"/>.
    /// </summary>
    /// <remarks>
    /// Unity only advances an async operation to 0.9 while <c>allowSceneActivation</c> is off; the
    /// remaining budget is the actual activation step. <see cref="m_activationBudget"/> keeps that
    /// slice reserved so reported progress never sits at 100% before the scene is usable.
    /// </remarks>
    [DisallowMultipleComponent]
    public sealed class SceneLoader : Singleton<SceneLoader>
    {
        /// <summary>Build-settings name of the main menu scene.</summary>
        public const string MainMenuSceneName = "MainMenu";

        /// <summary>Headroom so a sub-frame load still reads as a real transition.</summary>
        private const float ProgressEpsilon = 0.001f;

        [SerializeField, Range(0f, 2f)]
        [Tooltip("Minimum seconds a load stays active, so short loads never flash.")]
        private float m_minimumLoadSeconds = 0.2f;

        [SerializeField, Range(0.05f, 0.9f)]
        [Tooltip("Progress share held back for the final activation step.")]
        private float m_activationBudget = 0.1f;

        /// <summary>True while a scene load is in flight.</summary>
        public bool IsLoading { get; private set; }

        /// <summary>0..1 progress of the in-flight load.</summary>
        public float Progress { get; private set; }

        /// <summary>Scene currently loading, or empty when idle.</summary>
        public string PendingScene { get; private set; } = string.Empty;

        protected override void OnSingletonCreated()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        protected override void OnSingletonDestroyed()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        /// <summary>Loads the main menu scene.</summary>
        public void LoadMainMenu()
        {
            LoadScene(MainMenuSceneName);
        }

        /// <summary>
        /// Starts an asynchronous single-mode load. Ignored while another load is already in flight.
        /// </summary>
        public void LoadScene(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogError("[SceneLoader] refused load: scene name is empty.");
                return;
            }

            if (IsLoading)
            {
                Debug.LogWarning($"[SceneLoader] ignored '{sceneName}' because '{PendingScene}' is still loading.");
                return;
            }

            StartCoroutine(LoadRoutine(sceneName));
        }

        private IEnumerator LoadRoutine(string sceneName)
        {
            IsLoading = true;
            PendingScene = sceneName;
            Progress = 0f;

            var startedAt = Time.realtimeSinceStartup;

            var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            if (operation == null)
            {
                Debug.LogError($"[SceneLoader] LoadSceneAsync('{sceneName}') failed - is the scene listed in Build Settings?");
                FinishLoad();
                yield break;
            }

            var streamLimit = Mathf.Clamp(1f - m_activationBudget, 0.05f, 0.9f);

            operation.allowSceneActivation = false;

            while (operation.progress < streamLimit - ProgressEpsilon)
            {
                Progress = Mathf.Min(operation.progress, streamLimit);
                yield return null;
            }

            while (Time.realtimeSinceStartup - startedAt < m_minimumLoadSeconds)
                yield return null;

            Progress = streamLimit;
            operation.allowSceneActivation = true;

            while (!operation.isDone)
                yield return null;

            FinishLoad();
            Debug.Log($"[SceneLoader] loaded '{sceneName}'");
        }

        private void FinishLoad()
        {
            Progress = 1f;
            IsLoading = false;
            PendingScene = string.Empty;
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (mode == LoadSceneMode.Single)
                EventBus.RaiseSceneLoaded(scene.name);
        }

        /// <summary>Clears the cached instance between play sessions (domain reload is disabled).</summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            ClearInstance();
        }
    }
}
