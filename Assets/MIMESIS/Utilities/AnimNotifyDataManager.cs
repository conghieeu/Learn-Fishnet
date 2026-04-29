using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReluProtocol;
using UnityEngine;

public class AnimNotifyDataManager
{
	public class ProtoActorAnimInfo
	{
		public readonly string Key;

		public readonly string Type;

		public ProtoActorAnimInfo(string key, string type)
		{
			Key = key;
			Type = type;
		}
	}

	public class ProjectileProtoActorAnimInfo : ProtoActorAnimInfo
	{
		public readonly Vector3 Offset;

		public readonly Vector3 Direction;

		public ProjectileProtoActorAnimInfo(string key, string type, Vector3 offset, Vector3 direction)
			: base(key, type)
		{
			Offset = offset;
			Direction = direction;
		}
	}

	private class ActorAnimRawData
	{
		public string actorPrefabName { get; set; }

		public List<MotionAnimData> allmotionsAnimEvent { get; set; }
	}

	public class EyeRaycastSocketInfo
	{
		public string SocketName { get; set; }

		public Vector3 Position { get; set; }

		public Vector3 Rotation { get; set; }
	}

	public class CommonInfo
	{
		public IHitCheck HurtBox { get; set; }

		public EyeRaycastSocketInfo EyeRaycastSocket { get; set; }
	}

	public class ProtoActorInfo
	{
		public ImmutableDictionary<string, ProjectileProtoActorAnimInfo> ProjectileInfos { get; set; }

		public ImmutableDictionary<string, IHitCheck> HitBoxInfos { get; set; }
	}

	public class PuppetInfo
	{
		public CommonInfo CommonInfo { get; set; }

		public ImmutableDictionary<string, ProjectileProtoActorAnimInfo> ProjectileInfos { get; set; }

		public ImmutableDictionary<string, IHitCheck> HitBoxInfos { get; set; }
	}

	private class MotionAnimData
	{
		public string motion;

		public float duration;

		public List<AnimEvent> animEvents;
	}

	private class AnimEvent
	{
		public double timingInSec;

		public string eventString;
	}

	private ResourceDataHandler? _dataHandler;

	private ImmutableDictionary<string, ActorAnimInfo> _actorAnimInfos;

	private ProtoActorInfo _protoActorInfo;

	private ImmutableDictionary<string, PuppetInfo> _puppetInfos;

	public AnimNotifyDataManager(ResourceDataHandler dataHandler)
	{
		_dataHandler = dataHandler;
	}

	private AnimNotifyDataManager()
	{
		_actorAnimInfos = ImmutableDictionary<string, ActorAnimInfo>.Empty;
	}

	public bool LoadAnimNotify()
	{
		string[] files = _dataHandler.GetFiles("anim/");
		if (files == null)
		{
			Logger.RError("AnimNotiData Load Failed : No Data");
			return false;
		}
		if (files.Length == 0)
		{
			Logger.RError("AnimNotiData Load Failed : No Data");
			return false;
		}
		foreach (string item in files.Where((string x) => x.EndsWith(".json")))
		{
			try
			{
				using Stream stream = _dataHandler.GetStream("anim//" + item);
				if (stream == null)
				{
					Logger.RError("AnimNotiData Load Failed : Stream is null for " + item);
					return false;
				}
				if (item.Equals("AnimationEvents.json", StringComparison.OrdinalIgnoreCase))
				{
					if (!LoadAnimEvent(stream))
					{
						Logger.RError("AnimNotiData Load Failed : Failed to load animation events from " + item);
						return false;
					}
					continue;
				}
				if (item.Equals("ProtoActorAnimInfo.json", StringComparison.OrdinalIgnoreCase))
				{
					if (!LoadProtoActorAnimInfo(stream))
					{
						Logger.RError("AnimNotiData Load Failed : Failed to load proto actor anim info from " + item);
						return false;
					}
					continue;
				}
				Logger.RError("AnimNotiData Load Failed : Unknown file " + item);
				return false;
			}
			catch (Exception ex)
			{
				Logger.RError("Failed to parse anim notify data from " + item + ": " + ex.Message);
				return false;
			}
		}
		return true;
	}

