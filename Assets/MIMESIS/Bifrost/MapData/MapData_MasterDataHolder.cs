using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.MapData
{
	public class MapData_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<MapData_MasterData> dataHolder = new List<MapData_MasterData>();

		public MapData_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public MapData_MasterDataHolder()
			: base(1583666409u, "MapData_MasterDataHolder")
		{
		}

		public void CopyTo(MapData_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (MapData_MasterData item in dataHolder)
			{
				MapData_MasterData mapData_MasterData = new MapData_MasterData();
				item.CopyTo(mapData_MasterData);
				dest.dataHolder.Add(mapData_MasterData);
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
			foreach (MapData_MasterData item in dataHolder)
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
				MapData_MasterData mapData_MasterData = new MapData_MasterData();
				mapData_MasterData.LoadInternal(br);
				dataHolder.Add(mapData_MasterData);
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
			foreach (MapData_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(MapData_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (MapData_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (MapData_MasterData item2 in comp.dataHolder)
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

		public MapData_MasterDataHolder Clone()
		{
			MapData_MasterDataHolder mapData_MasterDataHolder = new MapData_MasterDataHolder();
			CopyTo(mapData_MasterDataHolder);
			return mapData_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
