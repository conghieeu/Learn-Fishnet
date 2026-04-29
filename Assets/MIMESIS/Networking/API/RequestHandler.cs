using ReluProtocol;

public class RequestHandler<T> : IRequestHandler where T : IResMsg, new()
{
	public OnClientDispatchEventHandler<T> ResponseHandler;

	public int TimerID;

	public RequestHandler(OnClientDispatchEventHandler<T> responseHandler, int timerID)
	{
		ResponseHandler = responseHandler;
		TimerID = timerID;
	}

	public void ExecuteResponseHandler(IMsg msg)
	{
		if (ResponseHandler != null && msg is T msg2)
		{
			ResponseHandler(msg2);
		}
		else
		{
			Logger.RError($"ResponseHandler is null or msg is not of type {typeof(T)}");
		}
	}
}
