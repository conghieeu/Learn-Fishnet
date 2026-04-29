using System.Collections.Generic;
using Bifrost.SkillTargetEffectData;
using UnityEngine;

public class SkillTargetEffectDataInfo
{
	public readonly int MasterID;

	public readonly string Name = "";

	public readonly string StartParticle = "";

	public readonly string LoopParticle = "";

	public readonly int LoopParticleDurationMSec;

	public readonly string EndParticle = "";

	public readonly List<string> PlaySounds = new List<string>();

	public readonly bool StopSoundOnAbnormalEnd;

	public readonly string StartScreenEffect = "";

	public readonly string LoopScreenEffect = "";

	public readonly int LoopScreenEffectDurationMSec;

	public readonly string EndScreenEffect = "";

	public readonly int ScreenEffectLayerOrder;

	public readonly string PostProcessPath = "";

	public readonly int PostProcessDurationMSec;

	public readonly string DecalPathWithSocket = "";

	public readonly string DecalColorId = "";

	public readonly bool IsDecalRotateRandomly;

	public readonly float DecalDistanceFromSpawnPoint = 1f;

	public readonly float DecalPrintIntervalDist;

	public readonly int DecalLifetimeMSec;

	public readonly int DecalfadeoutMSec;

	public readonly float DistanceFromSpawnPosition = 1f;

	public readonly Vector3 DecalRoatation = new Vector3(0f, 0f, 0f);

	public readonly string ActorDecalColorId = "";

	public readonly int ActorDecalFadeoutMSec;

	public string animation = "";

	public bool loopAnimation;

	public SkillTargetEffectDataInfo(SkillTargetEffectData_MasterData masterData)
	{
		MasterID = masterData.id;
		if (masterData.effect_path.Count > 0)
		{
			StartParticle = masterData.effect_path[0].Trim();
			LoopParticle = masterData.effect_path[1].Trim();
			EndParticle = masterData.effect_path[2].Trim();
			LoopParticleDurationMSec = int.Parse(masterData.effect_path[3].Trim());
		}
		if (masterData.screen_effect_path.Count > 0)
		{
			StartScreenEffect = masterData.screen_effect_path[0].Trim();
			LoopScreenEffect = masterData.screen_effect_path[1].Trim();
			EndScreenEffect = masterData.screen_effect_path[2].Trim();
			LoopScreenEffectDurationMSec = int.Parse(masterData.screen_effect_path[3].Trim());
			ScreenEffectLayerOrder = int.Parse(masterData.screen_effect_path[4].Trim());
		}
		animation = masterData.animation_state_name;
		loopAnimation = masterData.animation_state_loop_type;
		masterData.sound_path.ForEach(delegate(string s)
		{
			PlaySounds.Add(s.Trim());
		});
		if (masterData.decal_path.Count > 0)
		{
			DecalPathWithSocket = masterData.decal_path[0].Trim();
			DecalColorId = masterData.decal_path[1].Trim();
			IsDecalRotateRandomly = masterData.decal_path[2].Trim() == "1";
			DecalDistanceFromSpawnPoint = float.Parse(masterData.decal_path[3].Trim());
			DecalPrintIntervalDist = float.Parse(masterData.decal_path[4].Trim());
			DecalLifetimeMSec = int.Parse(masterData.decal_path[5].Trim());
			DecalfadeoutMSec = int.Parse(masterData.decal_path[6].Trim());
			float x = float.Parse(masterData.decal_path[7].Trim());
			float y = float.Parse(masterData.decal_path[8].Trim());
			float z = float.Parse(masterData.decal_path[9].Trim());
			DecalRoatation = new Vector3(x, y, z);
		}
		if (masterData.actor_paint_material.Count > 0)
		{
			ActorDecalColorId = masterData.actor_paint_material[0].Trim();
			ActorDecalFadeoutMSec = int.Parse(masterData.actor_paint_material[1].Trim());
		}
		StopSoundOnAbnormalEnd = masterData.stop_sound_on_abnormal_end;
	}
}
