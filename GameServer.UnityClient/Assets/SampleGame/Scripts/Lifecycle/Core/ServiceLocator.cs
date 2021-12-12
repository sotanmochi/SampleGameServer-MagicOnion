// This code is a modified version of the code in the following article.
// https://blog.gigacreation.jp/entry/2021/01/24/223859

using System;
using System.Collections.Generic;
using UnityEngine;
using SampleGame.Utility;

namespace SampleGame.Lifecycle
{
    public static class ServiceLocator
    {
        static readonly Dictionary<Type, object> instances = new Dictionary<Type, object>();

        /// <summary>
        /// Unregister all instances.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Initialize()
        {
            instances.Clear();
            DebugLogger.Log($"[ServiceLocator] Initialize");
        }

        public static T Register<T>(T instance) where T : class
        {
            var type = typeof(T);

            if (instances.TryGetValue(type, out object registeredInstance))
            {
                DebugLogger.LogWarning($"[ServiceLocator] An instance of the same type has already been registered: {type.Name}");
                return registeredInstance as T;
            }

            instances[type] = instance;
            return instance;
        }

        public static void Unregister<T>(T instance) where T : class
        {
            var type = typeof(T);

            if (!instances.ContainsKey(type))
            {
                DebugLogger.LogWarning($"[ServiceLocator] No instance of the requested type has been registered: {type.Name}");
                return;
            }

            if (!Equals(instances[type], instance))
            {
                DebugLogger.LogWarning($"[ServiceLocator] Instances do not match: {type.Name}");
                return;
            }

            instances.Remove(type);
        }

        public static bool IsRegistered<T>() where T : class
        {
            return instances.ContainsKey(typeof(T));
        }

        public static bool IsRegistered<T>(T instance) where T : class
        {
            var type = typeof(T);

            return instances.ContainsKey(type) && Equals(instances[type], instance);
        }

        public static T GetInstance<T>() where T : class
        {
            var type = typeof(T);

            if (instances.ContainsKey(type))
            {
                return instances[type] as T;
            }

            DebugLogger.LogError($"[ServiceLocator] No instance of the requested type has been registered: {type.Name}");
            return null;
        }

        public static bool TryGetInstance<T>(out T instance) where T : class
        {
            var type = typeof(T);

            instance = instances.ContainsKey(type) ? instances[type] as T : null;

            return instance != null;
        }
    }
}