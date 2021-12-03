using System.Linq;
using UnityEngine;
using Grpc.Core;
using MagicOnion;
using GameServer.UnityClient;

namespace SampleGame.Application
{
    public class Main : MonoBehaviour
    {
        private ChannelBase _channel;

        async void Start()
        {
            _channel = GrpcChannelx.ForAddress("http://localhost:5000");
            var service = new MyFirstService(_channel);

            var x = 1;
            var y = 2;
            var z = await service.SumAsync(x, y);
            Debug.Log($"MyFirstService.SumAsync({x}, {y}): {z}");

            var worldService = new WorldServiceClient(_channel);
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