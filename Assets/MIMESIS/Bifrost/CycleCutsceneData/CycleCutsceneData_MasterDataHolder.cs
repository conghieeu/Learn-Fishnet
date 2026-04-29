using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.CycleCutsceneData
{
	public class CycleCutsceneData_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<CycleCutsceneData_MasterData> dataHolder = new List<CycleCutsceneData_MasterData>();

		public CycleCutsceneData_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public CycleCutsceneData_MasterDataHolder()
			: base(1411452649u, "CycleCutsceneData_MasterDataHolder")
		{
		}

		public void CopyTo(CycleCutsceneData_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (CycleCutsceneData_MasterData item in dataHolder)
			{
				CycleCutsceneData_MasterData cycleCutsceneData_MasterData = new CycleCutsceneData_MasterData();
				item.CopyTo(cycleCutsceneData_MasterData);
				dest.dataHolder.Add(cycleCutsceneData_MasterData);
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
			foreach (CycleCutsceneData_MasterData item in dataHolder)
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
				CycleCutsceneData_MasterData cycleCutsceneData_MasterData = new CycleCutsceneData_MasterData();
				cycleCutsceneData_MasterData.LoadInternal(br);
				dataHolder.Add(cycleCutsceneData_MasterData);
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
			foreach (CycleCutsceneData_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(CycleCutsceneData_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (CycleCutsceneData_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (CycleCutsceneData_MasterData item2 in comp.dataHolder)
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

		public CycleCutsceneData_MasterDataHolder Clone()
		{
			CycleCutsceneData_MasterDataHolder cycleCutsceneData_MasterDataHolder = new CycleCutsceneData_MasterDataHolder();
			CopyTo(cycleCutsceneData_MasterDataHolder);
			return cycleCutsceneData_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
