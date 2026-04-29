using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UIPrefab_Pedometer : MonoBehaviour
{
	[SerializeField]
	private UIPrefab_Pedometer_Numbers numbers;

	[SerializeField]
	private UIPrefab_Pedometer_ProgressRing progressRing;

	[SerializeField]
	[Range(0f, 9999f)]
	private int _number;

	[SerializeField]
	private float blinkDuration = 0.5f;

	private bool _isBlinking;

	private CancellationTokenSource _blinkCts;

	public bool IsBlinking => _isBlinking;

	public void SetNumber(int number, int maxNumber)
	{
		if (!(numbers == null) && !(progressRing == null) && number >= 0 && number <= maxNumber)
		{
			if (number == 9999)
			{
				numbers.SetError();
			}
			else
			{
				numbers.SetNumber(number);
			}
			int maxProgressSegmentCount = progressRing.GetMaxProgressSegmentCount();
			int progress = Mathf.RoundToInt((float)number / (float)maxNumber * (float)maxProgressSegmentCount);
			progressRing.SetProgress(progress);
			_number = number;
		}
	}

	public void StartBlinking()
	{
		if (!_isBlinking)
		{
			_isBlinking = true;
			_blinkCts = new CancellationTokenSource();
			BlinkAsync(_blinkCts.Token).Forget();
		}
	}

	public void StopBlinking()
	{
		if (_isBlinking)
		{
			_isBlinking = false;
			_blinkCts?.Cancel();
			_blinkCts?.Dispose();
			_blinkCts = null;
			progressRing?.SetProgressEnabled(on: true);
			numbers?.SetNumberEnabld(on: true);
		}
	}

	private async UniTask BlinkAsync(CancellationToken cancellationToken)
	{
		bool turnOn = true;
		while (!cancellationToken.IsCancellationRequested && progressRing != null)
		{
			turnOn = !turnOn;
			progressRing.SetProgressEnabled(turnOn);
			numbers.SetNumberEnabld(turnOn);
			await UniTask.WaitForSeconds(blinkDuration, ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken, cancelImmediately: true).SuppressCancellationThrow();
		}
	}
}
