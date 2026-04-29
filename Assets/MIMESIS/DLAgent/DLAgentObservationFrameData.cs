using System.Collections.Generic;
using System.Linq;
using Mimic.Actors;
using UnityEngine;

namespace DLAgent
{
	public class DLAgentObservationFrameData
	{
		public float time;

		public float obs_actor_w_pos_x;

		public float obs_actor_w_pos_y;

		public float obs_actor_w_pos_z;

		public float obs_actor_w_rot_x;

		public float obs_actor_w_rot_y;

		public float obs_actor_w_rot_z;

		public bool obs_actor_flashlight_on;

		public int obs_actor_area;

		public float obs_target_w_pos_x;

		public float obs_target_w_pos_y;

		public float obs_target_w_pos_z;

		public int[,] obs_actor_raysensor_tags_per_range = new int[19, 5];

		public float obs_distance_to_target_too_far;

		public float obs_distance_to_target_too_near;

		public float obs_future_w_rot_x;

		public float obs_future_w_rot_y;

		public float obs_future_w_rot_z;

		public float[] mu_from_prev_ouput = new float[4];

		public float[] logvar_from_prev_ouput = new float[4];

		public static DLAgentObservationFrameData MakeFrameData(ProtoActor avatar, VCreature mimicCreature, VCreature targetCreature, ProtoActor targetProtoActor, float distanceToTargetTooFar, float distanceToTargetTooNear, Vector3 futureRot, float[] zFromPrevOutput, float lambda)
		{
			Vector3 zero = Vector3.zero;
			if (targetCreature != null)
			{
				zero.x = targetCreature.Position.x;
				zero.y = targetCreature.Position.y;
				zero.z = targetCreature.Position.z;
			}
			else
			{
				zero.x = targetProtoActor.transform.position.x;
				zero.y = targetProtoActor.transform.position.y;
				zero.z = targetProtoActor.transform.position.z;
			}
			return MakeFrameData(avatar, mimicCreature, zero, distanceToTargetTooFar, distanceToTargetTooNear, futureRot, zFromPrevOutput, lambda);
		}

		public static DLAgentObservationFrameData MakeFrameData(ActorFrameData frame, long targetUID)
		{
			DLAgentObservationFrameData dLAgentObservationFrameData = new DLAgentObservationFrameData();
			dLAgentObservationFrameData.time = frame.Time;
			dLAgentObservationFrameData.obs_actor_w_pos_x = frame.ActorInfo.position.x;
			dLAgentObservationFrameData.obs_actor_w_pos_y = frame.ActorInfo.position.y;
			dLAgentObservationFrameData.obs_actor_w_pos_z = frame.ActorInfo.position.z;
			dLAgentObservationFrameData.obs_actor_w_rot_y = frame.ActorInfo.rotation.y;
			dLAgentObservationFrameData.obs_actor_flashlight_on = frame.ActorInfo.UseLight;
			dLAgentObservationFrameData.obs_actor_area = frame.ActorInfo.Area;
			TargetActorInfo targetActorInfo = frame.ActorInfo.TargetActorArray.FirstOrDefault((TargetActorInfo t) => t.UID == targetUID);
			dLAgentObservationFrameData.obs_target_w_pos_x = targetActorInfo.Position.x;
			dLAgentObservationFrameData.obs_target_w_pos_y = targetActorInfo.Position.y;
			dLAgentObservationFrameData.obs_target_w_pos_z = targetActorInfo.Position.z;
			int length = dLAgentObservationFrameData.obs_actor_raysensor_tags_per_range.GetLength(0);
			int length2 = dLAgentObservationFrameData.obs_actor_raysensor_tags_per_range.GetLength(1);
			for (int num = 0; num < length; num++)
			{
				List<int> hitTypes = frame.ActorInfo.raySensorResults[num].hitTypes;
				for (int num2 = 0; num2 < length2; num2++)
				{
					if (num2 < hitTypes.Count)
					{
						dLAgentObservationFrameData.obs_actor_raysensor_tags_per_range[num, num2] = hitTypes[num2];
						continue;
					}
					dLAgentObservationFrameData.obs_actor_raysensor_tags_per_range[num, num2] = 0;
					Debug.Log($"[RaySensor] 결과[{num}]의 hitTypes.Count({hitTypes.Count}) < 인덱스 j({num2}) — " + "값이 없으므로 None(0)으로 셋팅합니다.");
				}
			}
			dLAgentObservationFrameData.obs_distance_to_target_too_far = frame.ActorInfo.distanceToTargetTooFar;
			dLAgentObservationFrameData.obs_distance_to_target_too_near = frame.ActorInfo.distnaceToTargetTooNear;
			dLAgentObservationFrameData.obs_future_w_rot_x = frame.ActorInfo.futureRot.x;
			dLAgentObservationFrameData.obs_future_w_rot_y = frame.ActorInfo.futureRot.y;
			dLAgentObservationFrameData.obs_future_w_rot_z = frame.ActorInfo.futureRot.z;
			for (int num3 = 0; num3 < 4; num3++)
			{
				dLAgentObservationFrameData.mu_from_prev_ouput[num3] = 0f;
				dLAgentObservationFrameData.logvar_from_prev_ouput[num3] = 0f;
			}
			return dLAgentObservationFrameData;
		}

