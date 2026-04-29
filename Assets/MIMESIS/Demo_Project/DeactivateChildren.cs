using UnityEngine;

namespace Demo_Project
{
	public class DeactivateChildren : MonoBehaviour
	{
		private void Start()
		{
			_ = base.transform.childCount;
			for (int i = 0; i < base.transform.childCount; i++)
			{
				base.transform.GetChild(i).gameObject.SetActive(value: false);
			}
		}

		private void Update()
		{
		}
	}
}
