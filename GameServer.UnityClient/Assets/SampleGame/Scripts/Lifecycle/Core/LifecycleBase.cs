// using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace SampleGame.Lifecycle
{
    public abstract class LifecycleBase : MonoBehaviour
    {
        public abstract UniTask InitializeAsync();
        public abstract UniTask DisposeAsync();
    }
}