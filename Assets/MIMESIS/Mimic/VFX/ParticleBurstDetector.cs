using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mimic.VFX
{
	public class ParticleBurstDetector : MonoBehaviour
	{
		[Header("Target Settings")]
		[SerializeField]
		[Tooltip("감지할 파티클 시스템들 (비어있으면 자동으로 모든 파티클 시스템 찾기)")]
		private ParticleSystem[] targetParticleSystems;

		[Header("Burst Detection")]
		[SerializeField]
		[Tooltip("감지 간격 (초)")]
		private float checkInterval = 0.1f;

		[Header("Audio")]
		[SerializeField]
		[Tooltip("재생할 사운드 ID")]
		private string soundId = "spark_blue";

		[SerializeField]
		[Tooltip("3D 사운드로 재생")]
		private bool use3D = true;

		private float lastBurstTime;

		private float burstCooldown;

		private Dictionary<ParticleSystem, int> lastParticleCounts = new Dictionary<ParticleSystem, int>();

		private bool hasDetectedInitialBurst;

		private void Start()
		{
			FindTargetParticleSystem();
			if (targetParticleSystems != null && targetParticleSystems.Length != 0)
			{
				CalculateBurstCooldown();
				StartCoroutine(MonitorBurst());
			}
			else
			{
				Logger.RWarn("[ParticleBurstDetector] No target particle systems found on " + base.gameObject.name, sendToLogServer: false, useConsoleOut: true, "audio");
			}
		}

		private void FindTargetParticleSystem()
		{
			if (targetParticleSystems != null && targetParticleSystems.Length != 0)
			{
				return;
			}
			ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
			if (componentsInChildren.Length != 0)
			{
				targetParticleSystems = componentsInChildren;
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					lastParticleCounts[componentsInChildren[i]] = 0;
				}
			}
		}

		private void CalculateBurstCooldown()
		{
			if (targetParticleSystems.Length != 0)
			{
				_ = targetParticleSystems[0].main;
				ParticleSystem.EmissionModule emission = targetParticleSystems[0].emission;
				if (emission.enabled && emission.burstCount > 0)
				{
					ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[emission.burstCount];
					emission.GetBursts(bursts);
					burstCooldown = 0.05f;
				}
				else
				{
					burstCooldown = 0.05f;
				}
			}
		}

		private IEnumerator MonitorBurst()
		{
			while (base.gameObject.activeInHierarchy)
			{
				if (targetParticleSystems != null && targetParticleSystems.Length != 0)
				{
					float time = Time.time;
					bool flag = false;
					ParticleSystem[] array = targetParticleSystems;
					foreach (ParticleSystem particleSystem in array)
					{
						if (particleSystem != null && particleSystem.isPlaying && CheckParticleCountBurst(particleSystem, time))
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						PlayAudio();
					}
				}
				yield return new WaitForSeconds(checkInterval);
			}
		}

		private bool CheckParticleCountBurst(ParticleSystem ps, float currentTime)
		{
			int particleCount = ps.particleCount;
			int num = (lastParticleCounts.ContainsKey(ps) ? lastParticleCounts[ps] : 0);
			if (particleCount > num)
			{
				int num2 = particleCount - num;
				if ((!hasDetectedInitialBurst || currentTime - lastBurstTime > burstCooldown) && (num2 >= 5 || (!hasDetectedInitialBurst && num2 > 0)))
				{
					lastBurstTime = currentTime;
					hasDetectedInitialBurst = true;
					lastParticleCounts[ps] = particleCount;
					return true;
				}
			}
			lastParticleCounts[ps] = particleCount;
			return false;
		}

		private void PlayAudio()
		{
			if (!string.IsNullOrEmpty(soundId) && !(Hub.s?.audioman == null))
			{
				if (use3D)
				{
					Hub.s.audioman.PlaySfxAtTransform(soundId, base.transform);
				}
				else
				{
					Hub.s.audioman.PlaySfx(soundId);
				}
			}
		}

		private void OnEnable()
		{
			lastBurstTime = 0f;
			hasDetectedInitialBurst = false;
			lastParticleCounts.Clear();
		}

		public void TriggerAudioManually()
		{
			PlayAudio();
		}
	}
}
