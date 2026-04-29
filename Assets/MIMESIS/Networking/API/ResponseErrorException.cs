using System;

public class ResponseErrorException : Exception
{
	public int error { get; private set; }

	public ResponseErrorException(int error, string message)
		: base(message)
	{
		this.error = error;
	}
}
