using Unity.Sentis;
using UnityEngine;

namespace DLAgent
{
	[CreateAssetMenu(fileName = "ModelProfile", menuName = "MimicDL/Model Profile", order = 1)]
	public class ModelProfile : ScriptableObject
	{
		public ModelAsset asset;

		[Range(0f, 1f)]
		public float lambda = 0.001f;

		public bool pathFinding;

		public bool useAngleBoost;

		public bool alignRotationToMovement;

		public bool movementExtension = true;
	}
}
