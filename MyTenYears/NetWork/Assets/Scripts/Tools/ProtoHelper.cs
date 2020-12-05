using ProtoBuf;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class ProtoHelper
{
    public static string Serialize<T>(T t) {
        using (MemoryStream ms = new MemoryStream())
        {
            Serializer.Serialize<T>(ms, t);
            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }

    public static T Deserialize<T>(string str)
    {
        using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(str)))
        {
            return Serializer.Deserialize<T>(ms);
        }
    }
}
