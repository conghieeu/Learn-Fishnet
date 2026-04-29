using UnityEngine;

public class InteractiveSquishSSU : MonoBehaviour
{
	[Header("Settings:")]
	public float squishSpeed = 5f;

	public bool staySquished = true;

	public float squishDuration = 0.1f;

	private Material mat;

	private float currentSquish;

	private float lastTriggerStayTime;

	private void Start()
	{
		mat = GetComponent<SpriteRenderer>().material;
		currentSquish = 0f;
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (staySquished)
		{
			lastTriggerStayTime = Time.time;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		lastTriggerStayTime = Time.time;
	}

	private void Update()
	{
		float a = currentSquish;
		a = ((!(Time.time > lastTriggerStayTime + squishDuration)) ? Mathf.Lerp(a, 1.1f, Time.deltaTime * squishSpeed) : Mathf.Lerp(a, -0.1f, Time.deltaTime * squishSpeed));
		a = Mathf.Clamp01(a);
		if (a != currentSquish)
		{
			currentSquish = a;
			UpdateSquish();
		}
	}

	private void UpdateSquish()
	{
		mat.SetFloat("_SquishFade", currentSquish);
	}
}
