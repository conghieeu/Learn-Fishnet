using System;

public struct BTKey : IEquatable<BTKey>
{
	public readonly string AIName;

	public readonly string TemplateName;

	public BTKey(string aiName, string templateName)
	{
		AIName = aiName.ToLower();
		TemplateName = templateName.ToLower();
	}

	public override bool Equals(object? obj)
	{
		if (obj is BTKey other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(BTKey other)
	{
		return (AIName == other.AIName) & (TemplateName == other.TemplateName);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(AIName, TemplateName);
	}

	public override string ToString()
	{
		return AIName + " : " + TemplateName;
	}
}
