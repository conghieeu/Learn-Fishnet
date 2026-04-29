using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[HelpURL("https://wiki.krafton.com/x/J4RPOQE")]
public class NodeMarker : MonoBehaviour
{
	public List<string> tags;

	private void Awake()
	{
		if (tags != null)
		{
			tags = tags.Select((string x) => x.Trim()).ToList();
		}
	}
}
