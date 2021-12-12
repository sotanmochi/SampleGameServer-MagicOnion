using System.Collections.Generic;
using UnityEngine;

namespace SampleGame.Lifecycle
{
    [DefaultExecutionOrder(-1100)]
    public sealed class LifecycleContext : MonoBehaviour
    {
        [SerializeField] private List<LifecycleBase> _lifecycles;

        private async void Awake()
        {
            foreach (var lifecycle in _lifecycles)
            {
                await lifecycle.InitializeAsync();
            }
        }

        private async void OnDestroy()
        {
            foreach (var lifecycle in _lifecycles)
            {
                await lifecycle.DisposeAsync();
            }
        }
    }  
}