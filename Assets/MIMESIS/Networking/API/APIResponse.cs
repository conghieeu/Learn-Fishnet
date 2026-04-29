public class APIResponse
{
	public long ResponseCode { get; private set; }

	public string Body { get; private set; } = string.Empty;

	public string ErrorMessage { get; private set; } = string.Empty;

	public APIResponse(long responseCode, string body, string errorMessage)
	{
		ResponseCode = responseCode;
		Body = body;
		ErrorMessage = errorMessage;
	}
}
