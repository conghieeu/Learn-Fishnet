using System;

public class PacketHandlerAttribute : Attribute
{
	public bool isAvatarResPacket { get; set; }

	public PacketHandlerAttribute(bool isAvatarResPacket = false)
	{
		this.isAvatarResPacket = isAvatarResPacket;
	}
}
