using System.Collections;
using System.ComponentModel;
using TMPro;
using UnityEngine;

namespace ClearSkyStudio
{
	public class GlitchOnce : MonoBehaviour
	{
		[Header("Play Option")]
		[SerializeField]
		private bool playOnEnable = true;

		[Description("Set -1 for infinite replay")]
		[SerializeField]
		private int rePlay = -1;

		[SerializeField]
		private float rePlayCoolTime = 1f;

		[Space(10f)]
		[Range(0f, 10f)]
		public float playTime = 0.5f;

		[Range(0f, 1000f)]
		public float startSpeed = 66.1f;

		public float Speed;

		public float Distance = 1f;

		public float Amplitude = 1f;

		private Material TextMat;

		private TMP_Text selfText;

		private IEnumerator curGlitch;

		private float curRePlayCoolTime;

		private int replayCount;

		private float curTime;

		private void Awake()
		{
			selfText = GetComponent<TMP_Text>();
		}

		private void OnEnable()
		{
			replayCount = rePlay;
			if (playOnEnable)
			{
				Glitch();
			}
		}

		private void Update()
		{
			if (curGlitch == null || (replayCount <= 0 && rePlay != -1) || !(curRePlayCoolTime > 0f))
			{
				return;
			}
			curRePlayCoolTime -= Time.deltaTime;
			if (curRePlayCoolTime < 0f)
			{
				if (replayCount > 0)
				{
					replayCount--;
				}
				Glitch();
			}
		}

		private void OnDisable()
		{
			if (curGlitch != null)
			{
				replayCount = rePlay;
				StopCoroutine(curGlitch);
				TextMat.SetFloat("_BienDo", 0f);
				TextMat.SetFloat("_Distance", 0f);
				TextMat.SetFloat("_Speed", 0f);
			}
		}

		public void Glitch()
		{
			TextMat = selfText.fontSharedMaterial;
			curGlitch = GlichCoroutine();
			StartCoroutine(curGlitch);
		}

		private IEnumerator GlichCoroutine()
		{
			curTime = playTime;
			Speed = startSpeed;
			while (curTime > 0f)
			{
				curTime -= Time.deltaTime;
				float value = Speed - Mathf.Pow(playTime - curTime, 5f);
				float value2 = Mathf.Lerp(Amplitude, 0f, curTime);
				float value3 = Mathf.Lerp(Distance, 0f, curTime);
				TextMat.SetFloat("_Speed", value);
				if (curTime > playTime * 0.45f)
				{
					TextMat.SetFloat("_BienDo", value2);
					TextMat.SetFloat("_Distance", value3);
				}
				else
				{
					TextMat.SetFloat("_BienDo", 0f);
					TextMat.SetFloat("_Distance", 0f);
				}
				yield return null;
			}
			curRePlayCoolTime = rePlayCoolTime;
			TextMat.SetFloat("_Speed", 0f);
		}
	}
}
