using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Xml;
using UnityEngine;

public class BehaviorTreeBuilder
{
	public class BuilderConfig
	{
		internal Dictionary<string, Type> Actions;

		internal Dictionary<string, Type> Conditions;

		public BuilderConfig(IEnumerable<Type> actions, IEnumerable<Type> conditions)
		{
			Actions = new Dictionary<string, Type>();
			foreach (Type action in actions)
			{
				if (!typeof(IComposite).IsAssignableFrom(action))
				{
					throw new Exception("invalid node faction.FullName)");
				}
				Actions.Add(action.Name.ToLower(), action);
			}
			Conditions = new Dictionary<string, Type>();
			foreach (Type condition in conditions)
			{
				if (!typeof(Conditional).IsAssignableFrom(condition))
				{
					throw new Exception("invalid node (cond.FullName)");
				}
				Conditions.Add(condition.Name.ToLower(), condition);
			}
		}
	}

	public readonly BuilderConfig Config;

	public readonly string Filename;

	public readonly XmlNode RootXmlNode;

	public Dictionary<string, string>? Params;

	public BehaviorTreeBuilder(BuilderConfig config, string filename, XmlNode rootXmlNode)
	{
		Config = config;
		Filename = filename;
		RootXmlNode = rootXmlNode;
	}

	public GroupComposite? Build()
	{
		if (RootXmlNode.Name != "BehaviorTree")
		{
			return null;
		}
		XmlNode firstChild = RootXmlNode.FirstChild;
		if (firstChild == null)
		{
			return null;
		}
		ImmutableArray<IComposite> children = recursionParse(firstChild.ChildNodes).ToImmutableArray();
		if (firstChild.Name == CompositeType.Sequence.ToString())
		{
			return new Sequence(children, null);
		}
		if (firstChild.Name == CompositeType.Selector.ToString())
		{
			return new Selector(children, null);
		}
		if (firstChild.Name == CompositeType.ShuffleSelector.ToString())
		{
			return new ShuffleSelector(children, null);
		}
		return null;
	}

	private static OnBeforeExecuteDelegate? parseCooltimeNode(XmlNode xmlNode)
	{
		XmlNode xmlNode2 = xmlNode.Attributes?.GetNamedItem("id");
		if (xmlNode2 == null)
		{
			return null;
		}
		int id = Convert.ToInt32(xmlNode2.Value);
		return (IBehaviorTreeState state) => state.CheckBTNodeCooltime(id);
	}

	private XmlNode? ReplaceNodeByTag(XmlNode? node)
	{
		if (Params == null)
		{
			return node;
		}
		if (node == null)
		{
			return null;
		}
		string text = node.Attributes.GetNamedItem("tag")?.Value;
		if (text == null || string.IsNullOrEmpty(text))
		{
			return node;
		}
		if (!Params.TryGetValue(text, out string value))
		{
			return node;
		}
		XmlNode namedItem = node.Attributes.GetNamedItem("param");
		if (namedItem != null)
		{
			namedItem.Value = value;
			return node;
		}
		throw new Exception("Node with tag '" + text + "' does not have a 'param' attribute.");
	}

	private List<IComposite> recursionParse(XmlNodeList nodeList)
	{
		List<IComposite> list = new List<IComposite>();
		foreach (XmlNode node in nodeList)
		{
			if (node != null && node.NodeType == XmlNodeType.Comment)
			{
				continue;
			}
			XmlNode xmlNode2 = ReplaceNodeByTag(node);
			if (xmlNode2?.Name == CompositeType.Sequence.ToString())
			{
				ImmutableArray<IComposite> children = recursionParse(xmlNode2.ChildNodes).ToImmutableArray();
				OnBeforeExecuteDelegate onBeforeExecute = parseCooltimeNode(xmlNode2);
				IComposite item = parseDecoratorFromXml(new Sequence(children, onBeforeExecute), xmlNode2);
				list.Add(item);
			}
			else if (xmlNode2?.Name == CompositeType.Selector.ToString())
			{
				ImmutableArray<IComposite> children2 = recursionParse(xmlNode2.ChildNodes).ToImmutableArray();
				OnBeforeExecuteDelegate onBeforeExecute2 = parseCooltimeNode(xmlNode2);
				IComposite item2 = parseDecoratorFromXml(new Selector(children2, onBeforeExecute2), xmlNode2);
				list.Add(item2);
			}
			else if (xmlNode2?.Name == CompositeType.ShuffleSelector.ToString())
			{
				ImmutableArray<IComposite> children3 = recursionParse(xmlNode2.ChildNodes).ToImmutableArray();
				OnBeforeExecuteDelegate onBeforeExecute3 = parseCooltimeNode(xmlNode2);
				IComposite item3 = parseDecoratorFromXml(new ShuffleSelector(children3, onBeforeExecute3), xmlNode2);
				list.Add(item3);
			}
			else if (xmlNode2?.Name == "Action")
			{
				XmlNode xmlNode3 = xmlNode2.Attributes?.GetNamedItem("name");
				if (xmlNode3 != null)
				{
					string[] array = null;
					XmlNode xmlNode4 = xmlNode2.Attributes?.GetNamedItem("param");
					if (xmlNode4 != null)
					{
						array = xmlNode4.Value.Split(',');
					}
					IComposite composite;
					try
					{
						composite = createBehaviorAction(xmlNode3.Value, array);
					}
					catch (Exception ex)
					{
						string text = "[" + Filename + "] " + xmlNode3.Value + " " + string.Join(",", array ?? new string[0]) + ": " + ex.Message;
						Debug.LogError(text);
						composite = new DebugLog(new string[1] { text });
					}
					composite = parseDecoratorFromXml(composite, xmlNode2);
					list.Add(composite);
				}
			}
			else if (xmlNode2?.Name == "DelayedAction")
			{
				XmlNode xmlNode5 = xmlNode2.Attributes?.GetNamedItem("name");
				if (xmlNode5 == null)
				{
					continue;
				}
				XmlNode xmlNode6 = xmlNode2.Attributes?.GetNamedItem("delayTime");
				if (xmlNode6 != null)
				{
					long delayTime = Convert.ToInt64(xmlNode6.Value);
					string[] array2 = null;
					XmlNode xmlNode7 = xmlNode2.Attributes?.GetNamedItem("param");
					if (xmlNode7 != null)
					{
						array2 = xmlNode7.Value.Split(',');
					}
					IComposite composite2;
					try
					{
						composite2 = createBehaviorAction(xmlNode5.Value, array2);
					}
					catch (Exception ex2)
					{
						string text2 = "[" + Filename + "] " + xmlNode5.Value + " " + string.Join(",", array2 ?? new string[0]) + ": " + ex2.Message;
						Debug.LogError(text2);
						composite2 = new DebugLog(new string[1] { text2 });
					}
					composite2 = parseDecoratorFromXml(composite2, xmlNode2);
					list.Add(new BehaviorDelayedAction(composite2, delayTime));
				}
			}
			else
			{
				if (!(xmlNode2?.Name == "Condition"))
				{
					continue;
				}
				XmlNode xmlNode8 = xmlNode2.Attributes?.GetNamedItem("name");
				if (xmlNode8 != null)
				{
					string[] array3 = null;
					XmlNode xmlNode9 = xmlNode2.Attributes?.GetNamedItem("param");
					if (xmlNode9 != null)
					{
						array3 = xmlNode9.Value.Split(',');
					}
					ImmutableArray<IComposite> immutableArray = recursionParse(xmlNode2.ChildNodes).ToImmutableArray();
					IComposite composite3;
					try
					{
						composite3 = createCondition(immutableArray[0], xmlNode8.Value, array3);
					}
					catch (Exception ex3)
					{
						string text3 = "[" + Filename + "] " + xmlNode8.Value + " " + string.Join(",", array3 ?? new string[0]) + ": " + ex3.Message;
						Debug.LogError(text3);
						composite3 = new DebugLog(new string[1] { text3 });
					}
					composite3 = parseDecoratorFromXml(composite3, xmlNode2);
					list.Add(composite3);
				}
			}
		}
		return list;
	}

