using System;
using System.IO;
using ReluProtocol;

public abstract class IMsgTypePair
{
	public abstract byte[] Serialize(IMsg msg);

	public abstract IMsg Deserialize(MemoryStream stream);

	public abstract IMsg? Deserialize(in ArraySegment<byte> data);

	public abstract IMsg Deserialize(string jsonBody);
}
