using UnityEngine;

namespace GameServer.UnityClient
{
    public class DebugLogger
    {
        /// <summary>
        /// Logs a message to the Unity Console 
        /// only when DEVELOPMENT_BUILD or UNITY_EDITOR is defined.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [
            System.Diagnostics.Conditional("DEVELOPMENT_BUILD"), 
            System.Diagnostics.Conditional("UNITY_EDITOR"),
        ]
        public static void Log(object message)
        {
            Debug.Log(message);
        }

        /// <summary>
        /// Logs a warning message to the Unity Console 
        /// only when DEVELOPMENT_BUILD or UNITY_EDITOR is defined.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [
            System.Diagnostics.Conditional("DEVELOPMENT_BUILD"),
            System.Diagnostics.Conditional("UNITY_EDITOR"),
        ]
        public static void LogWarning(object message)
        {
            Debug.LogWarning(message);
        }

        /// <summary>
        /// Logs an error message to the Unity Console 
        /// only when DEVELOPMENT_BUILD or UNITY_EDITOR is defined.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [
            System.Diagnostics.Conditional("DEVELOPMENT_BUILD"),
            System.Diagnostics.Conditional("UNITY_EDITOR"),
        ]
        public static void LogError(object message)
        {
            Debug.LogError(message);
        }
    }
}