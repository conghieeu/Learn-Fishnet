using UnityEngine;

namespace Demo_Project
{
	public class Body : MonoBehaviour
	{
		private void Start()
		{
			SceneManager.listOfBodies.Add(base.gameObject);
		}

		private void Update()
		{
		}
	}
}
