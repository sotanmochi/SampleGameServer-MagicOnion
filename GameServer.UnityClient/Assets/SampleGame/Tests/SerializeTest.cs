using System.Runtime.InteropServices;
using UnityEngine;
using MagicOnion;
using MessagePack;
using MessagePack.Resolvers;
using SampleGame.Utility;

namespace SampleGame.Application
{
    [MessagePackObject]
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct TestObjectStruct
    {
        [Key(0)]
        public ushort PlayerId; // 2[Bytes]

        [Key(1)]
        public Vector3 Position; // 12[Bytes]

        [Key(2)]
        public Quaternion RotationAngles; // 16[Bytes]
    }

    [MessagePackObject]
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct TestObjectStruct2
    {
        [Key(0)]
        public ushort PlayerId; // 2[Bytes]

        [Key(1)]
        public Vector3 Position; // 12[Bytes]

        [Key(2)]
        public Quaternion RotationAngles; // 16[Bytes]
    }

    [MessagePackObject]
    public class TestObjectClass
    {
        [Key(0)]
        public ushort PlayerId;

        [Key(1)]
        public Vector3 Position;

        [Key(2)]
        public Quaternion RotationAngles;
    }

    public class SerializeTest : MonoBehaviour
    {
        void Start()
        {
            Initialize();
            UnsafeDirectBlitResolverTest();
            StandardResolverTest1();
            StandardResolverTest2();
            ByteArrayTest();
        }

