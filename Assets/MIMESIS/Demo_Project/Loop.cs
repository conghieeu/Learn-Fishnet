using UnityEngine;

namespace Demo_Project
{
	public class Loop : MonoBehaviour
	{
		public Vector3 loopPosition = new Vector3(0f, 0f, 0f);

		private void Start()
		{
			SceneManager.listOfLoops.Add(base.gameObject);
		}

		private void Update()
		{
		}
	}
}
