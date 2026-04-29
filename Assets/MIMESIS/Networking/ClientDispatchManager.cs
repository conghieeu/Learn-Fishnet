using System;
using System.Collections.Generic;
using ReluProtocol;
using ReluProtocol.Enum;

public class ClientDispatchManager
{
	public Dictionary<MsgType, IClientDispatcher> MessageDispatcherMap = new Dictionary<MsgType, IClientDispatcher>();

	public Dictionary<MsgType, IClientDispatcher> SessionOnceDispatcherMap = new Dictionary<MsgType, IClientDispatcher>();

	public void Dispatch(ISession session, IMsg msg)
	{
		MsgType msgType = msg.msgType;
		if (MessageDispatcherMap.TryGetValue(msgType, out var value) && value.Fetch(msg) && SessionOnceDispatcherMap.TryGetValue(msgType, out var value2))
		{
			value2.Fetch(msg);
		}
	}

	public bool Dispatch(ISession session, (int size, int pktType, ArraySegment<byte> data) rawPkt)
	{
		MsgType item = (MsgType)rawPkt.pktType;
		if (!MessageDispatcherMap.TryGetValue(item, out var value))
		{
			return false;
		}
		if (!value.Fetch(rawPkt.data))
		{
			return false;
		}
		if (SessionOnceDispatcherMap.TryGetValue(item, out var value2) && !value2.Fetch(rawPkt.data))
		{
			return false;
		}
		return true;
	}

	private void RegisterDispatcher<T>(MsgType msgType, OnClientDispatchEventHandler<T> handler) where T : IMsg, new()
	{
		if (!MessageDispatcherMap.ContainsKey(msgType))
		{
			MessageDispatcherMap[msgType] = new ClientDispatcher<T>(handler);
		}
	}

	public void RegisterDispatcher<T>(OnClientDispatchEventHandler<T> handler) where T : IMsg, new()
	{
		MsgType msgType = new T().msgType;
		RegisterDispatcher(msgType, handler);
	}

	public bool RegisterDispatcherOnce<T>(OnClientDispatchEventHandler<T> handler) where T : IMsg, new()
	{
		MsgType msgType = new T().msgType;
		if (SessionOnceDispatcherMap.ContainsKey(msgType))
		{
			return false;
		}
		return SessionOnceDispatcherMap.TryAdd(msgType, new ClientDispatcher<T>(handler));
	}

	public void UnregisterDispatcherOnce(MsgType msgType)
	{
		SessionOnceDispatcherMap.Remove(msgType, out var _);
	}

	public void Dispose()
	{
		SessionOnceDispatcherMap.Clear();
		MessageDispatcherMap.Clear();
	}
}
