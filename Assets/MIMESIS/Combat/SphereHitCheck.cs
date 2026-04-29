using System;
using System.Globalization;
using System.Xml;
using UnityEngine;

public class SphereHitCheck : IHitCheck
{
	public float Rad { get; protected set; }

	public HitCheckShapeType ShapeType => HitCheckShapeType.Sphere;

	public Vector3 Center { get; protected set; }

	public Rotator Rotation { get; protected set; }

	public float CheckRadius { get; protected set; }

	public string Key { get; protected set; }

	public override string ToString()
	{
		return $"Center : {Center} / Rotation : {Rotation} / Rad : {Rad}";
	}

	public SphereHitCheck(XmlNode node)
	{
		Key = node.Attributes.GetNamedItem("key")?.Value ?? string.Empty;
		Rotation = default(Rotator);
		Center = new Vector3(Convert.ToSingle(node.Attributes.GetNamedItem("x").Value, CultureInfo.InvariantCulture), Convert.ToSingle(node.Attributes.GetNamedItem("y").Value, CultureInfo.InvariantCulture), Convert.ToSingle(node.Attributes.GetNamedItem("z").Value, CultureInfo.InvariantCulture));
		Rad = Convert.ToSingle(node.Attributes.GetNamedItem("radius").Value, CultureInfo.InvariantCulture);
		CheckRadius = Rad;
	}

	public SphereHitCheck(Vector3 center, float rad, string key)
	{
		Center = center;
		Rad = rad;
		CheckRadius = Rad;
		Key = key;
	}

	public IMutableHitCheck Clone()
	{
		return new SphereMutableHitCheck(Center, Rad);
	}
}
