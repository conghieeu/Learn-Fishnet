using System;
using System.Globalization;
using System.Xml;
using UnityEngine;

public class TorusHitCheck : IHitCheck
{
	public float Height { get; protected set; }

	public float InnerRad { get; protected set; }

	public float OuterRad { get; protected set; }

	public HitCheckShapeType ShapeType => HitCheckShapeType.Torus;

	public Vector3 Center { get; protected set; }

	public Rotator Rotation { get; protected set; }

	public float CheckRadius { get; protected set; }

	public string Key { get; protected set; }

	public override string ToString()
	{
		return $"Center : {Center} / Rotation : {Rotation} / InnerRad : {InnerRad} / OuterRad : {OuterRad} / Height : {Height}";
	}

	public TorusHitCheck(XmlNode node)
	{
		Key = node.Attributes.GetNamedItem("key")?.Value ?? string.Empty;
		Center = new Vector3(Convert.ToSingle(node.Attributes.GetNamedItem("x").Value, CultureInfo.InvariantCulture), Convert.ToSingle(node.Attributes.GetNamedItem("y").Value, CultureInfo.InvariantCulture), Convert.ToSingle(node.Attributes.GetNamedItem("z").Value, CultureInfo.InvariantCulture));
		Rotation = default(Rotator);
		Height = Convert.ToSingle(node.Attributes.GetNamedItem("height").Value, CultureInfo.InvariantCulture);
		InnerRad = Convert.ToSingle(node.Attributes.GetNamedItem("innerradius").Value, CultureInfo.InvariantCulture);
		OuterRad = Convert.ToSingle(node.Attributes.GetNamedItem("outerradius").Value, CultureInfo.InvariantCulture);
		CheckRadius = GetRadius();
	}

	public TorusHitCheck(Vector3 center, float height, float outerRad, float innerRad, string key)
	{
		Center = center;
		Height = height;
		OuterRad = outerRad;
		InnerRad = innerRad;
		Key = key;
		CheckRadius = GetRadius();
	}

	public IMutableHitCheck Clone()
	{
		return new TorusMutableHitCheck(Center, Height, OuterRad, InnerRad);
	}

	private float GetRadius()
	{
		float f = Rotation.Pitch.toRadian();
		float num = Height * 0.5f * Mathf.Abs(Mathf.Sin(f));
		return OuterRad + num;
	}
}
