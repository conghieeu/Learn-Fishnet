using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.BattleActionData
{
	public class BattleActionData_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<BattleActionData_MasterData> dataHolder = new List<BattleActionData_MasterData>();

		public BattleActionData_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public BattleActionData_MasterDataHolder()
			: base(2538373921u, "BattleActionData_MasterDataHolder")
		{
		}

		public void CopyTo(BattleActionData_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (BattleActionData_MasterData item in dataHolder)
			{
				BattleActionData_MasterData battleActionData_MasterData = new BattleActionData_MasterData();
				item.CopyTo(battleActionData_MasterData);
				dest.dataHolder.Add(battleActionData_MasterData);
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
			foreach (BattleActionData_MasterData item in dataHolder)
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
				BattleActionData_MasterData battleActionData_MasterData = new BattleActionData_MasterData();
				battleActionData_MasterData.LoadInternal(br);
				dataHolder.Add(battleActionData_MasterData);
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
			foreach (BattleActionData_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(BattleActionData_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (BattleActionData_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (BattleActionData_MasterData item2 in comp.dataHolder)
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

		public BattleActionData_MasterDataHolder Clone()
		{
			BattleActionData_MasterDataHolder battleActionData_MasterDataHolder = new BattleActionData_MasterDataHolder();
			CopyTo(battleActionData_MasterDataHolder);
			return battleActionData_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
