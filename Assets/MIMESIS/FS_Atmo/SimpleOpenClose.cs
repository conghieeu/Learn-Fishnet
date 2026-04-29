using UnityEngine;

namespace FS_Atmo
{
	public class SimpleOpenClose : MonoBehaviour
	{
		private Animator myAnimator;

		private Animator additionalAnimator;

		public bool objectOpen;

		public bool objectOpenAdditional;

		public GameObject animateAdditional;

		private bool hasAdditional;

		private float myNormalizedTime;

		private void Start()
		{
			myAnimator = GetComponent<Animator>();
			if (myAnimator == null)
			{
				myAnimator = GetComponentInParent<Animator>();
			}
			if (objectOpen)
			{
				myAnimator.Play("Open", 0, 1f);
			}
			if (animateAdditional != null)
			{
				if ((bool)animateAdditional.GetComponent<SimpleOpenClose>())
				{
					additionalAnimator = animateAdditional.GetComponent<Animator>();
					hasAdditional = true;
					objectOpenAdditional = animateAdditional.GetComponent<SimpleOpenClose>().objectOpen;
				}
				else
				{
					hasAdditional = false;
				}
			}
		}

		private void ObjectClicked()
		{
			myNormalizedTime = myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
			if (!hasAdditional && (double)myNormalizedTime >= 1.0)
			{
				if (objectOpen)
				{
					myAnimator.Play("Close", 0, 0f);
					objectOpen = false;
				}
				else
				{
					myAnimator.Play("Open", 0, 0f);
					objectOpen = true;
				}
			}
			if (!hasAdditional || !((double)myNormalizedTime >= 1.0))
			{
				return;
			}
			if (objectOpen)
			{
				myAnimator.Play("Close", 0, 0f);
				objectOpen = false;
				animateAdditional.GetComponent<SimpleOpenClose>().objectOpenAdditional = false;
				if (objectOpenAdditional)
				{
					additionalAnimator.Play("Close", 0, 0f);
					objectOpenAdditional = false;
					animateAdditional.GetComponent<SimpleOpenClose>().objectOpen = false;
				}
			}
			else
			{
				myAnimator.Play("Open", 0, 0f);
				objectOpen = true;
				animateAdditional.GetComponent<SimpleOpenClose>().objectOpenAdditional = true;
				if (!objectOpenAdditional)
				{
					additionalAnimator.Play("Open", 0, 0f);
					objectOpenAdditional = true;
					animateAdditional.GetComponent<SimpleOpenClose>().objectOpen = true;
				}
			}
		}
	}
}
