using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.Interactor
{
	public class Interactor_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<Interactor_MasterData> dataHolder = new List<Interactor_MasterData>();

		public Interactor_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public Interactor_MasterDataHolder()
			: base(374564527u, "Interactor_MasterDataHolder")
		{
		}

		public void CopyTo(Interactor_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (Interactor_MasterData item in dataHolder)
			{
				Interactor_MasterData interactor_MasterData = new Interactor_MasterData();
				item.CopyTo(interactor_MasterData);
				dest.dataHolder.Add(interactor_MasterData);
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
			foreach (Interactor_MasterData item in dataHolder)
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
				Interactor_MasterData interactor_MasterData = new Interactor_MasterData();
				interactor_MasterData.LoadInternal(br);
				dataHolder.Add(interactor_MasterData);
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
			foreach (Interactor_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(Interactor_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (Interactor_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (Interactor_MasterData item2 in comp.dataHolder)
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

		public Interactor_MasterDataHolder Clone()
		{
			Interactor_MasterDataHolder interactor_MasterDataHolder = new Interactor_MasterDataHolder();
			CopyTo(interactor_MasterDataHolder);
			return interactor_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
