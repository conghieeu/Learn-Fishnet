using System;
using System.Collections.Generic;
using UnityEngine;

namespace ECE
{
	[Serializable]
	public class EasyColliderAutoSkinnedBone
	{
		public SkinnedMeshRenderer renderer;

		public bool Enabled = true;

		public SKINNED_MESH_COLLIDER_TYPE ColliderType;

		public float BoneWeight = 0.5f;

		public string BoneName = "Default";

		public bool IsPaired;

		public bool IsPairDisplayBone;

		public bool IsValid;

		public Matrix4x4 Matrix;

		public Matrix4x4 BindPose;

		public int BoneIndex;

		public Transform Transform;

		public List<Vector3> WorldSpaceVertices = new List<Vector3>();

		[SerializeField]
		public List<int> PairedBones = new List<int>();

		public Collider Collider;

		public int IndentLevel = -1;

		public EasyColliderAutoSkinnedBone(Matrix4x4 bp, int index, Transform t)
		{
			BoneIndex = index;
			BindPose = bp;
			Transform = t;
		}
	}
}
