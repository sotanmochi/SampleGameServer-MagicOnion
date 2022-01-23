using System.Runtime.InteropServices;
using UnityEngine;
using MagicOnion;
using MessagePack;
using MessagePack.Resolvers;
using GameServer.Shared.MessagePackObject;
using SampleGame.Utility;

namespace SampleGame.Application
{
    public class SerializeTest : MonoBehaviour
    {
        void Start()
        {
            Initialize();
            UnsafeDirectBlitResolverTest();
        }

        void Initialize()
        {
            UnsafeDirectBlitResolver.Instance.Register<PlayerState>();

            // NOTE: Currently, CompositeResolver doesn't work on Unity IL2CPP build. Use StaticCompositeResolver instead of it.
            StaticCompositeResolver.Instance.Register(
                UnsafeDirectBlitResolver.Instance,
                MessagePack.Unity.Extension.UnityBlitResolver.Instance,
                StandardResolver.Instance
            );

            MessagePackSerializer.DefaultOptions = MessagePackSerializer.DefaultOptions.WithResolver(StaticCompositeResolver.Instance);
        }

        /// <summary>
        /// References:<br/>
        ///  - https://tech.cygames.co.jp/archives/3181/<br/>
        ///  - https://neuecc.medium.com/magiconion-unified-realtime-api-engine-for-net-core-and-unity-21e02a57a3ff<br/>
        ///  - https://github.com/Cysharp/MagicOnion/blob/master/src/MagicOnion.Client.Unity/Assets/Scenes/Sandbox.cs<br/>
        /// </summary>
        void UnsafeDirectBlitResolverTest()
        {
            var data = new PlayerState();
            var bytes = MessagePackSerializer.Serialize<PlayerState>(data);

            var dataSize = Marshal.SizeOf(typeof(PlayerState));

            DebugLogger.Log($"[{nameof(SerializeTest)}] Size of PlayerState: {dataSize} [Bytes]");
            DebugLogger.Log($"[{nameof(SerializeTest)}] Size of serialized: {bytes.Length} [Bytes]");

            var count = 64;
            var array = new PlayerState[count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new PlayerState();
            }

            var arrayBytes = MessagePackSerializer.Serialize<PlayerState[]>(array);
            DebugLogger.Log($"[{nameof(SerializeTest)}] Size of PlayerState array: {dataSize}*{array.Length} = {dataSize*array.Length} [Bytes]");
            DebugLogger.Log($"[{nameof(SerializeTest)}] Size of serialized: {arrayBytes.Length} [Bytes]");
        }
    }
}