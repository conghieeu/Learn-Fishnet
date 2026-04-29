using System.Collections.Generic;
using Newtonsoft.Json;

namespace MimicData
{
	public class animationInfoData
	{
		public class PuppetAnimationInfo
		{
			public class Clip
			{
				public class Event
				{
					public string name = "N/A";

					public string parameter = "N/A";

					public float time;
				}

				public string name = "N/A";

				public float length;

				public List<Event> events = new List<Event>();
			}

			public const string dataFileDir = "Assets/_mimic/Resources/data/puppetAnimationInfo";

			public const string dataResourceDir = "data/puppetAnimationInfo";

			public string name = "N/A";

			public List<Clip> clips = new List<Clip>();

			public string ToJson(bool prettyPrint = false)
			{
				return JsonConvert.SerializeObject(this, prettyPrint ? Formatting.Indented : Formatting.None);
			}
		}

		public List<PuppetAnimationInfo> puppetAnimationInfos = new List<PuppetAnimationInfo>();
	}
}