	private static IComposite parseDecoratorFromXml(IComposite composite, XmlNode nodeBehaviorTree)
	{
		IComposite composite2 = composite;
		XmlNode xmlNode = nodeBehaviorTree.Attributes?.GetNamedItem("decorator");
		if (xmlNode != null)
		{
			string[] array = xmlNode.Value.Split(',');
			foreach (string obj in array)
			{
				string[] array2 = null;
				string text = obj.Trim();
				if (text.Contains("("))
				{
					string[] array3 = text.TrimEnd(')').Split('(');
					text = array3[0];
					array2 = new string[array3.Length - 1];
					for (int j = 1; j < array3.Length; j++)
					{
						array2[j - 1] = array3[j];
					}
				}
				IComposite composite3 = createDecorator(composite2, text, array2);
				if (composite3 != null)
				{
					composite2 = composite3;
				}
			}
		}
		return composite2;
	}

	private IComposite createBehaviorAction(string actionName, string[]? param)
	{
		if (Config.Actions.TryGetValue(actionName.ToLower(), out Type value))
		{
			return (IComposite)(Activator.CreateInstance(value, new object[1] { param }) ?? throw new NullReferenceException());
		}
		if (actionName.ToLower() == "DebugLog".ToLower())
		{
			return new DebugLog(param);
		}
		throw new Exception("Invalid Action((actionName).");
	}

	private static IComposite? createDecorator(IComposite composite, string decorator, params string[]? value)
	{
		if (decorator.ToLower() == DecoratorType.Random.ToString().ToLower())
		{
			return new CompositeRandom(Convert.ToInt64((value != null) ? value[0] : null), composite);
		}
		if (decorator.ToLower() == DecoratorType.Failer.ToString().ToLower())
		{
			return new Failer(composite);
		}
		if (decorator.ToLower() == DecoratorType.Inverter.ToString().ToLower())
		{
			return new Inverter(composite);
		}
		if (decorator.ToLower() == DecoratorType.Succeeder.ToString().ToLower())
		{
			return new Succeeder(composite);
		}
		if (decorator.ToLower() == DecoratorType.RepeatUntilSucceed.ToString().ToLower())
		{
			return new RepeatUntilSucceed(composite);
		}
		if (decorator.ToLower() == DecoratorType.RepeatUntilFail.ToString().ToLower())
		{
			return new RepeatUntilFail(composite);
		}
		if (decorator.ToLower() == DecoratorType.Repeater.ToString().ToLower())
		{
			return new Repeater(Convert.ToInt32((value != null) ? value[0] : null), composite);
		}
		if (decorator.ToLower() == DecoratorType.Once.ToString().ToLower())
		{
			return new Once(composite);
		}
		if (decorator.ToLower() == DecoratorType.Cooltime.ToString().ToLower())
		{
			return new Cooltime(composite, Convert.ToInt64((value != null) ? value[0] : null));
		}
		return null;
	}

	private Conditional createCondition(IComposite child, string condition, string[]? param)
	{
		if (Config.Conditions.TryGetValue(condition.ToLower(), out Type value))
		{
			return (Conditional)(Activator.CreateInstance(value, child, param) ?? throw new NullReferenceException());
		}
		throw new Exception("createCondition Invalid Condition(fcondition))");
	}
}
