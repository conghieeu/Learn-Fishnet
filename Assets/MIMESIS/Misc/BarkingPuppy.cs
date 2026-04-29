using DarkTonic.MasterAudio;
using MoreMountains.Feedbacks;
using ReluProtocol.Enum;
using UnityEngine;

public class BarkingPuppy : LootingLevelObject
{
	[SerializeField]
	private MMShaker _shaker;

	[SerializeField]
	private string _barkingSfx = "barking_puppy";

	[SerializeField]
	private string _destroySfx = "barking_puppy_destroy";

	private PlaySoundResult? _barkingSfxResult;

	private bool _isBarking;

	private float _soundCheckTimer;

	private const float SOUND_CHECK_INTERVAL = 0.1f;

	private void OnDestroy()
	{
		if (Hub.s != null && Hub.s.audioman != null && _barkingSfxResult != null && _barkingSfxResult.ActingVariation != null)
		{
			_barkingSfxResult.ActingVariation.Stop();
		}
	}

	public override void OnSpawn(ReasonOfSpawn reason)
	{
		base.OnSpawn(reason);
		if (reason == ReasonOfSpawn.Release)
		{
			if (_shaker != null)
			{
				_shaker.StartShaking();
			}
			_isBarking = true;
			if (Hub.s != null && Hub.s.audioman != null)
			{
				_barkingSfxResult = Hub.s.audioman.PlaySfxAtTransform(_barkingSfx, base.transform);
			}
		}
	}

	private void Update()
	{
		if (!_isBarking)
		{
			return;
		}
		_soundCheckTimer += Time.deltaTime;
		if (_soundCheckTimer >= 0.1f)
		{
			_soundCheckTimer = 0f;
			if ((_barkingSfxResult == null || _barkingSfxResult.ActingVariation == null || !_barkingSfxResult.ActingVariation.IsPlaying) && Hub.s != null && Hub.s.audioman != null)
			{
				_barkingSfxResult = Hub.s.audioman.PlaySfxAtTransform(_barkingSfx, base.transform);
			}
		}
	}

	public override void OnDespawnByDestroy()
	{
		_isBarking = false;
		if (Hub.s != null && Hub.s.audioman != null)
		{
			if (_barkingSfxResult != null && _barkingSfxResult.ActingVariation != null)
			{
				_barkingSfxResult.ActingVariation.Stop();
				_barkingSfxResult = null;
			}
			Hub.s.audioman.PlaySfxAtTransform(_destroySfx, base.transform);
		}
	}
}
