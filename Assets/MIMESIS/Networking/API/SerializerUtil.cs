using MemoryPack;
using UnityEngine;

public static class SerializerUtil
{
	[RuntimeInitializeOnLoadMethod]
	private static void OnRuntimeInitializeOnLoadMethod()
	{
		Logger.RLog("[SerializerUtil] OnRuntimeInitializeOnLoadMethod");
	}

	public static string GetSerializerType()
	{
		return "MP";
	}

	public static byte[] Serialize<T>(string fileName, T obj)
	{
		return MemoryPackSerializer.Serialize(in obj);
	}

	public static T Deserialize<T>(byte[] zipBytes)
	{
		return MemoryPackSerializer.Deserialize<T>(zipBytes);
	}
}
