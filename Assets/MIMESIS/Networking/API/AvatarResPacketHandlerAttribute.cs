using ReluProtocol.Enum;

public class AvatarResPacketHandlerAttribute : PacketHandlerAttribute
{
	public MsgType msgType;

	public AvatarResPacketHandlerAttribute(MsgType msgType)
		: base(isAvatarResPacket: true)
	{
		this.msgType = msgType;
	}
}
