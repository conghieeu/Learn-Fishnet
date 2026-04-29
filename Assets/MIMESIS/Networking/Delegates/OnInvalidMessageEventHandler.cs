using ReluProtocol.Enum;

public delegate bool OnInvalidMessageEventHandler(ISession session, MsgType MsgType, long recvLength);
