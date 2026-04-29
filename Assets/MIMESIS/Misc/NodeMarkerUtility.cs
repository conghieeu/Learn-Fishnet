using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class NodeMarkerUtility
{
	public static List<NodeMarker> CollectAllNodeMarkers(Transform rootNode, bool ignoreInactive)
	{
		return rootNode.GetComponentsInChildren<NodeMarker>(ignoreInactive).ToList();
	}

	public static List<NodeMarker> CollectAllNodeMarkersIncludeTag(Transform rootNode, string? includeTag = null, string? excludeTag = null, bool ignoreInactive = true)
	{
		return rootNode.GetComponentsInChildren<NodeMarker>(ignoreInactive).Where(delegate(NodeMarker x)
		{
			bool flag = includeTag == null || x.tags.Contains(includeTag);
			bool flag2 = excludeTag != null && x.tags.Contains(excludeTag);
			return flag && !flag2;
		}).ToList();
	}
}
