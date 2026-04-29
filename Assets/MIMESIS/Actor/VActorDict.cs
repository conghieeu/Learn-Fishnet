using System;
using System.Collections.Generic;

public class VActorDict<V, T> : Dictionary<V, T> where V : notnull where T : VActor
{
	private readonly int m_MaxCount;

	public event ActorListChangeEventHandler? OnActorListChange;

	public VActorDict(int maxCount)
	{
		m_MaxCount = maxCount;
	}

	public T? FindActor(V index)
	{
		try
		{
			if (TryGetValue(index, out var value))
			{
				return value;
			}
		}
		catch (Exception e)
		{
			Logger.RError(e);
		}
		return null;
	}

	public VWorldErrorCode AddActor(V index, T actor)
	{
		try
		{
			if (actor == null)
			{
				return VWorldErrorCode.ActorIsNull;
			}
			if (base.Count >= m_MaxCount)
			{
				return VWorldErrorCode.ChannelFull;
			}
			if (ContainsKey(index))
			{
				return VWorldErrorCode.ActorAlreadyInChannel;
			}
			if (!TryAdd(index, actor))
			{
				return VWorldErrorCode.Unknown;
			}
			this.OnActorListChange?.Invoke(actor, new ActorListChangeEventArgs(ActorListEventType.Add));
			return VWorldErrorCode.None;
		}
		catch (Exception e)
		{
			Logger.RError(e);
			return VWorldErrorCode.Unknown;
		}
	}

	public bool RemoveActor(V index)
	{
		try
		{
			if (Remove(index, out var value))
			{
				this.OnActorListChange?.Invoke(value, new ActorListChangeEventArgs(ActorListEventType.Remove));
				return true;
			}
			return false;
		}
		catch (Exception e)
		{
			Logger.RError(e);
			return false;
		}
	}
}
