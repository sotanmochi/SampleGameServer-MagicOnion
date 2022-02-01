using System;
using System.Collections.Concurrent;

namespace GameServer.Streaming
{
    public static class ClientIdPoolStorage
    {
        public static readonly ushort DefaultCapacity = 1024;
        private static readonly ConcurrentDictionary<string, ConcurrentQueue<ushort>> _poolStorage = new ConcurrentDictionary<string, ConcurrentQueue<ushort>>();

        public static int GetClientId(string roomId)
        {
            if (!_poolStorage.TryGetValue(roomId, out var clientIdPool))
            {
                clientIdPool = CreateOrGetPool(roomId, DefaultCapacity);
            }
            return clientIdPool.TryDequeue(out ushort clientId) ? clientId : -1;
        }

        public static void ReturnToPool(string roomId, ushort clientId)
        {
            if (_poolStorage.TryGetValue(roomId, out var clientIdPool))
            {
                clientIdPool.Enqueue(clientId);
            }
        }

        public static ConcurrentQueue<ushort> CreateOrGetPool(string roomId, ushort capacity)
        {
            var clientIdPool = new ConcurrentQueue<ushort>();

            for (ushort i = 0; i < capacity; i++)
            {
                clientIdPool.Enqueue(i);
            }

            return _poolStorage.GetOrAdd(roomId, clientIdPool);
        }

        public static ushort[] GetPoolArray(string roomId)
        {
            if (_poolStorage.TryGetValue(roomId, out var clientIdPool))
            {
                return clientIdPool.ToArray();
            }
            else
            {
                return Array.Empty<ushort>();
            }
        }
    }
}