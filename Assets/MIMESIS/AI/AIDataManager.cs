using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Xml;

public class AIDataManager
{
	public struct BTNameKey : IEquatable<BTNameKey>
	{
		public readonly string AIName;

		public readonly string BTName;

		public BTNameKey(string aiName, string btName)
		{
			AIName = aiName.ToLower();
			BTName = btName.ToLower();
		}

		public override bool Equals(object? obj)
		{
			if (obj is BTNameKey other)
			{
				return Equals(other);
			}
			return false;
		}

		public bool Equals(BTNameKey other)
		{
			if (AIName == other.AIName)
			{
				return BTName == other.BTName;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(AIName, BTName);
		}
	}

	private class ParamData
	{
		public readonly string BTName;

		public readonly int Phase;

		public readonly Dictionary<string, string> Params;

		public ParamData(string btName, int phase, Dictionary<string, string> params_)
		{
			BTName = btName;
			Phase = phase;
			Params = params_;
		}
	}

	private Dictionary<BTKey, BTData> BTData;

	private Dictionary<BTNameKey, BTData> BTNameData;

	private List<BTKey> BotAvatarBTs;

	private ResourceDataHandler? _dataHandler;

	public AIDataManager(ResourceDataHandler resourceDataHandler)
	{
		_dataHandler = resourceDataHandler;
	}

	public void Dispose()
	{
		BTData.Clear();
		BTNameData.Clear();
		BotAvatarBTs.Clear();
	}

	public bool LoadLocalData(BehaviorTreeBuilder.BuilderConfig config)
	{
		try
		{
			Dictionary<string, XmlNode> btXmlData = LoadBT();
			Dictionary<string, Dictionary<string, ParamData>> paramData = LoadParam();
			(Dictionary<BTKey, BTData>, Dictionary<BTNameKey, BTData>) tuple = Merge(config, btXmlData, paramData);
			Dictionary<BTKey, BTData> item = tuple.Item1;
			Dictionary<BTNameKey, BTData> item2 = tuple.Item2;
			ImmutableList<BTKey> source = item.Keys.Where((BTKey x) => x.AIName.Contains("botavatar".ToLower()) && x.TemplateName != "active".ToLower()).ToImmutableList();
			BTData = item;
			BTNameData = item2;
			BotAvatarBTs = source.ToList();
			return true;
		}
		catch (Exception ex)
		{
			Logger.RError("AIData [BT] Error. " + ex.Message);
			return false;
		}
	}

	private (Dictionary<BTKey, BTData>, Dictionary<BTNameKey, BTData>) Merge(BehaviorTreeBuilder.BuilderConfig config, Dictionary<string, XmlNode> btXmlData, Dictionary<string, Dictionary<string, ParamData>> paramData)
	{
		Dictionary<BTKey, BTData> dictionary = new Dictionary<BTKey, BTData>();
		Dictionary<BTNameKey, BTData> dictionary2 = new Dictionary<BTNameKey, BTData>();
		HashSet<string> hashSet = new HashSet<string>();
		string key;
		foreach (KeyValuePair<string, Dictionary<string, ParamData>> paramDatum in paramData)
		{
			paramDatum.Deconstruct(out key, out var value);
			string text = key;
			foreach (KeyValuePair<string, ParamData> item in value)
			{
				item.Deconstruct(out key, out var value2);
				string text2 = key;
				ParamData paramData2 = value2;
				if (!btXmlData.TryGetValue(paramData2.BTName.ToLower(), out XmlNode value3))
				{
					Logger.RError("BT not found " + paramData2.BTName + " from " + text + " / " + text2);
					continue;
				}
				GroupComposite groupComposite = new BehaviorTreeBuilder(config, paramData2.BTName, value3)
				{
					Params = paramData2.Params
				}.Build();
				if (groupComposite == null)
				{
					Logger.RError("BT parse failed " + paramData2.BTName + " from " + text + " / " + text2);
					continue;
				}
				BTKey key2 = new BTKey(text, text2);
				BTData value4 = new BTData(key2, paramData2.Phase, groupComposite);
				dictionary.Add(key2, value4);
				BTNameKey key3 = new BTNameKey(text, paramData2.BTName);
				if (!dictionary2.ContainsKey(key3))
				{
					dictionary2.Add(key3, value4);
				}
				hashSet.Add(paramData2.BTName);
			}
		}
		foreach (KeyValuePair<string, XmlNode> btXmlDatum in btXmlData)
		{
			btXmlDatum.Deconstruct(out key, out var value5);
			string text3 = key;
			XmlNode rootXmlNode = value5;
			if (!hashSet.Contains(text3))
			{
				GroupComposite groupComposite2 = new BehaviorTreeBuilder(config, text3, rootXmlNode).Build();
				if (groupComposite2 == null)
				{
					Logger.RError("BT parse failed (btName)");
					continue;
				}
				BTKey key4 = new BTKey(text3, "passive");
				dictionary.TryAdd(key4, new BTData(key4, 0, groupComposite2));
			}
		}
		return (dictionary, dictionary2);
	}

