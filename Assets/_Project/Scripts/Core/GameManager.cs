using System.Collections;
using UnityEngine;

namespace VelocityRush.Core
{
    /// <summary>
    /// Owns the top-level game state machine and the boot scene flow; gameplay systems talk through <see cref="EventBus"/>, not this type.
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-1000)]
    public sealed class GameManager : Singleton<GameManager>
    {
        [SerializeField]
        [Tooltip("State the game starts in.")]
        private GameState m_initialState = GameState.Boot;

        [SerializeField]
        [Tooltip("Kick off the Boot -> Loading -> MainMenu flow on startup.")]
        private bool m_loadMainMenuOnBoot = true;

        private GameState m_state;
        private bool m_hasStarted;

        /// <summary>Current top-level game state.</summary>
        public GameState State => m_state;

        /// <summary>True while a race is counting down or running.</summary>
        public bool IsRaceActive => m_state == GameState.Countdown || m_state == GameState.Racing;

        /// <summary>True while the race is suspended.</summary>
        public bool IsPaused => m_state == GameState.Paused;

        protected override void OnSingletonCreated()
        {
            m_state = m_initialState;
            Debug.Log($"[GameManager] initial state: {m_state}");

            EventBus.OnSceneLoaded += HandleSceneLoaded;
        }

        protected override void OnSingletonDestroyed()
        {
            EventBus.OnSceneLoaded -= HandleSceneLoaded;
        }

        private IEnumerator Start()
        {
            if (m_hasStarted)
                yield break;

            m_hasStarted = true;

            if (!m_loadMainMenuOnBoot || m_state != GameState.Boot)
                yield break;

            // One frame so every object in the boot scene has finished Awake/OnEnable.
            yield return null;

            if (!SceneLoader.HasInstance)
            {
                Debug.LogError("[GameManager] SceneLoader missing - cannot leave Boot state.");
                yield break;
            }

            SetState(GameState.Loading);
            SceneLoader.Instance.LoadMainMenu();
        }

        /// <summary>
        /// Moves to <paramref name="next"/>, logging the transition and notifying subscribers.
        /// </summary>
        public void SetState(GameState next)
        {
            if (next == m_state)
                return;

            var previous = m_state;
            m_state = next;

            Debug.Log($"[GameManager] state: {previous} -> {next}");
            EventBus.RaiseStateChanged(previous, next);
        }

        private void HandleSceneLoaded(string sceneName)
        {
            if (m_state != GameState.Loading)
                return;

            if (sceneName == SceneLoader.MainMenuSceneName)
            {
                SetState(GameState.MainMenu);
                return;
            }

            Debug.LogWarning($"[GameManager] no state mapped for scene '{sceneName}' - remaining in {m_state}.");
        }

        /// <summary>Clears the cached instance between play sessions (domain reload is disabled).</summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            ClearInstance();
        }

        /// <summary>
        /// Keeps the player loop ticking while the Editor window is unfocused.
        /// Unity freezes play mode after ~2 frames when the Editor is in the background and
        /// runInBackground is false, which stalls every coroutine and async scene load.
        /// Runs at SubsystemRegistration, i.e. before the first frame, so it always wins that race.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ConfigureEditorBackgroundRun()
        {
#if UNITY_EDITOR
            Application.runInBackground = true;
            Debug.Log($"[GameManager] editor run-in-background enabled at frame {Time.frameCount}");
#endif
        }

        /// <summary>
        /// Ensures the core systems exist no matter which scene is opened, so an empty scene is playable.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (HasInstance)
                return;

            var host = new GameObject("[Core]");
            host.AddComponent<SceneLoader>();
            host.AddComponent<GameManager>();
        }
    }
}
