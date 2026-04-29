using ReluProtocol;

public delegate void OnPlayerDispatchEventHandler<T>(VPlayer sender, T msg) where T : IMsg, new();
