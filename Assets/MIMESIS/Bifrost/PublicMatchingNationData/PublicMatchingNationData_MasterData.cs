using System;
using System.IO;
using System.Net;

namespace Bifrost.PublicMatchingNationData
{
	public class PublicMatchingNationData_MasterData : ISchema
	{
		public int id;

		public string steam_nation_code = string.Empty;

		public string flag_icon_name = string.Empty;

		public PublicMatchingNationData_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public PublicMatchingNationData_MasterData()
			: base(3249137478u, "PublicMatchingNationData_MasterData")
		{
		}

		public void CopyTo(PublicMatchingNationData_MasterData dest)
		{
			dest.id = id;
			dest.steam_nation_code = steam_nation_code;
			dest.flag_icon_name = flag_icon_name;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			return 0 + Serializer.GetLength(id) + Serializer.GetLength(steam_nation_code) + Serializer.GetLength(flag_icon_name);
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
			Serializer.Load(br, ref id);
			Serializer.Load(br, ref steam_nation_code);
			Serializer.Load(br, ref flag_icon_name);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, id);
			Serializer.Save(bw, steam_nation_code);
			Serializer.Save(bw, flag_icon_name);
		}

		public bool Equal(PublicMatchingNationData_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (steam_nation_code != comp.steam_nation_code)
			{
				return false;
			}
			if (flag_icon_name != comp.flag_icon_name)
			{
				return false;
			}
			return true;
		}

		public PublicMatchingNationData_MasterData Clone()
		{
			PublicMatchingNationData_MasterData publicMatchingNationData_MasterData = new PublicMatchingNationData_MasterData();
			CopyTo(publicMatchingNationData_MasterData);
			return publicMatchingNationData_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			steam_nation_code = string.Empty;
			flag_icon_name = string.Empty;
		}
	}
}
