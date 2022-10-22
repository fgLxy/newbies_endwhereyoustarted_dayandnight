using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
namespace SunAndMoon
{
    [Serializable]
    public class SaveData 
    {
        public int Level;

        public byte[] Serialize()
        {
            using var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
            return stream.ToArray();
        }

        public static SaveData Deserialize(byte[] datas)
        {
            using var stream = new MemoryStream(datas);
            var formatter = new BinaryFormatter();
            return formatter.Deserialize(stream) as SaveData;
        }
    }
}