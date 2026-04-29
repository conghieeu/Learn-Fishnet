public class ElectricFlyswatter : SocketAudioAttachable, IGaugeableItem
{
	public void OnGaugeChanged(long itemID, int remainGauge)
	{
		if (remainGauge <= 0)
		{
			OnDetachFromSocket();
		}
		else if (_sfxResult != null && _sfxResult.ActingVariation != null && !_sfxResult.ActingVariation.IsPlaying && !_sfxResult.ActingVariation.IsStopRequested)
		{
			OnAttachToSocket();
		}
	}
}