	private bool LoadProtoActorAnimInfo(Stream stream)
	{
		try
		{
			ImmutableDictionary<string, PuppetInfo>.Builder builder = ImmutableDictionary.CreateBuilder<string, PuppetInfo>();
			using (StreamReader streamReader = new StreamReader(stream))
			{
				JObject jObject = JObject.Parse(streamReader.ReadToEnd());
				if (jObject["protoActorInfo"] is JObject jObject2)
				{
					ImmutableDictionary<string, ProjectileProtoActorAnimInfo>.Builder builder2 = ImmutableDictionary.CreateBuilder<string, ProjectileProtoActorAnimInfo>();
					ImmutableDictionary<string, IHitCheck>.Builder builder3 = ImmutableDictionary.CreateBuilder<string, IHitCheck>();
					if (jObject2["SpecificInfo"] is JArray jArray)
					{
						foreach (JToken item in jArray)
						{
							ProcessSpecificInfoItem(item, builder2, builder3);
						}
					}
					_protoActorInfo = new ProtoActorInfo
					{
						ProjectileInfos = builder2.ToImmutable(),
						HitBoxInfos = builder3.ToImmutable()
					};
				}
				if (jObject["puppets"] is JObject jObject3)
				{
					foreach (KeyValuePair<string, JToken> item2 in jObject3)
					{
						string key = item2.Key;
						if (!(item2.Value is JObject jObject4))
						{
							continue;
						}
						CommonInfo commonInfo = null;
						if (jObject4["CommonInfo"] is JObject commonInfoObj)
						{
							commonInfo = ParseCommonInfo(commonInfoObj);
						}
						ImmutableDictionary<string, ProjectileProtoActorAnimInfo>.Builder builder4 = ImmutableDictionary.CreateBuilder<string, ProjectileProtoActorAnimInfo>();
						ImmutableDictionary<string, IHitCheck>.Builder builder5 = ImmutableDictionary.CreateBuilder<string, IHitCheck>();
						if (jObject4["SpecificInfo"] is JArray jArray2)
						{
							foreach (JToken item3 in jArray2)
							{
								ProcessSpecificInfoItem(item3, builder4, builder5);
							}
						}
						PuppetInfo value = new PuppetInfo
						{
							CommonInfo = commonInfo,
							ProjectileInfos = builder4.ToImmutable(),
							HitBoxInfos = builder5.ToImmutable()
						};
						builder[key] = value;
					}
				}
			}
			_puppetInfos = builder.ToImmutable();
		}
		catch (Exception arg)
		{
			Logger.RError($"Failed to parse proto actor anim info: {arg}");
			return false;
		}
		return true;
	}

