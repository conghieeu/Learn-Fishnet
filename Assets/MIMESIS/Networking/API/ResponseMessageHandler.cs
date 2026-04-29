using ReluProtocol;

public delegate void ResponseMessageHandler<T>(T response, bool success) where T : IMsg, new();
public delegate void ResponseMessageHandler(IResMsg response, bool success);
