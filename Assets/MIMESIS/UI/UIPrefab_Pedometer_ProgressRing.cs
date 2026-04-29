using UnityEngine;
using UnityEngine.UI;

public class UIPrefab_Pedometer_ProgressRing : MonoBehaviour
{
	[SerializeField]
	private Image[] ringSegments;

	[SerializeField]
	[Range(0f, 10f)]
	private int _progressVal;

	[SerializeField]
	private Color[] progressColors = new Color[5];

	private void Awake()
	{
		ResetProgress();
	}

	public void SetProgressEnabled(bool on)
	{
		if (ringSegments != null && ringSegments.Length != 0)
		{
			Image[] array = ringSegments;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = on;
			}
		}
	}

	public void SetProgress(int value)
	{
		if (ringSegments == null || ringSegments.Length == 0)
		{
			return;
		}
		_progressVal = Mathf.Clamp(value, 0, ringSegments.Length);
		Color progressColor = GetProgressColor(_progressVal);
		for (int i = 0; i < ringSegments.Length; i++)
		{
			if (i < _progressVal)
			{
				ringSegments[i].enabled = true;
				ringSegments[i].color = progressColor;
			}
			else
			{
				ringSegments[i].enabled = false;
			}
		}
	}

	private Color GetProgressColor(int value)
	{
		if (value <= 5)
		{
			return progressColors[0];
		}
		return value switch
		{
			6 => progressColors[1], 
			7 => progressColors[2], 
			8 => progressColors[3], 
			_ => progressColors[4], 
		};
	}

	public int GetMaxProgressSegmentCount()
	{
		return ringSegments.Length;
	}

	private void ChangeProgressColorToDefault()
	{
		ChangeProgressColol(in progressColors[0]);
	}

	private void ChangeProgressColol(in Color color)
	{
		if (ringSegments != null && ringSegments.Length != 0)
		{
			Image[] array = ringSegments;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].color = color;
			}
		}
	}

	private void ResetProgress()
	{
		Image[] array = ringSegments;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
	}
}
