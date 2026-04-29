using System;
using Newtonsoft.Json;
using ReluProtocol;

public class PosWithRotConverter : JsonConverter<PosWithRot>
{
	public override void WriteJson(JsonWriter writer, PosWithRot value, JsonSerializer serializer)
	{
		writer.WriteStartObject();
		writer.WritePropertyName("x");
		writer.WriteValue(value.x);
		writer.WritePropertyName("y");
		writer.WriteValue(value.y);
		writer.WritePropertyName("z");
		writer.WriteValue(value.z);
		writer.WritePropertyName("pitch");
		writer.WriteValue(value.pitch);
		writer.WritePropertyName("yaw");
		writer.WriteValue(value.yaw);
		writer.WritePropertyName("roll");
		writer.WriteValue(value.roll);
		writer.WriteEndObject();
	}

	public override PosWithRot ReadJson(JsonReader reader, Type objectType, PosWithRot existingValue, bool hasExistingValue, JsonSerializer serializer)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		float num6 = 0f;
		while (reader.Read())
		{
			if (reader.TokenType == JsonToken.PropertyName)
			{
				string text = (string)reader.Value;
				reader.Read();
				switch (text)
				{
				case "x":
					num = Convert.ToSingle(reader.Value);
					break;
				case "y":
					num2 = Convert.ToSingle(reader.Value);
					break;
				case "z":
					num3 = Convert.ToSingle(reader.Value);
					break;
				case "pitch":
					num4 = Convert.ToSingle(reader.Value);
					break;
				case "yaw":
					num5 = Convert.ToSingle(reader.Value);
					break;
				case "roll":
					num6 = Convert.ToSingle(reader.Value);
					break;
				}
			}
			else if (reader.TokenType == JsonToken.EndObject)
			{
				break;
			}
		}
		PosWithRot posWithRot = new PosWithRot();
		posWithRot.x = num;
		posWithRot.y = num2;
		posWithRot.z = num3;
		posWithRot.pitch = num4;
		posWithRot.yaw = num5;
		posWithRot.roll = num6;
		return posWithRot;
	}
}