        void Initialize()
        {
            UnsafeDirectBlitResolver.Instance.Register<TestObjectStruct>();

            // NOTE: Currently, CompositeResolver doesn't work on Unity IL2CPP build. Use StaticCompositeResolver instead of it.
            StaticCompositeResolver.Instance.Register(
                UnsafeDirectBlitResolver.Instance,
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
            DebugLogger.Log($"[{nameof(SerializeTest.UnsafeDirectBlitResolverTest)}] ***** UnsafeDirectBlitResolverTest *****");

            var testObject = new TestObjectStruct();
            var serializedValue = MessagePackSerializer.Serialize<TestObjectStruct>(testObject);

            var sizeOfObject = Marshal.SizeOf(typeof(TestObjectStruct));

            DebugLogger.Log($"[{nameof(SerializeTest.UnsafeDirectBlitResolverTest)}] Size of {nameof(TestObjectStruct)}: {sizeOfObject} [Bytes]");
            DebugLogger.Log($"[{nameof(SerializeTest.UnsafeDirectBlitResolverTest)}] Size of serialized value: {serializedValue.Length} [Bytes]");

            // var bytesOfUshort = sizeof(ushort);
            // var bytesOfVector3 = Marshal.SizeOf(typeof(Vector3));
            // var bytesOfQuaternion = Marshal.SizeOf(typeof(Quaternion));
            // DebugLogger.Log($"[{nameof(SerializeTest.UnsafeDirectBlitResolverTest)}] Size of ushort: {bytesOfUshort} [Bytes]");
            // DebugLogger.Log($"[{nameof(SerializeTest.UnsafeDirectBlitResolverTest)}] Size of Vector3: {bytesOfVector3} [Bytes]");
            // DebugLogger.Log($"[{nameof(SerializeTest.UnsafeDirectBlitResolverTest)}] Size of Quaternion: {bytesOfQuaternion} [Bytes]");

            var count = 64;
            var objectArray = new TestObjectStruct[count];
            for (int i = 0; i < objectArray.Length; i++)
            {
                objectArray[i] = new TestObjectStruct();
            }

            var serializedArray = MessagePackSerializer.Serialize<TestObjectStruct[]>(objectArray);
            DebugLogger.Log($"[{nameof(SerializeTest.UnsafeDirectBlitResolverTest)}] Size of serialized array: {serializedArray.Length} [Bytes]");
            DebugLogger.Log($"[{nameof(SerializeTest.UnsafeDirectBlitResolverTest)}] sizeOfObject * count: {sizeOfObject}*{count} = {sizeOfObject*count} [Bytes]");
            DebugLogger.Log($"[{nameof(SerializeTest.UnsafeDirectBlitResolverTest)}] serializedValue.Length * count: {serializedValue.Length}*{count} = {serializedValue.Length*count} [Bytes]");

            DebugLogger.Log($"[{nameof(SerializeTest)}] ***** End of UnsafeDirectBlitResolverTest *****");
        }

        void StandardResolverTest1()
        {
            DebugLogger.Log($"[{nameof(SerializeTest.StandardResolverTest1)}] ***** StandardResolverTest1 *****");

            var testObject = new TestObjectClass();
            var serializedValue = MessagePackSerializer.Serialize<TestObjectClass>(testObject);

            DebugLogger.Log($"[{nameof(SerializeTest.StandardResolverTest1)}] {nameof(TestObjectClass)}");
            DebugLogger.Log($"[{nameof(SerializeTest.StandardResolverTest1)}] Size of serialized value: {serializedValue.Length} [Bytes]");

            DebugLogger.Log($"[{nameof(SerializeTest.StandardResolverTest1)}] ***** End of StandardResolverTest1 *****");
        }

        void StandardResolverTest2()
        {
            DebugLogger.Log($"[{nameof(SerializeTest.StandardResolverTest2)}] ***** StandardResolverTest2 *****");

            var testObject = new TestObjectStruct2();
            var serializedValue = MessagePackSerializer.Serialize<TestObjectStruct2>(testObject);

            var sizeOfObject = Marshal.SizeOf(typeof(TestObjectStruct2));

            DebugLogger.Log($"[{nameof(SerializeTest.StandardResolverTest2)}] Size of {nameof(TestObjectStruct2)}: {sizeOfObject} [Bytes]");
            DebugLogger.Log($"[{nameof(SerializeTest.StandardResolverTest2)}] Size of serialized value: {serializedValue.Length} [Bytes]");

            DebugLogger.Log($"[{nameof(SerializeTest.StandardResolverTest2)}] ***** End of StandardResolverTest2 *****");
        }

        void ByteArrayTest()
        {
            DebugLogger.Log($"[{nameof(SerializeTest.ByteArrayTest)}] ***** ByteArrayTest *****");

            var textData1 = "1234567890123456"; // 16
            var textData2 = "803A3037E7E42BF6D5446D12B803221A5C993D987D7980E81AB0E058F6B92F888AC26F13B48AD6A1A63DC9ABEE078A85731792535C44FED0397AAC0569ED6232" // 128
                            + "803A3037E7E42BF6D5446D12B803221A5C993D987D7980E81AB0E058F6B92F888AC26F13B48AD6A1A63DC9ABEE078A85731792535C44FED0397AAC0569ED6232" // 128
                            + "803A3037E7E42BF6D5446D12B803221A5C993D987D7980E81AB0E058F6B92F888AC26F13B48AD6A1A63DC9ABEE078A85731792535C44FED0397AAC0569ED6232" // 128
                            + "803A3037E7E42BF6D5446D12B803221A5C993D987D7980E81AB0E058F6B92F888AC26F13B48AD6A1A63DC9ABEE078A85731792535C44FED0397AAC0569ED6232" // 128
                            + "803A3037E7E42BF6D5446D12B803221A5C993D987D7980E81AB0E058F6B92F888AC26F13B48AD6A1A63DC9ABEE078A85731792535C44FED0397AAC0569ED6232" // 128
                            + "803A3037E7E42BF6D5446D12B803221A5C993D987D7980E81AB0E058F6B92F888AC26F13B48AD6A1A63DC9ABEE078A85731792535C44FED0397AAC0569ED6232" // 128
                            + "803A3037E7E42BF6D5446D12B803221A5C993D987D7980E81AB0E058F6B92F888AC26F13B48AD6A1A63DC9ABEE078A85731792535C44FED0397AAC0569ED6232" // 128
                            + "803A3037E7E42BF6D5446D12B803221A5C993D987D7980E81AB0E058F6B92F888AC26F13B48AD6A1A63DC9ABEE078A85731792535C44FED0397AAC0569ED6232" // 128
                            + "803A3037E7E42BF6D5446D12B803221A5C993D987D7980E81AB0E058F6B92F888AC26F13B48AD6A1A63DC9ABEE078A85731792535C44FED0397AAC0569ED6232" // 128
                            + "803A3037E7E42BF6D5446D12B803221A5C993D987D7980E81AB0E058F6B92F888AC26F13B48AD6A1A63DC9ABEE078A85731792535C44FED0397AAC0569ED6232"; // 128

            var byteArray1 = System.Text.Encoding.UTF8.GetBytes(textData1);
            var serializedValue1 = MessagePackSerializer.Serialize<byte[]>(byteArray1);

            var byteArray2 = System.Text.Encoding.UTF8.GetBytes(textData2);
            var serializedValue2 = MessagePackSerializer.Serialize<byte[]>(byteArray2);

            DebugLogger.Log($"[{nameof(SerializeTest.ByteArrayTest)}] Size of byteArray1: {byteArray1.Length} [Bytes]");
            DebugLogger.Log($"[{nameof(SerializeTest.ByteArrayTest)}] Size of serializedValue1: {serializedValue1.Length} [Bytes]");

            DebugLogger.Log($"[{nameof(SerializeTest.ByteArrayTest)}] Size of byteArray2: {byteArray2.Length} [Bytes]");
            DebugLogger.Log($"[{nameof(SerializeTest.ByteArrayTest)}] Size of serializedValue2: {serializedValue2.Length} [Bytes]");

            var deserialized1 = MessagePackSerializer.Deserialize<byte[]>(serializedValue1);
            var text1 = System.Text.Encoding.UTF8.GetString(deserialized1);
            // DebugLogger.Log($"[{nameof(SerializeTest.ByteArrayTest)}] Text1: {text1}");

            var deserialized2 = MessagePackSerializer.Deserialize<byte[]>(serializedValue2);
            var text2 = System.Text.Encoding.UTF8.GetString(deserialized2);
            // DebugLogger.Log($"[{nameof(SerializeTest.ByteArrayTest)}] Text2: {text2}");

            if (textData1 == text1)
            {
                DebugLogger.Log($"[{nameof(SerializeTest.ByteArrayTest)}] Test for text1 is success.");
            }
            if (textData2 == text2)
            {
                DebugLogger.Log($"[{nameof(SerializeTest.ByteArrayTest)}] Test for text2 is success.");
            }

            DebugLogger.Log($"[{nameof(SerializeTest.ByteArrayTest)}] ***** End of ByteArrayTest *****");
        }
    }
}