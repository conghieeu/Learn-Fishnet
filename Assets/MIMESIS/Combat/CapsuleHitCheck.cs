using System;
using System.Globalization;
using System.Xml;
using UnityEngine;

public class CapsuleHitCheck : IHitCheck
{
	public float Rad { get; protected set; }

	public float Length { get; protected set; }

	public HitCheckShapeType ShapeType => HitCheckShapeType.Capsule;

	public Vector3 Center { get; protected set; }

	public Rotator Rotation { get; protected set; }

	public float CheckRadius { get; protected set; }

	public string Key { get; protected set; }

	public override string ToString()
	{
		return $"Center : {Center} / Rotation : {Rotation} / Rad : {Rad} / Length : {Length}";
	}

	public CapsuleHitCheck(XmlNode node)
	{
		Key = node.Attributes.GetNamedItem("key")?.Value ?? string.Empty;
		Center = new Vector3(Convert.ToSingle(node.Attributes.GetNamedItem("x").Value, CultureInfo.InvariantCulture), Convert.ToSingle(node.Attributes.GetNamedItem("y").Value, CultureInfo.InvariantCulture), Convert.ToSingle(node.Attributes.GetNamedItem("z").Value, CultureInfo.InvariantCulture));
		Rotation = new Rotator(Convert.ToSingle(node.Attributes.GetNamedItem("RotPitch").Value, CultureInfo.InvariantCulture), Convert.ToSingle(node.Attributes.GetNamedItem("RotYaw").Value, CultureInfo.InvariantCulture), Convert.ToSingle(node.Attributes.GetNamedItem("RotRoll").Value, CultureInfo.InvariantCulture));
		Rad = Convert.ToSingle(node.Attributes.GetNamedItem("radius").Value, CultureInfo.InvariantCulture);
		Length = Convert.ToSingle(node.Attributes.GetNamedItem("length").Value, CultureInfo.InvariantCulture);
		CheckRadius = GetRadius();
	}

	public CapsuleHitCheck(float rad, float length, Vector3 center, Rotator rotation, string key)
	{
		Rad = rad;
		Length = length;
		Center = center;
		Rotation = rotation;
		Key = key;
		CheckRadius = GetRadius();
	}

	public CapsuleHitCheck(float rad, float length, string key)
	{
		Rad = rad;
		Length = length;
		Center = Vector3.zero;
		Rotation = default(Rotator);
		Key = key;
		CheckRadius = GetRadius();
	}

	public IMutableHitCheck Clone()
	{
		return new CapsuleMutableHitCheck(Rad, Length, Center, Rotation);
	}

	private float GetRadius()
	{
		float f = Rotation.Pitch.toRadian();
		Rotation.Roll.toRadian();
		float num = Length * 0.5f * Mathf.Abs(Mathf.Sin(f));
		return Rad + num;
	}
}
