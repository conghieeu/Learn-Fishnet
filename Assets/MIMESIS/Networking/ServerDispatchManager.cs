using System;
using System.Collections.Generic;
using ReluProtocol;
using ReluProtocol.Enum;

public class ServerDispatchManager
{
	private Dictionary<MsgType, IDispatcher> _messageDispatcherMap = new Dictionary<MsgType, IDispatcher>();

	public OnSocketBufferOverflowEventHandler? OnBufferOverflowHandler { get; private set; }

	public OnPreDispatchEventHandler? OnDefaultPreDispatcherHandler { get; private set; }

	public OnInvalidMessageEventHandler? OnInvalidMessageHandler { get; private set; }

	public OnPreSendEventHandler? OnPreSendEventHandler { get; private set; }

	public void SetDefaultPreDispatcher(OnPreDispatchEventHandler handler)
	{
		OnDefaultPreDispatcherHandler = (OnPreDispatchEventHandler)Delegate.Combine(OnDefaultPreDispatcherHandler, handler);
	}

	public void SetPreSendEventHandler(OnPreSendEventHandler handler)
	{
		OnPreSendEventHandler = (OnPreSendEventHandler)Delegate.Combine(OnPreSendEventHandler, handler);
	}

	public void SetSocketBufferOverflowHandler(OnSocketBufferOverflowEventHandler handler)
	{
		OnBufferOverflowHandler = (OnSocketBufferOverflowEventHandler)Delegate.Combine(OnBufferOverflowHandler, handler);
	}

	public void SetSocketInvalidMessageHandler(OnInvalidMessageEventHandler handler)
	{
		OnInvalidMessageHandler = (OnInvalidMessageEventHandler)Delegate.Combine(OnInvalidMessageHandler, handler);
	}

	public void RegisterDispatcher<T>(OnDispatchEventHandler<T> handler, OnPreDispatchEventHandler? preDispatchEventHandler = null, ParsingType parsingType = ParsingType.MemoryPack) where T : IMsg, new()
	{
		T val = new T
		{
			parsingType = parsingType
		};
		if (_messageDispatcherMap.ContainsKey(val.msgType))
		{
			Logger.RError($"[DispatcherManager] Duplicated MessageID Insertion : {val.msgType}");
			return;
		}
		preDispatchEventHandler = (OnPreDispatchEventHandler)Delegate.Combine(preDispatchEventHandler, OnDefaultPreDispatcherHandler);
		_messageDispatcherMap[val.msgType] = new ServerDispatcher<T>(handler, preDispatchEventHandler, val.parsingType);
	}

	public void Dispatch(ISession session, IMsg msg)
	{
		if (_messageDispatcherMap.TryGetValue(msg.msgType, out IDispatcher value) && session.Context != null)
		{
			if (ReluProtocol.Version.IsEnable && msg.hashCode != ReluProtocol.Version.GetProtocolHashCode())
			{
				Logger.RError($"packet parsing failed, invalid protocol version, {msg.hashCode} != {ReluProtocol.Version.GetProtocolHashCode()}");
			}
			else
			{
				value.Fetch(session.Context, msg);
			}
		}
	}

	public bool Dispatch(ISession session, int pktLength, int pktType, in ArraySegment<byte> data)
	{
		if (!_messageDispatcherMap.TryGetValue((MsgType)pktType, out IDispatcher value))
		{
			Logger.RError($"{session.Context} Invalid Message - {(MsgType)pktType} / {pktLength}");
			OnInvalidMessageHandler?.Invoke(session, (MsgType)pktType, pktLength);
			return false;
		}
		return value.Fetch(session.Context, in data);
	}

	public bool LoopbackLink(ISession session, IMsg msg)
	{
		Dispatch(session, msg);
		return true;
	}
}
