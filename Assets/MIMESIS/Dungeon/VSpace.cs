using System;
using System.Collections.Generic;

public class VSpace : IDisposable
{
	private ISpaceGroup m_parent;

	private Dictionary<int, ISpaceActor> m_objects;

	private AtomicFlag _disposed = new AtomicFlag(value: false);

	public int X { get; }

	public int Y { get; }

	public VSpace[] AroundSpaces { get; private set; }

	public VSpace(ISpaceGroup spaceGroup, int x, int y)
	{
		X = X;
		Y = y;
		m_parent = spaceGroup;
		m_objects = new Dictionary<int, ISpaceActor>();
		AroundSpaces = new VSpace[0];
	}

	public void Init()
	{
		AroundSpaces = m_parent.GetAroundSectors(X, Y);
	}

	public bool ExistObject(int objectID)
	{
		return m_objects.ContainsKey(objectID);
	}

	public bool AddObject(ISpaceActor sectorObject)
	{
		return m_objects.TryAdd(sectorObject.ObjectID, sectorObject);
	}

	public bool RemoveObject(ISpaceActor sectorObject)
	{
		return m_objects.Remove(sectorObject.ObjectID);
	}

	public void IterateObject(Action<ISpaceActor> action)
	{
		foreach (KeyValuePair<int, ISpaceActor> @object in m_objects)
		{
			action(@object.Value);
		}
	}

	public void IterateAroundObject(Action<ISpaceActor> action)
	{
		VSpace[] aroundSpaces = AroundSpaces;
		for (int i = 0; i < aroundSpaces.Length; i++)
		{
			aroundSpaces[i].IterateObject(action);
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (disposing && _disposed.On())
		{
			m_objects.Clear();
		}
	}
}
