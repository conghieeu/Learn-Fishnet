using System;
using System.IO;
using System.Net;

public struct CompositedLinkHeader
{
	[Flags]
	public enum PacketFlags
	{
		None = 0,
		LZ4Compressed = 1,
		DeflateCompressed = 2,
		AESEncrypted = 4
	}

	public const uint MessageID = 0u;

	public const int MessageSize = 9;

	public uint Length;

	public PacketFlags Flags;

	public bool Load(byte[] source, int count)
	{
		if (count < 9)
		{
			return false;
		}
		using MemoryStream input = new MemoryStream(source, 0, count, writable: false);
		using BinaryReader binaryReader = new BinaryReader(input);
		Length = binaryReader.ReadUInt32();
		if (binaryReader.ReadUInt32() != (uint)IPAddress.NetworkToHostOrder(0))
		{
			return false;
		}
		Flags = (PacketFlags)binaryReader.ReadByte();
		return true;
	}

	public void Save(BinaryWriter writer)
	{
		writer.Write(Length);
		writer.Write(IPAddress.HostToNetworkOrder(0));
		writer.Write((byte)Flags);
	}
}
