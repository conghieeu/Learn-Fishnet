using System;
using System.Runtime.Serialization;

public class MimicException : Exception
{
	public MimicException()
		: base("MimicException")
	{
	}

	public MimicException(string message)
		: base("MimicException: " + message)
	{
	}

	public MimicException(string message, Exception innerException)
		: base("MimicException: " + message, innerException)
	{
	}

	protected MimicException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
