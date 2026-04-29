using System.Collections.Generic;
using Mimic.Actors;
using Mimic.Voice.SpeechSystem;

namespace DLAgent
{
	public class DLHelper
	{
		private ProtoActor? avatar;

		private ProtoActor? targetActor;

		private Dictionary<int, ProtoActor> _protoActorsRef;

		private bool _isExtractingTrainingData;

		public bool ShowVoiceType { get; private set; }

		public void OnOwnerPlayerSpawn(Dictionary<int, ProtoActor> protoActorsRef, int ownerActorID)
		{
		}

		public void OnOwnerPlayerDespawn()
		{
		}

		public void RecordVoiceFile(long PlayerUID, SpeechEvent speechEvent)
		{
			if (!(Hub.s == null) && Hub.s.replayManager.IsRecordMode())
			{
				ProtoActor protoActor = Hub.s.pdata.main?.GetActorByPlayerUID(PlayerUID);
				if ((object)protoActor != null)
				{
					Hub.s.replayManager.OnRecordVoiceFile(protoActor, speechEvent);
				}
			}
		}
	}
}
