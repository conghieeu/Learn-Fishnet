using ReluProtocol;

public delegate void OnClientSessionDispatchEventHandler<T>(SessionContext ctx, T msg) where T : IMsg, new();