	private Dictionary<string, XmlNode> LoadBT()
	{
		if (_dataHandler == null)
		{
			throw new Exception("DataHandler is null");
		}
		string[] array = (_dataHandler.GetFiles("aidata/bt/") ?? throw new Exception("EMPTY Directory.")).Where((string x) => x.EndsWith(".xml")).ToArray();
		if (array.Length == 0)
		{
			throw new Exception("EMPTY Directory.");
		}
		Dictionary<string, XmlNode> dictionary = new Dictionary<string, XmlNode>();
		string[] array2 = array;
		foreach (string text in array2)
		{
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				using (Stream inStream = _dataHandler.GetStream("aidata/bt//" + text))
				{
					xmlDocument.Load(inStream);
				}
				XmlElement documentElement = xmlDocument.DocumentElement;
				if (documentElement?.Name != "AITreeInfo")
				{
					throw new Exception("root element name is invalid");
				}
				if (documentElement.ChildNodes.Count != 1)
				{
					throw new Exception("BehaviorTree node not found");
				}
				XmlNode firstChild = documentElement.FirstChild;
				if (firstChild.Name != "BehaviorTree")
				{
					throw new Exception("BehaviorTree node not found");
				}
				XmlNode xmlNode = firstChild.Attributes?.GetNamedItem("name");
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
				if (xmlNode?.Value?.ToLower() != fileNameWithoutExtension.ToLower())
				{
					throw new Exception("BehaviorTree Node name is invalid. NodeName[" + xmlNode?.Value + "] FileName[" + fileNameWithoutExtension + "]");
				}
				dictionary.Add(fileNameWithoutExtension.ToLower(), firstChild);
			}
			catch (Exception ex)
			{
				Logger.RError("BehaviorTree FileName[" + text + "] Load Error " + ex.Message);
			}
		}
		return dictionary;
	}

	private Dictionary<string, Dictionary<string, ParamData>> LoadParam()
	{
		if (_dataHandler == null)
		{
			throw new Exception("DataHandler is null");
		}
		string[] array = (_dataHandler.GetFiles("aidata/param/") ?? throw new Exception("EMPTY Directory.")).Where((string x) => x.EndsWith(".xml")).ToArray();
		if (array.Length == 0)
		{
			throw new Exception("EMPTY Directory.");
		}
		Dictionary<string, Dictionary<string, ParamData>> dictionary = new Dictionary<string, Dictionary<string, ParamData>>();
		string[] array2 = array;
		foreach (string text in array2)
		{
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				using (Stream inStream = _dataHandler.GetStream("aidata/param//" + text))
				{
					xmlDocument.Load(inStream);
				}
				XmlElement documentElement = xmlDocument.DocumentElement;
				if (documentElement?.Name != "AIDataRoot")
				{
					throw new Exception("root element name is invalid");
				}
				if (documentElement.ChildNodes.Count != 1)
				{
					throw new Exception("AIData node not found");
				}
				XmlNode firstChild = documentElement.FirstChild;
				if (firstChild.Name != "AIData")
				{
					throw new Exception("AIData node not found");
				}
				XmlNode xmlNode = firstChild.Attributes?.GetNamedItem("name");
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
				if (xmlNode?.Value?.ToLower() != fileNameWithoutExtension.ToLower())
				{
					throw new Exception("AIData. Node name is invalid. NodeName[" + xmlNode?.Value + "] FileName[" + fileNameWithoutExtension + "]");
				}
				Dictionary<string, ParamData> dictionary2 = new Dictionary<string, ParamData>();
				for (XmlNode xmlNode2 = firstChild.FirstChild; xmlNode2 != null; xmlNode2 = xmlNode2.NextSibling)
				{
					if (xmlNode2.Name != "AITemplate")
					{
						throw new Exception("AITemplate name invalid");
					}
					string text2 = xmlNode2.Attributes?.GetNamedItem("name")?.Value;
					string text3 = xmlNode2.Attributes?.GetNamedItem("btname")?.Value;
					int phase = Convert.ToInt32(xmlNode2.Attributes?.GetNamedItem("phaseid")?.Value ?? "0");
					if (string.IsNullOrEmpty(text2) || string.IsNullOrEmpty(text3))
					{
						throw new Exception("AITemplateName is empty. AIData name: (btFileName)");
					}
					Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
					for (XmlNode xmlNode3 = xmlNode2.FirstChild; xmlNode3 != null; xmlNode3 = xmlNode3.NextSibling)
					{
						string text4 = xmlNode3.Attributes?.GetNamedItem("tag")?.Value;
						if (!string.IsNullOrEmpty(text4))
						{
							string value = xmlNode3.Attributes?.GetNamedItem("param")?.Value;
							if (!string.IsNullOrEmpty(value))
							{
								dictionary3.Add(text4, value);
							}
						}
					}
					dictionary2.Add(text2.ToLower(), new ParamData(text3.ToLower(), phase, dictionary3));
				}
				dictionary.Add(fileNameWithoutExtension.ToLower(), dictionary2);
			}
			catch (Exception ex)
			{
				Logger.RError("AIData FileName[" + text + "] Load Error " + ex.Message);
			}
		}
		return dictionary;
	}

	public BTData? GetBT(string aiName, string templateName)
	{
		BTData.TryGetValue(new BTKey(aiName, templateName), out BTData value);
		return value;
	}

	public BTData? GetBTByBTName(string aiName, string btName)
	{
		BTNameData.TryGetValue(new BTNameKey(aiName, btName), out BTData value);
		return value;
	}

	public List<BTKey> GetBotAvatarBTs()
	{
		return BotAvatarBTs;
	}
}
