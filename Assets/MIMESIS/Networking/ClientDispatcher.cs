using System;
using Bifrost;
using ReluProtocol;

public class ClientDispatcher<T> : IClientDispatcher where T : IMsg, new()
{
	private OnClientDispatchEventHandler<T> DispatchHandler;

	public ClientDispatcher(OnClientDispatchEventHandler<T> handler)
	{
		DispatchHandler = handler;
	}

	public bool Fetch(ArraySegment<byte> segment)
	{
		T msg = Serializer.Deserialize<T>(in segment);
		try
		{
			DispatchHandler(msg);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public bool Fetch(IMsg msg)
	{
		if (!(msg is T))
		{
			return false;
		}
		try
		{
			DispatchHandler(msg as T);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}
}
