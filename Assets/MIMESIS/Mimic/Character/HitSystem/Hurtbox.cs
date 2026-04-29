using Mimic.Actors;
using Mimic.Animation;
using UnityEngine;

namespace Mimic.Character.HitSystem
{
	public class Hurtbox : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("피격 판정용 실린더의 높이")]
		private float height = 1f;

		[SerializeField]
		[Tooltip("피격 판정용 실린더의 반지름")]
		private float radius = 1f;

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

		public float Height => height;

		public float Radius => radius;

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

		public Vector3 Bottom => base.transform.position;

		public Vector3 Top => Bottom + Up * height;

		public Vector3 Right => base.transform.right;

		public Vector3 Up => base.transform.up;

		public Vector3 Forward => base.transform.forward;

		private void Awake()
		{
			if (puppet == null)
			{
				puppet = base.transform.parent.GetComponent<PuppetScript>();
			}
			if (puppet == null)
			{
				Logger.RError("Hurtbox의 부모에 PuppetScript가 없습니다.");
			}
		}
	}
}
