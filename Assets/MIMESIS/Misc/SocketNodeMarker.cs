using System.Collections.Generic;
using UnityEngine;

[HelpURL("https://wiki.krafton.com/x/XoCtNgE")]
public class SocketNodeMarker : MonoBehaviour
{
	public static List<Transform>? FindAll(Transform? root, string socketName)
	{
		if (root != null)
		{
			SocketNodeMarker[] componentsInChildren = root.GetComponentsInChildren<SocketNodeMarker>(includeInactive: true);
			if (componentsInChildren != null && componentsInChildren.Length != 0)
			{
				List<Transform> list = new List<Transform>();
				SocketNodeMarker[] array = componentsInChildren;
				foreach (SocketNodeMarker socketNodeMarker in array)
				{
					if (socketNodeMarker.name == socketName)
					{
						list.Add(socketNodeMarker.transform);
					}
				}
				return list;
			}
		}
		return null;
	}

	public static List<Transform>? FindAllSocketsInHierarchyByDepth(Transform? root, string socketName = "", int depth = 0)
	{
		if (root == null || depth < 0)
		{
			return null;
		}
		List<Transform> list = new List<Transform>();
		Queue<Transform> queue = new Queue<Transform>();
		Dictionary<Transform, int> dictionary = new Dictionary<Transform, int>();
		dictionary[root] = 0;
		queue.Enqueue(root);
		while (queue.Count > 0)
		{
			Transform transform = queue.Dequeue();
			int num = dictionary[transform];
			if (transform.GetComponent<SocketNodeMarker>() != null && (string.IsNullOrEmpty(socketName) || transform.name == socketName))
			{
				list.Add(transform);
			}
			if (num >= depth)
			{
				continue;
			}
			foreach (Transform item in transform)
			{
				queue.Enqueue(item);
				dictionary[item] = num + 1;
			}
		}
		if (list.Count <= 0)
		{
			return null;
		}
		return list;
	}

	public static Transform? FindFirstInHierarchyByDepth(Transform? root, string socketName = "", int depth = 0)
	{
		if (root == null || depth < 0)
		{
			return null;
		}
		List<Transform> list = FindAllSocketsInHierarchyByDepth(root, socketName, depth);
		if (list == null || list.Count == 0)
		{
			return null;
		}
		return list[0];
	}

	public static Transform? FindFirstInHierarchy(Transform? root, string socketName, bool includeInactive = true)
	{
		foreach (SocketNodeMarker item in ReLUGameKitUtility.MakeListByHierarchyOrder<SocketNodeMarker>(root, includeInactive))
		{
			if (item.name == socketName)
			{
				return item.transform;
			}
		}
		return null;
	}
}
