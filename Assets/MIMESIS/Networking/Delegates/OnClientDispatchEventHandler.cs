using ReluProtocol;

public delegate void OnClientDispatchEventHandler<T>(T msg) where T : IMsg, new();
