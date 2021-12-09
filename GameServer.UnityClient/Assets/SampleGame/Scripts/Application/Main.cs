using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace SampleGame.Application
{
    /// <summary>
    /// Entry point
    /// </summary>
    public class Main : MonoBehaviour
    {
        async void Start()
        {
            await LoadScenesAsync();
        }

        private async UniTask LoadScenesAsync()
        {
            // await SceneManager.LoadSceneAsync("System", LoadSceneMode.Additive);
            // await SceneManager.LoadSceneAsync("ChatScreen", LoadSceneMode.Additive);
        }
    }
}