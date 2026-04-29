using Bifrost.MapData;

namespace Bifrost.Cooked
{
	public class MapMasterInfo
	{
		public int MasterID { get; set; }

		public string Desc { get; set; }

		public int SizeX { get; set; }

		public int SizeY { get; set; }

		public int SectorSize { get; set; }

		public string SceneName { get; set; }

		public MapMasterInfo(MapData_MasterData masterData)
		{
			MasterID = masterData.id;
			SceneName = masterData.scene_name;
			SizeX = (int)masterData.width;
			SizeY = (int)masterData.height;
			SectorSize = (int)masterData.sector_size;
		}
	}
}
