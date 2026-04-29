using UnityEngine;

namespace Demo_Project
{
	public class Target : MonoBehaviour
	{
		public GameObject hitObj;

		public float increaseSizeRate = 0.01f;

		public float targetRandomOffset = 0.5f;

		public float targetShrinkSize = 0.75f;

		private void Start()
		{
			SceneManager.listOfTargets.Add(base.gameObject);
		}

		private void ReturnToNormalSize()
		{
			if (base.transform.localScale.x < 1f)
			{
				Vector3 localScale = base.transform.localScale;
				localScale.x += increaseSizeRate;
				localScale.y += increaseSizeRate;
				localScale.z += increaseSizeRate;
				base.transform.localScale = localScale;
			}
		}

		private void GenerateHitObj()
		{
			float num = Random.Range(0f - targetRandomOffset, targetRandomOffset);
			float num2 = Random.Range(0f - targetRandomOffset, targetRandomOffset);
			GameObject item = Object.Instantiate(hitObj, new Vector3(base.transform.position.x + num, base.transform.position.y + num2, base.transform.position.z), Quaternion.Euler(0f, 0f, 0f));
			SceneManager.listOfImpacts.Add(item);
		}

		private void ShrinkTarget()
		{
			base.transform.localScale = new Vector3(targetShrinkSize, targetShrinkSize, targetShrinkSize);
		}

		private void OnCollisionEnter2D(Collision2D col)
		{
			ShrinkTarget();
			GenerateHitObj();
		}

		private void Update()
		{
			ReturnToNormalSize();
		}
	}
}
