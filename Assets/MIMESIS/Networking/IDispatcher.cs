using System;
using ReluProtocol;

public interface IDispatcher
{
	bool Fetch(IContext context, in ArraySegment<byte> data);

	bool Fetch(IContext context, IMsg msg);
}
