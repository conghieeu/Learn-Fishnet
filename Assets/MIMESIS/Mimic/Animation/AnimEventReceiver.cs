using UnityEngine;

namespace Mimic.Animation
{
	public class AnimEventReceiver : MonoBehaviour
	{
		public void OnAnimationEvent(string statement)
		{
			AnimationEventHandler.Execute(base.gameObject, statement, base.gameObject.transform);
		}
	}
}
