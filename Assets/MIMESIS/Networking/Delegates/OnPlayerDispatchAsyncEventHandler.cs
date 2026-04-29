using Cysharp.Threading.Tasks;
using ReluProtocol;

public delegate UniTask OnPlayerDispatchAsyncEventHandler<T>(VPlayer sender, T msg) where T : IMsg, new();
