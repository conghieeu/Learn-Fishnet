using UnityEngine;

namespace SpriteShadersUltimate.Demo
{
	public class Demo_Camera : MonoBehaviour
	{
		private void LateUpdate()
		{
			Vector3 position = base.transform.position;
			position.x = Mathf.Lerp(position.x, Demo_Player.instance.transform.position.x, Time.deltaTime * 3f);
			base.transform.position = position;
		}
	}
}
