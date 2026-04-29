using UnityEngine;

namespace TobyFredson
{
	public class ShowHide : MonoBehaviour
	{
		public void showIt(GameObject obj)
		{
			obj.SetActive(value: true);
		}

		public void hideIt(GameObject obj)
		{
			obj.SetActive(value: false);
		}
	}
}
