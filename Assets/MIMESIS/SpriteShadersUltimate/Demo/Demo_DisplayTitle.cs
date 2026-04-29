using UnityEngine;

namespace SpriteShadersUltimate.Demo
{
	public class Demo_DisplayTitle : MonoBehaviour
	{
		public Transform target;

		private RectTransform rectTransform;

		private RectTransform rectParent;

		private void Start()
		{
			rectTransform = GetComponent<RectTransform>();
			rectParent = base.transform.parent.GetComponent<RectTransform>();
		}

		private void LateUpdate()
		{
			Vector3 vector = RectTransformUtility.WorldToScreenPoint(Camera.main, target.position);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, vector, Camera.main, out var localPoint);
			rectTransform.anchoredPosition = localPoint;
			base.transform.localScale = target.lossyScale;
		}
	}
}
