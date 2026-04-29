using System;
using System.Net;

public struct HostInfo : IEquatable<HostInfo>
{
	public string Host { get; set; }

	public int Port { get; set; }

	public string HostName { get; set; }

	public int ManagementPort { get; set; }

	public static HostInfo Dummy => default(HostInfo);

	public override bool Equals(object? obj)
	{
		if (obj is HostInfo other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(HostInfo other)
	{
		if (Host == other.Host)
		{
			return Port == other.Port;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Host, Port);
	}

	public override string ToString()
	{
		return $"{Host}:{Port}";
	}

	public IPEndPoint GetIPEndPoint()
	{
		return new IPEndPoint(IPAddress.Parse(Host), Port);
	}
}
