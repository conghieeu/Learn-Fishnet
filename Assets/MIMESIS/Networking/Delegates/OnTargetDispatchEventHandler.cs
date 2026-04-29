using ReluProtocol;

public delegate void OnTargetDispatchEventHandler<T>(T msg, IContext ctx) where T : IMsg, new();
