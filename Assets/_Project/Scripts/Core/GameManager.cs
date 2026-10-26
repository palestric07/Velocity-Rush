using System.Collections;
using UnityEngine;
using VelocityRush.Utils;

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

        private GameState _state;
        private bool _hasStarted;

        /// <summary>Current top-level game state.</summary>
        public GameState State => _state;

        /// <summary>True while a race is counting down or running.</summary>
        public bool IsRaceActive => _state == GameState.Countdown || _state == GameState.Racing;

        /// <summary>True while the race is suspended.</summary>
        public bool IsPaused => _state == GameState.Paused;

        protected override void OnSingletonCreated()
        {
            _state = m_initialState;
            Log.Info($"[GameManager] initial state: {_state}");

            EventBus.OnSceneLoaded += HandleSceneLoaded;
        }

        protected override void OnSingletonDestroyed()
        {
            EventBus.OnSceneLoaded -= HandleSceneLoaded;
        }

        private IEnumerator Start()
        {
            if (_hasStarted)
                yield break;

            _hasStarted = true;

            if (!m_loadMainMenuOnBoot || _state != GameState.Boot)
                yield break;

            // One frame so every object in the boot scene has finished Awake/OnEnable.
            yield return null;

            if (!SceneLoader.HasInstance)
            {
                Log.Error("[GameManager] SceneLoader missing - cannot leave Boot state.");
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
            if (next == _state)
                return;

            var previous = _state;
            _state = next;

            Log.Info($"[GameManager] state: {previous} -> {next}");
            EventBus.RaiseStateChanged(previous, next);
        }

        private void HandleSceneLoaded(string sceneName)
        {
            if (_state != GameState.Loading)
                return;

            if (sceneName == SceneLoader.MainMenuSceneName)
            {
                SetState(GameState.MainMenu);
                return;
            }

            Log.Warning($"[GameManager] no state mapped for scene '{sceneName}' - remaining in {_state}.");
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
            Log.Info($"[GameManager] editor run-in-background enabled at frame {Time.frameCount}");
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
