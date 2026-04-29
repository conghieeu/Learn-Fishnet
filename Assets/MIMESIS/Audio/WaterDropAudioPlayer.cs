using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class WaterDropAudioPlayer : MonoBehaviour
{
	[SerializeField]
	protected string sfxId = "water_drop_hit";

	[SerializeField]
	protected LayerMask groundLayerMask = -1;

	[SerializeField]
	private ParticleSystem ps;

	private List<ParticleCollisionEvent> collisionEvents;

	protected int maxCollisionEventsPerFrame = 1;

	private static bool hubChecked;

	private static bool hasValidHub;

	private void Awake()
	{
		collisionEvents = new List<ParticleCollisionEvent>(maxCollisionEventsPerFrame);
		if (!hubChecked)
		{
			hasValidHub = Hub.s != null && Hub.s.audioman != null;
			hubChecked = true;
		}
	}

	private void OnParticleCollision(GameObject other)
	{
		if (hasValidHub && ((int)groundLayerMask == -1 || (((int)groundLayerMask >> other.layer) & 1) != 0) && ps.GetCollisionEvents(other, collisionEvents) > 0)
		{
			PlayDropSoundOptimized();
		}
	}

	private void PlayDropSoundOptimized()
	{
		if (!(Hub.s == null) && !(Hub.s.audioman == null))
		{
			Hub.s.audioman.PlaySfxTransform(sfxId, base.transform);
		}
	}
}
