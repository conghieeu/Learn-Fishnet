using UnityEngine;

namespace BzKovSoft.RagdollTemplate.Scripts.Tools
{
	public class BzSpeedController : MonoBehaviour
	{
		private float _fixedDeltaTime;

		private float _timeScale;

		private string text = "Speed:\nPress 1 - normal\nPress 2 - 1/2\nPress 3 - 1/5\nPress 4 - 1/10";

		private void Start()
		{
			_fixedDeltaTime = Time.fixedDeltaTime;
			_timeScale = Time.timeScale;
		}

		private void Update()
		{
		}

		private void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.UpperLeft;
			GUI.Label(new Rect(10f, 10f, 100f, 100f), text);
		}
	}
}
