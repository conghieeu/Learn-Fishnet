using UnityEngine;
using UnityEngine.UI;

public class UICircularGauge : MonoBehaviour
{
	private float current;

	private float duration = 1f;

	[SerializeField]
	private Image gaugeImage;

	private void Awake()
	{
		base.gameObject.SetActive(value: false);
	}

	private void OnDisable()
	{
		StopAnimation();
	}

	public void StartAnimation(float duration)
	{
		base.gameObject.SetActive(value: true);
		this.duration = duration;
		current = 0f;
	}

	public void StopAnimation()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Update()
	{
		current += Time.deltaTime;
		float num = current / duration;
		Mathf.Clamp(num, 0f, 1f);
		gaugeImage.fillAmount = num;
	}
}
