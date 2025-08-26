using System;

namespace UnicoCaseStudy.Utilities
{
    public interface ISerializer
    {
        byte[] SerializeObject(object obj);

        T DeserializeObject<T>(byte[] serializedObj);

        object DeserializeObject(byte[] serializedObj, Type type);
    }
}