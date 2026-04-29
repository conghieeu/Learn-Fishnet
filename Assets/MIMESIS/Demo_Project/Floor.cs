using UnityEngine;

namespace Demo_Project
{
	public class Floor : MonoBehaviour
	{
		private void Start()
		{
			SceneManager.listOfFloors.Add(base.gameObject);
		}

		private void Update()
		{
		}
	}
}
