using System.Linq;
using UnityEngine;
using GameServer.UnityClient;

namespace SampleGame.Application
{
    public class ServiceTest : MonoBehaviour
    {
        async void Start()
        {
            var uri = new System.Uri("http://localhost:5000");
            var service = new MyFirstService(uri);

            var x = 1;
            var y = 2;
            var z = await service.SumAsync(x, y);
            Debug.Log($"MyFirstService.SumAsync({x}, {y}): {z}");

            var worldService = new WorldServiceClient(uri);
            var worldIds = await worldService.FindWorldIdOrderByRank();
            Debug.Log($"WorldServiceClient.FindWorldIdOrderByRank: {worldIds.Count}");

            Debug.Log($"First world: {worldIds.FirstOrDefault()}");
            var firstWorld = await worldService.FindWorld(worldIds.FirstOrDefault());
            if (firstWorld != null)
            {
                Debug.Log($"FirstWorld: ");
                Debug.Log($"{{");
                Debug.Log($"    WorldId: {firstWorld.WorldId}");
                Debug.Log($"    Name: {firstWorld.Name}");
                Debug.Log($"    Description: {firstWorld.Description}");
                Debug.Log($"}}");
            }
        }
    }
}