	private CommonInfo ParseCommonInfo(JObject commonInfoObj)
	{
		CommonInfo commonInfo = new CommonInfo();
		if (commonInfoObj["hurtbox"] is JObject jObject)
		{
			float[] array = jObject["center"]?.ToObject<float[]>();
			float[] array2 = jObject["rotation"]?.ToObject<float[]>();
			string text = jObject["type"]?.ToString();
			HitCheckShapeType hitCheckShapeType = HitCheckShapeType.Invalid;
			if (text.Equals("SphereHitbox"))
			{
				hitCheckShapeType = HitCheckShapeType.Sphere;
			}
			else if (text.Equals("CubeHitbox"))
			{
				hitCheckShapeType = HitCheckShapeType.Cube;
			}
			else if (text.Equals("FanHitbox"))
			{
				hitCheckShapeType = HitCheckShapeType.Fan;
			}
			else if (text.Equals("CapsuleHitBox"))
			{
				hitCheckShapeType = HitCheckShapeType.Capsule;
			}
			switch (hitCheckShapeType)
			{
			case HitCheckShapeType.Sphere:
				commonInfo.HurtBox = new SphereHitCheck((array != null && array.Length >= 3) ? new Vector3(array[0], array[1], array[2]) : Vector3.zero, jObject["radius"]?.ToObject<float>() ?? 0f, jObject["key"]?.ToString() ?? "default_hurtbox");
				break;
			case HitCheckShapeType.Cube:
				commonInfo.HurtBox = new CubeHitCheck((array != null && array.Length >= 3) ? new Vector3(array[0], array[1], array[2]) : Vector3.zero, (array2 != null && array2.Length >= 3) ? new Vector3(array2[0], array2[1], array2[2]) : Vector3.zero, (jObject["extent"]?.ToObject<float[]>() != null && jObject["extent"].ToObject<float[]>().Length >= 3) ? new Vector3(jObject["extent"].ToObject<float[]>()[0], jObject["extent"].ToObject<float[]>()[1], jObject["extent"].ToObject<float[]>()[2]) : Vector3.zero, jObject["key"]?.ToString() ?? "default_hurtbox");
				break;
			case HitCheckShapeType.Fan:
				commonInfo.HurtBox = new FanHitCheck((array != null && array.Length >= 3) ? new Vector3(array[0], array[1], array[2]) : Vector3.zero, (array2 != null && array2.Length >= 3) ? new Vector3(array2[0], array2[1], array2[2]) : Vector3.zero, jObject["height"]?.ToObject<float>() ?? 0f, jObject["innerradius"]?.ToObject<float>() ?? 0f, jObject["outerradius"]?.ToObject<float>() ?? 0f, jObject["angle"]?.ToObject<float>() ?? 0f, jObject["key"]?.ToString() ?? "default_hurtbox");
				break;
			case HitCheckShapeType.Capsule:
				commonInfo.HurtBox = new CapsuleHitCheck(jObject["radius"]?.ToObject<float>() ?? 0f, jObject["height"]?.ToObject<float>() ?? 0f, (array != null && array.Length >= 3) ? new Vector3(array[0], array[1], array[2]) : Vector3.zero, (array2 != null && array2.Length >= 3) ? new Vector3(array2[0], array2[1], array2[2]) : Vector3.zero, jObject["key"]?.ToString() ?? "default_hurtbox");
				break;
			default:
				commonInfo.HurtBox = null;
				break;
			}
		}
		if (commonInfoObj["eyeRaycastSocket"] is JObject jObject2)
		{
			float[] array3 = jObject2["position"]?.ToObject<float[]>();
			float[] array4 = jObject2["rotation"]?.ToObject<float[]>();
			commonInfo.EyeRaycastSocket = new EyeRaycastSocketInfo
			{
				SocketName = jObject2["socketName"]?.ToString(),
				Position = ((array3 != null && array3.Length >= 3) ? new Vector3(array3[0], array3[1], array3[2]) : Vector3.zero),
				Rotation = ((array4 != null && array4.Length >= 3) ? new Vector3(array4[0], array4[1], array4[2]) : Vector3.zero)
			};
		}
		return commonInfo;
	}

	private void ProcessSpecificInfoItem(JToken item, IDictionary<string, ProjectileProtoActorAnimInfo> projectileBuilder, IDictionary<string, IHitCheck> hitBoxBuilder)
	{
		string text = item["type"]?.ToString();
		string text2 = item["key"]?.ToString();
		if (string.IsNullOrEmpty(text2) || string.IsNullOrEmpty(text))
		{
			return;
		}
		if (text == "projectile")
		{
			float[] array = item["offset"]?.ToObject<float[]>();
			float[] array2 = item["direction"]?.ToObject<float[]>();
			ProjectileProtoActorAnimInfo value = new ProjectileProtoActorAnimInfo(text2, text, (array != null && array.Length >= 3) ? new Vector3(array[0], array[1], array[2]) : Vector3.zero, (array2 != null && array2.Length >= 3) ? new Vector3(array2[0], array2[1], array2[2]) : Vector3.zero);
			projectileBuilder[text2] = value;
		}
		else if (text == "hitbox")
		{
			float[] array3 = item["center"]?.ToObject<float[]>();
			string text3 = item["hitboxtype"]?.ToString();
			HitCheckShapeType hitCheckShapeType = HitCheckShapeType.Invalid;
			if (text3.Equals("SphereHitbox"))
			{
				hitCheckShapeType = HitCheckShapeType.Sphere;
			}
			else if (text3.Equals("CubeHitbox"))
			{
				hitCheckShapeType = HitCheckShapeType.Cube;
			}
			else if (text3.Equals("FanHitbox"))
			{
				hitCheckShapeType = HitCheckShapeType.Fan;
			}
			switch (hitCheckShapeType)
			{
			case HitCheckShapeType.Sphere:
			{
				SphereHitCheck value4 = new SphereHitCheck((array3 != null && array3.Length >= 3) ? new Vector3(array3[0], array3[1], array3[2]) : Vector3.zero, item["radius"]?.ToObject<float>() ?? 0f, text2);
				hitBoxBuilder[text2] = value4;
				break;
			}
			case HitCheckShapeType.Cube:
			{
				float[] array5 = item["rotation"]?.ToObject<float[]>();
				float[] array6 = item["extent"]?.ToObject<float[]>();
				CubeHitCheck value3 = new CubeHitCheck((array3 != null && array3.Length >= 3) ? new Vector3(array3[0], array3[1], array3[2]) : Vector3.zero, (array6 != null && array6.Length >= 3) ? new Vector3(array6[0], array6[1], array6[2]) : Vector3.zero, (array5 != null && array5.Length >= 3) ? new Vector3(array5[0], array5[1], array5[2]) : Vector3.zero, text2);
				hitBoxBuilder[text2] = value3;
				break;
			}
			case HitCheckShapeType.Fan:
			{
				float[] array4 = item["rotation"]?.ToObject<float[]>();
				FanHitCheck value2 = new FanHitCheck((array3 != null && array3.Length >= 3) ? new Vector3(array3[0], array3[1], array3[2]) : Vector3.zero, (array4 != null && array4.Length >= 3) ? new Vector3(array4[0], array4[1], array4[2]) : Vector3.zero, item["height"]?.ToObject<float>() ?? 0f, item["innerradius"]?.ToObject<float>() ?? 0f, item["outerradius"]?.ToObject<float>() ?? 0f, item["angle"]?.ToObject<float>() ?? 0f, text2);
				hitBoxBuilder[text2] = value2;
				break;
			}
			case HitCheckShapeType.Capsule:
				break;
			}
		}
	}

