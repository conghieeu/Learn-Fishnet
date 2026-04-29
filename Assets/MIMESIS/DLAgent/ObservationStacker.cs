using System;
using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace DLAgent
{
	public class ObservationStacker
	{
		public enum CoordMode
		{
			LocalRelativeToCurrent = 0,
			World = 1
		}

		private const int RAY_COUNT = 19;

		private const int RAYS_WITH_RANGES = 16;

		private const int RANGE_BINS = 5;

		private const int RAY_TAG_CLASSES = 5;

		private const int MU_DIM = 4;

		private const int LOGVAR_DIM = 4;

		private DLAgentObservationFrameData[] observationStack;

		private int stackSize;

		private int currentIndex;

		private int observationSize;

		private float rotationScaleFactor = 360f;

		private bool _useWorldCoord;

		private static int PerFrameSize()
		{
			return 434;
		}

		public ObservationStacker(int stackSize, int inObservationSize, float angScaleFactor, bool useWorldCoord)
		{
			if (stackSize <= 0)
			{
				throw new ArgumentException("Stack size must be greater than zero.");
			}
			this.stackSize = stackSize;
			observationStack = new DLAgentObservationFrameData[stackSize];
			currentIndex = 0;
			observationSize = inObservationSize;
			rotationScaleFactor = angScaleFactor;
			_useWorldCoord = useWorldCoord;
		}

		public void AddObservation(DLAgentObservationFrameData newObservation)
		{
			observationStack[currentIndex] = newObservation;
			currentIndex = (currentIndex + 1) % stackSize;
		}

		private float CheckEpsilon(float value, float epsilon = 1E-05f)
		{
			if (!(Mathf.Abs(value) < epsilon))
			{
				return value;
			}
			return 0f;
		}

		public void GetStackedObservations(VectorSensor sensor)
		{
			List<DLAgentObservationFrameData> list = new List<DLAgentObservationFrameData>(stackSize);
			int num = currentIndex;
			for (int i = 0; i < stackSize; i++)
			{
				if (observationStack[num] != null)
				{
					list.Add(observationStack[num]);
				}
				num = (num + 1) % stackSize;
			}
			int num2 = stackSize - list.Count;
			for (int j = 0; j < num2; j++)
			{
				for (int k = 0; k < observationSize; k++)
				{
					sensor.AddObservation(0f);
				}
			}
			if (list.Count == 0)
			{
				return;
			}
			Vector3 vector = default(Vector3);
			float num3 = 0f;
			Quaternion quaternion = Quaternion.identity;
			DLAgentObservationFrameData dLAgentObservationFrameData = list[list.Count - 1];
			CoordMode coordMode = (_useWorldCoord ? GetCoordMode(new Vector3(dLAgentObservationFrameData.obs_actor_w_pos_x, dLAgentObservationFrameData.obs_actor_w_pos_y, dLAgentObservationFrameData.obs_actor_w_pos_z)) : CoordMode.LocalRelativeToCurrent);
			if (coordMode == CoordMode.LocalRelativeToCurrent)
			{
				vector = new Vector3(dLAgentObservationFrameData.obs_actor_w_pos_x, dLAgentObservationFrameData.obs_actor_w_pos_y, dLAgentObservationFrameData.obs_actor_w_pos_z);
				num3 = dLAgentObservationFrameData.obs_actor_w_rot_y;
				quaternion = Quaternion.Inverse(Quaternion.Euler(0f, num3, 0f));
			}
			foreach (DLAgentObservationFrameData item in list)
			{
				Vector3 vector2 = new Vector3(item.obs_actor_w_pos_x, item.obs_actor_w_pos_y, item.obs_actor_w_pos_z);
				Vector3 vector3 = new Vector3(item.obs_target_w_pos_x, item.obs_target_w_pos_y, item.obs_target_w_pos_z);
				float observation;
				float observation2;
				float observation3;
				float observation4;
				float observation5;
				if (coordMode == CoordMode.LocalRelativeToCurrent)
				{
					Vector3 vector4 = quaternion * (vector2 - vector);
					Vector3 vector5 = quaternion * (vector3 - vector);
					observation = CheckEpsilon(vector4.x);
					observation2 = CheckEpsilon(vector4.z);
					observation3 = CheckEpsilon(vector5.x);
					observation4 = CheckEpsilon(vector5.z);
					observation5 = CheckEpsilon(Mathf.DeltaAngle(num3, item.obs_actor_w_rot_y) / rotationScaleFactor);
				}
				else
				{
					observation = CheckEpsilon(item.obs_actor_w_pos_x);
					observation2 = CheckEpsilon(item.obs_actor_w_pos_z);
					observation3 = CheckEpsilon(item.obs_target_w_pos_x);
					observation4 = CheckEpsilon(item.obs_target_w_pos_z);
					observation5 = CheckEpsilon(Mathf.DeltaAngle(0f, item.obs_actor_w_rot_y) / rotationScaleFactor);
				}
				sensor.AddObservation(observation);
				sensor.AddObservation(observation2);
				sensor.AddObservation(observation5);
				sensor.AddObservation(observation3);
				sensor.AddObservation(observation4);
				sensor.AddObservation(item.obs_actor_area);
				sensor.AddObservation(0f);
				sensor.AddObservation(item.obs_actor_flashlight_on);
				for (int l = 0; l < 19; l++)
				{
					if (l < 16)
					{
						for (int m = 0; m < 5; m++)
						{
							sensor.AddOneHotObservation(item.obs_actor_raysensor_tags_per_range[l, m], 5);
						}
					}
					else
					{
						sensor.AddOneHotObservation(item.obs_actor_raysensor_tags_per_range[l, 0], 5);
					}
				}
				sensor.AddObservation(item.obs_distance_to_target_too_far);
				sensor.AddObservation(item.obs_distance_to_target_too_near);
				sensor.AddObservation(item.obs_future_w_rot_y);
				for (int n = 0; n < 4; n++)
				{
					sensor.AddObservation(item.mu_from_prev_ouput[n]);
				}
				for (int num4 = 0; num4 < 4; num4++)
				{
					sensor.AddObservation(item.logvar_from_prev_ouput[num4]);
				}
			}
		}

		public void Reset()
		{
			Array.Clear(observationStack, 0, observationStack.Length);
			currentIndex = 0;
		}

		public CoordMode GetCoordMode(Vector3 pos)
		{
			if (Hub.s == null)
			{
				return CoordMode.LocalRelativeToCurrent;
			}
			if (Hub.s.dLAcademyManager == null)
			{
				return CoordMode.LocalRelativeToCurrent;
			}
			if (Hub.s.dLAcademyManager.GetAreaForDL(pos, out var _) == 1)
			{
				DLDecisionAgent.OutdoorArea outdoorArea = Hub.s.dLAcademyManager.GetOutdoorArea(pos);
				if (outdoorArea == DLDecisionAgent.OutdoorArea.MainStreet || outdoorArea == DLDecisionAgent.OutdoorArea.MainDoorOutside || outdoorArea == DLDecisionAgent.OutdoorArea.BackDoorOutside || outdoorArea == DLDecisionAgent.OutdoorArea.Tram || outdoorArea == DLDecisionAgent.OutdoorArea.TramInside || outdoorArea == DLDecisionAgent.OutdoorArea.MainStreetFromBackdoor)
				{
					return CoordMode.World;
				}
				return CoordMode.LocalRelativeToCurrent;
			}
			return CoordMode.LocalRelativeToCurrent;
		}

		public void OverwriteWithStraightLine(DLAgentObservationFrameData current, Vector3 forwardDir, float stepDistance)
		{
			Vector3 normalized = forwardDir.normalized;
			float obs_actor_w_rot_y = Mathf.Atan2(normalized.x, normalized.z) * 57.29578f;
			for (int i = 0; i < stackSize; i++)
			{
				DLAgentObservationFrameData dLAgentObservationFrameData = current.Clone();
				float num = stepDistance * (float)(stackSize - 1 - i);
				dLAgentObservationFrameData.obs_actor_w_pos_x = current.obs_actor_w_pos_x - normalized.x * num;
				dLAgentObservationFrameData.obs_actor_w_pos_z = current.obs_actor_w_pos_z - normalized.z * num;
				dLAgentObservationFrameData.obs_actor_w_rot_y = obs_actor_w_rot_y;
				observationStack[i] = dLAgentObservationFrameData;
			}
			currentIndex = 0;
		}
	}
}
