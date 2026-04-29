public class STSocketOption
{
	public bool DontLinger { get; set; }

	public bool KeepAlive { get; set; }

	public bool ReuseAddr { get; set; }

	public bool OutOfBandInline { get; set; }

	public bool Broadcast { get; set; }

	public bool AcceptConnection { get; set; }

	public bool Debug { get; set; }

	public bool DontFragment { get; set; }

	public bool DualIPMode { get; set; }

	public bool UseLoopback { get; set; }

	public bool HeaderIncluded { get; set; }

	public bool MulticastLoopback { get; set; }

	public bool NoDelay { get; set; }

	public bool BsdUrgent { get; set; }

	public bool Expedited { get; set; }

	public bool NoChecksum { get; set; }

	public int TCPKeepAliveInterval { get; set; }

	public int TCPKeepAliveRetryCount { get; set; }

	public int TCPKeepAliveTime { get; set; }
}