		public static DLAgentObservationFrameData MakeFrameData(ProtoActor avatar, VCreature mimicCreature, Vector3 targetPos, float distanceToTargetTooFar, float distanceToTargetTooNear, Vector3 futureRot, float[] zFromPrevOutput, float lambda)
		{
			DLAgentObservationFrameData dLAgentObservationFrameData = new DLAgentObservationFrameData();
			dLAgentObservationFrameData.time = Time.time;
			Vector3 vector = mimicCreature?.PositionVector ?? avatar.transform.position;
			float rotation = ((mimicCreature != null) ? mimicCreature.Position.yaw : avatar.transform.rotation.eulerAngles.y);
			dLAgentObservationFrameData.obs_actor_w_pos_x = vector.x;
			dLAgentObservationFrameData.obs_actor_w_pos_y = vector.y;
			dLAgentObservationFrameData.obs_actor_w_pos_z = vector.z;
			dLAgentObservationFrameData.obs_actor_w_rot_x = 0f;
			dLAgentObservationFrameData.obs_actor_w_rot_y = rotation;
			dLAgentObservationFrameData.obs_actor_w_rot_z = 0f;
			dLAgentObservationFrameData.obs_actor_area = Hub.s.dLAcademyManager.GetAreaForDL(vector, out var _);
			dLAgentObservationFrameData.obs_actor_flashlight_on = avatar.isLightOn;
			dLAgentObservationFrameData.obs_target_w_pos_x = targetPos.x;
			dLAgentObservationFrameData.obs_target_w_pos_y = targetPos.y;
			dLAgentObservationFrameData.obs_target_w_pos_z = targetPos.z;
			RaySensorHitResult[] array = avatar.raySensor.Shoot(vector, rotation);
			int length = dLAgentObservationFrameData.obs_actor_raysensor_tags_per_range.GetLength(0);
			int length2 = dLAgentObservationFrameData.obs_actor_raysensor_tags_per_range.GetLength(1);
			for (int i = 0; i < length; i++)
			{
				List<int> hitTypes = array[i].hitTypes;
				for (int j = 0; j < length2; j++)
				{
					if (j < hitTypes.Count)
					{
						dLAgentObservationFrameData.obs_actor_raysensor_tags_per_range[i, j] = hitTypes[j];
					}
					else
					{
						dLAgentObservationFrameData.obs_actor_raysensor_tags_per_range[i, j] = 0;
					}
				}
			}
			dLAgentObservationFrameData.obs_distance_to_target_too_far = distanceToTargetTooFar;
			dLAgentObservationFrameData.obs_distance_to_target_too_near = distanceToTargetTooNear;
			dLAgentObservationFrameData.obs_future_w_rot_x = futureRot.x;
			dLAgentObservationFrameData.obs_future_w_rot_y = futureRot.y;
			dLAgentObservationFrameData.obs_future_w_rot_z = futureRot.z;
			for (int k = 0; k < 4; k++)
			{
				dLAgentObservationFrameData.mu_from_prev_ouput[k] = zFromPrevOutput[k] * lambda;
				dLAgentObservationFrameData.logvar_from_prev_ouput[k] = 0.5f * Mathf.Log(1f - lambda * lambda);
			}
			return dLAgentObservationFrameData;
		}

		public DLAgentObservationFrameData Clone()
		{
			DLAgentObservationFrameData dLAgentObservationFrameData = new DLAgentObservationFrameData();
			dLAgentObservationFrameData.time = time;
			dLAgentObservationFrameData.obs_actor_w_pos_x = obs_actor_w_pos_x;
			dLAgentObservationFrameData.obs_actor_w_pos_y = obs_actor_w_pos_y;
			dLAgentObservationFrameData.obs_actor_w_pos_z = obs_actor_w_pos_z;
			dLAgentObservationFrameData.obs_actor_w_rot_x = obs_actor_w_rot_x;
			dLAgentObservationFrameData.obs_actor_w_rot_y = obs_actor_w_rot_y;
			dLAgentObservationFrameData.obs_actor_w_rot_z = obs_actor_w_rot_z;
			dLAgentObservationFrameData.obs_actor_flashlight_on = obs_actor_flashlight_on;
			dLAgentObservationFrameData.obs_actor_area = obs_actor_area;
			dLAgentObservationFrameData.obs_target_w_pos_x = obs_target_w_pos_x;
			dLAgentObservationFrameData.obs_target_w_pos_y = obs_target_w_pos_y;
			dLAgentObservationFrameData.obs_target_w_pos_z = obs_target_w_pos_z;
			int length = obs_actor_raysensor_tags_per_range.GetLength(0);
			int length2 = obs_actor_raysensor_tags_per_range.GetLength(1);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					dLAgentObservationFrameData.obs_actor_raysensor_tags_per_range[i, j] = obs_actor_raysensor_tags_per_range[i, j];
				}
			}
			dLAgentObservationFrameData.obs_distance_to_target_too_far = obs_distance_to_target_too_far;
			dLAgentObservationFrameData.obs_distance_to_target_too_near = obs_distance_to_target_too_near;
			dLAgentObservationFrameData.obs_future_w_rot_x = obs_future_w_rot_x;
			dLAgentObservationFrameData.obs_future_w_rot_y = obs_future_w_rot_y;
			dLAgentObservationFrameData.obs_future_w_rot_z = obs_future_w_rot_z;
			for (int k = 0; k < 4; k++)
			{
				dLAgentObservationFrameData.mu_from_prev_ouput[k] = mu_from_prev_ouput[k];
				dLAgentObservationFrameData.logvar_from_prev_ouput[k] = logvar_from_prev_ouput[k];
			}
			return dLAgentObservationFrameData;
		}
	}
}
