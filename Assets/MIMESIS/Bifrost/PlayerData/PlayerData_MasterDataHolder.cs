using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.PlayerData
{
	public class PlayerData_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<PlayerData_MasterData> dataHolder = new List<PlayerData_MasterData>();

		public PlayerData_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public PlayerData_MasterDataHolder()
			: base(4003468917u, "PlayerData_MasterDataHolder")
		{
		}

		public void CopyTo(PlayerData_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (PlayerData_MasterData item in dataHolder)
			{
				PlayerData_MasterData playerData_MasterData = new PlayerData_MasterData();
				item.CopyTo(playerData_MasterData);
				dest.dataHolder.Add(playerData_MasterData);
			}
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			int num = 0;
			num += Serializer.GetLength(versionInfo);
			num += 4;
			foreach (PlayerData_MasterData item in dataHolder)
			{
				num += item.GetLengthInternal();
			}
			return num;
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
			Serializer.Load(br, ref versionInfo);
			int intValue = 0;
			Serializer.Load(br, ref intValue);
			while (intValue-- > 0)
			{
				PlayerData_MasterData playerData_MasterData = new PlayerData_MasterData();
				playerData_MasterData.LoadInternal(br);
				dataHolder.Add(playerData_MasterData);
			}
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, versionInfo);
			Serializer.Save(bw, dataHolder.Count);
			foreach (PlayerData_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(PlayerData_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (PlayerData_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (PlayerData_MasterData item2 in comp.dataHolder)
				{
					if (item.Equal(item2))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		public PlayerData_MasterDataHolder Clone()
		{
			PlayerData_MasterDataHolder playerData_MasterDataHolder = new PlayerData_MasterDataHolder();
			CopyTo(playerData_MasterDataHolder);
			return playerData_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
