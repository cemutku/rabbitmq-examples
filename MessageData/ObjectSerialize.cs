using System;
using System.Text;
using System.Text.Json;

namespace MessageData
{
    public static class ObjectSerialize
    {
        public static byte[] SerializeObject(this object obj)
        {
            byte[] result = null;

            if (obj != null)
            {
                var json = JsonSerializer.Serialize(obj);
                result = Encoding.ASCII.GetBytes((json));
            }

            return result;
        }

        public static object DeserializeObject(this ReadOnlyMemory<byte> byteArray, Type type)
        {
            var json = Encoding.Default.GetString(byteArray.ToArray());
            return JsonSerializer.Deserialize(json, type);
        }

        public static string DeserializeText(this ReadOnlyMemory<byte> byteArray)
        {
            return Encoding.Default.GetString(byteArray.ToArray());
        }
    }
}
