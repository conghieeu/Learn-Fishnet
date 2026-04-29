using DarkTonic.MasterAudio;
using UnityEngine;

namespace Mimic.Audio
{
	[CreateAssetMenu(fileName = "UISfxTable", menuName = "_Mimic/Audio/UISfxTable")]
	public class UISfxTable : ScriptableObject
	{
		public string uiBusName = "UI";

		public DynamicSoundGroupCreator uiSoundGroupCreator;

		[Header("SFX Keys")]
		public string titleSfxId = string.Empty;
	}
}
