using UnityEngine;

namespace TobyFredson
{
	public class MenuManager : MonoBehaviour
	{
		public GameObject optionsMenu;

		public GameObject forest;

		public GameObject hills;

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.H))
			{
				bool activeSelf = optionsMenu.activeSelf;
				optionsMenu.SetActive(!activeSelf);
			}
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				bool activeSelf2 = forest.activeSelf;
				forest.SetActive(!activeSelf2);
			}
			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				bool activeSelf3 = hills.activeSelf;
				hills.SetActive(!activeSelf3);
			}
		}
	}
}
