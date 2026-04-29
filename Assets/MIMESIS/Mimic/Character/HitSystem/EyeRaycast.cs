using Mimic.Actors;
using Mimic.Animation;
using UnityEngine;

namespace Mimic.Character.HitSystem
{
	public class EyeRaycast : MonoBehaviour
	{
		[Space(10f)]
		[Header("========= Debug 용 설정 =========")]
		[Space(5f)]
		[SerializeField]
		[Tooltip("Scene 뷰에 표시 여부")]
		private bool drawInSceneView;

		[SerializeField]
		private Color gizmoColor = Color.green;

		[SerializeField]
		[Range(4f, 36f)]
		private int gizmoCircleSegments = 4;

		private PuppetScript puppet;

		public ProtoActor Owner
		{
			get
			{
				if (puppet == null)
				{
					puppet = base.transform.parent.GetComponent<PuppetScript>();
				}
				return puppet?.Owner;
			}
		}

		public Vector3 Position => base.transform.position;

		public Vector3 Forward => base.transform.forward;

		private void Awake()
		{
			if (puppet == null)
			{
				puppet = base.transform.parent.GetComponent<PuppetScript>();
			}
		}

		public void SetPuppet(PuppetScript puppet)
		{
			this.puppet = puppet;
		}
	}
}
