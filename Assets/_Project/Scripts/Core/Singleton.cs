using UnityEngine;

namespace VelocityRush.Core
{
    /// <summary>
    /// Base for scene-owned singletons: guarantees one live instance per play session and optional persistence across scene loads.
    /// </summary>
    /// <typeparam name="T">The concrete singleton type deriving from this base.</typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T s_instance;

        /// <summary>Live instance, or <c>null</c> before creation and after teardown.</summary>
        public static T Instance => s_instance;

        /// <summary>True while an instance is alive. Prefer this over null-checking <see cref="Instance"/>.</summary>
        public static bool HasInstance => s_instance != null;

        [SerializeField]
        [Tooltip("Survive scene loads (DontDestroyOnLoad).")]
        private bool m_persistAcrossScenes = true;

        /// <summary>Called exactly once, right after this object becomes the active instance.</summary>
        protected virtual void OnSingletonCreated()
        {
        }

        /// <summary>Called when the active instance is being destroyed.</summary>
        protected virtual void OnSingletonDestroyed()
        {
        }

        protected virtual void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                Debug.LogWarning($"[Singleton<{typeof(T).Name}>] duplicate instance on '{name}' destroyed.", this);
                Destroy(gameObject);
                return;
            }

            s_instance = (T)(object)this;

            if (m_persistAcrossScenes)
                DontDestroyOnLoad(gameObject);

            OnSingletonCreated();
        }

        protected virtual void OnDestroy()
        {
            // ReferenceEquals, not Unity's == : by the time OnDestroy runs the object already compares as null.
            if (!ReferenceEquals(s_instance, this))
                return;

            s_instance = null;
            OnSingletonDestroyed();
        }

        /// <summary>
        /// Returns the active instance, creating a root object for it when none exists.
        /// </summary>
        public static T EnsureExists()
        {
            if (s_instance != null)
                return s_instance;

            return new GameObject(typeof(T).Name).AddComponent<T>();
        }

        /// <summary>
        /// Drops the cached instance. Required between play sessions because domain reload is disabled.
        /// </summary>
        protected static void ClearInstance()
        {
            s_instance = null;
        }
    }
}
