using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlinkAnimation : MonoBehaviour
{
	[SerializeField]
	[HideInInspector]
	private float mul;

	[SerializeField]
	[HideInInspector]
	private float add;

	[SerializeField]
	[HideInInspector]
	private float prog;

	private Image image;

	private Material material;

	private float _mul = float.NaN;

	private float _add = float.NaN;

	private float _prog = float.NaN;

	private Animator animator;

	[SerializeField]
	private float openAnimationDuration = 1f;

	private Coroutine openCompletion;

	private void Awake()
	{
		image = GetComponent<Image>();
		if (!(image == null))
		{
			image.enabled = false;
			animator = GetComponent<Animator>();
			material = image.material;
		}
	}

	private void Update()
	{
		if (!(material == null))
		{
			if (_mul != mul)
			{
				_mul = mul;
				material.SetFloat("_mul", mul);
			}
			if (_add != add)
			{
				material.SetFloat("_add", add);
				_add = add;
			}
			if (_prog != prog)
			{
				material.SetFloat("_prog", prog);
				_prog = prog;
			}
		}
	}

	private void TurnOn()
	{
		if (!image.enabled)
		{
			image.enabled = true;
		}
	}

	private void TurnOff()
	{
		image.enabled = false;
	}

	public void CloseEyeImmediate()
	{
		TurnOn();
		animator?.SetTrigger("closed");
		AbortOpenCompletion();
	}

	public void OpenEyeImmediate()
	{
		TurnOff();
	}

	public void OpenEye()
	{
		TurnOn();
		animator?.SetTrigger("open");
		openCompletion = StartCoroutine(OpenCompletion());
	}

	public void CloseEye()
	{
		TurnOn();
		animator?.SetTrigger("close");
		AbortOpenCompletion();
	}

	private IEnumerator OpenCompletion()
	{
		yield return new WaitForSeconds(openAnimationDuration);
		TurnOff();
		openCompletion = null;
	}

	private void AbortOpenCompletion()
	{
		if (openCompletion != null)
		{
			StopCoroutine(openCompletion);
			openCompletion = null;
		}
	}

	public void OnAnimationEvent(string statement)
	{
	}
}
