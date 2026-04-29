using System;
using System.IO;
using System.Net;

namespace Bifrost.SpawnableMiscGroup
{
	public class SpawnableMiscGroup_candidate : ISchema
	{
		public int misc_id;

		public int rate;

		public SpawnableMiscGroup_candidate(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public SpawnableMiscGroup_candidate()
			: base(1013246949u, "SpawnableMiscGroup_candidate")
		{
		}

		public void CopyTo(SpawnableMiscGroup_candidate dest)
		{
			dest.misc_id = misc_id;
			dest.rate = rate;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			return 0 + Serializer.GetLength(misc_id) + Serializer.GetLength(rate);
		}

		public override bool Load(BinaryReader br)
		{
			uint uintValue = 0u;
			Serializer.Load(br, ref uintValue);
			if (MsgID != (uint)IPAddress.NetworkToHostOrder((int)uintValue))
			{
				return false;
			}
			try
			{
				LoadInternal(br);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public void LoadInternal(BinaryReader br)
		{
			Serializer.Load(br, ref misc_id);
			Serializer.Load(br, ref rate);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, misc_id);
			Serializer.Save(bw, rate);
		}

		public bool Equal(SpawnableMiscGroup_candidate comp)
		{
			if (misc_id != comp.misc_id)
			{
				return false;
			}
			if (rate != comp.rate)
			{
				return false;
			}
			return true;
		}

		public SpawnableMiscGroup_candidate Clone()
		{
			SpawnableMiscGroup_candidate spawnableMiscGroup_candidate = new SpawnableMiscGroup_candidate();
			CopyTo(spawnableMiscGroup_candidate);
			return spawnableMiscGroup_candidate;
		}

		public override void Clean()
		{
			misc_id = 0;
			rate = 0;
		}
	}
}
