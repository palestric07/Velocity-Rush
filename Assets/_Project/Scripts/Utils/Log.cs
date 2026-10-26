using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace VelocityRush.Utils
{
    /// <summary>
    /// The single gate every gameplay log line passes through — game code never calls <c>UnityEngine.Debug</c> directly.
    /// </summary>
    /// <remarks>
    /// <see cref="Info"/> and <see cref="Warning"/> carry <see cref="ConditionalAttribute"/>, so their call
    /// sites — and the string interpolation feeding them — are deleted outright from players built without
    /// <c>UNITY_EDITOR</c> or <c>DEVELOPMENT_BUILD</c>. That is stronger than wrapping the body in
    /// <c>#if</c>: nothing is evaluated, so release builds pay zero cost for logging.
    /// <see cref="Error"/> is deliberately unconditional — a shipped build must still report failures.
    /// </remarks>
    public static class Log
    {
        /// <summary>Gates <see cref="Info"/> only. Warnings and errors always report.</summary>
        public static bool Verbose = true;

        /// <summary>Logs a diagnostic line. Compiled out of release players.</summary>
        /// <param name="message">Text to log; keep the leading <c>[Tag]</c> for greppability.</param>
        /// <param name="context">Optional object that the Console highlights when clicked.</param>
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void Info(string message, Object context = null)
        {
            if (Verbose)
                Debug.Log(message, context);
        }

        /// <summary>Logs a recoverable problem. Compiled out of release players.</summary>
        /// <param name="message">Text to log; keep the leading <c>[Tag]</c> for greppability.</param>
        /// <param name="context">Optional object that the Console highlights when clicked.</param>
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void Warning(string message, Object context = null)
        {
            Debug.LogWarning(message, context);
        }

        /// <summary>Logs a failure. Kept in release players, unlike <see cref="Info"/> and <see cref="Warning"/>.</summary>
        /// <param name="message">Text to log; keep the leading <c>[Tag]</c> for greppability.</param>
        /// <param name="context">Optional object that the Console highlights when clicked.</param>
        public static void Error(string message, Object context = null)
        {
            Debug.LogError(message, context);
        }
    }
}
