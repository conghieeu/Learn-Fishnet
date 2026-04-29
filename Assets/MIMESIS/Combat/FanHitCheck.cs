using System;
using System.Globalization;
using System.Xml;
using UnityEngine;

public class FanHitCheck : IHitCheck
{
	public float InnerRad { get; protected set; }

	public float OuterRad { get; protected set; }

	public float Height { get; protected set; }

	public float Angle { get; protected set; }

	public HitCheckShapeType ShapeType => HitCheckShapeType.Fan;

	public Vector3 Center { get; protected set; }

	public Rotator Rotation { get; protected set; }

	public float CheckRadius { get; protected set; }

	public string Key { get; protected set; }

	public override string ToString()
	{
		return $"Center : {Center} / Rotation : {Rotation} / InnerRad : {InnerRad} / OuterRad : {OuterRad} / Height : {Height} / Angle : {Angle}";
	}

	public FanHitCheck(XmlNode node)
	{
		Key = node.Attributes.GetNamedItem("key")?.Value ?? string.Empty;
		Center = new Vector3(Convert.ToSingle(node.Attributes.GetNamedItem("x").Value, CultureInfo.InvariantCulture), Convert.ToSingle(node.Attributes.GetNamedItem("y").Value, CultureInfo.InvariantCulture), Convert.ToSingle(node.Attributes.GetNamedItem("z").Value, CultureInfo.InvariantCulture));
		Rotation = new Rotator(Convert.ToSingle(node.Attributes.GetNamedItem("RotPitch").Value, CultureInfo.InvariantCulture), Convert.ToSingle(node.Attributes.GetNamedItem("RotYaw").Value, CultureInfo.InvariantCulture), Convert.ToSingle(node.Attributes.GetNamedItem("RotRoll").Value, CultureInfo.InvariantCulture));
		InnerRad = Convert.ToSingle(node.Attributes.GetNamedItem("radius").Value, CultureInfo.InvariantCulture);
		OuterRad = Convert.ToSingle(node.Attributes.GetNamedItem("outerradius").Value, CultureInfo.InvariantCulture);
		Angle = Convert.ToSingle(node.Attributes.GetNamedItem("length").Value, CultureInfo.InvariantCulture);
		CheckRadius = GetRadius();
	}

	public FanHitCheck(Vector3 center, Rotator rotation, float height, float innerRad, float outerRad, float angle, string key)
	{
		Center = center;
		Rotation = rotation;
		Height = height;
		Angle = angle;
		InnerRad = innerRad;
		OuterRad = outerRad;
		CheckRadius = GetRadius();
		Key = key;
	}

	public IMutableHitCheck Clone()
	{
		return new FanMutableHitCheck(Center, Rotation, Height, InnerRad, OuterRad, Angle);
	}

	private float GetRadius()
	{
		float f = Rotation.Pitch.toRadian();
		float num = Mathf.Abs(Mathf.Cos(f));
		float num2 = OuterRad * num;
		float num3 = Height * Mathf.Abs(Mathf.Sin(f));
		return num2 + num3;
	}
}
