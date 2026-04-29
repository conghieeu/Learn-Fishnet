using MoreMountains.Feedbacks;
using UnityEngine;

namespace Mimic
{
	[RequireComponent(typeof(MMF_Player))]
	public class LootingEventFeedbackTrigger : MonoBehaviour
	{
		[SerializeField]
		private LootingEvent _targetEvent;

		public void Trigger(LootingEvent lootingEvent)
		{
			if (_targetEvent == lootingEvent && TryGetComponent<MMF_Player>(out var component))
			{
				component.PlayFeedbacks();
			}
		}
	}
}