	private bool LoadAnimEvent(Stream stream)
	{
		ImmutableDictionary<string, ActorAnimInfo>.Builder builder = ImmutableDictionary.CreateBuilder<string, ActorAnimInfo>();
		using (StreamReader streamReader = new StreamReader(stream))
		{
			List<ActorAnimRawData> list = JsonConvert.DeserializeObject<List<ActorAnimRawData>>(streamReader.ReadToEnd());
			if (list == null)
			{
				Logger.RError("AnimNotiData Load Failed : No Data in JSON");
				return false;
			}
			foreach (ActorAnimRawData item in list)
			{
				string actorPrefabName = item.actorPrefabName;
				if (string.IsNullOrEmpty(actorPrefabName))
				{
					return false;
				}
				ImmutableDictionary<string, SkillAnimInfo>.Builder builder2 = ImmutableDictionary.CreateBuilder<string, SkillAnimInfo>();
				foreach (MotionAnimData item2 in item.allmotionsAnimEvent)
				{
					string motion = item2.motion;
					if (string.IsNullOrEmpty(motion))
					{
						Logger.RError("Motion name is empty for prefab '" + actorPrefabName + "'");
						return false;
					}
					SkillAnimInfoBuilder skillAnimInfoBuilder = new SkillAnimInfoBuilder(HashCode.Combine(actorPrefabName, motion), item2.duration);
					if (item2.animEvents != null)
					{
						foreach (AnimEvent animEvent in item2.animEvents)
						{
							if (!ProcessAnimEvent(skillAnimInfoBuilder, animEvent))
							{
								return false;
							}
						}
					}
					skillAnimInfoBuilder.Sort();
					if (!builder2.ContainsKey(motion))
					{
						builder2.Add(motion, new SkillAnimInfo(skillAnimInfoBuilder));
						continue;
					}
					Debug.LogWarning("Duplicate animation data found for " + actorPrefabName + "." + motion + ", skipping.");
				}
				if (builder.ContainsKey(actorPrefabName))
				{
					Debug.LogWarning("Duplicate actor animation data found for prefab '" + actorPrefabName + "', overwriting existing data.");
				}
				builder[actorPrefabName] = new ActorAnimInfo(actorPrefabName, builder2.ToImmutableDictionary());
			}
		}
		_actorAnimInfos = builder.ToImmutable();
		return true;
	}

