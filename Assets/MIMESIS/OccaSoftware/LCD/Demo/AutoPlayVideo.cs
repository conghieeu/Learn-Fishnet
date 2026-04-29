using UnityEngine;
using UnityEngine.Video;

namespace OccaSoftware.LCD.Demo
{
	[ExecuteAlways]
	public class AutoPlayVideo : MonoBehaviour
	{
		public VideoPlayer player;

		private void OnEnable()
		{
			player.Play();
		}
	}
}
