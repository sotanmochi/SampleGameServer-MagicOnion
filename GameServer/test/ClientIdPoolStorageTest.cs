namespace GameServer.Streaming.Test
{
    public class ClientIdPoolStorageTest
    {
        public void Test1()
        {
            var roomId = "TEST_ROOM";
            var firstClientId = ClientIdPoolStorage.GetClientId(roomId);
            var poolArrayLength = ClientIdPoolStorage.GetPoolArray(roomId).Length;

            // Expected results:
            //  - The first clientId is 0.
            //  - Pool array length is (DefaultCapacity - 1).

            // Console.WriteLine($"[ClientIdPoolStorageTest.Test1] First clientId: {firstClientId}");
            // Console.WriteLine($"[ClientIdPoolStorageTest.Test1] Pool array length: {poolArrayLength}");
            // Console.WriteLine($"[ClientIdPoolStorageTest.Test1] DefaultCapacity: {ClientIdPoolStorage.DefaultCapacity}");
        }

        public void Test2()
        {
            var roomId = "TEST_ROOM2";
            var capacity = (ushort)128;

            var pool = ClientIdPoolStorage.CreateOrGetPool(roomId, capacity);
            var poolArray = ClientIdPoolStorage.GetPoolArray(roomId);

            // Expected result: 
            //   pool.Count == capacity
            //   poolArray.Length == capacity

            // Console.WriteLine($"[ClientIdPoolStorageTest.Test2] Queue.Count: {pool.Count}");
            // Console.WriteLine($"[ClientIdPoolStorageTest.Test2] Array.Length: {poolArray.Length}");

            var invalidClientIdCount = 0;
            for (int i = 0; i < poolArray.Length; i++)
            {
                // Console.WriteLine($"{i}: {poolArray[i]}");
                if (poolArray[i] != i) invalidClientIdCount++;
            }

            // Expected result:
            //   invalidClientId == 0

            // Console.WriteLine($"[ClientIdPoolStorageTest.Test2] InvalidClientIdCount: {invalidClientIdCount}");

            for (int i = 0; i < capacity; i++)
            {
                var id = ClientIdPoolStorage.GetClientId(roomId);

                if (i % 2 == 0 && id >= 0)
                {
                    ClientIdPoolStorage.ReturnToPool(roomId, (ushort)id);
                }
            }

            invalidClientIdCount = 0;
            for (int i = 0; i < capacity; i++)
            {
                var id = ClientIdPoolStorage.GetClientId(roomId);

                if (id != 2*i)
                {
                    invalidClientIdCount++;
                }
            }

            // Expected result:
            //   invalidCLientIdCount == capacity / 2;

            // Console.WriteLine($"[ClientIdPoolStorageTest.Test2] InvalidClientIdCount: {invalidClientIdCount}");

            ClientIdPoolStorage.CreateOrGetPool(roomId, 512);
            var length = ClientIdPoolStorage.GetPoolArray(roomId).Length;

            // Expected result: 
            //   length == 0

            // Console.WriteLine($"[ClientIdPoolStorageTest.Test2] Length: {length}");
        }
    }
}