using System;
using Cysharp.Threading.Tasks;
using ReluProtocol;

public interface APIRequestKV
{
	void SendMsg();
}
public class APIRequestKV<V> : APIRequestKV where V : IResMsg, new()
{
	public IMsg RequestPkt;

	public Action<V> Callback;

	public void SendMsg()
	{
		Hub.s.apihandler.SendIMsg<V>(TargetServerType.APIAuth, RequestPkt).ContinueWith(delegate(V resMsg)
		{
			Callback?.Invoke(resMsg);
		});
	}
}
