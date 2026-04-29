using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace ECE
{
	[Serializable]
	public class EasyColliderAutoSkinned : ScriptableObject, ISerializationCallbackReceiver
	{
		private struct ShiftData
		{
			public Vector3 Direction;

			public float Size;

			public ShiftData(Vector3 direction, float size)
			{
				Direction = direction;
				Size = size;
			}
		}

		private SKINNED_MESH_DEPENETRATE_ORDER _autoSkinnedDepenetrateOrder;

		private bool _autoSkinnedForce256Triangles = true;

		private float _autoSkinnedMinBoneWeight = 0.5f;

		private float _autoSkinnedMinRealignAngle;

		private bool _autoSkinnedAllowRealign;

		private SKINNED_MESH_COLLIDER_TYPE _autoSkinnedColliderType;

		private bool _autoSkinnedDepenetrate;

		private bool _autoSkinnedIndents = true;

		private int _autoSkinnedIterativeDepenetrationCount = 15;

		private bool _autoSkinnedPairing = true;

		private bool _autoSkinnedPerBoneSettings;

		private float _autoSkinnedShrinkAmount = 0.5f;

		private bool _autoSkinnedUseDistanceDeltaPairing = true;

		private float _autoSkinnedPairedDistanceDelta = 0.01f;

		private EasyColliderAutoSkinnedBone _selectedBone;

		private GameObject _initialScannedObject;

		[SerializeField]
		public List<EasyColliderAutoSkinnedBone> BoneList = new List<EasyColliderAutoSkinnedBone>();

		[SerializeField]
		public List<EasyColliderAutoSkinnedBone> SortedBoneList = new List<EasyColliderAutoSkinnedBone>();

		public int transformHashCode;

		public SkinnedMeshRenderer renderer;

		public bool reverse;

		private int ShrinkCount;

		private Vector3 ShrinkAmount;

		private int ShiftCount;

		private Vector3 ShiftAmount;

		public SKINNED_MESH_DEPENETRATE_ORDER AutoSkinnedDepenetrateOrder
		{
			get
			{
				return _autoSkinnedDepenetrateOrder;
			}
			set
			{
				_autoSkinnedDepenetrateOrder = value;
			}
		}

		public bool AutoSkinnedForce256Triangles
		{
			get
			{
				return _autoSkinnedForce256Triangles;
			}
			set
			{
				_autoSkinnedForce256Triangles = value;
			}
		}

		public float AutoSkinnedMinBoneWeight
		{
			get
			{
				return _autoSkinnedMinBoneWeight;
			}
			set
			{
				_autoSkinnedMinBoneWeight = value;
			}
		}

		public float AutoSkinnedMinRealignAngle
		{
			get
			{
				return _autoSkinnedMinRealignAngle;
			}
			set
			{
				_autoSkinnedMinRealignAngle = value;
			}
		}

		public bool AutoSkinnedAllowRealign
		{
			get
			{
				return _autoSkinnedAllowRealign;
			}
			set
			{
				_autoSkinnedAllowRealign = value;
			}
		}

		public SKINNED_MESH_COLLIDER_TYPE AutoSkinnedColliderType
		{
			get
			{
				return _autoSkinnedColliderType;
			}
			set
			{
				_autoSkinnedColliderType = value;
			}
		}

		public bool AutoSkinnedDepenetrate
		{
			get
			{
				return _autoSkinnedDepenetrate;
			}
			set
			{
				_autoSkinnedDepenetrate = value;
			}
		}

		public bool AutoSkinnedIndents
		{
			get
			{
				return _autoSkinnedIndents;
			}
			set
			{
				_autoSkinnedIndents = value;
			}
		}

		public int AutoSkinnedIterativeDepenetrationCount
		{
			get
			{
				return _autoSkinnedIterativeDepenetrationCount;
			}
			set
			{
				_autoSkinnedIterativeDepenetrationCount = value;
			}
		}

		public bool AutoSkinnedPairing
		{
			get
			{
				return _autoSkinnedPairing;
			}
			set
			{
				_autoSkinnedPairing = value;
			}
		}

		public bool AutoSkinnedPerBoneSettings
		{
			get
			{
				return _autoSkinnedPerBoneSettings;
			}
			set
			{
				_autoSkinnedPerBoneSettings = value;
			}
		}

		public float AutoSkinnedShrinkAmount
		{
			get
			{
				return _autoSkinnedShrinkAmount;
			}
			set
			{
				_autoSkinnedShrinkAmount = value;
			}
		}

		public bool AutoSkinnedUseDistanceDeltaPairing
		{
			get
			{
				return _autoSkinnedUseDistanceDeltaPairing;
			}
			set
			{
				_autoSkinnedUseDistanceDeltaPairing = value;
			}
		}

		public float AutoSkinnedPairedDistanceDelta
		{
			get
			{
				return _autoSkinnedPairedDistanceDelta;
			}
			set
			{
				_autoSkinnedPairedDistanceDelta = value;
			}
		}

		public void Clean()
		{
			BoneList = new List<EasyColliderAutoSkinnedBone>();
			SortedBoneList = new List<EasyColliderAutoSkinnedBone>();
			renderer = null;
			transformHashCode = -1;
			_selectedBone = null;
			_initialScannedObject = null;
		}

		public void SetSelectedBone(EasyColliderAutoSkinnedBone bone)
		{
			_selectedBone = bone;
		}

		public EasyColliderAutoSkinnedBone GetSelectedBone()
		{
			return _selectedBone;
		}

		public GameObject GetInitialScannedObject()
		{
			return _initialScannedObject;
		}

		public bool HasSkinnedMeshRendererTransformed()
		{
			if (renderer == null)
			{
				return false;
			}
			int num = renderer.transform.position.GetHashCode() + renderer.transform.rotation.GetHashCode() + renderer.transform.lossyScale.GetHashCode();
			if (transformHashCode != num)
			{
				transformHashCode = num;
				return true;
			}
			return false;
		}

		private void SetBoneValidity(SkinnedMeshRenderer smr, List<EasyColliderAutoSkinnedBone> boneList)
		{
			BoneWeight[] boneWeights = smr.sharedMesh.boneWeights;
			for (int i = 0; i < boneWeights.Length; i++)
			{
				BoneWeight boneWeight = boneWeights[i];
				if (boneWeight.boneIndex0 >= 0 && boneWeight.weight0 > 0f)
				{
					boneList[boneWeight.boneIndex0].IsValid = true;
				}
				if (boneWeight.boneIndex1 >= 0 && boneWeight.weight1 > 0f)
				{
					boneList[boneWeight.boneIndex1].IsValid = true;
				}
				if (boneWeight.boneIndex2 >= 0 && boneWeight.weight2 > 0f)
				{
					boneList[boneWeight.boneIndex2].IsValid = true;
				}
				if (boneWeight.boneIndex3 >= 0 && boneWeight.weight3 > 0f)
				{
					boneList[boneWeight.boneIndex3].IsValid = true;
				}
			}
		}

		public void InitialScanBones(GameObject selectedObject, float weight = 0.5f)
		{
			SkinnedMeshRenderer componentInChildren = selectedObject.GetComponentInChildren<SkinnedMeshRenderer>();
			_initialScannedObject = selectedObject;
			if (componentInChildren == null)
			{
				return;
			}
			EasyColliderAutoSkinnedBone[] skinnedMeshBones = GetSkinnedMeshBones(componentInChildren);
			if (skinnedMeshBones == null)
			{
				return;
			}
			BoneList = skinnedMeshBones.ToList();
			if (BoneList.Count == 0)
			{
				return;
			}
			SetBoneValidity(componentInChildren, BoneList);
			foreach (EasyColliderAutoSkinnedBone bone in BoneList)
			{
				bone.BoneWeight = weight;
			}
			List<Transform> boneTransforms = componentInChildren.bones.ToList();
			PairBones(BoneList, boneTransforms);
			if (componentInChildren.rootBone != null)
			{
				Transform rootBone = componentInChildren.rootBone;
				if (rootBone.parent != null)
				{
					SetIndentRecursive(rootBone.parent, boneTransforms, 0);
				}
				else
				{
					SetIndentRecursive(rootBone, boneTransforms, 0);
				}
				if (rootBone.parent != null)
				{
					SortedBoneList = SortBonesRecursive(rootBone.parent, boneTransforms, BoneList, new List<EasyColliderAutoSkinnedBone>());
				}
				else
				{
					SortedBoneList = SortBonesRecursive(rootBone, boneTransforms, BoneList, new List<EasyColliderAutoSkinnedBone>());
				}
				return;
			}
			List<EasyColliderAutoSkinnedBone> list = IdentifyRootBones(BoneList);
			foreach (EasyColliderAutoSkinnedBone item in list)
			{
				SetIndentRecursive(item.Transform, boneTransforms, 0);
			}
			SortedBoneList = NoRootBoneSort(list, boneTransforms);
		}

		private List<EasyColliderAutoSkinnedBone> IdentifyRootBones(List<EasyColliderAutoSkinnedBone> boneList)
		{
			HashSet<EasyColliderAutoSkinnedBone> hashSet = new HashSet<EasyColliderAutoSkinnedBone>();
			List<EasyColliderAutoSkinnedBone> list = new List<EasyColliderAutoSkinnedBone>();
			foreach (EasyColliderAutoSkinnedBone bone in boneList)
			{
				if (bone.Transform == null || hashSet.Contains(bone))
				{
					continue;
				}
				hashSet.Add(bone);
				EasyColliderAutoSkinnedBone easyColliderAutoSkinnedBone = bone;
				bool flag = true;
				while (flag)
				{
					flag = false;
					Transform parent = easyColliderAutoSkinnedBone.Transform.parent;
					if (!(parent != null))
					{
						continue;
					}
					foreach (EasyColliderAutoSkinnedBone bone2 in boneList)
					{
						if (bone2.Transform == parent)
						{
							if (list.Contains(bone2) || hashSet.Contains(bone2))
							{
								easyColliderAutoSkinnedBone = null;
								flag = false;
							}
							else
							{
								easyColliderAutoSkinnedBone = bone2;
								flag = true;
							}
							break;
						}
					}
				}
				hashSet.Add(easyColliderAutoSkinnedBone);
				if (easyColliderAutoSkinnedBone != null)
				{
					list.Add(easyColliderAutoSkinnedBone);
				}
			}
			return list;
		}

		private List<EasyColliderAutoSkinnedBone> NoRootBoneSort(List<EasyColliderAutoSkinnedBone> rootBones, List<Transform> boneTransforms)
		{
			List<EasyColliderAutoSkinnedBone> list = new List<EasyColliderAutoSkinnedBone>();
			foreach (EasyColliderAutoSkinnedBone rootBone in rootBones)
			{
				List<EasyColliderAutoSkinnedBone> list2 = new List<EasyColliderAutoSkinnedBone>();
				SortBonesRecursive(rootBone.Transform, boneTransforms, BoneList, list2);
				list.AddRange(list2);
			}
			return list;
		}

		private void PairBones(List<EasyColliderAutoSkinnedBone> boneList, List<Transform> boneTransforms)
		{
			foreach (EasyColliderAutoSkinnedBone bone in boneList)
			{
				if (bone.IsPaired || bone.Transform == null)
				{
					continue;
				}
				int childCount = bone.Transform.childCount;
				List<int> list = new List<int>();
				for (int i = 0; i < childCount; i++)
				{
					if (boneTransforms.Contains(bone.Transform.GetChild(i)))
					{
						list.Add(i);
					}
				}
				if (list.Count <= 1)
				{
					continue;
				}
				List<int> list2 = new List<int>(new int[list.Count]);
				List<float> list3 = new List<float>(new float[list.Count]);
				for (int j = 0; j < list.Count; j++)
				{
					Transform child = bone.Transform.GetChild(list[j]);
					List<Transform> list4 = child.GetComponentsInChildren<Transform>().ToList();
					int num = 0;
					foreach (Transform item in list4)
					{
						if (boneTransforms.Contains(item))
						{
							num++;
						}
						list3[j] += Vector3.Distance(child.position, item.position);
					}
					list2[j] = num;
				}
				for (int k = 0; k < list.Count; k++)
				{
					int num2 = boneTransforms.IndexOf(bone.Transform.GetChild(k));
					if (num2 < 0 || boneList[num2].IsPaired)
					{
						continue;
					}
					List<int> list5 = new List<int>();
					for (int l = 0; l < list.Count; l++)
					{
						if (l == k)
						{
							continue;
						}
						bool flag = list2[k] == list2[l];
						if (AutoSkinnedUseDistanceDeltaPairing)
						{
							if (flag && (list3[k] == list3[l] || Mathf.Abs(list3[k] - list3[l]) < AutoSkinnedPairedDistanceDelta))
							{
								flag = true;
							}
							else if (flag)
							{
								flag = false;
							}
						}
						if (flag)
						{
							Transform child2 = bone.Transform.GetChild(l);
							int num3 = boneTransforms.IndexOf(child2);
							if (num3 >= 0)
							{
								list5.Add(num3);
							}
						}
					}
					if (list5.Count <= 0)
					{
						continue;
					}
					boneList[num2].PairedBones = list5;
					boneList[num2].IsPaired = true;
					foreach (int item2 in list5)
					{
						boneList[item2].IsPaired = true;
					}
					List<Transform> list6 = boneList[num2].Transform.GetComponentsInChildren<Transform>().ToList();
					List<List<Transform>> list7 = new List<List<Transform>>();
					foreach (int item3 in list5)
					{
						list7.Add(boneTransforms[item3].GetComponentsInChildren<Transform>().ToList());
					}
					for (int m = 0; m < list6.Count; m++)
					{
						int num4 = boneTransforms.IndexOf(list6[m]);
						if (num4 < 0)
						{
							continue;
						}
						List<int> list8 = new List<int>();
						foreach (List<Transform> item4 in list7)
						{
							int num5 = boneTransforms.IndexOf(item4[m]);
							if (num5 >= 0)
							{
								list8.Add(num5);
								boneList[num5].IsPaired = true;
							}
						}
						if (list8.Count > 0)
						{
							boneList[num4].IsPaired = true;
							boneList[num4].PairedBones = list8;
							boneList[num4].IsPairDisplayBone = true;
						}
					}
				}
			}
		}

		private List<EasyColliderAutoSkinnedBone> SortBonesRecursive(Transform current, List<Transform> boneTransforms, List<EasyColliderAutoSkinnedBone> boneList, List<EasyColliderAutoSkinnedBone> sortedList)
		{
			int num = boneTransforms.IndexOf(current);
			if (num >= 0)
			{
				EasyColliderAutoSkinnedBone easyColliderAutoSkinnedBone = BoneList[num];
				easyColliderAutoSkinnedBone.BoneName = current.name;
				sortedList.Add(easyColliderAutoSkinnedBone);
				for (int i = 0; i < current.childCount; i++)
				{
					Transform child = current.GetChild(i);
					SortBonesRecursive(child, boneTransforms, boneList, sortedList);
				}
			}
			else
			{
				for (int j = 0; j < current.childCount; j++)
				{
					Transform child2 = current.GetChild(j);
					SortBonesRecursive(child2, boneTransforms, boneList, sortedList);
				}
			}
			return sortedList;
		}

		private void SetIndentRecursive(Transform current, List<Transform> boneTransforms, int currentIndent)
		{
			int num = boneTransforms.IndexOf(current);
			if (num >= 0)
			{
				if (BoneList[num].IsValid)
				{
					BoneList[num].IndentLevel = currentIndent;
					currentIndent++;
				}
				for (int i = 0; i < current.childCount; i++)
				{
					SetIndentRecursive(current.GetChild(i), boneTransforms, currentIndent);
				}
			}
			else
			{
				for (int j = 0; j < current.childCount; j++)
				{
					SetIndentRecursive(current.GetChild(j), boneTransforms, currentIndent);
				}
			}
		}

		public GameObject CreateRealignedObject(Transform parent, Transform child, bool forPreview = false)
		{
			GameObject gameObject = new GameObject(parent.transform.name + "_RotatedCollider");
			Vector3 vector = child.position - parent.position;
			Vector3 vector2 = parent.transform.right;
			if (vector == vector2)
			{
				vector2 = parent.transform.forward;
			}
			Vector3 upwards = Vector3.Cross(vector, vector2);
			gameObject.transform.rotation = Quaternion.LookRotation(vector, upwards);
			gameObject.transform.SetParent(parent.transform);
			gameObject.layer = gameObject.transform.parent.gameObject.layer;
			gameObject.transform.localPosition = Vector3.zero;
			return gameObject;
		}

		public Matrix4x4 GetMatrixForObject(Transform parent, Transform child)
		{
			Vector3 vector = child.position - parent.position;
			Vector3 vector2 = parent.transform.right;
			if (vector == vector2)
			{
				vector2 = parent.transform.forward;
			}
			Vector3 upwards = Vector3.Cross(vector, vector2);
			Quaternion q = Quaternion.LookRotation(vector, upwards);
			return Matrix4x4.TRS(parent.position, q, Vector3.one);
		}

		private List<Vector3> ToLocalVerts(Transform transform, List<Vector3> worldVertices)
		{
			List<Vector3> list = new List<Vector3>(worldVertices.Count);
			foreach (Vector3 worldVertex in worldVertices)
			{
				list.Add(transform.InverseTransformPoint(worldVertex));
			}
			return list;
		}

		private Collider GenerateCollider(SKINNED_MESH_COLLIDER_TYPE colliderType, EasyColliderProperties properties, EasyColliderAutoSkinnedBone s, string savePath, bool forPreview = false)
		{
			EasyColliderCreator easyColliderCreator = new EasyColliderCreator();
			switch (colliderType)
			{
			case SKINNED_MESH_COLLIDER_TYPE.Box:
				return easyColliderCreator.CreateBoxCollider(s.WorldSpaceVertices, properties);
			case SKINNED_MESH_COLLIDER_TYPE.Capsule:
				return easyColliderCreator.CreateCapsuleCollider_MinMax(s.WorldSpaceVertices, properties, CAPSULE_COLLIDER_METHOD.MinMax);
			case SKINNED_MESH_COLLIDER_TYPE.Sphere:
				return easyColliderCreator.CreateSphereCollider_MinMax(s.WorldSpaceVertices, properties);
			case SKINNED_MESH_COLLIDER_TYPE.Convex_Mesh:
			{
				s.WorldSpaceVertices = ToLocalVerts(s.Transform, s.WorldSpaceVertices);
				EasyColliderQuickHull easyColliderQuickHull = EasyColliderQuickHull.CalculateHull(s.WorldSpaceVertices);
				Mesh result = easyColliderQuickHull.Result;
				if (AutoSkinnedForce256Triangles && result != null && result.triangles.Length / 3 > 255)
				{
					int num = 0;
					float prevWeldDist = 0f;
					while (result.triangles.Length / 3 > 255 && num < 25)
					{
						prevWeldDist = ReduceMeshVerticesOnBone(s, result, prevWeldDist);
						easyColliderQuickHull = EasyColliderQuickHull.CalculateHull(s.WorldSpaceVertices);
						result = easyColliderQuickHull.Result;
						num++;
					}
				}
				if (result != null)
				{
					return easyColliderCreator.CreateConvexMeshCollider(easyColliderQuickHull.Result, s.Transform.gameObject, properties);
				}
				break;
			}
			}
			return null;
		}

		public void ChangeBoneEnabled(EasyColliderAutoSkinnedBone bone, bool enabled, bool includeChildren)
		{
			bone.Enabled = enabled;
			if (includeChildren)
			{
				foreach (EasyColliderAutoSkinnedBone bone2 in BoneList)
				{
					if (bone2.Transform.IsChildOf(bone.Transform))
					{
						bone2.Enabled = enabled;
					}
				}
			}
			if (!AutoSkinnedPairing)
			{
				return;
			}
			foreach (int pairedBone in bone.PairedBones)
			{
				ChangeBoneEnabled(BoneList[pairedBone], enabled, includeChildren);
			}
		}

		public void ChangeBoneWeight(EasyColliderAutoSkinnedBone bone, float weight, bool includeChildren)
		{
			bone.BoneWeight = weight;
			if (includeChildren)
			{
				foreach (EasyColliderAutoSkinnedBone bone2 in BoneList)
				{
					if (bone2.Transform.IsChildOf(bone.Transform))
					{
						bone2.BoneWeight = weight;
					}
				}
			}
			if (!AutoSkinnedPairing)
			{
				return;
			}
			foreach (int pairedBone in bone.PairedBones)
			{
				ChangeBoneWeight(BoneList[pairedBone], weight, includeChildren);
			}
		}

		public void ChangeBoneColliderType(EasyColliderAutoSkinnedBone bone, SKINNED_MESH_COLLIDER_TYPE colliderType, bool includeChildren)
		{
			bone.ColliderType = colliderType;
			if (includeChildren)
			{
				foreach (EasyColliderAutoSkinnedBone bone2 in BoneList)
				{
					if (bone2.Transform.IsChildOf(bone.Transform))
					{
						bone2.ColliderType = colliderType;
					}
				}
			}
			if (!AutoSkinnedPairing)
			{
				return;
			}
			foreach (int pairedBone in bone.PairedBones)
			{
				ChangeBoneColliderType(BoneList[pairedBone], colliderType, includeChildren);
			}
		}

		private float ReduceMeshVerticesOnBone(EasyColliderAutoSkinnedBone bone, Mesh m, float prevWeldDist)
		{
			List<Vector3> list = new List<Vector3>();
			HashSet<Vector3> hashSet = new HashSet<Vector3>();
			int num = (int)(256f * ((float)bone.WorldSpaceVertices.Count / ((float)m.triangles.Length / 3f)));
			if (num >= bone.WorldSpaceVertices.Count || (float)num > (float)bone.WorldSpaceVertices.Count * 0.9f)
			{
				num = (int)((float)bone.WorldSpaceVertices.Count * 0.9f);
			}
			_ = bone.WorldSpaceVertices.Count;
			float num2 = m.bounds.size.magnitude / 50f;
			if (num2 < prevWeldDist)
			{
				num2 = prevWeldDist + num2 * 0.25f;
			}
			int num3 = 0;
			while (num < bone.WorldSpaceVertices.Count && num3 < 25)
			{
				_ = bone.WorldSpaceVertices.Count;
				for (int i = 0; i < bone.WorldSpaceVertices.Count; i++)
				{
					int num4 = 1;
					Vector3 vector = bone.WorldSpaceVertices[i];
					if (hashSet.Contains(vector))
					{
						continue;
					}
					for (int j = i; j < bone.WorldSpaceVertices.Count; j++)
					{
						Vector3 vector2 = bone.WorldSpaceVertices[j];
						if (!hashSet.Contains(vector2) && Vector3.Distance(vector, vector2) < num2)
						{
							vector = (vector * num4 + vector2) / (num4 + 1);
							num4++;
							hashSet.Add(vector2);
							hashSet.Add(vector);
						}
					}
					list.Add(vector);
				}
				num2 *= 1.25f;
				hashSet.Clear();
				bone.WorldSpaceVertices = list;
				list = new List<Vector3>();
				num3++;
			}
			return num2;
		}

		private List<Vector3> ToLocalVerts(List<Vector3> worldVertices, Matrix4x4 m)
		{
			List<Vector3> list = new List<Vector3>(worldVertices.Count);
			Matrix4x4 inverse = m.inverse;
			foreach (Vector3 worldVertex in worldVertices)
			{
				list.Add(inverse.MultiplyPoint3x4(worldVertex));
			}
			return list;
		}

		private EasyColliderData CalculateColliderData(SKINNED_MESH_COLLIDER_TYPE colliderType, EasyColliderAutoSkinnedBone s, EasyColliderProperties properties)
		{
			EasyColliderCreator easyColliderCreator = new EasyColliderCreator();
			List<Vector3> list = ToLocalVerts(s.WorldSpaceVertices, s.Matrix);
			switch (colliderType)
			{
			case SKINNED_MESH_COLLIDER_TYPE.Box:
			{
				BoxColliderData boxColliderData = easyColliderCreator.CalculateBoxLocal(list);
				boxColliderData.Matrix = s.Matrix;
				return boxColliderData;
			}
			case SKINNED_MESH_COLLIDER_TYPE.Capsule:
			{
				CapsuleColliderData capsuleColliderData = easyColliderCreator.CalculateCapsuleMinMaxLocal(list, CAPSULE_COLLIDER_METHOD.MinMax);
				capsuleColliderData.Matrix = s.Matrix;
				return capsuleColliderData;
			}
			case SKINNED_MESH_COLLIDER_TYPE.Sphere:
			{
				SphereColliderData sphereColliderData = easyColliderCreator.CalculateSphereMinMaxLocal(list);
				sphereColliderData.Matrix = s.Matrix;
				return sphereColliderData;
			}
			case SKINNED_MESH_COLLIDER_TYPE.Convex_Mesh:
			{
				MeshColliderData meshColliderData = easyColliderCreator.CalculateMeshColliderQuickHullLocal(list);
				meshColliderData.Matrix = s.Matrix;
				return meshColliderData;
			}
			default:
				return new EasyColliderData();
			}
		}

		public void SetColliderTypeOnAllBones(SKINNED_MESH_COLLIDER_TYPE colliderType)
		{
			foreach (EasyColliderAutoSkinnedBone bone in BoneList)
			{
				bone.ColliderType = colliderType;
			}
		}

		public void SetColliderTypeAndWeightOnAllBones(SKINNED_MESH_COLLIDER_TYPE colliderType, float weight)
		{
			foreach (EasyColliderAutoSkinnedBone bone in BoneList)
			{
				bone.BoneWeight = weight;
				bone.ColliderType = colliderType;
			}
		}

		private void BakeSkinnedMesh(SkinnedMeshRenderer skinnedMesh, Mesh m)
		{
			Vector3 lossyScale = skinnedMesh.transform.lossyScale;
			Vector3 localScale = skinnedMesh.transform.localScale;
			if (lossyScale != Vector3.one)
			{
				localScale.x /= lossyScale.x;
				localScale.y /= lossyScale.y;
				localScale.z /= lossyScale.z;
			}
			skinnedMesh.transform.localScale = localScale;
			skinnedMesh.BakeMesh(m);
		}

		public List<EasyColliderData> CalculateSkinnedMeshPreview(SkinnedMeshRenderer skinnedMesh, SKINNED_MESH_COLLIDER_TYPE colliderType, EasyColliderProperties properties, float minBoneWeight, bool realignBones = false, float minRealignAngle = 20f, string savePath = "Assets/")
		{
			if (skinnedMesh == null)
			{
				return new List<EasyColliderData>();
			}
			List<GameObject> list = new List<GameObject>();
			renderer = skinnedMesh;
			List<EasyColliderData> list2 = new List<EasyColliderData>();
			Mesh mesh = new Mesh();
			Vector3 localScale = skinnedMesh.transform.localScale;
			BakeSkinnedMesh(skinnedMesh, mesh);
			Vector3[] vertices = mesh.vertices;
			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i] = skinnedMesh.transform.TransformPoint(vertices[i]);
			}
			skinnedMesh.transform.localScale = localScale;
			EasyColliderAutoSkinnedBone[] array = BoneList.ToArray();
			if (array == null || BoneList.Count == 0)
			{
				return null;
			}
			EasyColliderAutoSkinnedBone[] array2 = array;
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j].WorldSpaceVertices.Clear();
			}
			List<Collider> list3 = new List<Collider>();
			SetWorldVertices(array, skinnedMesh.sharedMesh.GetAllBoneWeights(), skinnedMesh.sharedMesh.GetBonesPerVertex(), vertices, minBoneWeight);
			Transform[] bones = skinnedMesh.bones;
			array2 = array;
			foreach (EasyColliderAutoSkinnedBone easyColliderAutoSkinnedBone in array2)
			{
				if (!easyColliderAutoSkinnedBone.Enabled || !easyColliderAutoSkinnedBone.IsValid || easyColliderAutoSkinnedBone.renderer != skinnedMesh || easyColliderAutoSkinnedBone == null || easyColliderAutoSkinnedBone.WorldSpaceVertices.Count == 0)
				{
					continue;
				}
				properties.AttachTo = easyColliderAutoSkinnedBone.Transform.gameObject;
				easyColliderAutoSkinnedBone.Matrix = easyColliderAutoSkinnedBone.Transform.localToWorldMatrix;
				if (bones.Length != 0 && realignBones && colliderType != SKINNED_MESH_COLLIDER_TYPE.Convex_Mesh && minRealignAngle > 0f)
				{
					Transform childBone = GetChildBone(easyColliderAutoSkinnedBone.Transform, bones);
					if (childBone != null && GetMinimumChildAngle(easyColliderAutoSkinnedBone.Transform, childBone) >= minRealignAngle)
					{
						easyColliderAutoSkinnedBone.Matrix = GetMatrixForObject(easyColliderAutoSkinnedBone.Transform, childBone);
						if (AutoSkinnedDepenetrate)
						{
							GameObject gameObject = CreateRealignedObject(easyColliderAutoSkinnedBone.Transform, childBone);
							list.Add(gameObject);
							properties.AttachTo = gameObject;
						}
					}
				}
				EasyColliderData item = CalculateColliderData(easyColliderAutoSkinnedBone.ColliderType, easyColliderAutoSkinnedBone, properties);
				if (AutoSkinnedDepenetrate)
				{
					Collider collider = GenerateCollider(easyColliderAutoSkinnedBone.ColliderType, properties, easyColliderAutoSkinnedBone, savePath, forPreview: true);
					if (collider != null)
					{
						list3.Add(collider);
						list2.Add(item);
						easyColliderAutoSkinnedBone.Collider = collider;
					}
				}
				else if (!AutoSkinnedDepenetrate)
				{
					list2.Add(item);
				}
			}
			if (list3.Count == list2.Count && AutoSkinnedDepenetrate)
			{
				CheckDoDepenetration(list3);
				for (int k = 0; k < list3.Count; k++)
				{
					Collider collider2 = list3[k];
					EasyColliderData easyColliderData = list2[k];
					if (collider2 is BoxCollider)
					{
						if (easyColliderData is BoxColliderData boxColliderData)
						{
							BoxCollider boxCollider = collider2 as BoxCollider;
							boxColliderData.Center = boxCollider.center;
							boxColliderData.Size = boxCollider.size;
						}
					}
					else if (collider2 is CapsuleCollider)
					{
						if (easyColliderData is CapsuleColliderData capsuleColliderData)
						{
							CapsuleCollider capsuleCollider = collider2 as CapsuleCollider;
							capsuleColliderData.Center = capsuleCollider.center;
							capsuleColliderData.Radius = capsuleCollider.radius;
							capsuleColliderData.Height = capsuleCollider.height;
						}
					}
					else if (collider2 is SphereCollider && easyColliderData is SphereColliderData sphereColliderData)
					{
						SphereCollider sphereCollider = collider2 as SphereCollider;
						sphereColliderData.Center = sphereCollider.center;
						sphereColliderData.Radius = sphereCollider.radius;
					}
				}
			}
			for (int l = 0; l < list3.Count; l++)
			{
				UnityEngine.Object.DestroyImmediate(list3[l]);
			}
			for (int m = 0; m < list.Count; m++)
			{
				UnityEngine.Object.DestroyImmediate(list[m]);
			}
			return list2;
		}

		public List<Collider> GenerateSkinnedMeshColliders(SkinnedMeshRenderer skinnedMesh, SKINNED_MESH_COLLIDER_TYPE colliderType, EasyColliderProperties properties, float minBoneWeight, bool realignBones = false, float minRealignAngle = 20f, string savePath = "Assets/")
		{
			if (skinnedMesh == null)
			{
				return new List<Collider>();
			}
			renderer = skinnedMesh;
			Mesh mesh = new Mesh();
			Vector3 localScale = skinnedMesh.transform.localScale;
			BakeSkinnedMesh(skinnedMesh, mesh);
			Vector3[] vertices = mesh.vertices;
			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i] = skinnedMesh.transform.TransformPoint(vertices[i]);
			}
			skinnedMesh.transform.localScale = localScale;
			EasyColliderAutoSkinnedBone[] array = BoneList.ToArray();
			if (array == null || BoneList.Count == 0)
			{
				return null;
			}
			EasyColliderAutoSkinnedBone[] array2 = array;
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j].WorldSpaceVertices.Clear();
			}
			List<Collider> list = new List<Collider>();
			SetWorldVertices(array, skinnedMesh.sharedMesh.GetAllBoneWeights(), skinnedMesh.sharedMesh.GetBonesPerVertex(), vertices, minBoneWeight);
			Transform[] bones = skinnedMesh.bones;
			array2 = array;
			foreach (EasyColliderAutoSkinnedBone easyColliderAutoSkinnedBone in array2)
			{
				if (!easyColliderAutoSkinnedBone.Enabled || !easyColliderAutoSkinnedBone.IsValid || easyColliderAutoSkinnedBone.renderer != skinnedMesh || easyColliderAutoSkinnedBone == null || easyColliderAutoSkinnedBone.WorldSpaceVertices.Count == 0)
				{
					continue;
				}
				properties.AttachTo = easyColliderAutoSkinnedBone.Transform.gameObject;
				colliderType = easyColliderAutoSkinnedBone.ColliderType;
				if (bones.Length != 0 && realignBones && colliderType != SKINNED_MESH_COLLIDER_TYPE.Convex_Mesh)
				{
					Transform childBone = GetChildBone(easyColliderAutoSkinnedBone.Transform, bones);
					if (childBone != null && GetMinimumChildAngle(easyColliderAutoSkinnedBone.Transform, childBone) >= minRealignAngle)
					{
						properties.AttachTo = CreateRealignedObject(easyColliderAutoSkinnedBone.Transform, childBone);
					}
				}
				Collider collider = GenerateCollider(colliderType, properties, easyColliderAutoSkinnedBone, savePath);
				if (collider != null)
				{
					list.Add(collider);
					easyColliderAutoSkinnedBone.Collider = collider;
				}
			}
			CheckDoDepenetration(list);
			return list;
		}

		private void CheckDoDepenetration(List<Collider> generatedColliders)
		{
			if (!AutoSkinnedDepenetrate)
			{
				return;
			}
			List<Collider> list = null;
			if (AutoSkinnedDepenetrateOrder == SKINNED_MESH_DEPENETRATE_ORDER.OutsideIn || AutoSkinnedDepenetrateOrder == SKINNED_MESH_DEPENETRATE_ORDER.InsideOut)
			{
				list = new List<Collider>(generatedColliders.Count);
				foreach (EasyColliderAutoSkinnedBone item in BoneList.OrderByDescending((EasyColliderAutoSkinnedBone bone) => bone.IndentLevel))
				{
					if (!(item.Collider == null) && item.Enabled && item.IsValid)
					{
						list.Add(item.Collider);
					}
				}
				if (AutoSkinnedDepenetrateOrder == SKINNED_MESH_DEPENETRATE_ORDER.InsideOut)
				{
					list.Reverse();
				}
			}
			else
			{
				list = new List<Collider>(generatedColliders);
				if (AutoSkinnedDepenetrateOrder == SKINNED_MESH_DEPENETRATE_ORDER.Reverse)
				{
					list.Reverse();
				}
			}
			IterativeShrinkAndShift(list);
		}

		private void IterativeShrinkAndShift(List<Collider> generatedColliders)
		{
			for (int i = 0; i < AutoSkinnedIterativeDepenetrationCount; i++)
			{
				ShrinkCount = 0;
				ShiftCount = 0;
				ShrinkAmount = Vector3.zero;
				ShiftAmount = Vector3.zero;
				if (AutoSkinnedShrinkAmount > 0f)
				{
					ShrinkDepenetrate(generatedColliders, AutoSkinnedShrinkAmount);
				}
				ShiftDepenetrate(generatedColliders);
				if (ShrinkCount == 0 && ShiftCount == 0)
				{
					break;
				}
			}
		}

		public string LogVector(Vector3 vector3)
		{
			return "(" + vector3.x + "," + vector3.y + "," + vector3.z + ")";
		}

		private void LogRemainingOverlaps(List<Collider> generatedColliders)
		{
			List<List<ShiftData>> list = new List<List<ShiftData>>();
			foreach (Collider generatedCollider in generatedColliders)
			{
				List<ShiftData> list2 = new List<ShiftData>();
				Transform transform = generatedCollider.transform;
				foreach (Collider generatedCollider2 in generatedColliders)
				{
					if (generatedCollider != generatedCollider2)
					{
						ShiftData data = default(ShiftData);
						if (GetShiftWorld(generatedCollider, transform, generatedCollider2, out data) && !(data.Direction == Vector3.zero) && data.Size != 0f)
						{
							list2.Add(data);
						}
					}
				}
				list.Add(list2);
			}
			Vector3 zero = Vector3.zero;
			int num = 0;
			for (int i = 0; i < generatedColliders.Count; i++)
			{
				if (list[i].Count <= 0)
				{
					continue;
				}
				Vector3 zero2 = Vector3.zero;
				foreach (ShiftData item in list[i])
				{
					Vector3 vector = generatedColliders[i].transform.InverseTransformVector(item.Direction * item.Size);
					vector.x = Mathf.Abs(vector.x);
					vector.y = Mathf.Abs(vector.y);
					vector.z = Mathf.Abs(vector.z);
					zero2.x = Mathf.Max(zero2.x, vector.x);
					zero2.y = Mathf.Max(zero2.y, vector.y);
					zero2.z = Mathf.Max(zero2.z, vector.z);
				}
				if (zero2 != Vector3.zero)
				{
					Debug.Log("Collider:" + generatedColliders[i].transform.name + " Overlaps with:" + list[i].Count + " Max Shift Vector:" + LogVector(zero2));
					zero += zero2;
					num++;
				}
			}
			Debug.Log("Total Overlapped Colliders:" + num + " Total Shift Vector:" + LogVector(zero));
		}

		private void ShrinkDepenetrate(List<Collider> generatedColliders, float mult = 1f)
		{
			foreach (Collider generatedCollider in generatedColliders)
			{
				List<ShiftData> list = new List<ShiftData>(generatedColliders.Count - 1);
				Transform transform = generatedCollider.transform;
				foreach (Collider generatedCollider2 in generatedColliders)
				{
					if (!(generatedCollider == generatedCollider2))
					{
						ShiftData data = default(ShiftData);
						if (GetShiftWorld(generatedCollider, transform, generatedCollider2, out data))
						{
							list.Add(data);
						}
					}
				}
				if (list.Count == 0)
				{
					continue;
				}
				Vector3 zero = Vector3.zero;
				foreach (ShiftData item in list)
				{
					Vector3 vector = transform.InverseTransformVector(item.Direction * item.Size);
					vector.x = Mathf.Abs(vector.x);
					vector.y = Mathf.Abs(vector.y);
					vector.z = Mathf.Abs(vector.z);
					if (vector.x > zero.x)
					{
						zero.x = vector.x;
					}
					if (vector.y > zero.y)
					{
						zero.y = vector.y;
					}
					if (vector.z > zero.z)
					{
						zero.z = vector.z;
					}
				}
				if (!(zero != Vector3.zero))
				{
					continue;
				}
				ShrinkCount++;
				ShrinkAmount += zero;
				if (generatedCollider is BoxCollider)
				{
					BoxCollider obj = generatedCollider as BoxCollider;
					Vector3 size = obj.size;
					size -= zero * mult;
					if (size.x < 0f || size.y < 0f || size.z < 0f)
					{
						size = Vector3.zero;
					}
					obj.size = size;
				}
				else if (generatedCollider is CapsuleCollider)
				{
					CapsuleCollider capsuleCollider = generatedCollider as CapsuleCollider;
					float num = 0f;
					float num2 = 0f;
					if (capsuleCollider.direction == 0)
					{
						num = zero.x;
						num2 = ((zero.y > zero.z) ? zero.y : zero.z);
					}
					else if (capsuleCollider.direction == 1)
					{
						num = zero.y;
						num2 = ((zero.x > zero.z) ? zero.x : zero.z);
					}
					else
					{
						num = zero.z;
						num2 = ((zero.y > zero.x) ? zero.y : zero.x);
					}
					num *= mult;
					num2 *= mult;
					capsuleCollider.height -= num;
					capsuleCollider.radius -= num2;
					if (capsuleCollider.height < 0f || capsuleCollider.radius < 0f)
					{
						capsuleCollider.height = 0f;
						capsuleCollider.radius = 0f;
					}
				}
				else if (generatedCollider is SphereCollider)
				{
					SphereCollider sphereCollider = generatedCollider as SphereCollider;
					sphereCollider.radius -= Mathf.Max(zero.x, Mathf.Max(zero.y, zero.z)) * mult;
					if (sphereCollider.radius < 0f)
					{
						sphereCollider.radius = 0f;
					}
				}
			}
		}

		private void ShiftCollider(Collider c, Vector3 localAmount)
		{
			if (c is BoxCollider)
			{
				(c as BoxCollider).center += localAmount;
			}
			else if (c is CapsuleCollider)
			{
				(c as CapsuleCollider).center += localAmount;
			}
			else if (c is SphereCollider)
			{
				(c as SphereCollider).center += localAmount;
			}
		}

		private void ShiftDepenetrate(List<Collider> generatedColliders)
		{
			foreach (Collider generatedCollider in generatedColliders)
			{
				List<ShiftData> list = new List<ShiftData>(generatedColliders.Count - 1);
				Transform transform = generatedCollider.transform;
				foreach (Collider generatedCollider2 in generatedColliders)
				{
					if (!(generatedCollider == generatedCollider2))
					{
						ShiftData data = default(ShiftData);
						if (GetShiftWorld(generatedCollider, transform, generatedCollider2, out data))
						{
							list.Add(data);
						}
					}
				}
				if (list.Count == 0)
				{
					continue;
				}
				Vector3 zero = Vector3.zero;
				foreach (ShiftData item in list)
				{
					zero += transform.InverseTransformVector(item.Direction * item.Size);
				}
				if (zero != Vector3.zero)
				{
					ShiftCount++;
					ShiftAmount += zero;
					zero /= (float)list.Count;
					ShiftCollider(generatedCollider, zero);
				}
			}
		}

		private bool GetShiftWorld(Collider c, Transform ct, Collider other, out ShiftData data)
		{
			Vector3 direction = Vector3.zero;
			float distance = 0f;
			Transform transform = other.transform;
			if (Physics.ComputePenetration(c, ct.position, ct.rotation, other, transform.position, transform.rotation, out direction, out distance))
			{
				data.Direction = direction;
				data.Size = distance;
				return true;
			}
			data.Direction = Vector3.zero;
			data.Size = -1f;
			return false;
		}

		private Transform GetChildBone(Transform bone, Transform[] bones)
		{
			int childCount = bone.childCount;
			Transform result = null;
			Transform transform = null;
			int num = 0;
			for (int i = 0; i < childCount; i++)
			{
				transform = bone.GetChild(i);
				if (Array.IndexOf(bones, transform) >= 0)
				{
					num++;
					result = transform;
				}
			}
			if (num == 1)
			{
				return result;
			}
			return null;
		}

		private float GetMinimumChildAngle(Transform transform, Transform child)
		{
			Vector3 to = child.position - transform.position;
			float num = float.PositiveInfinity;
			float num2 = Vector3.Angle(transform.right, to);
			num = ((num > num2) ? num2 : num);
			num2 = Vector3.Angle(-transform.right, to);
			num = ((num > num2) ? num2 : num);
			num2 = Vector3.Angle(transform.forward, to);
			num = ((num > num2) ? num2 : num);
			num2 = Vector3.Angle(-transform.forward, to);
			num = ((num > num2) ? num2 : num);
			num2 = Vector3.Angle(transform.up, to);
			num = ((num > num2) ? num2 : num);
			num2 = Vector3.Angle(-transform.up, to);
			return (num > num2) ? num2 : num;
		}

		private EasyColliderAutoSkinnedBone[] GetSkinnedMeshBones(SkinnedMeshRenderer skinnedMesh)
		{
			int num = 0;
			EasyColliderAutoSkinnedBone[] array = null;
			if (skinnedMesh.bones.Length != 0)
			{
				array = new EasyColliderAutoSkinnedBone[skinnedMesh.bones.Length];
				Transform[] bones = skinnedMesh.bones;
				for (int i = 0; i < bones.Length; i++)
				{
					if (bones[i] == null)
					{
						array[i] = new EasyColliderAutoSkinnedBone(default(Matrix4x4), i, bones[i]);
						continue;
					}
					array[i] = new EasyColliderAutoSkinnedBone(bones[i].localToWorldMatrix, i, bones[i]);
					array[i].BoneName = bones[i].name;
					array[i].renderer = skinnedMesh;
					num++;
				}
			}
			return array;
		}

		private void SetWorldVertices(EasyColliderAutoSkinnedBone[] skinnedMeshBones, NativeArray<BoneWeight1> boneWeights, NativeArray<byte> bonesPerVertex, Vector3[] worldVertices, float minBoneWeight)
		{
			int num = 0;
			for (int i = 0; i < worldVertices.Length; i++)
			{
				int num2 = bonesPerVertex[i];
				for (int j = 0; j < num2; j++)
				{
					BoneWeight1 boneWeight = boneWeights[num];
					if (boneWeight.boneIndex < skinnedMeshBones.Length && boneWeight.weight >= skinnedMeshBones[boneWeight.boneIndex].BoneWeight)
					{
						skinnedMeshBones[boneWeight.boneIndex].WorldSpaceVertices.Add(worldVertices[i]);
					}
					num++;
				}
			}
		}

		private void SetWorldVertices(EasyColliderAutoSkinnedBone[] skinnedMeshBones, BoneWeight[] boneWeights, Vector3[] worldVertices)
		{
			for (int i = 0; i < boneWeights.Length; i++)
			{
				if (skinnedMeshBones[boneWeights[i].boneIndex0] != null && boneWeights[i].weight0 >= skinnedMeshBones[boneWeights[i].boneIndex0].BoneWeight)
				{
					skinnedMeshBones[boneWeights[i].boneIndex0].WorldSpaceVertices.Add(worldVertices[i]);
				}
				if (skinnedMeshBones[boneWeights[i].boneIndex1] != null && boneWeights[i].weight1 >= skinnedMeshBones[boneWeights[i].boneIndex1].BoneWeight)
				{
					skinnedMeshBones[boneWeights[i].boneIndex1].WorldSpaceVertices.Add(worldVertices[i]);
				}
				if (skinnedMeshBones[boneWeights[i].boneIndex2] != null && boneWeights[i].weight2 >= skinnedMeshBones[boneWeights[i].boneIndex2].BoneWeight)
				{
					skinnedMeshBones[boneWeights[i].boneIndex2].WorldSpaceVertices.Add(worldVertices[i]);
				}
				if (skinnedMeshBones[boneWeights[i].boneIndex3] != null && boneWeights[i].weight3 >= skinnedMeshBones[boneWeights[i].boneIndex3].BoneWeight)
				{
					skinnedMeshBones[boneWeights[i].boneIndex3].WorldSpaceVertices.Add(worldVertices[i]);
				}
			}
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			for (int i = 0; i < SortedBoneList.Count; i++)
			{
				SortedBoneList[i] = BoneList[SortedBoneList[i].BoneIndex];
			}
		}
	}
}
