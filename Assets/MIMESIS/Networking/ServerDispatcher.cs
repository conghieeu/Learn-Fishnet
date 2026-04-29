using System;
using System.IO;
using Bifrost;
using Newtonsoft.Json;
using ReluProtocol;
using ReluProtocol.Enum;

public class ServerDispatcher<T> : IDispatcher where T : IMsg, new()
{
	private OnDispatchEventHandler<T>? DispatchHandler;

	private OnPreDispatchEventHandler? PreDispatchEventHandler;

	private ParsingType m_ParsingType;

	public ServerDispatcher(OnDispatchEventHandler<T> handler, OnPreDispatchEventHandler? preDispatchEventHandler, ParsingType parsingType)
	{
		DispatchHandler = handler;
		PreDispatchEventHandler = preDispatchEventHandler;
		m_ParsingType = parsingType;
	}

	public ServerDispatcher(ParsingType parsingType)
	{
		m_ParsingType = parsingType;
	}

	public bool Fetch(IContext context, in ArraySegment<byte> data)
	{
		try
		{
			if (data.Array == null)
			{
				Logger.RError("packet parsing failed, data.Array is null");
				return false;
			}
			T val;
			if (m_ParsingType == ParsingType.Json)
			{
				using MemoryStream stream = new MemoryStream(data.Array);
				using StreamReader streamReader = new StreamReader(stream);
				val = JsonConvert.DeserializeObject<T>(streamReader.ReadToEnd());
			}
			else
			{
				val = Serializer.Deserialize<T>(in data);
			}
			if (val == null)
			{
				Logger.RError($"packet parsing failed, parsingType: {m_ParsingType}");
				return false;
			}
			if (ReluProtocol.Version.IsEnable && val.hashCode != ReluProtocol.Version.GetProtocolHashCode())
			{
				Logger.RError($"packet parsing failed, invalid protocol version, {val.hashCode} != {ReluProtocol.Version.GetProtocolHashCode()}");
				return false;
			}
			if (PreDispatchEventHandler != null && !PreDispatchEventHandler(val, context))
			{
				return false;
			}
			DispatchHandler?.Invoke(context, val);
			return true;
		}
		catch (Exception e)
		{
			Logger.RError(e);
			return false;
		}
	}

	public bool Fetch(IContext context, IMsg msg)
	{
		if (!(msg is T msg2))
		{
			return false;
		}
		try
		{
			if (PreDispatchEventHandler != null && !PreDispatchEventHandler(msg, context))
			{
				return false;
			}
			DispatchHandler?.Invoke(context, msg2);
			return true;
		}
		catch (Exception e)
		{
			Logger.RError(e);
			return false;
		}
	}

	bool IDispatcher.Fetch(IContext context, in ArraySegment<byte> data)
	{
		return Fetch(context, in data);
	}
}
