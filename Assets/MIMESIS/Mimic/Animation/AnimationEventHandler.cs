using System;
using System.Globalization;
using Mimic.Actors;
using MoreMountains.Feedbacks;
using Unity.Cinemachine;
using UnityEngine;

namespace Mimic.Animation
{
	public static class AnimationEventHandler
	{
		private const char socketSeparator = '@';

		private const char parameterSeparator = ':';

		private static Transform GetSoket(string eventType, ProtoActor? actor, Transform socketRoot, string socketName, PuppetScript? puppet)
		{
			if (eventType == "hitbox")
			{
				if (puppet != null && puppet.transform == socketRoot)
				{
					return SocketNodeMarker.FindFirstInHierarchyByDepth(socketRoot, socketName, 1);
				}
				if (actor != null && actor.transform == socketRoot)
				{
					return SocketNodeMarker.FindFirstInHierarchyByDepth(socketRoot, socketName, 1);
				}
			}
			return SocketNodeMarker.FindFirstInHierarchy(socketRoot, socketName);
		}

		public static void Execute(GameObject receiver, string statement, Transform socketRoot, ProtoActor? actor = null, PuppetScript? puppet = null)
		{
			try
			{
				ExecuteInternal(statement, socketRoot, actor, puppet);
			}
			catch
			{
				Logger.RWarn("Failed to execute an animation event: statement=" + statement + ", receiver=" + receiver.name, sendToLogServer: false, useConsoleOut: true, "animevent");
			}
		}

		private static void ExecuteInternal(string statement, Transform socketRoot, ProtoActor? actor = null, PuppetScript? puppet = null)
		{
			if (Hub.s == null)
			{
				return;
			}
			if (!TryParse(statement, out var eventType, out var parameters, out var socketName, out var useWorldPosition))
			{
				Logger.RError("Invalid animation event: '" + statement + "'");
				return;
			}
			Transform transform = null;
			if (socketName != null)
			{
				transform = GetSoket(eventType, actor, socketRoot, socketName, puppet);
			}
			switch (eventType)
			{
			case "sound":
				if (IsValid(statement, parameters, 1, transform, requiresSocket: true, actor, requiresActor: false))
				{
					if (!transform.TryGetComponent<AudioSource>(out var component))
					{
						Logger.RError("AudioSource not found: " + statement);
						break;
					}
					string clipId = parameters[0];
					Hub.s.legacyAudio.Play(clipId, component);
				}
				break;
			case "sfx":
				if (!IsValid(statement, parameters, 1, transform, requiresSocket: false, actor, requiresActor: false))
				{
					break;
				}
				if (transform == null && actor == null)
				{
					if (socketRoot == null)
					{
						break;
					}
					transform = socketRoot;
				}
				if (parameters[0] == "stop")
				{
					if (parameters.Length > 1)
					{
						string sfxId = parameters[1];
						if (transform != null)
						{
							Hub.s.audioman.StopSfxTransform(sfxId, transform);
						}
						else if (actor != null)
						{
							Hub.s.audioman.StopSfxTransform(sfxId, actor.SfxRoot);
						}
					}
					else if (transform != null)
					{
						Hub.s.audioman.StopSfxTransform(transform);
					}
					else if (actor != null)
					{
						Hub.s.audioman.StopSfxTransform(actor.SfxRoot);
					}
				}
				else
				{
					string sfxId2 = parameters[0];
					if (actor != null)
					{
						Hub.s.audioman.PlaySfxAtTransform(sfxId2, actor.SfxRoot);
					}
					else
					{
						Hub.s.audioman.PlaySfxAtTransform(sfxId2, transform);
					}
				}
				break;
			case "vfx":
				if (IsValid(statement, parameters, 1, transform, requiresSocket: true, actor, requiresActor: false))
				{
					string key = parameters[0];
					if (useWorldPosition)
					{
						Hub.s.vfxman.InstantiateVfx(key, transform.position);
					}
					else
					{
						Hub.s.vfxman.InstantiateVfx(key, transform);
					}
				}
				break;
			case "screenFx":
			{
				if (!IsValid(statement, parameters, 2, transform, requiresSocket: false, actor, requiresActor: false))
				{
					break;
				}
				BlinkAnimation blinkAnimation = Hub.s.uiman.GetBlinkAnimation();
				if (!(blinkAnimation == null) && parameters[0] == "blink")
				{
					switch (parameters[1])
					{
					case "open":
						blinkAnimation.OpenEye();
						break;
					case "close":
						blinkAnimation.CloseEye();
						break;
					case "opened":
						blinkAnimation.OpenEyeImmediate();
						break;
					case "closed":
						blinkAnimation.CloseEyeImmediate();
						break;
					}
				}
				break;
			}
			case "feedback":
				if (IsValid(statement, parameters, 0, transform, requiresSocket: true, actor, requiresActor: false))
				{
					if (!transform.TryGetComponent<MMF_Player>(out var component3))
					{
						Logger.RError("MMF_Player not found: " + statement);
					}
					else
					{
						component3.PlayFeedbacks();
					}
				}
				break;
			case "ragDollOn":
				if (IsValid(statement, parameters, 0, transform, requiresSocket: false, actor, requiresActor: true))
				{
					actor.ActivateRagDoll(Vector3.zero);
				}
				break;
			case "camera":
			{
				if (actor == null || !actor.AmIAvatar() || !IsValid(statement, parameters, 2, transform, requiresSocket: true, actor, requiresActor: true))
				{
					break;
				}
				if (!transform.TryGetComponent<CinemachineCamera>(out var component2))
				{
					Logger.RError("CinemachineCamera not found: " + statement);
					break;
				}
				if (!TryParse(parameters[0], out float result2))
				{
					Logger.RError("Invalid duration: " + statement);
					break;
				}
				if (!TryParse(parameters[1], out float result3))
				{
					Logger.RError("Invalid blendTime: " + statement);
					break;
				}
				float result4 = 0f;
				if (parameters.Length >= 3)
				{
					TryParse(parameters[2], out result4);
				}
				Hub.s.cameraman.BlendTo(component2, result2, result3, result4);
				break;
			}
			case "mat":
				if (actor == null)
				{
					break;
				}
				if (parameters[0] == "turnon_dissolve")
				{
					actor?.TurnOnMaterialDissolve();
				}
				else if (parameters[0] == "turnoff_dissolve")
				{
					actor?.TurnOffMaterialDissolve();
				}
				else if (parameters[0] == "turnon_paint")
				{
					Color color = Color.white;
					if (parameters.Length > 1 && ColorUtility.TryParseHtmlString(parameters[1], out color))
					{
						actor?.TurnOnMaterialPaint(color);
					}
				}
				else if (parameters[0] == "turnoff_paint")
				{
					float vanishingAnimationDuration = 0.1f;
					if (parameters.Length > 1 && TryParse(parameters[1], out float result))
					{
						vanishingAnimationDuration = result;
					}
					actor?.TurnOffMaterialPaint(vanishingAnimationDuration);
				}
				else
				{
					Logger.RError("Unknown mat event: " + parameters[0] + " in " + statement + ". Only 'turnon_dissolve' and 'turnoff_dissolve' are supported.");
				}
				break;
			default:
				Logger.RWarn("Unknown animation event: " + statement, sendToLogServer: false, useConsoleOut: true, "animevent");
				break;
			case "hitbox":
				break;
			case "projectile":
				break;
			}
		}

