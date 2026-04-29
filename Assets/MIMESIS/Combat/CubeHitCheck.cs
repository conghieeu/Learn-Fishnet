using System;
using System.Globalization;
using System.Xml;
using UnityEngine;

public class CubeHitCheck : IHitCheck
{
	public Vector3 Extent { get; protected set; }

	public HitCheckShapeType ShapeType => HitCheckShapeType.Cube;

	public Vector3 Center { get; protected set; }

	public Rotator Rotation { get; protected set; }

	public float CheckRadius { get; protected set; }

	public string Key { get; protected set; }

	public override string ToString()
	{
		return $"Center : {Center} / Rotation : {Rotation} / Extent : {Extent}";
	}

	public CubeHitCheck(XmlNode node)
	{
		Key = node.Attributes.GetNamedItem("key")?.Value ?? string.Empty;
		Center = new Vector3(Convert.ToSingle(node.Attributes.GetNamedItem("BoxX").Value, CultureInfo.InvariantCulture), Convert.ToSingle(node.Attributes.GetNamedItem("BoxY").Value, CultureInfo.InvariantCulture), Convert.ToSingle(node.Attributes.GetNamedItem("BoxZ").Value, CultureInfo.InvariantCulture));
		Extent = new Vector3(Convert.ToSingle(node.Attributes.GetNamedItem("ExtX").Value, CultureInfo.InvariantCulture), Convert.ToSingle(node.Attributes.GetNamedItem("ExtY").Value, CultureInfo.InvariantCulture), Convert.ToSingle(node.Attributes.GetNamedItem("ExtZ").Value, CultureInfo.InvariantCulture));
		Rotation = new Rotator(Convert.ToSingle(node.Attributes.GetNamedItem("RotPitch").Value, CultureInfo.InvariantCulture), Convert.ToSingle(node.Attributes.GetNamedItem("RotYaw").Value, CultureInfo.InvariantCulture), Convert.ToSingle(node.Attributes.GetNamedItem("RotRoll").Value, CultureInfo.InvariantCulture));
		CheckRadius = Extent.magnitude;
	}

	public CubeHitCheck(Vector3 center, Vector3 extent, Rotator rotation, string key)
	{
		Center = center;
		Extent = extent;
		Rotation = rotation;
		CheckRadius = Extent.magnitude;
		Key = key;
	}

	public CubeHitCheck(Vector3 extent, string key)
	{
		Center = Vector3.zero;
		Extent = extent;
		Rotation = default(Rotator);
		CheckRadius = Extent.magnitude;
		Key = key;
	}

	public IMutableHitCheck Clone()
	{
		return new BoxMutableHitCheck(Center, Extent, Rotation);
	}
}
