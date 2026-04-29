using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.Const
{
	public class Const_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<Const_MasterData> dataHolder = new List<Const_MasterData>();

		public Const_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public Const_MasterDataHolder()
			: base(4226926959u, "Const_MasterDataHolder")
		{
		}

		public void CopyTo(Const_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (Const_MasterData item in dataHolder)
			{
				Const_MasterData const_MasterData = new Const_MasterData();
				item.CopyTo(const_MasterData);
				dest.dataHolder.Add(const_MasterData);
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
			foreach (Const_MasterData item in dataHolder)
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
				Const_MasterData const_MasterData = new Const_MasterData();
				const_MasterData.LoadInternal(br);
				dataHolder.Add(const_MasterData);
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
			foreach (Const_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(Const_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (Const_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (Const_MasterData item2 in comp.dataHolder)
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

		public Const_MasterDataHolder Clone()
		{
			Const_MasterDataHolder const_MasterDataHolder = new Const_MasterDataHolder();
			CopyTo(const_MasterDataHolder);
			return const_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
