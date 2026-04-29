using UnityEngine;

namespace FS_Atmo
{
	public class DisableAtStart : MonoBehaviour
	{
		private void Start()
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
