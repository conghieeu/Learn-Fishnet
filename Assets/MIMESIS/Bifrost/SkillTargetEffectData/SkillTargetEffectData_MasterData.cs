using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.SkillTargetEffectData
{
	public class SkillTargetEffectData_MasterData : ISchema
	{
		public int id;

		public List<string> effect_path = new List<string>();

		public List<string> sound_path = new List<string>();

		public bool stop_sound_on_abnormal_end;

		public List<string> screen_effect_path = new List<string>();

		public List<string> post_process_path = new List<string>();

		public List<string> decal_path = new List<string>();

		public List<string> actor_paint_material = new List<string>();

		public string animation_state_name = string.Empty;

		public bool animation_state_loop_type;

		public SkillTargetEffectData_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public SkillTargetEffectData_MasterData()
			: base(1279355320u, "SkillTargetEffectData_MasterData")
		{
		}

		public void CopyTo(SkillTargetEffectData_MasterData dest)
		{
			dest.id = id;
			dest.effect_path.Clear();
			foreach (string item in effect_path)
			{
				dest.effect_path.Add(item);
			}
			dest.sound_path.Clear();
			foreach (string item2 in sound_path)
			{
				dest.sound_path.Add(item2);
			}
			dest.stop_sound_on_abnormal_end = stop_sound_on_abnormal_end;
			dest.screen_effect_path.Clear();
			foreach (string item3 in screen_effect_path)
			{
				dest.screen_effect_path.Add(item3);
			}
			dest.post_process_path.Clear();
			foreach (string item4 in post_process_path)
			{
				dest.post_process_path.Add(item4);
			}
			dest.decal_path.Clear();
			foreach (string item5 in decal_path)
			{
				dest.decal_path.Add(item5);
			}
			dest.actor_paint_material.Clear();
			foreach (string item6 in actor_paint_material)
			{
				dest.actor_paint_material.Add(item6);
			}
			dest.animation_state_name = animation_state_name;
			dest.animation_state_loop_type = animation_state_loop_type;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			int num = 0;
			num += Serializer.GetLength(id);
			num += 4;
			foreach (string item in effect_path)
			{
				num += Serializer.GetLength(item);
			}
			num += 4;
			foreach (string item2 in sound_path)
			{
				num += Serializer.GetLength(item2);
			}
			num += Serializer.GetLength(stop_sound_on_abnormal_end);
			num += 4;
			foreach (string item3 in screen_effect_path)
			{
				num += Serializer.GetLength(item3);
			}
			num += 4;
			foreach (string item4 in post_process_path)
			{
				num += Serializer.GetLength(item4);
			}
			num += 4;
			foreach (string item5 in decal_path)
			{
				num += Serializer.GetLength(item5);
			}
			num += 4;
			foreach (string item6 in actor_paint_material)
			{
				num += Serializer.GetLength(item6);
			}
			num += Serializer.GetLength(animation_state_name);
			return num + Serializer.GetLength(animation_state_loop_type);
		}

		public override bool Load(BinaryReader br)
		{
			uint uintValue = 0u;
			Serializer.Load(br, ref uintValue);
			if (MsgID != (uint)IPAddress.NetworkToHostOrder((int)uintValue))
			{
				return false;
			}
			try
			{
				LoadInternal(br);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public void LoadInternal(BinaryReader br)
		{
			Serializer.Load(br, ref id);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				string strValue = string.Empty;
				Serializer.Load(br, ref strValue);
				effect_path.Add(strValue);
			}
			int intValue2 = 0;
			Serializer.Load(br, ref intValue2);
			while (intValue2-- > 0)
			{
				string strValue2 = string.Empty;
				Serializer.Load(br, ref strValue2);
				sound_path.Add(strValue2);
			}
			Serializer.Load(br, ref stop_sound_on_abnormal_end);
			int intValue3 = 0;
			Serializer.Load(br, ref intValue3);
			while (intValue3-- > 0)
			{
				string strValue3 = string.Empty;
				Serializer.Load(br, ref strValue3);
				screen_effect_path.Add(strValue3);
			}
			int intValue4 = 0;
			Serializer.Load(br, ref intValue4);
			while (intValue4-- > 0)
			{
				string strValue4 = string.Empty;
				Serializer.Load(br, ref strValue4);
				post_process_path.Add(strValue4);
			}
			int intValue5 = 0;
			Serializer.Load(br, ref intValue5);
			while (intValue5-- > 0)
			{
				string strValue5 = string.Empty;
				Serializer.Load(br, ref strValue5);
				decal_path.Add(strValue5);
			}
			int intValue6 = 0;
			Serializer.Load(br, ref intValue6);
			while (intValue6-- > 0)
			{
				string strValue6 = string.Empty;
				Serializer.Load(br, ref strValue6);
				actor_paint_material.Add(strValue6);
			}
			Serializer.Load(br, ref animation_state_name);
			Serializer.Load(br, ref animation_state_loop_type);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, id);
			Serializer.Save(bw, effect_path.Count);
			foreach (string item in effect_path)
			{
				Serializer.Save(bw, item);
			}
			Serializer.Save(bw, sound_path.Count);
			foreach (string item2 in sound_path)
			{
				Serializer.Save(bw, item2);
			}
			Serializer.Save(bw, stop_sound_on_abnormal_end);
			Serializer.Save(bw, screen_effect_path.Count);
			foreach (string item3 in screen_effect_path)
			{
				Serializer.Save(bw, item3);
			}
			Serializer.Save(bw, post_process_path.Count);
			foreach (string item4 in post_process_path)
			{
				Serializer.Save(bw, item4);
			}
			Serializer.Save(bw, decal_path.Count);
			foreach (string item5 in decal_path)
			{
				Serializer.Save(bw, item5);
			}
			Serializer.Save(bw, actor_paint_material.Count);
			foreach (string item6 in actor_paint_material)
			{
				Serializer.Save(bw, item6);
			}
			Serializer.Save(bw, animation_state_name);
			Serializer.Save(bw, animation_state_loop_type);
		}

		public bool Equal(SkillTargetEffectData_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (comp.effect_path.Count != effect_path.Count)
			{
				return false;
			}
			foreach (string item in effect_path)
			{
				if (!comp.effect_path.Contains(item))
				{
					return false;
				}
			}
			if (comp.sound_path.Count != sound_path.Count)
			{
				return false;
			}
			foreach (string item2 in sound_path)
			{
				if (!comp.sound_path.Contains(item2))
				{
					return false;
				}
			}
			if (stop_sound_on_abnormal_end != comp.stop_sound_on_abnormal_end)
			{
				return false;
			}
			if (comp.screen_effect_path.Count != screen_effect_path.Count)
			{
				return false;
			}
			foreach (string item3 in screen_effect_path)
			{
				if (!comp.screen_effect_path.Contains(item3))
				{
					return false;
				}
			}
			if (comp.post_process_path.Count != post_process_path.Count)
			{
				return false;
			}
			foreach (string item4 in post_process_path)
			{
				if (!comp.post_process_path.Contains(item4))
				{
					return false;
				}
			}
			if (comp.decal_path.Count != decal_path.Count)
			{
				return false;
			}
			foreach (string item5 in decal_path)
			{
				if (!comp.decal_path.Contains(item5))
				{
					return false;
				}
			}
			if (comp.actor_paint_material.Count != actor_paint_material.Count)
			{
				return false;
			}
			foreach (string item6 in actor_paint_material)
			{
				if (!comp.actor_paint_material.Contains(item6))
				{
					return false;
				}
			}
			if (animation_state_name != comp.animation_state_name)
			{
				return false;
			}
			if (animation_state_loop_type != comp.animation_state_loop_type)
			{
				return false;
			}
			return true;
		}

		public SkillTargetEffectData_MasterData Clone()
		{
			SkillTargetEffectData_MasterData skillTargetEffectData_MasterData = new SkillTargetEffectData_MasterData();
			CopyTo(skillTargetEffectData_MasterData);
			return skillTargetEffectData_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			effect_path.Clear();
			sound_path.Clear();
			stop_sound_on_abnormal_end = false;
			screen_effect_path.Clear();
			post_process_path.Clear();
			decal_path.Clear();
			actor_paint_material.Clear();
			animation_state_name = string.Empty;
			animation_state_loop_type = false;
		}
	}
}
