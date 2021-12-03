using System;
using UnityEngine;
using GameServer.Shared;
using GameServer.UnityClient;
using GameServer.UnityClient.Admin;

namespace SampleGame.Application
{
    public class AdminMain : MonoBehaviour
    {
        async void Start()
        {
            var uri = new Uri("http://localhost:5000");
            var worldService = new WorldServiceClient(uri);
            var worldAdminService = new WorldAdminServiceClient(uri);

            var world = new World()
            {
                WorldId = "World-999",
                Name = "TestWorld-999",
                Description = $"Created from admin client at {DateTime.Now}.",
            };

            await worldAdminService.CreateOrUpdateWorld(world);
            
            var createdWorld = await worldService.FindWorld(world.WorldId);
            if (createdWorld != null)
            {
                Debug.Log($"CreatedWorld: ");
                Debug.Log($"{{");
                Debug.Log($"    WorldId: {createdWorld.WorldId}");
                Debug.Log($"    Name: {createdWorld.Name}");
                Debug.Log($"    Description: {createdWorld.Description}");
                Debug.Log($"}}");
            }
        }
    }
}