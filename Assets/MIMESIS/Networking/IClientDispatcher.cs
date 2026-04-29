using System;
using ReluProtocol;

public interface IClientDispatcher
{
	bool Fetch(ArraySegment<byte> segment);

	bool Fetch(IMsg msg);
}
