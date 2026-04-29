using Cysharp.Threading.Tasks;
using ReluProtocol;

public delegate UniTask OnClientSessionDispatchAsyncEventHandler<T>(SessionContext ctx, T msg) where T : IMsg, new();
