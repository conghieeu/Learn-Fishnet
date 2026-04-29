using System;
using System.Collections.Generic;
using System.Reflection;
using ReluProtocol;
using ReluProtocol.Enum;
using UnityEngine;

public class MsgPairGenerator : MonoBehaviour
{
	private Dictionary<MsgType, IMsgTypePair> PairDict = new Dictionary<MsgType, IMsgTypePair>();

	private void Awake()
	{
		Logger.RLog("[AwakeLogs] MsgPairGenerator.Awake ->");
		Initialize();
		Logger.RLog("[AwakeLogs] MsgPairGenerator.Awake <-");
	}

	private void OnDestroy()
	{
		PairDict.Clear();
	}

	private void Initialize()
	{
		Type[] types = Assembly.Load(typeof(MsgType).Assembly.GetName().Name).GetTypes();
		if (types == null)
		{
			return;
		}
		Type[] array = types;
		foreach (Type type in array)
		{
			if (type.IsClass && typeof(IMsg).IsAssignableFrom(type) && (object)type.GetConstructor(new Type[0]) != null && Activator.CreateInstance(type) is IMsg msg)
			{
				Type type2 = typeof(MsgTypePair<>).MakeGenericType(type);
				PairDict.Add(msg.msgType, Activator.CreateInstance(type2) as IMsgTypePair);
			}
		}
	}

	public byte[] Serialize(IMsg msg)
	{
		if (PairDict.TryGetValue(msg.msgType, out IMsgTypePair value))
		{
			return value.Serialize(msg);
		}
		Logger.RError($"MsgPairGenerator Serialize failed, msgType: {msg.msgType}");
		return new byte[0];
	}

	public IMsg? Deserialize(MsgType type, in ArraySegment<byte> data)
	{
		if (PairDict.TryGetValue(type, out IMsgTypePair value))
		{
			return value.Deserialize(in data);
		}
		return new IMsg(MsgType.Invalid);
	}

	public IMsg? Deserialize(MsgType type, byte[] bytes)
	{
		if (PairDict.TryGetValue(type, out IMsgTypePair value))
		{
			return value.Deserialize((ArraySegment<byte>)bytes);
		}
		return new IMsg(MsgType.Invalid);
	}

	public IMsg Deserialize(MsgType type, string jsonBody)
	{
		if (PairDict.TryGetValue(type, out IMsgTypePair value))
		{
			return value.Deserialize(jsonBody);
		}
		return new IMsg(MsgType.Invalid);
	}
}
