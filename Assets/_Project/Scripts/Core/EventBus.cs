using System;
using UnityEngine;

namespace VelocityRush.Core
{
    /// <summary>
    /// Global decoupled event hub: systems broadcast here instead of holding references to each other.
    /// </summary>
    /// <remarks>
    /// Every event is exposed through a <c>Raise*</c> helper that null-checks before invoking, so
    /// publishers never need to know whether anyone subscribed. Static subscriptions do not survive
    /// play sessions when domain reload is disabled, hence the reset hook.
    /// </remarks>
    public static class EventBus
    {
        /// <summary>Fires after <c>GameManager</c> has moved to a new state.</summary>
        public static event Action<GameState, GameState> OnStateChanged;

        /// <summary>Fires once the race becomes drivable.</summary>
        public static event Action OnRaceStarted;

        /// <summary>Fires when a lap is completed; argument is the lap number just finished.</summary>
        public static event Action<int> OnLapCompleted;

        /// <summary>Fires when the player picks a car; argument is the stable car identifier.</summary>
        public static event Action<string> OnCarSelected;

        /// <summary>Fires each countdown tick; argument is the seconds still to wait.</summary>
        public static event Action<int> OnCountdownTick;

        /// <summary>Fires when a single-mode scene load has finished; argument is the scene name.</summary>
        public static event Action<string> OnSceneLoaded;

        public static void RaiseStateChanged(GameState previous, GameState next)
        {
            OnStateChanged?.Invoke(previous, next);
        }

        public static void RaiseRaceStarted()
        {
            OnRaceStarted?.Invoke();
        }

        public static void RaiseLapCompleted(int completedLap)
        {
            OnLapCompleted?.Invoke(completedLap);
        }

        public static void RaiseCarSelected(string carId)
        {
            OnCarSelected?.Invoke(carId);
        }

        public static void RaiseCountdownTick(int secondsRemaining)
        {
            OnCountdownTick?.Invoke(secondsRemaining);
        }

        public static void RaiseSceneLoaded(string sceneName)
        {
            OnSceneLoaded?.Invoke(sceneName);
        }

        /// <summary>Clears every subscription before each play session starts.</summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetForPlaySession()
        {
            OnStateChanged = null;
            OnRaceStarted = null;
            OnLapCompleted = null;
            OnCarSelected = null;
            OnCountdownTick = null;
            OnSceneLoaded = null;
        }
    }
}
