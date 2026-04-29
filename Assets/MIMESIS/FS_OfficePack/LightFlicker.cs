using UnityEngine;

namespace FS_OfficePack
{
	public class LightFlicker : MonoBehaviour
	{
		public float MinLightIntensity = 0.6f;

		public float MaxLightIntensity = 1f;

		public float AccelerateTime = 0.15f;

		private float _targetIntensity = 1f;

		private float _lastIntensity = 1f;

		private float _timePassed;

		private Light _lt;

		private const double Tolerance = 0.0001;

		private void Start()
		{
			_lt = GetComponent<Light>();
			_lastIntensity = _lt.intensity;
			FixedUpdate();
		}

		private void FixedUpdate()
		{
			_timePassed += Time.deltaTime;
			_lt.intensity = Mathf.Lerp(_lastIntensity, _targetIntensity, _timePassed / AccelerateTime);
			if ((double)Mathf.Abs(_lt.intensity - _targetIntensity) < 0.0001)
			{
				_lastIntensity = _lt.intensity;
				_targetIntensity = Random.Range(MinLightIntensity, MaxLightIntensity);
				_timePassed = 0f;
			}
		}
	}
}
