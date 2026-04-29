using System.Collections.Immutable;
using Bifrost.GrabData;

public class AttachMasterInfo
{
	public readonly int MasterID;

	public bool IsReverseGrab;

	public int SocketCount;

	public ImmutableList<GrabData_socket_info> SocketInfoList;

	public long GrabAttachDuration;

	public long GrabDetachDuration;

	public long Duration;

	public int CasterStartTimeAbnormalID;

	public int CasterEndTimeAbnormalID;

	public int VictimStartTimeAbnormalID;

	public int VictimEndTimeAbnormalID;

	public int CasterForceDetachAbnormalID;

	public int VictimForceDetachAbnormalID;

	public ImmutableList<ImmuneElementInfo> CasterImmunes;

	public ImmutableList<ImmuneElementInfo> VictimImmunes;

	public string UngrabAnimationStateName = string.Empty;

	public bool CanUseSkill;

	public readonly bool FrontDrop;

	public readonly float DropDistance;

	public AttachMasterInfo(GrabData_MasterData data)
	{
		MasterID = data.id;
		IsReverseGrab = data.is_reverse_grab;
		SocketCount = data.socket_count;
		ImmutableList<GrabData_socket_info>.Builder builder = ImmutableList.CreateBuilder<GrabData_socket_info>();
		foreach (GrabData_socket_info item in data.GrabData_socket_infoval)
		{
			builder.Add(item);
		}
		SocketInfoList = builder.ToImmutable();
		GrabAttachDuration = data.grab_attach_duration;
		GrabDetachDuration = data.grab_detatch_duration;
		Duration = data.duration;
		CasterStartTimeAbnormalID = data.caster_starttime_abnormal_id;
		CasterEndTimeAbnormalID = data.caster_endtime_abnormal_id;
		VictimStartTimeAbnormalID = data.victim_starttime_abnormal_id;
		VictimEndTimeAbnormalID = data.victim_endtime_abnormal_id;
		CasterForceDetachAbnormalID = data.caster_force_detach_abnormal_id;
		VictimForceDetachAbnormalID = data.victim_force_detach_abnormal_id;
		ImmutableList<ImmuneElementInfo>.Builder builder2 = ImmutableList.CreateBuilder<ImmuneElementInfo>();
		foreach (string item2 in data.caster_ongrab_immune)
		{
			builder2.Add(new ImmuneElementInfo(item2));
		}
		CasterImmunes = builder2.ToImmutable();
		ImmutableList<ImmuneElementInfo>.Builder builder3 = ImmutableList.CreateBuilder<ImmuneElementInfo>();
		foreach (string item3 in data.victim_ongrab_immune)
		{
			builder3.Add(new ImmuneElementInfo(item3));
		}
		VictimImmunes = builder3.ToImmutable();
		FrontDrop = data.is_victim_ungrap_direction_same_caster;
		DropDistance = data.victim_ungrap_distance;
	}
}
