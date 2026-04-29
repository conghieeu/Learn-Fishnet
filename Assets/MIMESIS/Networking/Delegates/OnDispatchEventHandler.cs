using ReluProtocol;

public delegate void OnDispatchEventHandler<T>(IContext ctx, T msg) where T : IMsg, new();
