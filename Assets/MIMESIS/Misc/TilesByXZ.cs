using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class TilesByXZ<T>
{
	private Dictionary<int, HashSet<T>> nodes = new Dictionary<int, HashSet<T>>();

	private readonly ObjectPool<HashSet<T>> setPool;

	public float tileSize { get; private set; } = 10f;

	public TilesByXZ(float tileSize)
	{
		this.tileSize = tileSize;
		setPool = new ObjectPool<HashSet<T>>(() => new HashSet<T>(), delegate(HashSet<T> set)
		{
			set.Clear();
		}, delegate(HashSet<T> set)
		{
			set.Clear();
		}, delegate(HashSet<T> set)
		{
			set.Clear();
		}, collectionCheck: false);
	}

	public void AddItem(Vector3 pos, T item)
	{
		if (item == null)
		{
			Logger.RWarn("Attempted to add a null item to TilesByXZ.");
			return;
		}
		int key = WorldToTileKey(pos);
		if (!nodes.TryGetValue(key, out var value))
		{
			value = setPool.Get();
			nodes[key] = value;
		}
		value.Add(item);
	}

	public void AddItems<U>(IEnumerable<U> items) where U : MonoBehaviour
	{
		if (items == null)
		{
			Logger.RWarn("Attempted to add a null IEnumerable to TilesByXZ.");
			return;
		}
		foreach (U item in items)
		{
			if (item == null)
			{
				Logger.RWarn("Attempted to add a null item in AddItems to TilesByXZ.");
			}
			else
			{
				AddItem(item.transform.position, (T)(object)item);
			}
		}
	}

	public void RemoveItem(Vector3 pos, T item)
	{
		if (item == null)
		{
			Logger.RWarn("Attempted to remove a null item from TilesByXZ.");
			return;
		}
		int key = WorldToTileKey(pos);
		if (nodes.TryGetValue(key, out var value))
		{
			value.Remove(item);
			if (value.Count == 0)
			{
				nodes.Remove(key);
				setPool.Release(value);
			}
		}
	}

	public void ForeachItems(Vector3 pos, Action<T> action, int expand = 1)
	{
		if (action == null)
		{
			Logger.RWarn("Action is null in ForeachItems.");
			return;
		}
		(int x, int z) tuple = WorldToTileIndex(pos);
		int item = tuple.x;
		int item2 = tuple.z;
		for (int i = item - expand; i <= item + expand; i++)
		{
			for (int j = item2 - expand; j <= item2 + expand; j++)
			{
				int key = TileIndexToKey(i, j);
				if (!nodes.TryGetValue(key, out var value) || value.Count <= 0)
				{
					continue;
				}
				foreach (T item3 in value)
				{
					action(item3);
				}
			}
		}
	}

	public void Clear()
	{
		foreach (HashSet<T> value in nodes.Values)
		{
			setPool.Release(value);
		}
		nodes.Clear();
	}

	private (int x, int z) WorldToTileIndex(Vector3 pos)
	{
		int item = Mathf.FloorToInt(pos.x / tileSize);
		int item2 = Mathf.FloorToInt(pos.z / tileSize);
		return (x: item, z: item2);
	}

	private int WorldToTileKey(Vector3 pos)
	{
		int value = Mathf.FloorToInt(pos.x / tileSize);
		int value2 = Mathf.FloorToInt(pos.z / tileSize);
		short num = (short)Mathf.Clamp(value, -32768, 32767);
		short num2 = (short)Mathf.Clamp(value2, -32768, 32767);
		return (num << 16) | (ushort)num2;
	}

	private int TileIndexToKey(int x, int z)
	{
		return ((short)x << 16) | (ushort)(short)z;
	}
}
