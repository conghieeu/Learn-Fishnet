using System;
using System.IO;
using System.Net;

namespace Bifrost.MapData
{
	public class MapData_MasterData : ISchema
	{
		public int id;

		public string scene_name = string.Empty;

		public long width;

		public long height;

		public long sector_size;

		public MapData_MasterData(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public MapData_MasterData()
			: base(2452349076u, "MapData_MasterData")
		{
		}

		public void CopyTo(MapData_MasterData dest)
		{
			dest.id = id;
			dest.scene_name = scene_name;
			dest.width = width;
			dest.height = height;
			dest.sector_size = sector_size;
		}

		public override int GetLength()
		{
			return Serializer.GetLength(MsgID) + GetLengthInternal();
		}

		public int GetLengthInternal()
		{
			return 0 + Serializer.GetLength(id) + Serializer.GetLength(scene_name) + Serializer.GetLength(width) + Serializer.GetLength(height) + Serializer.GetLength(sector_size);
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
			Serializer.Load(br, ref scene_name);
			Serializer.Load(br, ref width);
			Serializer.Load(br, ref height);
			Serializer.Load(br, ref sector_size);
		}

		public override void Save(BinaryWriter bw)
		{
			Serializer.Save(bw, IPAddress.HostToNetworkOrder((int)MsgID));
			SaveInternal(bw);
		}

		public void SaveInternal(BinaryWriter bw)
		{
			Serializer.Save(bw, id);
			Serializer.Save(bw, scene_name);
			Serializer.Save(bw, width);
			Serializer.Save(bw, height);
			Serializer.Save(bw, sector_size);
		}

		public bool Equal(MapData_MasterData comp)
		{
			if (id != comp.id)
			{
				return false;
			}
			if (scene_name != comp.scene_name)
			{
				return false;
			}
			if (width != comp.width)
			{
				return false;
			}
			if (height != comp.height)
			{
				return false;
			}
			if (sector_size != comp.sector_size)
			{
				return false;
			}
			return true;
		}

		public MapData_MasterData Clone()
		{
			MapData_MasterData mapData_MasterData = new MapData_MasterData();
			CopyTo(mapData_MasterData);
			return mapData_MasterData;
		}

		public override void Clean()
		{
			id = 0;
			scene_name = string.Empty;
			width = 0L;
			height = 0L;
			sector_size = 0L;
		}
	}
}
