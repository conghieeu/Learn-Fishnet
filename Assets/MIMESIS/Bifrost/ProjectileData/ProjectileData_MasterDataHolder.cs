using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bifrost.ProjectileData
{
	public class ProjectileData_MasterDataHolder : ISchema
	{
		public int versionInfo;

		public List<ProjectileData_MasterData> dataHolder = new List<ProjectileData_MasterData>();

		public ProjectileData_MasterDataHolder(uint msgID, string msgName)
			: base(msgID, msgName)
		{
		}

		public ProjectileData_MasterDataHolder()
			: base(2257361839u, "ProjectileData_MasterDataHolder")
		{
		}

		public void CopyTo(ProjectileData_MasterDataHolder dest)
		{
			dest.versionInfo = versionInfo;
			dest.dataHolder.Clear();
			foreach (ProjectileData_MasterData item in dataHolder)
			{
				ProjectileData_MasterData projectileData_MasterData = new ProjectileData_MasterData();
				item.CopyTo(projectileData_MasterData);
				dest.dataHolder.Add(projectileData_MasterData);
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
			foreach (ProjectileData_MasterData item in dataHolder)
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
				ProjectileData_MasterData projectileData_MasterData = new ProjectileData_MasterData();
				projectileData_MasterData.LoadInternal(br);
				dataHolder.Add(projectileData_MasterData);
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
			foreach (ProjectileData_MasterData item in dataHolder)
			{
				item.SaveInternal(bw);
			}
		}

		public bool Equal(ProjectileData_MasterDataHolder comp)
		{
			if (versionInfo != comp.versionInfo)
			{
				return false;
			}
			if (comp.dataHolder.Count != dataHolder.Count)
			{
				return false;
			}
			foreach (ProjectileData_MasterData item in dataHolder)
			{
				bool flag = false;
				foreach (ProjectileData_MasterData item2 in comp.dataHolder)
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

		public ProjectileData_MasterDataHolder Clone()
		{
			ProjectileData_MasterDataHolder projectileData_MasterDataHolder = new ProjectileData_MasterDataHolder();
			CopyTo(projectileData_MasterDataHolder);
			return projectileData_MasterDataHolder;
		}

		public override void Clean()
		{
			versionInfo = 0;
			dataHolder.Clear();
		}
	}
}
