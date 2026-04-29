using System;
using UnityEngine;

public class VSpaceSingleGroup : ISpaceGroup, IDisposable
{
	private SPoint m_center;

	private VSpace _space;

	private SRect m_terrainRange;

	private AtomicFlag _disposed = new AtomicFlag(value: false);

	public VSpaceSingleGroup(SRect terrainRange)
	{
		m_center = new SPoint
		{
			X = (terrainRange.Left + terrainRange.Right) / 2,
			Y = (terrainRange.Top + terrainRange.Bottom) / 2
		};
		m_terrainRange = terrainRange;
		_space = new VSpace(this, 0, 0);
		_space.Init();
	}

	private bool CheckInRange(Vector3 realCoord)
	{
		return m_terrainRange.Contains(new SPoint((int)realCoord.x, (int)realCoord.y));
	}

	public VSpace? GetSpace(Vector3 realCoord)
	{
		if (CheckInRange(realCoord))
		{
			return _space;
		}
		return null;
	}

	public bool CheckEnter(int objectID, Vector3 realCoord)
	{
		if (!CheckInRange(realCoord))
		{
			return false;
		}
		if (_space.ExistObject(objectID))
		{
			return false;
		}
		return true;
	}

	public bool AddObject(ISpaceActor actor, Vector3 realCoord)
	{
		if (!CheckInRange(realCoord))
		{
			return false;
		}
		if (!_space.AddObject(actor))
		{
			return false;
		}
		actor.OnEnterSpace(_space);
		return true;
	}

	public bool RemoveObject(ISpaceActor actor)
	{
		if (!_space.RemoveObject(actor))
		{
			return false;
		}
		actor.OnExitSpace(_space);
		return true;
	}

	public bool MoveObject(ISpaceActor sectorObject, Vector3 realCoord)
	{
		return CheckInRange(realCoord);
	}

	public void BeginBatchUpdate()
	{
	}

	public void EndBatchUpdate()
	{
	}

	public VSpace[] GetAroundSectors(int x, int y)
	{
		return new VSpace[1] { _space };
	}

	public SPoint GetCenter()
	{
		return m_center;
	}

	public VSpace[] GetAllAroundSectors(bool includeSelf = false)
	{
		return new VSpace[1] { _space };
	}

	public void Dispose()
	{
		Dispose(isDisposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool isDisposing)
	{
		if (isDisposing && _disposed.On())
		{
			_space.Dispose();
		}
	}
}
