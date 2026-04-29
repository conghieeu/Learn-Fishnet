using System.Collections.Generic;
using UnityEngine;

public static class ReLUGameKitUtility
{
	public static Dictionary<string, string> MakeKV(params string[] args)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		for (int i = 0; i < args.Length / 2; i++)
		{
			dictionary.Add(args[i * 2], args[i * 2 + 1]);
		}
		return dictionary;
	}

	public static Dictionary<T, S> Clone<T, S>(this Dictionary<T, S> src)
	{
		Dictionary<T, S> dictionary = new Dictionary<T, S>();
		foreach (KeyValuePair<T, S> item in src)
		{
			dictionary.Add(item.Key, item.Value);
		}
		return dictionary;
	}

	public static List<T> Clone<T>(this List<T> src)
	{
		List<T> list = new List<T>();
		foreach (T item in src)
		{
			list.Add(item);
		}
		return list;
	}

	public static List<T> MakeListByHierarchyOrder<T>(Transform root, bool includeInactive = false) where T : Behaviour
	{
		List<T> list = new List<T>();
		AppendListByHierarchyOrder(ref list, root, includeInactive);
		return list;
	}

	private static void AppendListByHierarchyOrder<T>(ref List<T> list, Transform node, bool includeInactive) where T : Behaviour
	{
		if (node == null)
		{
			return;
		}
		T[] components = node.GetComponents<T>();
		list.AddRange(components);
		foreach (Transform item in node)
		{
			if (includeInactive || item.gameObject.activeSelf)
			{
				AppendListByHierarchyOrder(ref list, item, includeInactive);
			}
		}
	}
}
