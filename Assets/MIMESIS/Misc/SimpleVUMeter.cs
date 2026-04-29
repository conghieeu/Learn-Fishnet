using UnityEngine;

public class SimpleVUMeter
{
	private float _smooth;

	private float _maxVolume = 0.01f;

	private float _peak;

	public float Peak => _peak;

	private void Clear()
	{
		_smooth = 0f;
		_maxVolume = 0f;
		_peak = 0f;
	}

	public void Update(float amplitude, float deltaTimeSec, bool clear)
	{
		_smooth = amplitude * 0.3f + _smooth * 0.7f;
		_maxVolume = Mathf.Max(_smooth, _maxVolume);
		if (clear)
		{
			Clear();
		}
		if (_smooth > _peak)
		{
			_peak = _smooth;
		}
		else
		{
			_peak -= deltaTimeSec * _maxVolume * 0.5f;
		}
		_peak = Mathf.Clamp01(_peak);
	}
}