	private bool ProcessAnimEvent(SkillAnimInfoBuilder builder, AnimEvent animEvent)
	{
		string eventString = animEvent.eventString;
		double timingInSec = animEvent.timingInSec;
		string[] array = eventString.Split('@');
		string obj = array[0];
		string text = ((array.Length > 1) ? array[1] : string.Empty);
		string[] array2 = obj.Split(':');
		string text2 = array2[0];
		try
		{
			switch (text2)
			{
			case "hitbox":
			{
				if (!double.TryParse(array2[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var result5))
				{
					Debug.LogError("Invalid hitbox duration in event '" + eventString + "'");
					return false;
				}
				if (!int.TryParse(array2[2], out var result6))
				{
					Debug.LogError("Invalid hitbox sequence index in event '" + eventString + "'");
					return false;
				}
				builder.HitChecks.Add(new AnimNotifyHitCheck(eventString, timingInSec, result5, text, result6));
				break;
			}
			case "projectile":
			{
				if (!int.TryParse(array2[1], out var result4))
				{
					Debug.LogError("Invalid projectile sequence index in event '" + eventString + "'");
					return false;
				}
				string socketName = text;
				builder.Projectiles.Add(new AnimNotifyProjectile(timingInSec, eventString, result4, socketName));
				break;
			}
			case "fieldskill":
			{
				if (!int.TryParse(array2[1], out var result2))
				{
					Debug.LogError("Invalid field skill sequence index in event '" + eventString + "'");
					return false;
				}
				builder.FieldSkills.Add(new AnimNotifyFieldSkill(timingInSec, eventString, result2));
				break;
			}
			case "reload":
				builder.ReloadWeapons.Add(new AnimNotifyReloadWeapon(timingInSec, eventString));
				break;
			case "destroyitem":
				builder.WeaponDestroyes.Add(new AnimNotifyDestroyWeapon(timingInSec, eventString));
				break;
			case "immune":
			{
				if (!bool.TryParse(array2[1], out var result3))
				{
					Debug.LogError("Invalid immune flag in event '" + eventString + "'");
					return false;
				}
				builder.ImmuneAppliers.Add(new AnimNotifyImmuneApplier(timingInSec, eventString, result3));
				break;
			}
			case "aura":
			{
				if (!int.TryParse(array2[1], out var result))
				{
					Debug.LogError("Invalid aura master id in event '" + eventString + "'");
					return false;
				}
				builder.ActivateAuras.Add(new AnimNotifyActivateAura(timingInSec, eventString, result));
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to process anim event '" + eventString + "': " + ex.Message);
			return false;
		}
		return true;
	}

	public SkillAnimInfo? GetSkillAnimInfo(string prefabID, string motionID)
	{
		if (!_actorAnimInfos.TryGetValue(prefabID, out ActorAnimInfo value))
		{
			return null;
		}
		if (!value.SkillAnims.TryGetValue(motionID, out SkillAnimInfo value2))
		{
			return null;
		}
		return value2;
	}

	public IHitCheck? GetHurtBox(string prefabID)
	{
		if (_puppetInfos != null && _puppetInfos.TryGetValue(prefabID, out PuppetInfo value))
		{
			return value.CommonInfo?.HurtBox;
		}
		return null;
	}

	public IHitCheck? GetHitBox(string prefabName, string key)
	{
		if (_puppetInfos != null && _puppetInfos.TryGetValue(prefabName, out PuppetInfo value) && value.HitBoxInfos.TryGetValue(key, out IHitCheck value2))
		{
			return value2;
		}
		return null;
	}

	public (Vector3, Vector3)? GetProjectilePosInfo(string prefabName, string key)
	{
		if (_puppetInfos != null && _puppetInfos.TryGetValue(prefabName, out PuppetInfo value) && value.ProjectileInfos.TryGetValue(key, out ProjectileProtoActorAnimInfo value2))
		{
			return (value2.Offset, value2.Direction);
		}
		if (_protoActorInfo != null && _protoActorInfo.ProjectileInfos.TryGetValue(key, out ProjectileProtoActorAnimInfo value3))
		{
			return (value3.Offset, value3.Direction);
		}
		return null;
	}

	public PosWithRot? GetEyeRaycastPos(string prefabID)
	{
		if (_puppetInfos != null && _puppetInfos.TryGetValue(prefabID, out PuppetInfo value))
		{
			EyeRaycastSocketInfo eyeRaycastSocketInfo = value.CommonInfo?.EyeRaycastSocket;
			if (eyeRaycastSocketInfo != null)
			{
				return new PosWithRot
				{
					pos = eyeRaycastSocketInfo.Position,
					rot = eyeRaycastSocketInfo.Rotation
				};
			}
		}
		return null;
	}
}