		private static bool IsValid(string statement, string[] parameters, int minRequiresParameters, Transform socket, bool requiresSocket, ProtoActor actor, bool requiresActor)
		{
			if (parameters.Length < minRequiresParameters)
			{
				Logger.RError("Insufficient number of parameters: " + statement);
				return false;
			}
			if (requiresActor && actor == null)
			{
				Logger.RError("Actor not found: " + statement);
				return false;
			}
			return true;
		}

		private static bool TryParse(string statement, out string eventType, out string[] parameters, out string socketName, out bool useWorldPosition)
		{
			eventType = string.Empty;
			parameters = Array.Empty<string>();
			socketName = string.Empty;
			useWorldPosition = false;
			if (string.IsNullOrWhiteSpace(statement))
			{
				return false;
			}
			string[] array = statement.Split('@');
			if (array != null && array.Length != 0)
			{
				string[] array2 = array[0].Split(':');
				if (array2 != null && array2.Length != 0)
				{
					eventType = array2[0];
					int num = array2.Length - 1;
					if (num > 0)
					{
						parameters = new string[num];
						Array.Copy(array2, 1, parameters, 0, num);
					}
					if (array.Length > 1)
					{
						string text = array[1];
						string[] array3 = text.Split(':');
						if (array3.Length == 1)
						{
							socketName = text;
						}
						else if (array3.Length == 2)
						{
							socketName = array3[0];
							useWorldPosition = array3[1].Equals("w");
						}
						else
						{
							Logger.RError($"'{statement}' has more than two separator({':'}). Only two of them will be used.");
						}
					}
					if (array.Length > 2)
					{
						Logger.RError($"'{statement}' has more than one separator({'@'}). Only the first one will be used.");
					}
					return true;
				}
			}
			return false;
		}

		private static bool TryParse(string s, out float result)
		{
			return float.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
		}

		private static bool TryParse(string s, out int result)
		{
			return int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
		}
	}
}
