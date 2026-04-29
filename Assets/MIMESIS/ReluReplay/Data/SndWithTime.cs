using Mimic.Voice.SpeechSystem;

namespace ReluReplay.Data
{
	public class SndWithTime
	{
		public int ActorID { get; set; }

		public long PlayerUID { get; set; } = -1L;

		public long Time { get; set; }

		public SpeechEvent SpeechEvent { get; set; }

		public byte[] CompressedSndData { get; set; }
	}
}
